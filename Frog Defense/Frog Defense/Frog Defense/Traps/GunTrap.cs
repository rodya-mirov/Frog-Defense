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
            get { return "Fires lots of little bullets."; }
        }

        public override int Cost { get { return 100; } }
        protected override float ProjectileDamage { get { return 10; } }

        //standard image stuff: Gun
        protected override int imageWidth { get { return 30; } }
        protected override int imageHeight { get { return 30; } }

        private const String upPath = "Images/Traps/GunTrap/GunTrapUp";
        private static Texture2D upTexture;
        protected override Texture2D UpTexture { get { return upTexture; } }

        private const String downPath = "Images/Traps/GunTrap/GunTrapDown";
        private static Texture2D downTexture;
        protected override Texture2D DownTexture { get { return downTexture; } }

        private const String rightPath = "Images/Traps/GunTrap/GunTrapRight";
        private static Texture2D rightTexture;
        protected override Texture2D RightTexture { get { return rightTexture; } }

        private const String leftPath = "Images/Traps/GunTrap/GunTrapLeft";
        private static Texture2D leftTexture;
        protected override Texture2D LeftTexture { get { return leftTexture; } }

        private const String upHighlightPath = "Images/Traps/GunTrap/HighlightUp";
        private static Texture2D upHighlightTexture;
        protected override Texture2D UpHighlightTexture { get { return upHighlightTexture; } }

        private const String downHighlightPath = "Images/Traps/GunTrap/HighlightDown";
        private static Texture2D downHighlightTexture;
        protected override Texture2D DownHighlightTexture { get { return downHighlightTexture; } }

        private const String rightHighlightPath = "Images/Traps/GunTrap/HighlightRight";
        private static Texture2D rightHighlightTexture;
        protected override Texture2D RightHighlightTexture { get { return rightHighlightTexture; } }

        private const String leftHighlightPath = "Images/Traps/GunTrap/HighlightLeft";
        private static Texture2D leftHighlightTexture;
        protected override Texture2D LeftHighlightTexture { get { return leftHighlightTexture; } }

        private const String previewPath = "Images/TrapPreviews/GunTrapPreview";
        private static Texture2D previewTexture;
        public override Texture2D PreviewTexture { get { return previewTexture; } }

        //standard image stuff: Bullet
        protected override int BulletImageWidth { get { return 2; } }
        protected override int BulletImageHeight { get { return 2; } }

        private const String bulletPath = "Images/Traps/GunTrap/Bullet";
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
        public GunTrap(ArenaManager env, int centerX, int centerY, int xSquare, int ySquare, Direction facing)
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
    }
}
