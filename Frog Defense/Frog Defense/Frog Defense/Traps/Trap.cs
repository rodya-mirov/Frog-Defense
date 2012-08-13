using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frog_Defense.Enemies;

namespace Frog_Defense.Traps
{
    enum TrapType { NoType, SpikeTrap, GunTrap, DartTrap, Dig };

    abstract class Trap
    {
        protected ArenaManager env;

        protected Trap(ArenaManager env)
        {
            this.env = env;
        }

        public static void LoadContent()
        {
            SpikeTrap.LoadContent();
            GunTrap.LoadContent();
            DartTrap.LoadContent();
            Dig.LoadContent();
        }

        public override string ToString()
        {
            return Name + "\nCost: $" + Cost + "\n" + Description;
        }

        public abstract TrapType trapType { get; }

        public abstract String Name { get; }
        public abstract int Cost { get; }
        public abstract Texture2D PreviewTexture { get; }
        public abstract String Description { get; }

        public abstract void Update(IEnumerable<Enemy> enemies);
        public abstract void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset, bool paused);

        public const int PreviewWidth = 20;
        public const int PreviewHeight = 20;
    }
}
