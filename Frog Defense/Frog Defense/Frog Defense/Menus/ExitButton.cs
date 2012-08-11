using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Frog_Defense.Menus
{
    class ExitButton : MenuItem
    {
        public ExitButton(String text, SpriteFont font)
            : base(text, font)
        {
        }

        public override void GetClicked()
        {
            TDGame.MainGame.Exit();
        }
    }
}
