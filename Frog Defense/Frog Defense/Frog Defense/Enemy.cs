using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frog_Defense
{
    class Enemy
    {
        private Arena arena;
        private EnvironmentUpdater env;

        //Health info
        private const float MAX_HEALTH = 100;
        private float health;

        //We're using ints for position because XNA gets glitchy when we draw on floats
        private int speed = 1;

        private int xPos, yPos;
        public int XPos
        {
            get { return xPos; }
        }
        public int YPos
        {
            get { return yPos; }
        }

        private int goalX, goalY;
        private bool hasGoal;

        //graphics stuff, the usual
        private const int imageWidth = 30;
        private const int imageHeight = 30;

        private const String imagePath = "Images/Enemies/Enemy";
        private static Texture2D imageTexture;

        public Enemy(Arena arena, EnvironmentUpdater env, int startX, int startY)
        {
            this.arena = arena;
            this.env = env;

            this.xPos = startX;
            this.yPos = startY;

            this.health = MAX_HEALTH;

            hasGoal = false;
        }

        public bool IsAlive()
        {
            return health > 0;
        }

        private const int hitFlashFrames = 3;
        private int framesSinceHit = 3;

        public bool containsPoint(Point p)
        {
            return (xPos - imageWidth / 2 <= p.X && p.X <= xPos + imageWidth / 2 && yPos - imageHeight / 2 <= p.Y && p.Y <= yPos + imageHeight / 2);
        }

        /// <summary>
        /// Reduces health by the specified amount and flashes red.  Note there
        /// is no invincibility frame!
        /// </summary>
        /// <param name="damage"></param>
        public void takeHit(float damage)
        {
            health -= damage;
            framesSinceHit = 0;
        }

        /// <summary>
        /// Loads up the textures if they aren't already.  Does nothing if the
        /// textures are already loaded.
        /// </summary>
        public static void LoadContent()
        {
            if (imageTexture == null)
                imageTexture = TDGame.MainGame.Content.Load<Texture2D>(imagePath);
        }

        //some parameters to slow the movement down
        private int framesBetweenMoves = 1;
        private int framesSinceMove = 0;

        /// <summary>
        /// Moves!
        /// </summary>
        public void Update()
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
                Point goal = arena.nextWayPoint(xPos, yPos);

                goalX = goal.X;
                goalY = goal.Y;

                hasGoal = true;
            }
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

            if (goalX < xPos)
            {
                xPos = Math.Max(goalX, xPos - speed);
            }
            else if (goalX > xPos)
            {
                xPos = Math.Min(goalX, xPos + speed);
            }
            if (goalY < yPos)
            {
                yPos = Math.Max(goalY, yPos - speed);
            }
            else if (goalY > yPos)
            {
                yPos = Math.Min(goalY, yPos + speed);
            }

            if (xPos == goalX && yPos == goalY)
                hasGoal = false;
        }

        public void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset)
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
                imageTexture,
                new Vector2(
                    xPos + xOffset - imageWidth / 2,
                    yPos + yOffset - imageHeight / 2
                    ),
                tint
                );
        }
    }
}
