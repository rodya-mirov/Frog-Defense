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
                return "Fires darts in a fixed direction." +
                    "\nThe darts poison what they hit.\n\nSuccessive hits replace existing" +
                    "\npoison effects completely.";
            }
        }

        public override int Cost
        {
            get { return 150; }
        }

        protected override float ProjectileDamage
        {
            get { return 4; }
        }

        #region IMAGE_STUFF
        //standard image stuff: Gun
        protected override int imageWidth { get { return 30; } }
        protected override int imageHeight { get { return 30; } }

        private const String upGunPath = "Images/Traps/DartTrapUp";
        private static Texture2D upGunTexture;
        protected override Texture2D UpTexture
        {
            get { return upGunTexture; }
        }

        private const String downGunPath = "Images/Traps/DartTrapDown";
        private static Texture2D downGunTexture;
        protected override Texture2D DownTexture
        {
            get { return downGunTexture; }
        }

        private const String rightGunPath = "Images/Traps/DartTrapRight";
        private static Texture2D rightGunTexture;
        protected override Texture2D RightTexture
        {
            get { return rightGunTexture; }
        }

        private const String leftGunPath = "Images/Traps/DartTrapLeft";
        private static Texture2D leftGunTexture;
        protected override Texture2D LeftTexture
        {
            get { return leftGunTexture; }
        }

        private const String previewGunPath = "Images/TrapPreviews/DartTrapPreview";
        private static Texture2D previewTexture;
        public override Texture2D PreviewTexture
        {
            get { return previewTexture; }
        }

        //standard image stuff: Bullet
        protected override int BulletImageWidth { get { return 2; } }
        protected override int BulletImageHeight { get { return 2; } }

        private const String bulletPath = "Images/Traps/Bullet";
        private static Texture2D bulletTexture;
        protected override Texture2D BulletTexture
        {
            get { return bulletTexture; }
        }
        #endregion

        #region POISON_STUFF
        /// <summary>
        /// The damage PER TICK of poison; this will happen roughly 60 times
        /// per second.
        /// </summary>
        public float PoisonDamage
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
        public DartTrap(ArenaManager env, int centerX, int centerY, Direction facing)
            : base(env, centerX, centerY, facing)
        {
        }

        /// <summary>
        /// Standard LoadContent method, gets all the textures up and ready.
        /// </summary>
        public static new void LoadContent()
        {
            if (upGunTexture == null)
                upGunTexture = TDGame.MainGame.Content.Load<Texture2D>(upGunPath);

            if (downGunTexture == null)
                downGunTexture = TDGame.MainGame.Content.Load<Texture2D>(downGunPath);

            if (leftGunTexture == null)
                leftGunTexture = TDGame.MainGame.Content.Load<Texture2D>(leftGunPath);

            if (rightGunTexture == null)
                rightGunTexture = TDGame.MainGame.Content.Load<Texture2D>(rightGunPath);

            if (bulletTexture == null)
                bulletTexture = TDGame.MainGame.Content.Load<Texture2D>(bulletPath);

            if (previewTexture == null)
                previewTexture = TDGame.MainGame.Content.Load<Texture2D>(previewGunPath);
        }

        protected override bool hitEnemy(Enemy e)
        {
            e.TakeHit(ProjectileDamage);
            e.GetPoisoned(PoisonDamage, PoisonDuration);

            return true;
        }
    }
}
