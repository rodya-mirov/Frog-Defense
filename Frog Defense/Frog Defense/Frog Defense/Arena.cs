﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Frog_Defense.Traps;

namespace Frog_Defense
{
    class Arena
    {
        private EnvironmentUpdater env;

        private bool[,] passable; //the passability grid.  Edges should never be passable.
        private bool[,] hasTrap; //a grid keeping track of whether there is a trap on this spot.
        private int width; //the width of the grid
        private int height; //the height of the grid

        //each (passable) point in the grid has a best next target, or possibly several,
        //on the way to the goal.  This is stored here.
        private Queue<Point>[,] bestPaths; 

        //enemy spawn points.  This may become multiple at some point.
        private int startX;
        private int startY;

        //enemy goal points.  This may also become multiple at some point.
        private int goalX;
        private int goalY;

        //Graphically, the size (in pixels) of grid squares.
        private const int squareWidth = 40;
        private const int squareHeight = 40;

        /// <summary>
        /// The most recent square that was moused over.  The default value,
        /// for "not on any square in particular" is (-1,-1), but other
        /// failure values are possible.  Do not assume this is either (-1,-1)
        /// or a valid square!
        /// </summary>
        private Point highlightedSquare;

        /// <summary>
        /// The most recent coordinates of the mouse.  This is relative to
        /// the arena, so the upper-left corner of the arena is (0,0).
        /// </summary>
        private Point mousePosition;

        public int PixelWidth
        {
            get { return squareWidth * width; }
        }

        public int PixelHeight
        {
            get { return squareHeight * height; }
        }

        //Graphically, the size (in pixels) of the arrows.
        private const int arrowWidth = 20;
        private const int arrowHeight = 20;

        //textures and paths to find those textures
        //for squares ...
        private const String passableSquarePath = "Images/Squares/PassableSquare";
        private static Texture2D passableSquareTexture;
        private const String impassableSquarePath = "Images/Squares/ImpassableSquare";
        private static Texture2D impassableSquareTexture;
        private const String startSquarePath = "Images/Squares/StartSquare";
        private static Texture2D startSquareTexture;
        private const String goalSquarePath = "Images/Squares/GoalSquare";
        private static Texture2D goalSquareTexture;
        private const String highlightedSquarePath = "Images/Squares/HighlightedSquare";
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

        //reloads all unloaded content.  Does nothing if the content is loaded
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

        public Arena(EnvironmentUpdater env)
        {
            this.env = env;

            setupDefaultArena();
        }

        public void Update()
        {
            //Does nothing at the moment.
        }

        /// <summary>
        /// Creates a new enemy, located at the center of the spawn point.
        /// Returns that enemy and does not handle it in any additional way.
        /// </summary>
        /// <returns>The new Enemy</returns>
        public Enemy makeEnemy()
        {
            int x = startX * squareWidth + squareWidth / 2;
            int y = startY * squareHeight + squareHeight / 2;

            return new Enemy(this, env, x, y);
        }

        /// <summary>
        /// Returns the next place an Enemy should go, assuming they're
        /// located at the specified coordinates.
        /// 
        /// Note: this cycles between the goal points, so if there are
        /// multiple good options, repeated calls will return DIFFERENT answers.
        /// 
        /// Note: this assumes the input is in pixel coordinates, and the
        /// output is also in pixel coordinates.  They are not array indices.
        /// 
        /// Note: the results of this method may be surprising if the enemy
        /// is sitting right on an edge :{
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Point nextWayPoint(int x, int y)
        {
            //transform to array indices
            int xArr = x / squareWidth;
            int yArr = y / squareHeight;

            //if there's no path from here, give up
            if (bestPaths[xArr, yArr] == null)
                return new Point(x, y);

            //cycle the best path list
            Point goalArr = bestPaths[xArr, yArr].Dequeue();
            bestPaths[xArr, yArr].Enqueue(goalArr);

            //return the new point in pixel coordinates, pointing
            //to the center of the next goal square
            return new Point(goalArr.X * squareWidth + squareWidth / 2, goalArr.Y * squareHeight + squareHeight / 2);
        }

        public List<Trap> makeTraps()
        {
            return new List<Trap>();
        }

