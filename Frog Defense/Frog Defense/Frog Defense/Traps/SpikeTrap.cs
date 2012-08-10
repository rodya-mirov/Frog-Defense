using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frog_Defense.Traps
{
    /// <summary>
    /// This trap lies on the floor and passively hurts everything that walks across it.
    /// There is no reload time, no motion, etc.
    /// </summary>
    class SpikeTrap : Trap
    {
        private int xPos, yPos;

        //the damage this trap inflicts on every critter that touches it
        private const float damagePerTick = 0.3f;

        //Typical graphics stuff
        private const int imageWidth = 40;
        private const int imageHeight = 40;
        private const String imagePath = "Images/Traps/Spikes";
        private static Texture2D imageTexture;

        public SpikeTrap(Arena arena, EnvironmentUpdater env, int x, int y)
            : base(arena, env)
        {
            this.xPos = x;
            this.yPos = y;
        }

        public static void LoadContent()
        {
            if (imageTexture == null)
                imageTexture = TDGame.MainGame.Content.Load<Texture2D>(imagePath);
        }

        /// <summary>
        /// Hurts all enemies whose position is contained in the zone of control of
        /// this trap.
        /// </summary>
        /// <param name="enemies">The collection of enemies to possibly hurt.</param>
        public override void Update(IEnumerable<Enemy> enemies)
        {
            int minX = xPos - imageWidth / 2;
            int maxX = xPos + imageWidth / 2;

            int minY = yPos - imageHeight / 2;
            int maxY = yPos + imageHeight / 2;

            foreach (Enemy e in enemies)
            {
                if (e.XPos >= minX && e.XPos <= maxX && e.YPos >= minY && e.YPos <= maxY)
                    e.takeHit(damagePerTick);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset)
        {
            batch.Draw(
                imageTexture,
                new Vector2(
                    xPos + xOffset - imageWidth / 2,
                    yPos + yOffset - imageHeight / 2
                    ),
                Color.White
                );
        }
    }
}
