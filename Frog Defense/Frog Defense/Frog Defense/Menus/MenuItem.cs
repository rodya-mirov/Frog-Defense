using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frog_Defense.Menus
{
    class MenuItem
    {
        private String text;

        protected SpriteFont font;

        protected Vector2 position;
        protected Vector2 size;

        protected Rectangle boundingRectangle;

        protected bool mousedOver;
        public bool MousedOver
        {
            get { return mousedOver; }
        }

        public virtual String Text
        {
            get { return text; }
        }

        public float Bottom
        {
            get { return position.Y + size.Y; }
        }

        public MenuItem(String text, SpriteFont font)
        {
            this.text = text;

            this.font = font;

            size = font.MeasureString(text);
        }

        public void setPosition(float x, float y)
        {
            position = new Vector2(x, y);

            boundingRectangle = new Rectangle((int)x, (int)y, (int)size.X, (int)size.Y);
        }

        public virtual void MousePosition(int x, int y)
        {
            if (boundingRectangle.Contains(x, y))
                mousedOver = true;
            else
                mousedOver = false;
        }

        public virtual void GetClicked()
        {
            //does nothing, override me
        }

        public void Draw(GameTime gameTime, SpriteBatch batch)
        {
            batch.DrawString(
                font,
                Text,
                position,
                mousedOver ? Color.White : Color.Gray
                );
        }
    }
}
