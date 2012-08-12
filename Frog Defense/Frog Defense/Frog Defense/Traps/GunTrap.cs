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
    /// This trap is affixed to the center of a wall, and fires at the closest target.
    /// It hits that target for flat damage.
    /// </summary>
    class GunTrap : WallProjectileTrap
    {
        public override TrapType trapType
        {
            get { return TrapType.GunTrap; }
        }
        public override string Name
        {
            get { return "Pellet Gun"; }
        }
        public override string Description
        {
            get { return "Fires bullets in a fixed direction."; }
        }

        public override int Cost
        {
            get { return 100; }
        }

        protected override float ProjectileDamage
        {
            get { return 10; }
        }

        //standard image stuff: Gun
        protected override int imageWidth { get { return 30; } }
        protected override int imageHeight { get { return 30; } }

        private const String upGunPath = "Images/Traps/GunTrapUp";
        private static Texture2D upGunTexture;
        protected override Texture2D UpTexture
        {
            get { return upGunTexture; }
        }

        private const String downGunPath = "Images/Traps/GunTrapDown";
        private static Texture2D downGunTexture;
        protected override Texture2D DownTexture
        {
            get { return downGunTexture; }
        }

        private const String rightGunPath = "Images/Traps/GunTrapRight";
        private static Texture2D rightGunTexture;
        protected override Texture2D RightTexture
        {
            get { return rightGunTexture; }
        }

        private const String leftGunPath = "Images/Traps/GunTrapLeft";
        private static Texture2D leftGunTexture;
        protected override Texture2D LeftTexture
        {
            get { return leftGunTexture; }
        }

        private const String previewGunPath = "Images/TrapPreviews/GunTrapPreview";
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

        /// <summary>
        /// Creates a new GunTrap at the specified location
        /// </summary>
        /// <param name="arena"></param>
        /// <param name="env"></param>
        /// <param name="centerX">The centerX of the gun</param>
        /// <param name="centerY">The centerY of the gun</param>
        /// <param name="facing"></param>
        public GunTrap(ArenaManager env, int centerX, int centerY, Direction facing)
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
    }
}
