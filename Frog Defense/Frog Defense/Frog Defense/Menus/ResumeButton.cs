using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Frog_Defense.Menus
{
    class ResumeButton : MenuItem
    {
        public ResumeButton(String text, SpriteFont font)
            : base(text, font)
        {
        }

        public override bool Active
        {
            get { return TDGame.MainGame.HasSuspendedGame; }
            set { throw new NotImplementedException(); }
        }


        public override void GetClicked()
        {
            base.GetClicked();
        }
    }
}
