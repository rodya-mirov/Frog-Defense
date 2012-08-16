using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Frog_Defense.Menus
{
    class GoToAchievementsItem : MenuItem
    {
        public GoToAchievementsItem(String text, SpriteFont font)
            : base(text, font)
        {
        }

        public override void GetClicked()
        {
            base.GetClicked();

            TDGame.MainGame.GoToAchievementsScreen();
        }
    }
}
