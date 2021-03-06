﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frog_Defense.Buttons
{
    class UpgradeButton : Button
    {
        public override bool Visible
        {
            get { return (env.Player.DetailViewType == DetailViewType.TrapExisting && env.ArenaManager.DetailTrap.CanUpgrade); }
        }

        public override string Text
        {
            get
            {
                return "Upgrade Tower";
            }
        }

        private GameUpdater env;

        private int textOffsetX, textOffsetY;

        public override int TextOffsetX { get { return textOffsetX; } }
        public override int TextOffsetY { get { return textOffsetY; } }

        public UpgradeButton(GameUpdater env, int width, int height, SpriteFont font)
            : base(width, height, font)
        {
            this.env = env;

            Vector2 measurement = font.MeasureString(Text);

            textOffsetX = (int)((width - measurement.X) / 2f);
            textOffsetY = (int)((height - measurement.Y) / 2f);
        }

        /// <summary>
        /// Sells a tower, if there is one
        /// </summary>
        public override void GetClicked()
        {
            if (env.Player.DetailViewType == DetailViewType.TrapExisting)
            {
                env.ArenaManager.UpgradeTrap();
            }
            else
            {
                //do nothing!
            }
        }
    }
}
