using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Frog_Defense.Enemies
{
    class QuickEnemy : BasicEnemy
    {
        protected override string EnemyType { get { return "Quick Enemy"; } }
        protected override string Description { get { return "Faster than normal."; } }

        protected override float BaseCashValue
        {
            get { return 15; }
        }

        protected override float Speed
        {
            get { return 2.5f; }
        }
        public override int TicksAfterSpawn { get { return 45; } }

        private const String previewPath = "Images/Enemies/QuickEnemy/Preview";
        private static Texture2D previewTexture;
        public override Texture2D PreviewTexture
        {
            get { return previewTexture; }
        }

        private const String leftPath = "Images/Enemies/QuickEnemy/Left";
        private const String rightPath = "Images/Enemies/QuickEnemy/Right";
        private const String upPath = "Images/Enemies/QuickEnemy/Up";
        private const String downPath = "Images/Enemies/QuickEnemy/Down";

        private static Texture2D leftTexture;
        private static Texture2D rightTexture;
        private static Texture2D upTexture;
        private static Texture2D downTexture;

        protected override Texture2D LeftTexture { get { return leftTexture; } }
        protected override Texture2D RightTexture { get { return rightTexture; } }
        protected override Texture2D UpTexture { get { return upTexture; } }
        protected override Texture2D DownTexture { get { return downTexture; } }

        public QuickEnemy(ArenaMap arena, ArenaManager env, int startX, int startY, float scale)
            : base(arena, env, startX, startY, scale)
        {
        }

        public static new void LoadContent()
        {
            if (leftTexture == null)
                leftTexture = TDGame.MainGame.Content.Load<Texture2D>(leftPath);

            if (rightTexture == null)
                rightTexture = TDGame.MainGame.Content.Load<Texture2D>(rightPath);

            if (upTexture == null)
                upTexture = TDGame.MainGame.Content.Load<Texture2D>(upPath);

            if (downTexture == null)
                downTexture = TDGame.MainGame.Content.Load<Texture2D>(downPath);

            if (previewTexture == null)
                previewTexture = TDGame.MainGame.Content.Load<Texture2D>(previewPath);
        }
    }
}
