using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Frog_Defense.Enemies;

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
        private const float damagePerTick = 1.5f;

        //Typical graphics stuff
        private const int mainImageWidth = 40;
        private const int mainImageHeight = 40;

        private const String imagePath = "Images/Traps/Spikes";
        private static Texture2D imageTexture;

        private const String previewImagePath = "Images/TrapPreviews/SpikesPreview";
        private static Texture2D previewTexture;

        public override Texture2D PreviewTexture
        {
            get { return previewTexture; }
        }

        public override TrapType trapType
        {
            get { return TrapType.SpikeTrap; }
        }

        public override string Name
        {
            get { return "Spike Trap"; }
        }

        public override string Description
        {
            get { return "Deals continuous damage to all\nenemies standing on it."; }
        }

        /// <summary>
        /// The cost, in "dollars," of placing a single SpikeTrap
        /// </summary>
        public override int Cost
        {
            get { return 100; }
        }

        public SpikeTrap(Arena arena, GameUpdater env, int x, int y)
            : base(arena, env)
        {
            this.xPos = x;
            this.yPos = y;
        }

        public static void LoadContent()
        {
            if (imageTexture == null)
                imageTexture = TDGame.MainGame.Content.Load<Texture2D>(imagePath);

            if (previewTexture == null)
                previewTexture = TDGame.MainGame.Content.Load<Texture2D>(previewImagePath);
        }

        /// <summary>
        /// Hurts all enemies whose position is contained in the zone of control of
        /// this trap.
        /// </summary>
        /// <param name="enemies">The collection of enemies to possibly hurt.</param>
        public override void Update(IEnumerable<Enemy> enemies)
        {
            int minX = xPos - mainImageWidth / 2;
            int maxX = xPos + mainImageWidth / 2;

            int minY = yPos - mainImageHeight / 2;
            int maxY = yPos + mainImageHeight / 2;

            foreach (Enemy e in enemies)
            {
                if (e.XCenter >= minX && e.XCenter <= maxX && e.YCenter >= minY && e.YCenter <= maxY)
                    e.TakeHit(damagePerTick);
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset)
        {
            batch.Draw(
                imageTexture,
                new Vector2(
                    xPos + xOffset - mainImageWidth / 2,
                    yPos + yOffset - mainImageHeight / 2
                    ),
                Color.White
                );
        }
    }
}
