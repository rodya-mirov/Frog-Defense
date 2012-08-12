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
        //Health info
        protected override float Health
        {
            get { return health; }
            set { health = value; }
        }
        private float health;

        protected override float MAX_HEALTH
        {
            get { return 100; }
        }

        /// <summary>
        /// The cash value dropped by a dead enemy
        /// </summary>
        public override int CashValue
        {
            get { return 10; }
        }

        private bool hasReachedGoal = false;

        /// <summary>
        /// Whether or not this enemy has hit a target
        /// </summary>
        public override bool HasReachedGoal
        {
            get { return hasReachedGoal; }
        }

        //We're using ints for position because XNA gets glitchy when we draw on floats
        private int speed = 2;

        private int xCenter, yCenter;
        public override int XCenter
        {
            get { return xCenter; }
        }
        public override int YCenter
        {
            get { return yCenter; }
        }

        private int goalX, goalY;
        private bool hasGoal;

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

        private const String imagePath = "Images/Enemies/BasicEnemy/Image";
        private static Texture2D imageTexture;
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

        public BasicEnemy(Arena arena, GameUpdater env, int startX, int startY)
            : base(env, arena)
        {
            this.xCenter = startX;
            this.yCenter = startY;

            this.health = MAX_HEALTH;

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
            if (imageTexture == null)
                imageTexture = TDGame.MainGame.Content.Load<Texture2D>(imagePath);

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
        /// </summary>
        private void UpdateGoal()
        {
            if (!hasGoal)
            {
                Point goal = arena.nextWayPoint(xCenter, yCenter);

                goalX = goal.X;
                goalY = goal.Y;

                if (xCenter == goalX && yCenter == goalY)
                    reachGoal();

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
                xCenter = Math.Max(goalX, xCenter - speed);
            }
            else if (goalX > xCenter)
            {
                xCenter = Math.Min(goalX, xCenter + speed);
            }
            if (goalY < yCenter)
            {
                yCenter = Math.Max(goalY, yCenter - speed);
            }
            else if (goalY > yCenter)
            {
                yCenter = Math.Min(goalY, yCenter + speed);
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
