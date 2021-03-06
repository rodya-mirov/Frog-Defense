﻿using System;
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

        public override float VisualXCenter { get { return xCenter; } }
        public override float VisualYCenter { get { return yCenter; } }

        public override string BuyString()
        {
            string output = base.BuyString();

            output += "\n\nDamage/sec: " + (int)(damagePerTick * 60);

            return output;
        }

        public override string SelfString()
        {
            string output = base.SelfString();

            if (CanUpgrade)
            {
                output += "\n\nDamage/sec: " + (int)(damagePerTick * 60) + " -> " + (int)(nextDamagePerTick * 60);
                output += "\nSlowing: " + String.Format("{0:0.00}", slowFactor) + " -> " + String.Format("{0:0.00}", nextSlowFactor);
            }
            else
            {
                output += "\n\nDamage/sec: " + (int)(damagePerTick * 60);
                output += "\nSlowing: " + String.Format("{0:0.00}", slowFactor);
            }

            return output;
        }

        protected override void upgradeStats()
        {
            damagePerTick = nextDamagePerTick;
            nextDamagePerTick *= upgradeDamageFactor;

            slowFactor = nextSlowFactor;
            nextSlowFactor *= slowUpgradeFactor;
        }

        public override TrapLocationType LocationType { get { return TrapLocationType.Floor; } }

        //the damage this trap inflicts on every critter that touches it
        //creatures are hit for roughly mainImageWidth ticks, so, for balancing ...
        private const float baseDamagePerTick = .5f;
        private const float upgradeDamageFactor = 1.2f;
        private float damagePerTick, nextDamagePerTick;

        private float slowFactor, nextSlowFactor;
        private const float slowUpgradeFactor = .85f;
        private const float baseSlowFactor = .7f;

        //Typical graphics stuff
        private const int mainImageWidth = 30;
        private const int mainImageHeight = 30;

        protected Texture2D DrawTexture
        {
            get
            {
                if (Highlighted)
                    return highlightTexture;
                else
                    return imageTexture;
            }
        }

        private const String imagePath = "Images/Traps/SpikeTrap/Spikes";
        private static Texture2D imageTexture;

        private const String highlightPath = "Images/Traps/SpikeTrap/Highlight";
        private static Texture2D highlightTexture;

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
            get { return "Deals continuous damage to\nall enemies standing on it.\nAlso slows enemies while\nthey are on it."; }
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
            this.damagePerTick = baseDamagePerTick;
            this.nextDamagePerTick = damagePerTick * upgradeDamageFactor;

            this.slowFactor = baseSlowFactor;
            this.nextSlowFactor = this.slowFactor * slowUpgradeFactor;

            this.xCenter = xCenter;
            this.yCenter = yCenter;
        }

        public static new void LoadContent()
        {
            if (imageTexture == null)
                imageTexture = TDGame.MainGame.Content.Load<Texture2D>(imagePath);

            if (highlightTexture == null)
                highlightTexture = TDGame.MainGame.Content.Load<Texture2D>(highlightPath);

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
                if (e.VisualXCenter >= minX && e.VisualXCenter <= maxX && e.VisualYCenter >= minY && e.VisualYCenter <= maxY)
                {
                    e.TakeHit(damagePerTick, this);
                    e.Slow(slowFactor);
                }
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
                DrawTexture,
                new Vector2(
                    xCenter + xOffset - mainImageWidth / 2,
                    yCenter + yOffset - mainImageHeight / 2
                    ),
                Color.White
                );
        }
    }
}
