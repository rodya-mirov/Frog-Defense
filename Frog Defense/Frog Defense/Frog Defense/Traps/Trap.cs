using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frog_Defense.Enemies;

namespace Frog_Defense.Traps
{
    abstract class Trap
    {
        protected Arena arena;
        protected GameUpdater env;

        protected Trap(Arena arena, GameUpdater env)
        {
            this.arena = arena;
            this.env = env;
        }

        public abstract int Cost
        {
            get;
        }

        public abstract void Update(IEnumerable<Enemy> enemies);
        public abstract void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset);
    }
}
