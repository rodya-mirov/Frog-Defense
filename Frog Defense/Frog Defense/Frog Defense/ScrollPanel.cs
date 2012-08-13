using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frog_Defense
{
    class ScrollPanel
    {
        private Rectangle topRect, leftRect, botRect, rightRect;
        private bool mouseInTop, mouseInRight, mouseInBottom, mouseInLeft;

        private int thickness, width, height;
        private int mouseX, mouseY;

        private const int mouseOverSpeed = 2;

        private ArenaManager manager;

        public ScrollPanel(ArenaManager manager, int thickness, int width, int height)
        {
            this.manager = manager;

            this.thickness = thickness;
            this.width = width;
            this.height = height;

            topRect = new Rectangle(0, 0, width, thickness);
            leftRect = new Rectangle(0, 0, thickness, height);
            botRect = new Rectangle(0, height - thickness, width, thickness);
            rightRect = new Rectangle(width - thickness, 0, thickness, height);

            mouseInBottom = false;
            mouseInLeft = false;
            mouseInRight = false;
            mouseInTop = false;
        }

        public void Update()
        {
            if (mouseInBottom)
                manager.scrollMap(0, -mouseOverSpeed);

            if (mouseInTop)
                manager.scrollMap(0, mouseOverSpeed);

            if (mouseInLeft)
                manager.scrollMap(mouseOverSpeed, 0);

            if (mouseInRight)
                manager.scrollMap(-mouseOverSpeed, 0);
        }

        /// <summary>
        /// Updates the stored mouse position in relative coordinates
        /// </summary>
        /// <param name="mouseX"></param>
        /// <param name="mouseY"></param>
        public void updateMousePosition(int mouseX, int mouseY)
        {
            this.mouseX = mouseX;
            this.mouseY = mouseY;

            mouseInBottom = botRect.Contains(mouseX, mouseY);
            mouseInLeft = leftRect.Contains(mouseX, mouseY);
            mouseInRight = rightRect.Contains(mouseX, mouseY);
            mouseInTop = topRect.Contains(mouseX, mouseY);
        }

        public void Draw(GameTime gameTime, SpriteBatch batch, int arenaOffsetX, int arenaOffsetY)
        {
            Rectangle top = new Rectangle(arenaOffsetX, arenaOffsetY, width, thickness);
            Rectangle bottom = new Rectangle(arenaOffsetX, arenaOffsetY + height - thickness, width, thickness);
            Rectangle left = new Rectangle(arenaOffsetX, arenaOffsetY, thickness, height);
            Rectangle right = new Rectangle(arenaOffsetX + width - thickness, arenaOffsetY, thickness, height);

            batch.Draw(TDGame.PureBlack, top, Color.White);
            batch.Draw(TDGame.PureBlack, bottom, Color.White);
            batch.Draw(TDGame.PureBlack, left, Color.White);
            batch.Draw(TDGame.PureBlack, right, Color.White);

            top.X++;
            top.Y++;
            top.Width -= 2;
            top.Height -= 2;

            bottom.X++;
            bottom.Y++;
            bottom.Width -= 2;
            bottom.Height -= 2;

            left.X++;
            left.Y++;
            left.Width -= 2;
            left.Height -= 2;

            right.X++;
            right.Y++;
            right.Width -= 2;
            right.Height -= 2;

            batch.Draw(TDGame.PureWhite, top, Color.White);
            batch.Draw(TDGame.PureWhite, bottom, Color.White);
            batch.Draw(TDGame.PureWhite, left, Color.White);
            batch.Draw(TDGame.PureWhite, right, Color.White);
        }
    }
}
