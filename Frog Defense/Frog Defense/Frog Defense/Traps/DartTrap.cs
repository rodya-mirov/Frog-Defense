using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Frog_Defense.Enemies;

namespace Frog_Defense.Traps
{
    class DartTrap : WallProjectileTrap
    {
        //string stuff
        public override string SelfString()
        {
            string output = base.SelfString();

            if (CanUpgrade)
            {
                output += "\n\nPoison Duration: " + (PoisonDuration / 60) + " s";
                output += "\nTotal Poison: " + (int)(poisonDamage * PoisonDuration) + " -> " + (int)(nextPoisonDamage * PoisonDuration);
            }
            else
            {
                output += "\n\nPoison Duration: " + (PoisonDuration / 60) + " s";
                output += "\nTotal Poison: " + (int)(poisonDamage * PoisonDuration);
            }

            return output;
        }

        public override string BuyString()
        {
            string output = base.SelfString();

            output += "\n\nPoison Duration: " + (PoisonDuration / 60) + " s";
            output += "\nTotal Poison Damage: " + (int)(poisonDamage * PoisonDuration);

            return output;
        }

        //basic properties
        public override TrapType trapType
        {
            get { return TrapType.DartTrap; }
        }
        public override string Name
        {
            get { return "Dart Launcher"; }
        }
        public override string Description
        {
            get
            {
                return "Fires poisonous darts." +
                    "\n\nSuccessive hits reset the" +
                    "\npoison's duration.";
            }
        }

        public override int Cost { get { return 150; } }
        protected override float BaseProjectileDamage { get { return 4; } }

        #region IMAGE_STUFF
        //standard image stuff: Gun
        protected override int imageWidth { get { return 30; } }
        protected override int imageHeight { get { return 30; } }

        private const String upPath = "Images/Traps/DartTrap/DartTrapUp";
        private static Texture2D upTexture;
        protected override Texture2D UpTexture { get { return upTexture; } }

        private const String downPath = "Images/Traps/DartTrap/DartTrapDown";
        private static Texture2D downTexture;
        protected override Texture2D DownTexture { get { return downTexture; } }

        private const String rightPath = "Images/Traps/DartTrap/DartTrapRight";
        private static Texture2D rightTexture;
        protected override Texture2D RightTexture { get { return rightTexture; } }

        private const String leftPath = "Images/Traps/DartTrap/DartTrapLeft";
        private static Texture2D leftTexture;
        protected override Texture2D LeftTexture { get { return leftTexture; } }

        private const String upHighlightPath = "Images/Traps/DartTrap/HighlightUp";
        private static Texture2D upHighlightTexture;
        protected override Texture2D UpHighlightTexture { get { return upHighlightTexture; } }

        private const String downHighlightPath = "Images/Traps/DartTrap/HighlightDown";
        private static Texture2D downHighlightTexture;
        protected override Texture2D DownHighlightTexture { get { return downHighlightTexture; } }

        private const String rightHighlightPath = "Images/Traps/DartTrap/HighlightRight";
        private static Texture2D rightHighlightTexture;
        protected override Texture2D RightHighlightTexture { get { return rightHighlightTexture; } }

        private const String leftHighlightPath = "Images/Traps/DartTrap/HighlightLeft";
        private static Texture2D leftHighlightTexture;
        protected override Texture2D LeftHighlightTexture { get { return leftHighlightTexture; } }

        private const String previewPath = "Images/TrapPreviews/DartTrapPreview";
        private static Texture2D previewTexture;
        public override Texture2D PreviewTexture { get { return previewTexture; } }

        //standard image stuff: Bullet
        protected override int BulletImageWidth { get { return 2; } }
        protected override int BulletImageHeight { get { return 2; } }

        private const String bulletPath = "Images/Traps/DartTrap/Bullet";
        private static Texture2D bulletTexture;
        protected override Texture2D BulletTexture
        {
            get { return bulletTexture; }
        }
        #endregion


        //scaling factors
        protected override float projectileDamageScalingFactor { get { return 1; } }
        protected float PoisonDamageScalingFactor { get { return 1.5f; } }

        protected override void upgradeStats()
        {
            base.upgradeStats();

            poisonDamage = nextPoisonDamage;
            nextPoisonDamage *= PoisonDamageScalingFactor;
        }

        #region POISON_STUFF

        private float poisonDamage, nextPoisonDamage;

        protected float BasePoisonDamage
        {
            get { return .1f; }
        }

        /// <summary>
        /// This is the number of FRAMES the poison should last,
        /// where the frames per second is roughly 60.
        /// </summary>
        public int PoisonDuration
        {
            get { return 300; }
        }
        #endregion

        /// <summary>
        /// Creates a new GunTrap at the specified location
        /// </summary>
        /// <param name="arena"></param>
        /// <param name="env"></param>
        /// <param name="centerX">The centerX of the gun</param>
        /// <param name="centerY">The centerY of the gun</param>
        /// <param name="facing"></param>
        public DartTrap(ArenaManager env, int centerX, int centerY, int xSquare, int ySquare, Direction facing)
            : base(env, centerX, centerY, xSquare, ySquare, facing)
        {
            this.poisonDamage = BasePoisonDamage;
            this.nextPoisonDamage = poisonDamage * PoisonDamageScalingFactor;
        }

        /// <summary>
        /// Standard LoadContent method, gets all the textures up and ready.
        /// </summary>
        public static new void LoadContent()
        {
            //regular textures
            if (upTexture == null)
                upTexture = TDGame.MainGame.Content.Load<Texture2D>(upPath);

            if (downTexture == null)
                downTexture = TDGame.MainGame.Content.Load<Texture2D>(downPath);

            if (leftTexture == null)
                leftTexture = TDGame.MainGame.Content.Load<Texture2D>(leftPath);

            if (rightTexture == null)
                rightTexture = TDGame.MainGame.Content.Load<Texture2D>(rightPath);

            //highlight textures
            if (upHighlightTexture == null)
                upHighlightTexture = TDGame.MainGame.Content.Load<Texture2D>(upHighlightPath);

            if (downHighlightTexture == null)
                downHighlightTexture = TDGame.MainGame.Content.Load<Texture2D>(downHighlightPath);

            if (leftHighlightTexture == null)
                leftHighlightTexture = TDGame.MainGame.Content.Load<Texture2D>(leftHighlightPath);

            if (rightHighlightTexture == null)
                rightHighlightTexture = TDGame.MainGame.Content.Load<Texture2D>(rightHighlightPath);

            //others
            if (bulletTexture == null)
                bulletTexture = TDGame.MainGame.Content.Load<Texture2D>(bulletPath);

            if (previewTexture == null)
                previewTexture = TDGame.MainGame.Content.Load<Texture2D>(previewPath);
        }

        protected override bool hitEnemy(Enemy e)
        {
            e.TakeHit(ProjectileDamage);
            e.GetPoisoned(poisonDamage, PoisonDuration);

            return true;
        }
    }
}
