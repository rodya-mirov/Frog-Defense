using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Frog_Defense.Enemies;
using Microsoft.Xna.Framework;

namespace Frog_Defense.Traps
{
    class CannonTrap : WallProjectileTrap
    {
        public override TrapType trapType
        {
            get { return TrapType.CannonTrap; }
        }
        public override string Name
        {
            get { return "Cannon"; }
        }
        public override string Description
        {
            get { return "Fires large cannonballs."; }
        }

        public override int Cost { get { return 200; } }
        protected override float ProjectileDamage { get { return 20; } }
        protected override int ReloadFrames { get { return 20; } }

        //standard image stuff: Gun
        protected override int imageWidth { get { return 30; } }
        protected override int imageHeight { get { return 30; } }

        private const String upGunPath = "Images/Traps/CannonTrap/CannonTrapUp";
        private static Texture2D upGunTexture;
        protected override Texture2D UpTexture
        {
            get { return upGunTexture; }
        }

        private const String downGunPath = "Images/Traps/CannonTrap/CannonTrapDown";
        private static Texture2D downGunTexture;
        protected override Texture2D DownTexture
        {
            get { return downGunTexture; }
        }

        private const String rightGunPath = "Images/Traps/CannonTrap/CannonTrapRight";
        private static Texture2D rightGunTexture;
        protected override Texture2D RightTexture
        {
            get { return rightGunTexture; }
        }

        private const String leftGunPath = "Images/Traps/CannonTrap/CannonTrapLeft";
        private static Texture2D leftGunTexture;
        protected override Texture2D LeftTexture
        {
            get { return leftGunTexture; }
        }

        private const String previewGunPath = "Images/TrapPreviews/CannonPreview";
        private static Texture2D previewTexture;
        public override Texture2D PreviewTexture
        {
            get { return previewTexture; }
        }

        //standard image stuff: Bullet
        protected override int BulletImageWidth { get { return 10; } }
        protected override int BulletImageHeight { get { return 10; } }

        private const String bulletPath = "Images/Traps/CannonTrap/CannonBall";
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
        public CannonTrap(ArenaManager env, int centerX, int centerY, int xSquare, int ySquare, Direction facing)
            : base(env, centerX, centerY, xSquare, ySquare, facing)
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

        /// <summary>
        /// Don't just hit them.  Hit them hard!
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected override bool hitEnemy(Enemy e)
        {
            e.TakeHit(ProjectileDamage);
            return true;
        }
    }
}
