using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frog_Defense.Enemies
{
    class BasicEnemy : Enemy
    {
        //string info
        public override string ToString()
        {
            string output = EnemyType +
                "\nHealth: " + (int)health + "\nMax Health: " + (int)MAX_HEALTH +
                "\nCash Value: $" + CashValue +
                "\n\n" + Description;

            if (isPoisoned)
            {
                output += "\n\nPoisoned for " + (poisonCounter.ticksRemaining / 60) + "s";
                output += "\n   " + (int)(poisonCounter.damagePerTick * poisonCounter.ticksRemaining) + " damage remaining";
            }

            return output;
        }

        protected virtual string EnemyType { get { return "Basic Enemy"; } }
        protected virtual string Description { get { return "No special properties."; } }

        //Health info
        protected override float Health
        {
            get { return health; }
            set { health = value; }
        }
        private float health;

        protected override float MAX_HEALTH
        {
            get { return 100f * scalingFactor; }
        }

        /// <summary>
        /// The cash value dropped by a dead enemy
        /// </summary>
        protected override float BaseCashValue { get { return 10; } }
        public override int TicksAfterSpawn { get { return 45; } }

        private bool hasReachedGoal;

        /// <summary>
        /// Whether or not this enemy has hit a target
        /// </summary>
        public override bool HasReachedGoal
        {
            get { return hasReachedGoal; }
        }

        /// <summary>
        /// The number of pixels per tick to move
        /// </summary>
        protected virtual float Speed
        {
            get { return 1.7f; }
        }

        private float xCenter, yCenter;
        public override float VisualXCenter
        {
            get { return xCenter; }
        }
        public override float VisualYCenter
        {
            get { return yCenter; }
        }

        private int goalX, goalY;
        private bool hasGoal;

        //Also, some data for the current square coordinates the enemy is occupying
        protected int currentSquareX, currentSquareY;
        protected int nextSquareX, nextSquareY;

        public override int CurrentSquareX { get { return currentSquareX; } }
        public override int CurrentSquareY { get { return currentSquareY; } }

        public override int NextSquareX { get { return nextSquareX; } }
        public override int NextSquareY { get { return nextSquareY; } }

        //graphics stuff, the usual
        private const int imageWidth = 30;
        private const int imageHeight = 30;

        public override int PixelWidth
        {
            get { return imageWidth; }
        }
        public override int PixelHeight
        {
            get { return imageHeight; }
        }

        private const String leftPath = "Images/Enemies/BasicEnemy/Left";
        private const String rightPath = "Images/Enemies/BasicEnemy/Right";
        private const String upPath = "Images/Enemies/BasicEnemy/Up";
        private const String downPath = "Images/Enemies/BasicEnemy/Down";

        private static Texture2D leftTexture;
        private static Texture2D rightTexture;
        private static Texture2D upTexture;
        private static Texture2D downTexture;

        protected virtual Texture2D LeftTexture { get { return leftTexture; } }
        protected virtual Texture2D RightTexture { get { return rightTexture; } }
        protected virtual Texture2D UpTexture { get { return upTexture; } }
        protected virtual Texture2D DownTexture { get { return downTexture; } }

        private Texture2D imageTexture;
        protected virtual Texture2D ImageTexture
        {
            get { return imageTexture; }
        }

        private const String previewPath = "Images/Enemies/BasicEnemy/Preview";
        private static Texture2D previewTexture;
        public override Texture2D PreviewTexture
        {
            get { return previewTexture; }
        }

        //facing is a visual indicator
        private Direction facing;
        protected Direction Facing
        {
            get { return facing; }
            set
            {
                facing = value;

                switch (facing)
                {
                    case Direction.UP:
                        imageTexture = UpTexture;
                        break;

                    case Direction.DOWN:
                        imageTexture = DownTexture;
                        break;

                    case Direction.LEFT:
                        imageTexture = LeftTexture;
                        break;

                    case Direction.RIGHT:
                        imageTexture = RightTexture;
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public BasicEnemy(ArenaMap arena, ArenaManager manager, int startX, int startY, float scale)
            : base(manager, arena, scale)
        {
            this.xCenter = startX;
            this.yCenter = startY;

            Facing = Direction.UP;

            this.health = MAX_HEALTH;

            hasGoal = false;
            hasReachedGoal = false;
        }

        public override bool isInvolvedWithSquare(int squareX, int squareY)
        {
            if (this.currentSquareX == squareX && this.currentSquareY == squareY)
                return true;

            if (this.nextSquareX == squareX && this.nextSquareY == squareY)
                return true;

            return false;
        }

        public override void shift(int xChange, int yChange, int xSquaresChange, int ySquaresChange)
        {
            xCenter += xChange;
            yCenter += yChange;

            goalX += xChange;
            goalY += yChange;

            currentSquareX += xSquaresChange;
            currentSquareY += ySquaresChange;

            nextSquareX += xSquaresChange;
            nextSquareY += ySquaresChange;
        }

        public override void setPosition(int xCenter, int yCenter, int xSquare, int ySquare)
        {
            this.xCenter = xCenter;
            this.yCenter = yCenter;

            this.currentSquareX = xSquare;
            this.currentSquareY = ySquare;

            this.nextSquareX = xSquare;
            this.nextSquareY = ySquare;

            hasGoal = false;
        }

        public override bool IsAlive
        {
            get { return health > 0; }
        }

        private const int hitFlashFrames = 3;
        private int framesSinceHit = 3;

        public override bool ContainsPoint(Point p)
        {
            return xCenter - imageWidth / 2 <= p.X && p.X <= xCenter + imageWidth / 2
                && yCenter - imageHeight / 2 <= p.Y && p.Y <= yCenter + imageHeight / 2;
        }

        public override bool touchesRay(int xOrigin, int yOrigin, Direction dir)
        {
            switch (dir)
            {
                case Direction.DOWN:
                    return xCenter - imageWidth / 2 <= xOrigin && xOrigin <= xCenter + imageWidth / 2
                        && yOrigin <= yCenter + imageHeight / 2;

                case Direction.UP:
                    return xCenter - imageWidth / 2 <= xOrigin && xOrigin <= xCenter + imageWidth / 2
                        && yOrigin >= yCenter - imageHeight / 2;

                case Direction.RIGHT:
                    return yCenter - imageHeight / 2 <= yOrigin && yOrigin <= yCenter + imageHeight / 2
                        && xOrigin <= xCenter + imageHeight / 2;

                case Direction.LEFT:
                    return yCenter - imageHeight / 2 <= yOrigin && yOrigin <= yCenter + imageHeight / 2
                        && xOrigin >= xCenter - imageHeight / 2;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Reduces health by the specified amount and flashes red.  Note there
        /// is no invincibility frame!
        /// </summary>
        /// <param name="damage"></param>
        public override void TakeHit(float damage)
        {
            health -= damage;
            framesSinceHit = 0;
        }

        /// <summary>
        /// Loads up the textures if they aren't already.  Does nothing if the
        /// textures are already loaded.
        /// </summary>
        public static new void LoadContent()
        {
            if (leftTexture == null)
                leftTexture = TDGame.MainGame.Content.Load<Texture2D>(leftPath);

            if (rightTexture == null)
                rightTexture = TDGame.MainGame.Content.Load<Texture2D>(rightPath);

            if (upTexture == null)
                upTexture = TDGame.MainGame.Content.Load<Texture2D>(upPath);

            if (downTexture == null)
                downTexture = TDGame.MainGame.Content.Load<Texture2D>(downPath);

            if (previewTexture == null)
                previewTexture = TDGame.MainGame.Content.Load<Texture2D>(previewPath);
        }

        //some parameters to slow the movement down
        private int framesBetweenMoves = 1;
        private int framesSinceMove = 0;

        /// <summary>
        /// Moves!
        /// </summary>
        protected override void update()
        {
            UpdateGoal();

            MoveTowardGoal();
        }

        /// <summary>
        /// Updates the goal if there isn't one.  Helper method for Update;
        /// not for use in other contexts.
        /// 
        /// If the resulting goal is the square it's sitting on,
        /// it decides it's gotten where it wants to be.
        /// </summary>
        private void UpdateGoal()
        {
            if (!hasGoal)
            {
                currentSquareX = nextSquareX;
                currentSquareY = nextSquareY;

                Point goal = arena.nextWayPointSquare(currentSquareX, currentSquareY);

                nextSquareX = goal.X;
                nextSquareY = goal.Y;

                goal = arena.squareCoordinatesToPixels(goal.X, goal.Y);

                goalX = goal.X;
                goalY = goal.Y;

                if (xCenter == goalX && yCenter == goalY)
                    reachGoal();
                else if (xCenter < goalX)
                    Facing = Direction.RIGHT;
                else if (xCenter > goalX)
                    Facing = Direction.LEFT;
                else if (yCenter < goalY)
                    Facing = Direction.DOWN;
                else
                    Facing = Direction.UP;

                hasGoal = true;
            }
        }

        /// <summary>
        /// Gets triggered when the enemy hits a goal.  Sucks for everyone actually!
        /// </summary>
        private void reachGoal()
        {
            hasReachedGoal = true;
        }

        /// <summary>
        /// Helper method for Update.  Does what it says.  Not
        /// for use in other contexts.
        /// </summary>
        private void MoveTowardGoal()
        {
            if (framesSinceMove < framesBetweenMoves)
            {
                framesSinceMove++;
                return;
            }

            framesSinceMove = 0;

            if (goalX < xCenter)
            {
                xCenter = Math.Max(goalX, xCenter - Speed);
            }
            else if (goalX > xCenter)
            {
                xCenter = Math.Min(goalX, xCenter + Speed);
            }

            if (goalY < yCenter)
            {
                yCenter = Math.Max(goalY, yCenter - Speed);
            }
            else if (goalY > yCenter)
            {
                yCenter = Math.Min(goalY, yCenter + Speed);
            }

            if (xCenter == goalX && yCenter == goalY)
                hasGoal = false;
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset, bool paused)
        {
            Color tint;

            if (framesSinceHit < hitFlashFrames)
            {
                framesSinceHit += 1;
                tint = Color.Red;
            }
            else
            {
                tint = Color.White;
            }

            batch.Draw(
                ImageTexture,
                new Vector2(
                    xCenter + xOffset - imageWidth / 2,
                    yCenter + yOffset - imageHeight / 2
                    ),
                tint
                );

            if (isPoisoned)
                DrawPoison(gameTime, batch, xCenter + xOffset - (imageWidth*4)/7, yCenter + yOffset - imageHeight / 2, paused);
        }
    }
}
