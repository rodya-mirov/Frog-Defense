using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Frog_Defense.Traps;

namespace Frog_Defense.Enemies
{
    abstract class Enemy
    {
        public bool Highlighted;

        /// <summary>
        /// The default number of ticks after this enemy and before the next one.
        /// </summary>
        public abstract int TicksAfterSpawn { get; }

        /// <summary>
        /// Whether or not this Enemy object has found its goal
        /// </summary>
        public abstract bool HasReachedGoal { get; }

        /// <summary>
        /// The cash value the player gets for killing a level 0 critter
        /// </summary>
        protected abstract float BaseCashValue { get; }

        public int CashValue
        {
            get { return (int)(BaseCashValue * Math.Log(scalingFactor + 1, 2)); }
        }

        /// <summary>
        /// Whether or not this Enemy is alive enough to keep on Updating
        /// </summary>
        public abstract bool IsAlive { get; }

        /// <summary>
        /// The center x-coordinate of the Enemy.
        /// </summary>
        public abstract float VisualXCenter { get; }

        /// <summary>
        /// The center y-coordinate of the enemy
        /// </summary>
        public abstract float VisualYCenter { get; }

        public abstract int PixelWidth { get; }
        public abstract int PixelHeight { get; }

        public abstract int CurrentSquareX { get; }
        public abstract int CurrentSquareY { get; }

        public abstract int NextSquareX { get; }
        public abstract int NextSquareY { get; }

        public Point NextSquare { get { return new Point(NextSquareX, NextSquareY); } }

        protected abstract float MAX_HEALTH { get; }
        protected abstract float Health { get; set; }

        protected ArenaMap arena;
        protected ArenaManager env;

        //preview images for WaveTracker
        public const int PreviewImageWidth = 20;
        public const int PreviewImageHeight = 20;

        public abstract Texture2D PreviewTexture { get; }

        //textures for health bars
        protected const int healthBarWidth = 30;
        protected const int healthBarHeight = 4;

        protected const String healthBarFullPath = "Images/Healthbars/FullBar";
        protected static Texture2D healthBarFullTexture;
        protected const String healthBarEmptyPath = "Images/Healthbars/EmptyBar";
        protected static Texture2D healthBarEmptyTexture;

        //for the poison drop
        protected const int poisonDropWidth = 8;
        protected const int poisonDropHeight = 8;

        protected const String poisonDropPath = "Images/EnemyEffects/PoisonDrop";
        protected static Texture2D poisonDropTexture;

        //stuff for getting poisoned
        protected bool isPoisoned;
        protected PoisonCounter poisonCounter;

        //stuff for scaling
        protected float scalingFactor;

        protected Enemy(ArenaManager env, ArenaMap arena, float scalingLevel)
        {
            this.Highlighted = false;

            this.env = env;
            this.arena = arena;

            this.isPoisoned = false;

            this.scalingFactor = scalingLevel;
        }

        public static void LoadContent()
        {
            loadEffects();

            BasicEnemy.LoadContent();
            ToughEnemy.LoadContent();
            QuickEnemy.LoadContent();
            ImmuneEnemy.LoadContent();
        }

        private static void loadEffects()
        {
            if (healthBarFullTexture == null)
                healthBarFullTexture = TDGame.MainGame.Content.Load<Texture2D>(healthBarFullPath);

            if (healthBarEmptyTexture == null)
                healthBarEmptyTexture = TDGame.MainGame.Content.Load<Texture2D>(healthBarEmptyPath);

            if (poisonDropTexture == null)
                poisonDropTexture = TDGame.MainGame.Content.Load<Texture2D>(poisonDropPath);
        }

        /// <summary>
        /// General Update method.  Does things that all enemies must do!
        /// Cannot be extended or overriden; specific behaviors should
        /// implement the update() method.
        /// </summary>
        public void Update()
        {
            update();

            takePoisonDamage();
        }

        /// <summary>
        /// This is how enemies will be allowed to extend their behavior-
        /// the main Update method calls this, then does all the default
        /// behavior (like taking poison damage).
        /// 
        /// The net effect is that it's impossible NOT to call base.Update,
        /// which is the point!
        /// </summary>
        protected virtual void update()
        {
        }

