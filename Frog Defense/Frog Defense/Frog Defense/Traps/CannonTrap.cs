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
        protected override float BaseProjectileDamage { get { return 20; } }
        protected override int ReloadFrames { get { return 20; } }

        //standard image stuff: Gun
        protected override int imageWidth { get { return 30; } }
        protected override int imageHeight { get { return 30; } }

        private const String upPath = "Images/Traps/CannonTrap/CannonTrapUp";
        private static Texture2D upTexture;
        protected override Texture2D UpTexture { get { return upTexture; } }

        private const String downPath = "Images/Traps/CannonTrap/CannonTrapDown";
        private static Texture2D downTexture;
        protected override Texture2D DownTexture { get { return downTexture; } }

        private const String rightPath = "Images/Traps/CannonTrap/CannonTrapRight";
        private static Texture2D rightTexture;
        protected override Texture2D RightTexture { get { return rightTexture; } }

        private const String leftPath = "Images/Traps/CannonTrap/CannonTrapLeft";
        private static Texture2D leftTexture;
        protected override Texture2D LeftTexture { get { return leftTexture; } }

        private const String upHighlightPath = "Images/Traps/CannonTrap/HighlightUp";
        private static Texture2D upHighlightTexture;
        protected override Texture2D UpHighlightTexture { get { return upHighlightTexture; } }

        private const String downHighlightPath = "Images/Traps/CannonTrap/HighlightDown";
        private static Texture2D downHighlightTexture;
        protected override Texture2D DownHighlightTexture { get { return downHighlightTexture; } }

        private const String rightHighlightPath = "Images/Traps/CannonTrap/HighlightRight";
        private static Texture2D rightHighlightTexture;
        protected override Texture2D RightHighlightTexture { get { return rightHighlightTexture; } }

        private const String leftHighlightPath = "Images/Traps/CannonTrap/HighlightLeft";
        private static Texture2D leftHighlightTexture;
        protected override Texture2D LeftHighlightTexture { get { return leftHighlightTexture; } }

        private const String previewPath = "Images/TrapPreviews/CannonPreview";
        private static Texture2D previewTexture;
        public override Texture2D PreviewTexture { get { return previewTexture; } }

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

        /// <summary>
        /// Don't just hit them.  Hit them hard!
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        protected override bool hitEnemy(Enemy e)
        {
            e.TakeHit(ProjectileDamage, this);
            return true;
        }
    }
}
