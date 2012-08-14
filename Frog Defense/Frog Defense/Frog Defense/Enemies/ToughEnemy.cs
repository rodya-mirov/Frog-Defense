using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Frog_Defense.Enemies
{
    class ToughEnemy : BasicEnemy
    {
        protected override string EnemyType { get { return "Tough Enemy"; } }
        protected override string Description { get { return "Takes less damage from\nguns and spikes."; } }

        /// <summary>
        /// This is the percentage damage that ToughEnemy actually takes
        /// </summary>
        private const float toughnessFactor = .5f;

        public override int CashValue { get { return 15; } }
        protected override float Speed { get { return 1.2f; } }
        public override int TicksAfterSpawn { get { return 65; } }

        private const String previewPath = "Images/Enemies/ToughEnemy/Preview";
        private static Texture2D previewTexture;
        public override Texture2D PreviewTexture
        {
            get { return previewTexture; }
        }

        private const String leftPath = "Images/Enemies/ToughEnemy/Left";
        private const String rightPath = "Images/Enemies/ToughEnemy/Right";
        private const String upPath = "Images/Enemies/ToughEnemy/Up";
        private const String downPath = "Images/Enemies/ToughEnemy/Down";

        private static Texture2D leftTexture;
        private static Texture2D rightTexture;
        private static Texture2D upTexture;
        private static Texture2D downTexture;

        protected override Texture2D LeftTexture { get { return leftTexture; } }
        protected override Texture2D RightTexture { get { return rightTexture; } }
        protected override Texture2D UpTexture { get { return upTexture; } }
        protected override Texture2D DownTexture { get { return downTexture; } }

        public ToughEnemy(ArenaMap arena, ArenaManager env, int startX, int startY, float scale)
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

        public override void TakeHit(float damage)
        {
            base.TakeHit(damage * toughnessFactor);
        }
    }
}
