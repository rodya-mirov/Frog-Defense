using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Frog_Defense.Enemies;

namespace Frog_Defense.Traps
{
    class MineWall : Trap
    {
        public override TrapLocationType LocationType { get { throw new NotImplementedException(); } }

        protected override void upgradeStats()
        {
            throw new NotImplementedException();
        }

        private int xCenter, yCenter;

        public override string Name
        {
            get { return "Mine Wall"; }
        }
        public override int Cost
        {
            get { return 20; }
        }
        public override string Description
        {
            get
            {
                return "Completely removes a wall,"
                  + "\nassuming that wall can be\nreached from a home tile.";
            }
        }
        public override TrapType trapType
        {
            get { return TrapType.MineWall; }
        }

        private const string previewPath = "Images/TrapPreviews/PickaxePreview";
        private static Texture2D previewTexture;
        public override Texture2D PreviewTexture
        {
            get { return previewTexture; }
        }

        private const int imageWidth = 30;
        private const int imageHeight = 30;
        private const string imagePath = "Images/Traps/Pickaxe";
        private static Texture2D imageTexture;

        public MineWall(ArenaManager env, int xCenter, int yCenter, int floorSquareX, int floorSquareY)
            : base(env, floorSquareX, floorSquareY)
        {
            this.xCenter = xCenter;
            this.yCenter = yCenter;
        }

        public static new void LoadContent()
        {
            if (previewTexture == null)
                previewTexture = TDGame.MainGame.Content.Load<Texture2D>(previewPath);

            if (imageTexture == null)
                imageTexture = TDGame.MainGame.Content.Load<Texture2D>(imagePath);
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset, bool paused)
        {
            batch.Draw(
                imageTexture,
                new Vector2(xOffset + xCenter - imageWidth / 2, yOffset + yCenter - imageHeight / 2),
                Color.White
                );
        }

        public override void Update(IEnumerable<Enemy> enemies)
        {
            //does nothing, nothing to do
        }

        public override void shift(int xChange, int yChange, int xSquaresChange, int ySquaresChange)
        {
            //this should never happen!
            throw new NotImplementedException();
        }

        /// <summary>
        /// This should never happen, since the wall is never "placed"
        /// </summary>
        public override float VisualXCenter
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// This should never happen, since the wall is never "placed"
        /// </summary>
        public override float VisualYCenter
        {
            get { throw new NotImplementedException(); }
        }
    }
}
