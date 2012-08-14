using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Frog_Defense.Enemies;
using Microsoft.Xna.Framework;

namespace Frog_Defense.Traps
{
    class Build : Trap
    {
        public override TrapType trapType
        {
            get { return TrapType.Wall; }
        }
        public override int Cost { get { return 100; } }
        public override string Name
        {
            get { return "Construction"; }
        }
        public override string Description
        {
            get { return "Builds an impassable wall."; }
        }

        //image stuff
        private const int imageWidth = 30;
        private const int imageHeight = 30;
        private const string imagePath = "Images/Traps/WallIndicator";
        private static Texture2D imageTexture;

        private const string previewPath = "Images/TrapPreviews/WallPreview";
        private static Texture2D previewTexture;
        public override Texture2D PreviewTexture
        {
            get { return previewTexture; }
        }

        //position stuff
        private int xCenter, yCenter;

        public Build(ArenaManager env, int xCenter, int yCenter, int xSquare, int ySquare)
            : base(env, xSquare, ySquare)
        {
            this.xCenter = xCenter;
            this.yCenter = yCenter;
        }

        public override void Update(IEnumerable<Enemy> enemies)
        {
            throw new NotImplementedException();
        }

        public static new void LoadContent()
        {
            if (imageTexture == null)
                imageTexture = TDGame.MainGame.Content.Load<Texture2D>(imagePath);

            if (previewTexture == null)
                previewTexture = TDGame.MainGame.Content.Load<Texture2D>(previewPath);
        }

        public override void shift(int xChange, int yChange, int xSquaresChange, int ySquaresChange)
        {
            //this should never happen!
            throw new NotImplementedException();
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset, bool paused)
        {
            Vector2 drawPosition = new Vector2(
                xOffset + xCenter - imageWidth / 2,
                yOffset + yCenter - imageHeight / 2
                );

            batch.Draw(imageTexture, drawPosition, Color.White);
        }
    }
}
