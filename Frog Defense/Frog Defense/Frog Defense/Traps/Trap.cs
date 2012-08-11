using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frog_Defense.Enemies;

namespace Frog_Defense.Traps
{
    enum TrapType { NoType, SpikeTrap, GunTrap };

    abstract class Trap
    {
        protected Arena arena;
        protected GameUpdater env;

        protected Trap(Arena arena, GameUpdater env)
        {
            this.arena = arena;
            this.env = env;
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
        public abstract void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset);

        public const int PreviewWidth = 20;
        public const int PreviewHeight = 20;
    }
}
