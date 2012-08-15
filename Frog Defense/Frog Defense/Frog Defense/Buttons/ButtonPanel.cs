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

        private const int buttonWidth = 110;
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
            output.buttons.Add(new MenuButton(buttonWidth, buttonHeight, font));
            output.buttons.Add(new NextWaveButton(env, buttonWidth, buttonHeight, font));
            output.buttons.Add(new SellButton(env, buttonWidth, buttonHeight, font));
            output.buttons.Add(new UpgradeButton(env, buttonWidth, buttonHeight, font));

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
                //if it's not visible, we can just skip it
                if (b.Visible)
                {
                    if (b.ContainsPoint(mouseX, mouseY))
                        b.GetClicked();

                    //we can re-relativize to the next step down by REDUCING the y-coordinate
                    mouseY -= buttonHeight;
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset, bool paused)
        {
            foreach (Button b in buttons)
            {
                if (b.Visible)
                {
                    b.Draw(gameTime, batch, xOffset, yOffset, paused);
                    yOffset += buttonHeight;
                }
            }
        }
    }
}
