using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Frog_Defense.Traps;
using Frog_Defense.Enemies;
using System.IO;

namespace Frog_Defense
{
    enum SquareType { FLOOR, WALL, VOID };

    /// <summary>
    /// This is just a map for the Arena; it should have as little unrelated functionality as possible!
    /// </summary>
    class ArenaMap
    {
        private ArenaManager manager;

        private SquareType[,] floorType; //the passability grid.  Edges should never be passable.
        private bool[,] hasFloorTrap; //a grid keeping track of whether there is a trap on this spot.
        private Dictionary<Direction, bool[,]> hasWallTrap; //a grid keeping track of whether there is a trap on a wall bordering this spot (can only be one per direction)

        private int width; //the width of the grid
        private int height; //the height of the grid

        //each (passable) point in the grid has a best next target, or possibly several,
        //on the way to the goal.  This is stored here.
        private Queue<Point>[,] bestPaths; 

        //enemy spawn points.
        private Queue<Point> spawnPositions;

        //enemy goal points.
        private Queue<Point> goalPositions;

        private Trap selectedTrap;

        //Graphically, the size (in pixels) of grid squares.
        private const int squareWidth = 30;
        private const int squareHeight = 30;

        /// <summary>
        /// The most recent square that was moused over.  The default value,
        /// for "not on any square in particular" is (-1,-1), but other
        /// failure values are possible.  Do not assume this is either (-1,-1)
        /// or a valid square!
        /// </summary>
        private Point highlightedSquare;

        /// <summary>
        /// Within the square, which edge is the mouse closest to?  If the top,
        /// favored direction is down (etc.) since a gun on the top wall would
        /// face down.
        /// </summary>
        private Direction favoredDirection;

        /// <summary>
        /// The most recent coordinates of the mouse.  This is relative to
        /// the arena, so the upper-left corner of the arena is (0,0).
        /// </summary>
        private Point mousePosition;

        /// <summary>
        /// The width, in pixels, of the arena
        /// </summary>
        public int PixelWidth
        {
            get { return squareWidth * width; }
        }

        /// <summary>
        /// The height, in pixels, of the arena
        /// </summary>
        public int PixelHeight
        {
            get { return squareHeight * height; }
        }

        //Graphically, the size (in pixels) of the arrows.
        private const int arrowWidth = 20;
        private const int arrowHeight = 20;

        //textures and paths to find those textures
        //for squares ...
        private const String passableSquarePath = "Images/Squares/PassableSquareMedium";
        private static Texture2D passableSquareTexture;
        private const String impassableSquarePath = "Images/Squares/ImpassableSquareMedium";
        private static Texture2D impassableSquareTexture;
        private const String startSquarePath = "Images/Squares/StartSquareMedium";
        private static Texture2D startSquareTexture;
        private const String goalSquarePath = "Images/Squares/GoalSquareMedium";
        private static Texture2D goalSquareTexture;
        private const String highlightedSquarePath = "Images/Squares/HighlightedSquareMedium";
        private static Texture2D highlightedSquareTexture;

        //for arrows ...
        private const String upArrowPath = "Images/Arrows/ArrowUp";
        private static Texture2D upArrowTexture;
        private const String downArrowPath = "Images/Arrows/ArrowDown";
        private static Texture2D downArrowTexture;
        private const String rightArrowPath = "Images/Arrows/ArrowRight";
        private static Texture2D rightArrowTexture;
        private const String leftArrowPath = "Images/Arrows/ArrowLeft";
        private static Texture2D leftArrowTexture;

        /// <summary>
        /// reloads all unloaded content.  Does nothing if the content is loaded
        /// </summary>
        public static void LoadContent()
        {
            if (impassableSquareTexture == null)
                impassableSquareTexture = TDGame.MainGame.Content.Load<Texture2D>(impassableSquarePath);

            if (passableSquareTexture == null)
                passableSquareTexture = TDGame.MainGame.Content.Load<Texture2D>(passableSquarePath);

            if (goalSquareTexture == null)
                goalSquareTexture = TDGame.MainGame.Content.Load<Texture2D>(goalSquarePath);

            if (startSquareTexture == null)
                startSquareTexture = TDGame.MainGame.Content.Load<Texture2D>(startSquarePath);

            if (highlightedSquareTexture == null)
                highlightedSquareTexture = TDGame.MainGame.Content.Load<Texture2D>(highlightedSquarePath);

            if (upArrowTexture == null)
                upArrowTexture = TDGame.MainGame.Content.Load<Texture2D>(upArrowPath);

            if (downArrowTexture == null)
                downArrowTexture = TDGame.MainGame.Content.Load<Texture2D>(downArrowPath);

            if (rightArrowTexture == null)
                rightArrowTexture = TDGame.MainGame.Content.Load<Texture2D>(rightArrowPath);

            if (leftArrowTexture == null)
                leftArrowTexture = TDGame.MainGame.Content.Load<Texture2D>(leftArrowPath);
        }

        private ArenaMap(ArenaManager env)
        {
            this.manager = env;
        }