        /// <summary>
        /// Called by default in Enemy.Update(); extensions should be
        /// aware of this behavior.
        /// 
        /// Takes poison damage.  Default behavior is, if poisoned,
        /// to reduce health by poisonCounter.damagePerTick, then
        /// reduce ticksRemaining by 1.  If ticksRemaining reaches 0,
        /// then is no longer poisoned.
        /// 
        /// Method does nothing if not poisoned.
        /// </summary>
        protected virtual void takePoisonDamage()
        {
            if (isPoisoned)
            {
                Health -= poisonCounter.damagePerTick;
                poisonCounter.ticksRemaining--;

                if (poisonCounter.ticksRemaining <= 0)
                    isPoisoned = false;
            }
        }

        /// <summary>
        /// Deal with a new hit that does some intended damage ...
        /// </summary>
        /// <param name="damage"></param>
        public abstract void TakeHit(float damage);

        /// <summary>
        /// Adds a PoisonCounter with the associated information.
        /// If the ID is already in use, this will replace the poison
        /// with that ID, effectively resetting the timer.
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="duration"></param>
        /// <param name="pid"></param>
        public virtual void GetPoisoned(float damage, int duration)
        {
            poisonCounter = new PoisonCounter(damage, duration);
            isPoisoned = true;
        }

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
        public abstract void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset, bool paused);

        /// <summary>
        /// Draws a poison drop in the specified location
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="batch"></param>
        /// <param name="leftX"></param>
        /// <param name="topY"></param>
        /// <param name="paused"></param>
        protected virtual void DrawPoison(GameTime gameTime, SpriteBatch batch, float leftX, float topY, bool paused)
        {
            Vector2 drawPosition = new Vector2(leftX, topY);

            batch.Draw(poisonDropTexture, drawPosition, Color.White);
        }

        /// <summary>
        /// Determines whether building a wall here would cause any serious trouble
        /// </summary>
        /// <param name="squareX"></param>
        /// <param name="squareY"></param>
        /// <returns></returns>
        public abstract bool isInvolvedWithSquare(int squareX, int squareY);

        /// <summary>
        /// This draws the health bar of the enemy in question.  This can be called separately and
        /// should *not* be called during the Draw method of the Enemy.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="batch"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        public void DrawHealthBar(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset, bool paused)
        {
            Rectangle healthRect = new Rectangle(
                (int)VisualXCenter - PixelWidth / 2 + xOffset,
                (int)VisualYCenter - PixelHeight / 2 + yOffset - healthBarHeight - 1,
                healthBarWidth,
                healthBarHeight
                );

            //first, draw the empty bar
            batch.Draw(healthBarEmptyTexture, healthRect, Color.White);
            
            //then draw the full bar, as appropriate
            healthRect.Width = (int)((healthBarWidth * Health) / (MAX_HEALTH));

            batch.Draw(healthBarFullTexture, healthRect, Color.White);
        }

        public abstract void setPosition(int xCenter, int yCenter, int xSquare, int ySquare);

        /// <summary>
        /// Determines whether a ray from the specified point, traveling
        /// along dir, would touch this enemy
        /// </summary>
        /// <param name="xOrigin"></param>
        /// <param name="yOrigin"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        public abstract bool touchesRay(int xOrigin, int yOrigin, Direction dir);

        /// <summary>
        /// Shifts the given enemy by the specified amount, given in both
        /// squares and pixels.  The two are guaranteed to be equivalent measures.
        /// </summary>
        /// <param name="xChange"></param>
        /// <param name="yChange"></param>
        /// <param name="xSquaresChange"></param>
        /// <param name="ySquaresChange"></param>
        public abstract void shift(int xChange, int yChange, int xSquaresChange, int ySquaresChange);
    }

    struct PoisonCounter
    {
        public float damagePerTick;
        public int ticksRemaining;

        public PoisonCounter(float damagePerTick, int ticksRemaining)
        {
            this.damagePerTick = damagePerTick;
            this.ticksRemaining = ticksRemaining;
        }
    }
}
