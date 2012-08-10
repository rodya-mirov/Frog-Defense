using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Frog_Defense.Enemies
{
    class BigBasicEnemy : BasicEnemy
    {
        protected override float MAX_HEALTH
        {
            get { return 200; }
        }

        private const String imagePath = "Images/Enemies/BigEnemy";
        private static Texture2D imageTexture;

        protected override Texture2D ImageTexture
        {
            get { return imageTexture; }
        }

        public BigBasicEnemy(Arena arena, EnvironmentUpdater env, int startX, int startY)
            : base(arena, env, startX, startY)
        {
        }

        public static new void LoadContent()
        {
            if (imageTexture == null)
                imageTexture = TDGame.MainGame.Content.Load<Texture2D>(imagePath);
        }
    }
}