        /// <summary>
        /// Parses a file into a map, with the following rules:
        ///     -   is a (passable) floor tile
        ///     s   is a spawn point (where enemies come from)
        ///     h   is a home point (what you defend)
        ///     everything else is defaulted to an impassable wall tile
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public static ArenaMap MakeMapFromTextFile(String filename, ArenaManager env)
        {
            List<string> lines = getLines(filename);

            //stretch every line until they're as long as their longest memer
            int maxWidth = 0;
            foreach (string s in lines)
                maxWidth = Math.Max(maxWidth, s.Length);

            String line;
            for (int i = 0; i < lines.Count; i++)
            {
                line = lines[i];
                while (line.Length < maxWidth)
                    line += " ";
                lines[i] = line;
            }

            //now we have a width and a height- add a buffer row/col on every side
            int width = lines[0].Length + 2;
            int height = lines.Count + 2;

            //now it's as simple as parsing the text
            ArenaMap output = new ArenaMap(env);
            output.spawnPositions = new Queue<Point>();
            output.goalPositions = new Queue<Point>();

            output.width = width;
            output.height = height;
            output.floorType = new SquareType[width, height];

            //put walls along the top and bottom
            for (int x = 0; x < width; x++)
            {
                output.floorType[x, 0] = SquareType.WALL;
                output.floorType[x, height - 1] = SquareType.WALL;
            }

            //now parse ...
            //Note the funny bounds are to compensate for the new top
            //and bottom walls which aren't in the text file
            for (int y = 1; y + 1 < height; y++)
            {
                //similarly here, add the walls ...
                line = " " + lines[y - 1] + " ";

                for(int x = 0; x < width; x++)
                {
                    char c = line[x];

                    if (c == '-')
                    {
                        output.floorType[x, y] = SquareType.FLOOR;
                    }
                    else if (c == 's')
                    {
                        output.floorType[x, y] = SquareType.FLOOR;
                        output.spawnPositions.Enqueue(new Point(x, y));
                    }
                    else if (c == 'h')
                    {
                        output.floorType[x, y] = SquareType.FLOOR;
                        output.goalPositions.Enqueue(new Point(x, y));
                    }
                    else
                    {
                        output.floorType[x, y] = SquareType.WALL;
                    }
                }
            }

            output.finishMap();

            return output;
        }

        /// <summary>
        /// Returns the list of lines which make up a file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static List<string> getLines(String filename)
        {
            StreamReader reader = new StreamReader(filename);
            List<string> lines = new List<string>();

            String line;

            //Can't resist the two-line-loop :) This just piles the lines into the list
            while ((line = reader.ReadLine()) != null)
                lines.Add(line);

            reader.Close();

            return lines;
        }

        /// <summary>
        /// Returns the default map
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        public static ArenaMap DefaultMap(ArenaManager env)
        {
            return ArenaMap.MakeMapFromTextFile("Maps/DefaultMap.txt", env);
        }

        /// <summary>
        /// Converts square coordinates into pixel coordinates (still
        /// relativized to the arenamap itself of course).
        /// </summary>
        /// <param name="squareX"></param>
        /// <param name="squareY"></param>
        /// <returns></returns>
        public Point squareCoordinatesToPixels(int squareX, int squareY)
        {
            return new Point(
                squareX * squareWidth + squareWidth / 2,
                squareY * squareHeight + squareHeight / 2
                );
        }

        /// <summary>
        /// Does the wayfinding in terms of square coordinates;
        /// if there is a waypoint from here, returns it, else
        /// returns the same point back again.
        /// </summary>
        /// <param name="squareX"></param>
        /// <param name="squareY"></param>
        /// <returns></returns>
        public Point nextWayPointSquare(int squareX, int squareY)
        {
            if (bestPaths[squareX, squareY] == null)
            {
                return new Point(squareX, squareY);
            }
            else
            {
                Point goal = bestPaths[squareX, squareY].Dequeue();
                bestPaths[squareX, squareY].Enqueue(goal);

                return goal;
            }
        }

        /// <summary>
        /// This is a preset arena for testing purposes.  It has blocked borders
        /// and a snaking internal path.
        /// 
        /// drawing:
        /// 
        /// WWWWW  WWW
        /// Wh-hW  WsW
        /// W-W-W  W-W
        /// W---W  W-W
        /// WW-WW  W-W
        ///  W-W   W-W
        ///  W-WWWWW-W
        ///  W-------W
        ///  WWWWWWWWW
        /// </summary>
        private void setupDefaultArena()
        {
            spawnPositions = new Queue<Point>();
            goalPositions = new Queue<Point>();

            width = 10;
            height = 9;

            goalPositions.Enqueue(new Point(1, 1));
            goalPositions.Enqueue(new Point(3, 1));

            spawnPositions.Enqueue(new Point(8, 1));

            floorType = new SquareType[width, height];

            //everything starts as walls
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    floorType[x, y] = SquareType.WALL;
                }
            }

            //carve out the right path
            for (int y = 1; y + 1 < height; y++)
            {
                floorType[width - 2, y] = SquareType.FLOOR;
            }

            //carve out the bottom path
            for (int x = 2; x + 1 < width; x++)
            {
                floorType[x, height - 2] = SquareType.FLOOR;
            }

            //carve out the upper-left square
            for (int x = 1; x < 4; x++)
            {
                for (int y = 1; y < 4; y++)
                {
                    if (x != 2 || y != 2)
                        floorType[x, y] = SquareType.FLOOR;
                }
            }