        /// <summary>
        /// This is a preset arena for testing purposes.  It has blocked borders
        /// and a snaking internal path.
        /// </summary>
        private void setupDefaultArena()
        {
            width = 11;
            height = 11;

            startX = width - 2;
            startY = 1;

            goalX = 1;
            goalY = height - 2;

            passable = new bool[width, height];

            //block the borders
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == 0 || x == width - 1)
                        passable[x, y] = false;
                    else if (y == 0 || y == height - 1)
                        passable[x, y] = false;
                    else
                        passable[x, y] = true;
                }
            }

            //toss in some walls
            for (int x = 2; x < width; x += 2)
            {
                for (int y = 2; y < height; y += 2)
                {
                    passable[x, y] = false;
                }
            }

            updatePathing();

            hasTrap = new bool[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    hasTrap[x, y] = false;
                }
            }
        }

        /// <summary>
        /// This is just a preset trap configuration that's used for testing.  It
        /// assumes the wall set of default arena, and other wall sets may not be
        /// compatible!
        /// </summary>
        /// <returns></returns>
        private List<Trap> defaultTraps()
        {
            Random ran = new Random();

            //first add spike traps
            List<Trap> output = new List<Trap>();

            for (int x = 1; x < width; x += 2)
            {
                for (int y = 1; y < height; y += 2)
                {
                    if ((x == goalX && y == goalY) || (x == startX && y == startY))
                        continue;

                    if (ran.NextDouble() < .8)
                        continue;

                    int trapX = x * squareWidth + squareWidth / 2;
                    int trapY = y * squareHeight + squareHeight / 2;

                    SpikeTrap t = new SpikeTrap(this, env, trapX, trapY);
                    output.Add(t);
                }
            }

            addDefaultGunTraps(output);

            return output;
        }

        /// <summary>
        /// Helper method for defaultTraps.
        /// </summary>
        /// <param name="output"></param>
        private void addDefaultGunTraps(List<Trap> output)
        {
            //then add some gun traps
            GunTrap gt;

            //the traps going right...
            for (int x = 1; x + 1 < width; x += 4)
            {
                for (int y = 2; y + 1 < height; y += 2)
                {
                    gt = new GunTrap(this, env, x * squareWidth, squareHeight * y + squareHeight / 2, Direction.RIGHT);
                    output.Add(gt);
                }
            }

            //the traps going left...
            for (int x = 4; x < width; x += 4)
            {
                for (int y = 2; y + 1 < height; y += 2)
                {
                    gt = new GunTrap(this, env, x * squareWidth, squareHeight * y + squareHeight / 2, Direction.LEFT);
                    output.Add(gt);
                }
            }

            //the traps going down...
            for (int x = 2; x + 1 < width; x += 2)
            {
                for (int y = 1; y + 1 < height; y += 4)
                {
                    gt = new GunTrap(this, env, x * squareWidth + squareWidth / 2, squareHeight * y, Direction.DOWN);
                    output.Add(gt);
                }
            }

            //the traps going up...
            for (int x = 2; x + 1 < width; x += 2)
            {
                for (int y = 4; y < height; y += 4)
                {
                    gt = new GunTrap(this, env, x * squareWidth + squareWidth / 2, squareHeight * y, Direction.UP);
                    output.Add(gt);
                }
            }
        }

        //this is a super-cool modified Dijkstra's algorithm which
        //stores all best paths from the goal position to every point on the board;
        //by following these backwards, we have the best paths from every point on
        //the board to the goal position
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

            recordPathLengths[goalX, goalY] = 0;
            activePoints.Enqueue(new PathTracker(goalX, goalY, 1));

            while (activePoints.Count > 0)
            {
                PathTracker active = activePoints.Dequeue();

                int x = active.x;
                int y = active.y;
                int pathLength = active.length;

                //Now check all the adjacent squares that are passable
                List<int> testXValues = new List<int>(4);
                List<int> testYValues = new List<int>(4);

                if (x > 0 && passable[x - 1, y])
                {
                    testXValues.Add(x - 1);
                    testYValues.Add(y);
                }
                if (x + 1 < width && passable[x + 1, y])
                {
                    testXValues.Add(x + 1);
                    testYValues.Add(y);
                }
                if (y > 0 && passable[x, y - 1])
                {
                    testXValues.Add(x);
                    testYValues.Add(y - 1);
                }
                if (y + 1 < height && passable[x, y + 1])
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
        public void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset)
        {
            drawSquares(batch, xOffset, yOffset);
            drawArrows(batch, xOffset, yOffset);
        }

        /// <summary>
        /// Helper method for Draw; not to be used anywhere else.
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        private void drawArrows(SpriteBatch batch, int xOffset, int yOffset)
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
        private void drawSquares(SpriteBatch batch, int xOffset, int yOffset)
        {
            //Draw the passability grid
            Texture2D toDraw;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == highlightedSquare.X && y == highlightedSquare.Y)
                        toDraw = highlightedSquareTexture;
                    else if (passable[x, y])
                        toDraw = passableSquareTexture;
                    else
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

            //now the start and end
            batch.Draw(
                startSquareTexture,
                new Vector2(
                    startX * squareWidth + xOffset,
                    startY * squareHeight + yOffset
                    ),
                Color.White
                );

            batch.Draw(
                goalSquareTexture,
                new Vector2(
                    goalX * squareWidth + xOffset,
                    goalY * squareHeight + yOffset
                    ),
                Color.White
                );
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

            return !passable[xArr, yArr];
        }

        /// <summary>
        /// Notifies the Arena of the current position of the mouse in coordinates
        /// relative to the Arena (so (0,0) should be the upper-left corner of the Arena).
        /// 
        /// The main thing it does is highlight a square, but it will use this stored info
        /// to process clicks as well.
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
            }
        }

        /// <summary>
        /// Creates a SpikeTrap at the current mouse position and returns it,
        /// if the position is valid and unoccupied.  Also marks the space as
        /// occupied afterward, so don't lose the Trap :D
        /// </summary>
        /// <returns></returns>
        public Trap addTrapAtMousePosition()
        {
            //first, if the mouse is off-screen, be done with it
            if (highlightedSquare.X < 0 || highlightedSquare.X >= width || highlightedSquare.Y < 0 || highlightedSquare.Y >= height)
                return null;

            if (passable[highlightedSquare.X, highlightedSquare.Y] && !hasTrap[highlightedSquare.X, highlightedSquare.Y])
            {
                hasTrap[highlightedSquare.X, highlightedSquare.Y] = true;
                return new SpikeTrap(this, env,
                    highlightedSquare.X * squareWidth + squareWidth / 2, highlightedSquare.Y * squareHeight + squareHeight / 2);
            }
            else
            {
                return null;
            }
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
