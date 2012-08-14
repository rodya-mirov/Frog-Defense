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
    /// 
    /// Damage-wise, the idea of spikes should be that guns are better damage, but
    /// if there's no good place for a gun, you can put spikes down for an OK substitute.
    /// </summary>
    class SpikeTrap : Trap
    {
        private int xCenter, yCenter;

        //the damage this trap inflicts on every critter that touches it
        //creatures are hit for roughly mainImageWidth ticks, so, for balancing ...
        private const float damagePerTick = .5f;

        //Typical graphics stuff
        private const int mainImageWidth = 30;
        private const int mainImageHeight = 30;

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
            get { return "Deals continuous damage to\nall enemies standing on it."; }
        }

        /// <summary>
        /// The cost, in "dollars," of placing a single SpikeTrap
        /// </summary>
        public override int Cost
        {
            get { return 100; }
        }

        public SpikeTrap(ArenaManager env, int xCenter, int yCenter, int floorSquareX, int floorSquareY)
            : base(env, floorSquareX, floorSquareY)
        {
            this.xCenter = xCenter;
            this.yCenter = yCenter;
        }

        public static new void LoadContent()
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
            int minX = xCenter - mainImageWidth / 2;
            int maxX = xCenter + mainImageWidth / 2;

            int minY = yCenter - mainImageHeight / 2;
            int maxY = yCenter + mainImageHeight / 2;

            foreach (Enemy e in enemies)
            {
                if (e.XCenter >= minX && e.XCenter <= maxX && e.YCenter >= minY && e.YCenter <= maxY)
                    e.TakeHit(damagePerTick);
            }
        }

        public override void shift(int xChange, int yChange, int xSquaresChange, int ySquaresChange)
        {
            base.shift(xChange, yChange, xSquaresChange, ySquaresChange);

            xCenter += xChange;
            yCenter += yChange;
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset, bool paused)
        {
            batch.Draw(
                imageTexture,
                new Vector2(
                    xCenter + xOffset - mainImageWidth / 2,
                    yCenter + yOffset - mainImageHeight / 2
                    ),
                Color.White
                );
        }
    }
}
