using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frog_Defense.Enemies
{
    abstract class Enemy
    {
        /// <summary>
        /// Whether or not this Enemy object has found its goal
        /// </summary>
        public abstract bool HasReachedGoal { get; }

        /// <summary>
        /// The cash value the player gets for killing this critter
        /// </summary>
        public abstract int CashValue { get; }

        /// <summary>
        /// Whether or not this Enemy is alive enough to keep on Updating
        /// </summary>
        public abstract bool IsAlive { get; }

        /// <summary>
        /// The center x-coordinate of the Enemy.
        /// </summary>
        public abstract int XCenter { get; }

        /// <summary>
        /// The center y-coordinate of the enemy
        /// </summary>
        public abstract int YCenter { get; }

        public abstract int PixelWidth { get; }
        public abstract int PixelHeight { get; }

        protected abstract float MAX_HEALTH { get; }
        protected abstract float Health { get; }

        protected Arena arena;
        protected EnvironmentUpdater env;

        //textures for health bars
        protected const int healthBarWidth = 30;
        protected const int healthBarHeight = 4;

        protected const String healthBarFullPath = "Images/Healthbars/FullBar";
        protected static Texture2D healthBarFullTexture;
        protected const String healthBarEmptyPath = "Images/Healthbars/EmptyBar";
        protected static Texture2D healthBarEmptyTexture;

        protected Enemy(EnvironmentUpdater env, Arena arena)
        {
            this.env = env;
            this.arena = arena;
        }

        public static void LoadContent()
        {
            if (healthBarFullTexture == null)
                healthBarFullTexture = TDGame.MainGame.Content.Load<Texture2D>(healthBarFullPath);

            if (healthBarEmptyTexture == null)
                healthBarEmptyTexture = TDGame.MainGame.Content.Load<Texture2D>(healthBarEmptyPath);
        }

        /// <summary>
        /// General Update method
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// What it sounds like!
        /// </summary>
        /// <param name="damage"></param>
        public abstract void TakeHit(float damage);

        /// <summary>
        /// Whether or not a specified point (relative to the Arena)
        /// is consider "inside" this Enemy, for hit detection purposes.
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public abstract bool ContainsPoint(Point p);

        /// <summary>
        /// Basic draw method
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="batch"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        public abstract void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset);

        /// <summary>
        /// This draws the health bar of the enemy in question.  This can be called separately and
        /// should *not* be called during the Draw method of the Enemy.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="batch"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        public void DrawHealthBar(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset)
        {
            Rectangle healthRect = new Rectangle(
                XCenter - PixelWidth / 2 + xOffset,
                YCenter - PixelHeight / 2 + yOffset - healthBarHeight - 1,
                healthBarWidth,
                healthBarHeight
                );

            //first, draw the empty bar
            batch.Draw(healthBarEmptyTexture, healthRect, Color.White);
            
            //then draw the full bar, as appropriate
            healthRect.Width = (int)((healthBarWidth * Health) / MAX_HEALTH);

            batch.Draw(healthBarFullTexture, healthRect, Color.White);
        }
    }
}
