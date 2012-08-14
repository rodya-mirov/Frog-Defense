using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frog_Defense.Enemies;

namespace Frog_Defense.Traps
{
    enum TrapType { NoType, SpikeTrap, GunTrap, DartTrap, MineWall, Wall, CannonTrap, DigPit };

    abstract class Trap
    {
        protected ArenaManager env;
        protected int floorSquareX, floorSquareY;

        protected Trap(ArenaManager env, int floorSquareX, int floorSquareY)
        {
            this.env = env;

            this.floorSquareX = floorSquareX;
            this.floorSquareY = floorSquareY;
        }

        public static void LoadContent()
        {
            SpikeTrap.LoadContent();

            GunTrap.LoadContent();
            DartTrap.LoadContent();
            CannonTrap.LoadContent();

            MineWall.LoadContent();
            DigPit.LoadContent();
            Build.LoadContent();
        }

        public string BuyString()
        {
            return Name + "\nCost: $" + Cost + "\n" + Description;
        }

        public string SelfString()
        {
            return Name + "\nSell Price: $" + SellPrice + "\n" + Description;
        }

        public abstract TrapType trapType { get; }

        public abstract String Name { get; }
        public abstract int Cost { get; }
        public virtual int SellPrice { get { return Cost / 2; } }

        public abstract Texture2D PreviewTexture { get; }
        public abstract String Description { get; }

        public abstract void Update(IEnumerable<Enemy> enemies);
        public abstract void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset, bool paused);

        public const int PreviewWidth = 20;
        public const int PreviewHeight = 20;

        /// <summary>
        /// Shifts the given enemy by the specified amount, given in both
        /// squares and pixels.  The two are guaranteed to be equivalent measures.
        /// 
        /// Make sure to call base.shift !
        /// </summary>
        /// <param name="xChange"></param>
        /// <param name="yChange"></param>
        /// <param name="xSquaresChange"></param>
        /// <param name="ySquaresChange"></param>
        public virtual void shift(int xChange, int yChange, int xSquaresChange, int ySquaresChange)
        {
            this.floorSquareX += xSquaresChange;
            this.floorSquareY += ySquaresChange;
        }

        /// <summary>
        /// Returns true iff this is a wall trap and attached to a wall
        /// with square coordinates (x, y).  All non-wall-traps will uniformly
        /// return false.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public virtual bool touchesWall(int x, int y)
        {
            return false;
        }

        /// <summary>
        /// Determines whether a specific trap is using the square
        /// supplied.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool isOnSquare(int x, int y)
        {
            return floorSquareX == x && floorSquareY == y;
        }
    }
}
