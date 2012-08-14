using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frog_Defense.Buttons
{
    class ButtonPanel
    {
        private List<Button> buttons;

        private const int buttonWidth = 80;
        private const int buttonHeight = 30;

        public int PixelWidth
        {
            get { return buttonWidth; }
        }

        public int PixelHeight
        {
            get { return buttons.Count * buttonHeight; }
        }

        private ButtonPanel()
        {
            buttons = new List<Button>();
        }

        public static ButtonPanel MakeDefaultPanel(GameUpdater env, SpriteFont font)
        {
            ButtonPanel output = new ButtonPanel();

            output.buttons.Add(new PauseButton(env, font, buttonWidth, buttonHeight));
            output.buttons.Add(new MenuButton(font, buttonWidth, buttonHeight));

            return output;
        }

        /// <summary>
        /// Processes a click, given RELATIVIZED coordinates for the mouse position.
        /// </summary>
        /// <param name="mouseX"></param>
        /// <param name="mouseY"></param>
        public void ProcessClick(int mouseX, int mouseY)
        {
            foreach (Button b in buttons)
            {
                if (b.ContainsPoint(mouseX, mouseY))
                    b.GetClicked();

                //we can re-relativize to the next step down by REDUCING the y-coordinate
                mouseY -= buttonHeight;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset, bool paused)
        {
            foreach (Button b in buttons)
            {
                b.Draw(gameTime, batch, xOffset, yOffset, paused);
                yOffset += buttonHeight;
            }
        }
    }
}