            //carve out the connecting path
            for (int y = 4; y + 2 < height; y++)
            {
                floorType[2, y] = SquareType.FLOOR;
            }

            finishMap();
        }

        /// <summary>
        /// Clears out trap spots, automatically assigns walls and voids, and rigs up the pathing.
        /// </summary>
        private void finishMap()
        {
            clearTraps();
            autoassignVoid();
            updatePathing();
        }

        /// <summary>
        /// Sets all squares to unoccupied (from a trap perspective).
        /// </summary>
        private void clearTraps()
        {
            hasFloorTrap = new bool[width, height];

            hasWallTrap = new Dictionary<Direction, bool[,]>();
            hasWallTrap[Direction.UP] = new bool[width, height];
            hasWallTrap[Direction.DOWN] = new bool[width, height];
            hasWallTrap[Direction.RIGHT] = new bool[width, height];
            hasWallTrap[Direction.LEFT] = new bool[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    hasFloorTrap[x, y] = false;
                    hasWallTrap[Direction.UP][x, y] = false;
                    hasWallTrap[Direction.DOWN][x, y] = false;
                    hasWallTrap[Direction.RIGHT][x, y] = false;
                    hasWallTrap[Direction.LEFT][x, y] = false;
                }
            }
        }

        /// <summary>
        /// This method automatically replaces all WALL tiles with VOID
        /// if none of its neighbors (including diagonal!) are FLOOR tiles
        /// </summary>
        private void autoassignVoid()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (floorType[x, y] == SquareType.VOID)
                        floorType[x, y] = SquareType.WALL;
                }
            }

            bool[,] canSwitch = new bool[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    canSwitch[x, y] = true;
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (floorType[x, y] == SquareType.FLOOR)
                    {
                        canSwitch[x, y] = false;
                        if (x > 0)
                            canSwitch[x - 1, y] = false;
                        if (x + 1 < width)
                            canSwitch[x + 1, y] = false;
                        if (y > 0)
                            canSwitch[x, y - 1] = false;
                        if (y + 1 < height)
                            canSwitch[x, y + 1] = false;

                        if (x > 0 && y > 0)
                            canSwitch[x - 1, y - 1] = false;
                        if (x > 0 && y + 1 < height)
                            canSwitch[x - 1, y + 1] = false;
                        if (x + 1 < width && y > 0)
                            canSwitch[x + 1, y - 1] = false;
                        if (x + 1 < width && y + 1 < height)
                            canSwitch[x + 1, y + 1] = false;
                    }
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (canSwitch[x, y])
                        floorType[x, y] = SquareType.VOID;
                }
            }
        }

        //this is a super-cool modified Dijkstra's algorithm which
        //stores all best paths from any goal position to every point on the board;
        //by following these backwards, we have the best paths from every point on
        //the board to the closest goal position
        private void updatePathing()
        {
            bestPaths = new Queue<Point>[width, height];
            int[,] recordPathLengths = new int[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    bestPaths[x, y] = null;
                    recordPathLengths[x, y] = int.MaxValue;
                }
            }

            Queue<PathTracker> activePoints = new Queue<PathTracker>();

            foreach (Point p in goalPositions)
            {
                recordPathLengths[p.X, p.Y] = 0;
                activePoints.Enqueue(new PathTracker(p.X, p.Y, 1));
            }

            while (activePoints.Count > 0)
            {
                PathTracker active = activePoints.Dequeue();

                int x = active.x;
                int y = active.y;
                int pathLength = active.length;

                //Now check all the adjacent squares that are passable
                List<int> testXValues = new List<int>(4);
                List<int> testYValues = new List<int>(4);

                if (x > 0 && (floorType[x - 1, y] == SquareType.FLOOR))
                {
                    testXValues.Add(x - 1);
                    testYValues.Add(y);
                }
                if (x + 1 < width && (floorType[x + 1, y] == SquareType.FLOOR))
                {
                    testXValues.Add(x + 1);
                    testYValues.Add(y);
                }
                if (y > 0 && (floorType[x, y - 1] == SquareType.FLOOR))
                {
                    testXValues.Add(x);
                    testYValues.Add(y - 1);
                }
                if (y + 1 < height && (floorType[x, y + 1] == SquareType.FLOOR))
                {
                    testXValues.Add(x);
                    testYValues.Add(y + 1);
                }

                for (int i = 0; i < testXValues.Count; ++i)
                {
                    int testX = testXValues[i];
                    int testY = testYValues[i];

                    if (pathLength < recordPathLengths[testX, testY])
                    {
                        recordPathLengths[testX, testY] = pathLength;
                        bestPaths[testX, testY] = new Queue<Point>(4);
                        bestPaths[testX, testY].Enqueue(new Point(x, y));
                        activePoints.Enqueue(new PathTracker(testX, testY, pathLength + 1));
                    }
                    else if (pathLength == recordPathLengths[testX, testY])
                    {
                        bestPaths[testX, testY].Enqueue(new Point(x, y));
                    }
                }
            }
        }

        /// <summary>
        /// Draws the arena.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="batch"></param>
        public void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset, bool paused)
        {
            drawSquares(gameTime, batch, xOffset, yOffset, paused);
            drawArrows(batch, xOffset, yOffset, paused);
        }

        /// <summary>
        /// Helper method for Draw; not to be used anywhere else.
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        private void drawArrows(SpriteBatch batch, int xOffset, int yOffset, bool paused)
        {
            Texture2D toDraw;
            int drawX, drawY;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (bestPaths[x, y] == null)
                        continue;

                    foreach (Point adj in bestPaths[x, y])
                    {
                        if (adj.X < x)
                        {
                            drawX = x * squareWidth + xOffset - arrowWidth / 2;
                            drawY = y * squareHeight + yOffset + squareWidth / 2 - arrowHeight / 2;

                            toDraw = leftArrowTexture;
                        }
                        else if (adj.X > x)
                        {
                            drawX = x * squareWidth + xOffset + squareWidth - arrowWidth / 2;
                            drawY = y * squareHeight + yOffset + squareWidth / 2 - arrowHeight / 2;

                            toDraw = rightArrowTexture;
                        }
                        else if (adj.Y < y)
                        {
                            drawX = x * squareWidth + xOffset + squareWidth / 2 - arrowWidth / 2;
                            drawY = y * squareHeight + yOffset - arrowHeight / 2;

                            toDraw = upArrowTexture;
                        }
                        else// if (adj.Y > y)
                        {
                            drawX = x * squareWidth + xOffset + squareWidth / 2 - arrowWidth / 2;
                            drawY = y * squareHeight + yOffset + squareHeight - arrowHeight / 2;

                            toDraw = downArrowTexture;
                        }

                        batch.Draw(
                            toDraw,
                            new Vector2(drawX, drawY),
                            Color.White
                            );
                    }
                }
            }
        }

        /// <summary>
        /// Helper method for Draw; not to be used anywhere else.  Draws the
        /// tiles all over, and draws the start/end portals on top of them.
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        private void drawSquares(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset, bool paused)
        {
            //Draw the passability grid
            Texture2D toDraw;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {

                    if (floorType[x, y] == SquareType.VOID)
                        continue;
                    else if (floorType[x, y] == SquareType.FLOOR)
                        toDraw = passableSquareTexture;
                    else //it must be the wall
                        toDraw = impassableSquareTexture;

                    batch.Draw( //draw the texture
                        toDraw, //texture to be drawn
                        new Vector2( //position to be drawn at
                            x * squareWidth + xOffset, //x-position, taking into account centering and current square
                            y * squareHeight + yOffset //y-position, taking into account centering and current square
                            ),
                        Color.White //Color of tint; white indicates no tint
                        );
                }
            }

            //now the highlighted square, if appropriate, along with the selected trap
            if (0 <= highlightedSquare.X && highlightedSquare.X < width
                && 0 <= highlightedSquare.Y && highlightedSquare.Y < height)
            {
                if (selectedTrap != null)
                    selectedTrap.Draw(gameTime, batch, xOffset, yOffset, paused);

                batch.Draw(
                    highlightedSquareTexture,
                    new Vector2(
                        highlightedSquare.X * squareWidth + xOffset,
                        highlightedSquare.Y * squareHeight + yOffset
                        ),
                    Color.White
                    );
            }
            
            //now the starts
            foreach (Point p in spawnPositions)
            {
                batch.Draw(
                    startSquareTexture,
                    new Vector2(
                        p.X * squareWidth + xOffset,
                        p.Y * squareHeight + yOffset
                        ),
                    Color.White
                    );
            }

            //and the ends
            foreach (Point p in goalPositions)
            {
                batch.Draw(
                    goalSquareTexture,
                    new Vector2(
                        p.X * squareWidth + xOffset,
                        p.Y * squareHeight + yOffset
                        ),
                    Color.White
                    );
            }
        }

        /// <summary>
        /// Determines whether the specified point touches any of the fixed walls
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool IsInWall(Point p)
        {
            int xArr = p.X / squareWidth;
            int yArr = p.Y / squareHeight;

            return floorType[xArr, yArr] == SquareType.WALL;
        }

        /// <summary>
        /// Notifies the Arena of the current position of the mouse in coordinates
        /// relative to the Arena (so (0,0) should be the upper-left corner of the Arena).
        /// 
        /// The first thing it does is highlight a square.
        /// 
        /// It will also actually create a hypothetical trap, so that if the user clicks
        /// here, that is the trap that will be placed.
        /// </summary>
        /// <param name="mouseX"></param>
        /// <param name="mouseY"></param>
        public void updateMousePosition(int mouseX, int mouseY)
        {
            mousePosition.X = mouseX;
            mousePosition.Y = mouseY;

            if (mouseX < 0 || mouseY < 0)
            {
                highlightedSquare.X = -1;
                highlightedSquare.Y = -1;
            }
            else
            {
                highlightedSquare.X = mouseX / squareWidth;
                highlightedSquare.Y = mouseY / squareHeight;

                determineCurrentDirection();
            }

            makeTrap();
        }

        private void determineCurrentDirection()
        {
            int distUp, distDown, distRight, distLeft;

            int x = highlightedSquare.X;
            int y = highlightedSquare.Y;

            //we don't need an accurate direction if the mouse is off-screen, and
            //it's not clear what accurate would even mean, anyway
            if (x < 0 || x >= width || y < 0 || y >= height)
                return;

            //check the distances to each of the walls, to determine the closest
            distUp = mousePosition.Y % squareHeight;
            distDown = squareHeight - distUp;
            distLeft = mousePosition.X % squareWidth;
            distRight = squareWidth - distLeft;

            //if any of them is an unoccupied wall, give it a bonus, since it's
            //what the user would actually want
            if (y > 0 && floorType[x, y - 1] == SquareType.WALL && !hasWallTrap[Direction.DOWN][x, y - 1])
                distUp -= squareHeight+squareWidth;

            if (y + 1 < height && floorType[x, y + 1] == SquareType.WALL && !hasWallTrap[Direction.UP][x, y + 1])
                distDown -= squareHeight + squareWidth;

            if (x > 0 && floorType[x - 1, y] == SquareType.WALL && !hasWallTrap[Direction.RIGHT][x - 1, y])
                distLeft -= squareHeight + squareWidth;

            if (x + 1 < width && floorType[x + 1, y] == SquareType.WALL && !hasWallTrap[Direction.LEFT][x + 1, y])
                distRight -= squareHeight + squareWidth;

            //now find the min and pick direction accordingly
            int minDist = Math.Min(distUp, Math.Min(distDown, Math.Min(distLeft, distRight)));

            if (minDist == distUp)
                favoredDirection = Direction.DOWN;
            else if (minDist == distDown)
                favoredDirection = Direction.UP;
            else if (minDist == distRight)
                favoredDirection = Direction.LEFT;
            else
                favoredDirection = Direction.RIGHT;
        }

        /// <summary>
        /// Processes clicks.  More specifically, it places a trap (of the
        /// selected type) on the moused-over square, assuming that position
        /// is valid for the trap and doesn't already have a trap there.
        /// 
        /// Valid positions are passable positions without traps on them, and
        /// which do not have the goal or start square on them.
        /// </summary>
        public void GetClicked()
        {
            //first, if the mouse is off-screen, be done with it
            if (highlightedSquare.X < 0 || highlightedSquare.X >= width || highlightedSquare.Y < 0 || highlightedSquare.Y >= height)
                return;

            placeTrap();
        }

        /// <summary>
        /// This attempts to place selectedTrap permanently in the arena.  Assumes that highlightedSquare
        /// is the appropriate position and selectedTrap is current!  If it's impossible to place the trap,
        /// it will silently do nothing (this is intended behavior, so it can be safely called whether or
        /// not a trap is ready to be placed here).
        /// </summary>
        private void placeTrap()
        {
            if (selectedTrap == null)
                return;

            switch (manager.SelectedTrapType)
            {
                case TrapType.NoType:
                    return;

                case TrapType.SpikeTrap:
                    if (manager.Player.AttemptSpend(selectedTrap.Cost))
                    {
                        manager.addTrap(selectedTrap);
                        hasFloorTrap[highlightedSquare.X, highlightedSquare.Y] = true;
                    }

                    return;

                case TrapType.GunTrap:
                case TrapType.DartTrap:
                    if (manager.Player.AttemptSpend(selectedTrap.Cost))
                    {
                        manager.addTrap(selectedTrap);
                        hasWallTrap[favoredDirection][highlightedSquare.X, highlightedSquare.Y] = true;
                    }

                    return;

                case TrapType.Dig:
                    if (manager.Player.AttemptSpend(selectedTrap.Cost))
                    {
                        dig();
                    }
                    return;

                case TrapType.Wall:
                    if (manager.CanBuildWall(highlightedSquare.X, highlightedSquare.Y) &&
                        manager.Player.AttemptSpend(selectedTrap.Cost))
                    {
                        buildWall();
                    }
                    return;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Builds a wall at the selected location.  Handles the cleanup
        /// as well!
        /// </summary>
        private void buildWall()
        {
            //these shouldn't happen, they're in there for testing
            if (highlightedSquare.X < 0 || highlightedSquare.X >= width
                || highlightedSquare.Y < 0 || highlightedSquare.Y >= height)
            {
                throw new IndexOutOfRangeException();
            }

            if (floorType[highlightedSquare.X, highlightedSquare.Y] != SquareType.FLOOR)
            {
                throw new ArgumentException();
            }

            manager.BuildWall(highlightedSquare.X, highlightedSquare.Y);

            floorType[highlightedSquare.X, highlightedSquare.Y] = SquareType.WALL;

            autoassignVoid();
            updatePathing();
        }

        /// <summary>
        /// Digs at the selected location.  Takes care of the cleanup as well.
        /// </summary>
        private void dig()
        {
            if (highlightedSquare.X < 0 || highlightedSquare.X >= width
                || highlightedSquare.Y < 0 || highlightedSquare.Y >= height)
            {
                throw new IndexOutOfRangeException();
            }

            if (floorType[highlightedSquare.X, highlightedSquare.Y] != SquareType.WALL
                && floorType[highlightedSquare.X, highlightedSquare.Y] != SquareType.VOID)
            {
                throw new ArgumentException();
            }

            manager.Dig(highlightedSquare.X, highlightedSquare.Y);


            if (highlightedSquare.X == width - 1)
            {
                addRightWall();
            }
            if (highlightedSquare.X == 0)
            {
                addLeftWall();
            }

            if (highlightedSquare.Y == height - 1)
            {
                addBottomWall();
            }
            if (highlightedSquare.Y == 0)
            {
                addTopWall();
            }

            floorType[highlightedSquare.X, highlightedSquare.Y] = SquareType.FLOOR;

            autoassignVoid();
            updatePathing();
        }

        #region EXPANDING_MAP
        private void addRightWall()
        {
            //move over the map
            SquareType[,] newFloorType = new SquareType[width + 1, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    newFloorType[x, y] = floorType[x, y];
                }

                newFloorType[width, y] = SquareType.WALL;
            }

            //and the trap data
            bool[,] newFloorTraps = new bool[width + 1, height];

            Dictionary<Direction, bool[,]> newWallTraps = new Dictionary<Direction, bool[,]>();
            newWallTraps[Direction.DOWN] = new bool[width + 1, height];
            newWallTraps[Direction.UP] = new bool[width + 1, height];
            newWallTraps[Direction.RIGHT] = new bool[width + 1, height];
            newWallTraps[Direction.LEFT] = new bool[width + 1, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    newFloorTraps[x, y] = hasFloorTrap[x, y];
                    newWallTraps[Direction.DOWN][x, y] = hasWallTrap[Direction.DOWN][x, y];
                    newWallTraps[Direction.UP][x, y] = hasWallTrap[Direction.UP][x, y];
                    newWallTraps[Direction.RIGHT][x, y] = hasWallTrap[Direction.RIGHT][x, y];
                    newWallTraps[Direction.LEFT][x, y] = hasWallTrap[Direction.LEFT][x, y];
                }

                newFloorTraps[width, y] = false;
                newWallTraps[Direction.DOWN][width, y] = false;
                newWallTraps[Direction.UP][width, y] = false;
                newWallTraps[Direction.RIGHT][width, y] = false;
                newWallTraps[Direction.LEFT][width, y] = false;
            }

            this.floorType = newFloorType;
            this.hasFloorTrap = newFloorTraps;
            this.hasWallTrap = newWallTraps;

            width++;

            autoassignVoid();
            updatePathing();
        }

        private void addLeftWall()
        {
            //move over the map
            SquareType[,] newFloorType = new SquareType[width + 1, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    newFloorType[x + 1, y] = floorType[x, y];
                }

                newFloorType[0, y] = SquareType.WALL;
            }

            //and the trap data
            bool[,] newFloorTraps = new bool[width + 1, height];

            Dictionary<Direction, bool[,]> newWallTraps = new Dictionary<Direction, bool[,]>();
            newWallTraps[Direction.DOWN] = new bool[width + 1, height];
            newWallTraps[Direction.UP] = new bool[width + 1, height];
            newWallTraps[Direction.RIGHT] = new bool[width + 1, height];
            newWallTraps[Direction.LEFT] = new bool[width + 1, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    newFloorTraps[x + 1, y] = hasFloorTrap[x, y];
                    newWallTraps[Direction.DOWN][x + 1, y] = hasWallTrap[Direction.DOWN][x, y];
                    newWallTraps[Direction.UP][x + 1, y] = hasWallTrap[Direction.UP][x, y];
                    newWallTraps[Direction.RIGHT][x + 1, y] = hasWallTrap[Direction.RIGHT][x, y];
                    newWallTraps[Direction.LEFT][x + 1, y] = hasWallTrap[Direction.LEFT][x, y];
                }

                newFloorTraps[0, y] = false;
                newWallTraps[Direction.DOWN][0, y] = false;
                newWallTraps[Direction.UP][0, y] = false;
                newWallTraps[Direction.RIGHT][0, y] = false;
                newWallTraps[Direction.LEFT][0, y] = false;
            }

            this.floorType = newFloorType;
            this.hasFloorTrap = newFloorTraps;
            this.hasWallTrap = newWallTraps;

            width++;
            highlightedSquare.X++;

            for (int i = 0; i < spawnPositions.Count; i++)
            {
                Point p = spawnPositions.Dequeue();
                p.X++;
                spawnPositions.Enqueue(p);
            }

            for (int i = 0; i < goalPositions.Count; i++)
            {
                Point p = goalPositions.Dequeue();
                p.X++;
                goalPositions.Enqueue(p);
            }

            manager.shiftObjects(squareWidth, 0, 1, 0);

            autoassignVoid();
            updatePathing();
        }

        private void addBottomWall()
        {
            //move over the map
            SquareType[,] newFloorType = new SquareType[width, height + 1];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    newFloorType[x, y] = floorType[x, y];
                }

                newFloorType[x, height] = SquareType.WALL;
            }

            //and the trap data
            bool[,] newFloorTraps = new bool[width, height + 1];

            Dictionary<Direction, bool[,]> newWallTraps = new Dictionary<Direction, bool[,]>();
            newWallTraps[Direction.DOWN] = new bool[width, height + 1];
            newWallTraps[Direction.UP] = new bool[width, height + 1];
            newWallTraps[Direction.RIGHT] = new bool[width, height + 1];
            newWallTraps[Direction.LEFT] = new bool[width, height + 1];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    newFloorTraps[x, y] = hasFloorTrap[x, y];
                    newWallTraps[Direction.DOWN][x, y] = hasWallTrap[Direction.DOWN][x, y];
                    newWallTraps[Direction.UP][x, y] = hasWallTrap[Direction.UP][x, y];
                    newWallTraps[Direction.RIGHT][x, y] = hasWallTrap[Direction.RIGHT][x, y];
                    newWallTraps[Direction.LEFT][x, y] = hasWallTrap[Direction.LEFT][x, y];
                }

                newFloorTraps[x, height] = false;
                newWallTraps[Direction.DOWN][x, height] = false;
                newWallTraps[Direction.UP][x, height] = false;
                newWallTraps[Direction.RIGHT][x, height] = false;
                newWallTraps[Direction.LEFT][x, height] = false;
            }

            this.floorType = newFloorType;
            this.hasFloorTrap = newFloorTraps;
            this.hasWallTrap = newWallTraps;

            height++;

            autoassignVoid();
            updatePathing();
        }

        private void addTopWall()
        {
            //move over the map
            SquareType[,] newFloorType = new SquareType[width, height + 1];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    newFloorType[x, y + 1] = floorType[x, y];
                }

                newFloorType[x, 0] = SquareType.WALL;
            }

            //and the trap data
            bool[,] newFloorTraps = new bool[width, height + 1];

            Dictionary<Direction, bool[,]> newWallTraps = new Dictionary<Direction, bool[,]>();
            newWallTraps[Direction.DOWN] = new bool[width, height + 1];
            newWallTraps[Direction.UP] = new bool[width, height + 1];
            newWallTraps[Direction.RIGHT] = new bool[width, height + 1];
            newWallTraps[Direction.LEFT] = new bool[width, height + 1];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    newFloorTraps[x, y + 1] = hasFloorTrap[x, y];
                    newWallTraps[Direction.DOWN][x, y + 1] = hasWallTrap[Direction.DOWN][x, y];
                    newWallTraps[Direction.UP][x, y + 1] = hasWallTrap[Direction.UP][x, y];
                    newWallTraps[Direction.RIGHT][x, y + 1] = hasWallTrap[Direction.RIGHT][x, y];
                    newWallTraps[Direction.LEFT][x, y + 1] = hasWallTrap[Direction.LEFT][x, y];
                }

                newFloorTraps[x, 0] = false;
                newWallTraps[Direction.DOWN][x, 0] = false;
                newWallTraps[Direction.UP][x, 0] = false;
                newWallTraps[Direction.RIGHT][x, 0] = false;
                newWallTraps[Direction.LEFT][x, 0] = false;
            }

            this.floorType = newFloorType;
            this.hasFloorTrap = newFloorTraps;
            this.hasWallTrap = newWallTraps;

            height++;
            highlightedSquare.Y++;

            for (int i = 0; i < spawnPositions.Count; i++)
            {
                Point p = spawnPositions.Dequeue();
                p.Y++;
                spawnPositions.Enqueue(p);
            }

            for (int i = 0; i < goalPositions.Count; i++)
            {
                Point p = goalPositions.Dequeue();
                p.Y++;
                goalPositions.Enqueue(p);
            }

            autoassignVoid();
            updatePathing();

            manager.shiftObjects(0, squareHeight, 0, 1);
        }
        #endregion EXPANDING_MAP

        private void makeTrap()
        {
            selectedTrap = null;

            //if the square is off the board, done
            if (highlightedSquare.X < 0 || highlightedSquare.X >= width
                || highlightedSquare.Y < 0 || highlightedSquare.Y >= height)
            {
                return;
            }

            //if the square is the start, done
            foreach (Point p in spawnPositions)
            {
                if (highlightedSquare.X == p.X && highlightedSquare.Y == p.Y)
                    return;
            }

            //if the square is the goal, done
            foreach (Point p in goalPositions)
            {
                if (highlightedSquare.X == p.X && highlightedSquare.Y == p.Y)
                    return;
            }

            //aside from that, how and where we place traps depends greatly on what kind of trap we're doing!
            switch (manager.SelectedTrapType)
            {
                //if there's no trap, just be done
                case TrapType.NoType:
                    selectedTrap = null;
                    return;

                //all "on the ground"-type traps are handled similarly
                case TrapType.SpikeTrap:
                    makeGroundTrap();
                    break;

                //as are "on the wall"-type traps
                case TrapType.GunTrap:
                case TrapType.DartTrap:
                    makeWallTrap();
                    break;

                //dig is different
                case TrapType.Dig:
                    makeDigIndicator();
                    break;

                //as is wall
                case TrapType.Wall:
                    makeWallIndicator();
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Makes SelectedTrap into a wall indicator, 
        /// assuming the highlighted square is a floor
        /// </summary>
        private void makeWallIndicator()
        {
            int x = highlightedSquare.X;
            int y = highlightedSquare.Y;

            if (floorType[x, y] == SquareType.FLOOR)
            {
                selectedTrap = new BuildWall(
                    manager,
                    x * squareWidth + squareWidth / 2,
                    y * squareHeight + squareHeight / 2,
                    x,
                    y
                    );
            }
            else
            {
                selectedTrap = null;
            }
        }

        /// <summary>
        /// Just makes SelectedTrap into a Dig (indicator),
        /// assuming the highlighted square is a wall or void
        /// </summary>
        private void makeDigIndicator()
        {
            int x = highlightedSquare.X;
            int y = highlightedSquare.Y;

            if (floorType[x, y] == SquareType.VOID || floorType[x, y] == SquareType.WALL)
            {
                selectedTrap = new Dig(
                    manager,
                    x * squareWidth + squareWidth / 2,
                    y * squareHeight + squareHeight / 2,
                    x,
                    y
                    );
            }
            else
            {
                selectedTrap = null;
            }
        }

        private void makeWallTrap()
        {
            //check if it's passable...
            if (floorType[highlightedSquare.X, highlightedSquare.Y] != SquareType.FLOOR)
                return;

            //...and unoccupied
            if (hasWallTrap[favoredDirection][highlightedSquare.X, highlightedSquare.Y])
                return;

            //now, account for centering the trap ON THE WALL,
            //which depends on the favoredDirection
            int xOffset, yOffset;


            //to save on switch blocks (they're not cheap!) we also
            //check here to see if there is a wall to mount the trap on!
            switch (favoredDirection)
            {
                case Direction.RIGHT:
                    if (highlightedSquare.X <= 0 || floorType[highlightedSquare.X - 1, highlightedSquare.Y] != SquareType.WALL)
                        return;

                    xOffset = 0;
                    yOffset = squareHeight / 2;
                    break;

                case Direction.LEFT:
                    if (highlightedSquare.X + 1 >= width || floorType[highlightedSquare.X + 1, highlightedSquare.Y] != SquareType.WALL)
                        return;

                    xOffset = squareWidth;
                    yOffset = squareHeight / 2;
                    break;

                case Direction.DOWN:
                    if (highlightedSquare.Y <= 0 || floorType[highlightedSquare.X, highlightedSquare.Y - 1] != SquareType.WALL)
                        return;

                    xOffset = squareWidth / 2;
                    yOffset = 0;
                    break;

                case Direction.UP:
                    if (highlightedSquare.Y + 1 >= height || floorType[highlightedSquare.X, highlightedSquare.Y + 1] != SquareType.WALL)
                        return;

                    xOffset = squareWidth / 2;
                    yOffset = squareHeight;
                    break;

                default:
                    throw new NotImplementedException();
            }

            int xpos = highlightedSquare.X * squareWidth + xOffset;
            int ypos = highlightedSquare.Y * squareHeight + yOffset;

            switch (manager.SelectedTrapType)
            {
                case TrapType.GunTrap:
                    selectedTrap = new GunTrap(manager, xpos, ypos, highlightedSquare.X, highlightedSquare.Y, favoredDirection);
                    return;

                case TrapType.DartTrap:
                    selectedTrap = new DartTrap(manager, xpos, ypos, highlightedSquare.X, highlightedSquare.Y, favoredDirection);
                    return;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// This makes a ground trap at the specified location, assuming that
        /// the selected trap is a ground trap (otherwise throws an exception).
        /// 
        /// Assumes selectedTrap == null at the start
        /// </summary>
        private void makeGroundTrap()
        {
            //check if it's passable...
            if (floorType[highlightedSquare.X, highlightedSquare.Y] != SquareType.FLOOR)
                return;

            //...and unoccupied
            if (hasFloorTrap[highlightedSquare.X, highlightedSquare.Y])
                return;

            switch (manager.SelectedTrapType)
            {
                case TrapType.SpikeTrap:
                    selectedTrap = new SpikeTrap(
                        manager,
                        highlightedSquare.X * squareWidth + squareWidth / 2,
                        highlightedSquare.Y * squareHeight + squareHeight / 2,
                        highlightedSquare.X,
                        highlightedSquare.Y
                        );
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Draws a huge PAUSED fact across the screen if it's paused.  Does nothing
        /// if the game isn't paused.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="batch"></param>
        /// <param name="arenaOffsetX"></param>
        /// <param name="arenaOffsetY"></param>
        /// <param name="paused"></param>
        public void DrawPausedOverlay(GameTime gameTime, SpriteBatch batch, int arenaOffsetX, int arenaOffsetY, bool paused)
        {
            if (paused)
            {
                Vector2 size = TDGame.HugeFont.MeasureString("PAUSED");

                Vector2 drawPosition = new Vector2(
                    arenaOffsetX + (int)((PixelWidth - size.X) / 2),
                    arenaOffsetY + (int)((PixelHeight - size.Y) / 2)
                    );

                batch.DrawString(TDGame.HugeFont, "PAUSED", drawPosition, Color.White);
            }
        }

        /// <summary>
        /// Fixes the enemy's position to be right in the spawn
        /// </summary>
        /// <param name="e"></param>
        public void putInSpawn(Enemy e)
        {
            Point nextSpawn = spawnPositions.Dequeue();
            spawnPositions.Enqueue(nextSpawn);

            e.setPosition(
                nextSpawn.X * squareWidth + squareWidth / 2,
                nextSpawn.Y * squareHeight + squareHeight / 2,
                nextSpawn.X,
                nextSpawn.Y
                );
        }
    }







    struct PathTracker
    {
        public int x;
        public int y;

        public int length;

        /// <summary>
        /// Creates a new PathTracker
        /// </summary>
        /// <param name="x">x-position</param>
        /// <param name="y">y-position</param>
        public PathTracker(int x, int y, int length)
        {
            this.x = x;
            this.y = y;
            this.length = length;
        }
    }
}
