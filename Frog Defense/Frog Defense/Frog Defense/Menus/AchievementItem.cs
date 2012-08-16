using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frog_Defense.PlayerData;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frog_Defense.Menus
{
    class AchievementItem : MenuItem
    {
        private GameUpdater env;

        public override string Text
        {
            get
            {
                return AchievementTracker.AchievementName(ach) + "\n     " + AchievementTracker.Conditions(ach);
            }
        }

        protected override Color TextColor
        {
            get
            {
                if (env.AchievementTracker.HasAchieved(ach))
                    return Color.Yellow;
                else
                    return Color.DarkGray;
            }
        }

        private Achievements ach;

        public AchievementItem(Achievements achievement, GameUpdater env, SpriteFont font)
            : base("A\nA", font)
        {
            this.ach = achievement;
            this.env = env;
        }
    }
}
