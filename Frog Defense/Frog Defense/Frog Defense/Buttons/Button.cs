using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frog_Defense.Buttons
{
    class Button
    {
        protected int width;
        protected int height;

        public int PixelWidth { get { return width; } }
        public int PixelHeight { get { return height; } }

        /// <summary>
        /// Constructs a new button filled with the supplied text
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Button(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public virtual void Update()
        {
        }

        public virtual void GetClicked()
        {
        }

        /// <summary>
        /// Determines whether a specified point, in RELATIVE coordinates, is
        /// inside the bounding rectangle of the button.
        /// </summary>
        /// <param name="mouseX"></param>
        /// <param name="mouseY"></param>
        public bool ContainsPoint(int mouseX, int mouseY)
        {
            return (mouseX >= 0 && mouseX < width && mouseY >= 0 && mouseY < height);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset, bool paused)
        {
            Rectangle drawRect = new Rectangle(xOffset, yOffset, width, height);

            batch.Draw(TDGame.PureBlack, drawRect, Color.White);

            drawRect.X += 1;
            drawRect.Y += 1;
            drawRect.Width -= 2;
            drawRect.Height -= 2;

            batch.Draw(TDGame.PureRed, drawRect, Color.White);
        }
    }
}
