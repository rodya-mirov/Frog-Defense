using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Frog_Defense.Enemies;

namespace Frog_Defense.Traps
{
    /// <summary>
    /// This trap is affixed to the center of a wall, and fires at the closest target.
    /// It hits that target for flat damage.
    /// </summary>
    class GunTrap : Trap
    {
        public override int Cost
        {
            get { return 100; }
        }

        private Point position;

        private Direction facing;
        public Direction Facing
        {
            get { return facing; }
        }

        //contains subobjects which are bullets
        private Queue<Point> bulletPositions;
        private const int maxBullets = 20;
        private int bulletSpeed = 2;

        private const float bulletDamage = 10;

        private const int reloadFrames = 15;
        private int framesSinceFiring = 0;

        //standard image stuff: Gun
        private Texture2D myGunTexture;

        private const int gunImageWidth = 30;
        private const int gunImageHeight = 30;

        private const String upGunPath = "Images/Traps/GunTrapUp";
        private static Texture2D upGunTexture;
        private const String downGunPath = "Images/Traps/GunTrapDown";
        private static Texture2D downGunTexture;
        private const String rightGunPath = "Images/Traps/GunTrapRight";
        private static Texture2D rightGunTexture;
        private const String leftGunPath = "Images/Traps/GunTrapLeft";
        private static Texture2D leftGunTexture;

        //standard image stuff: Bullet
        private const int bulletImageWidth = 2;
        private const int bulletImageHeight = 2;

        private const String bulletPath = "Images/Traps/Bullet";
        private static Texture2D bulletTexture;

        public GunTrap(Arena arena, GameUpdater env, int x, int y, Direction facing)
            : base(arena, env)
        {
            position.X = x;
            position.Y = y;

            this.facing = facing;

            switch (facing)
            {
                case Direction.UP:
                    myGunTexture = upGunTexture;
                    break;
                case Direction.RIGHT:
                    myGunTexture = rightGunTexture;
                    break;
                case Direction.LEFT:
                    myGunTexture = leftGunTexture;
                    break;
                case Direction.DOWN:
                    myGunTexture = downGunTexture;
                    break;
            }

            bulletPositions = new Queue<Point>(maxBullets);
        }

        public static void LoadContent()
        {
            if (upGunTexture == null)
                upGunTexture = TDGame.MainGame.Content.Load<Texture2D>(upGunPath);

            if (downGunTexture == null)
                downGunTexture = TDGame.MainGame.Content.Load<Texture2D>(downGunPath);

            if (leftGunTexture == null)
                leftGunTexture = TDGame.MainGame.Content.Load<Texture2D>(leftGunPath);

            if (rightGunTexture == null)
                rightGunTexture = TDGame.MainGame.Content.Load<Texture2D>(rightGunPath);

            if (bulletTexture == null)
                bulletTexture = TDGame.MainGame.Content.Load<Texture2D>(bulletPath);
        }

        /// <summary>
        /// Shoots the gun, if it sees anything, and continues the bullet rain if possible.
        /// Also updates the bullets themselves, which may hit enemies.
        /// </summary>
        /// <param name="enemies"></param>
        public override void Update(IEnumerable<Enemy> enemies)
        {
            fireIfPossible(enemies);

            //now update the bullets
            int numBullets = bulletPositions.Count;
            for (int i = 0; i < numBullets; i++)
            {
                Point p = bulletPositions.Dequeue();

                //first, move the bullet
                switch (facing)
                {
                    case Direction.UP:
                        p.Y -= bulletSpeed;
                        break;
                    case Direction.DOWN:
                        p.Y += bulletSpeed;
                        break;
                    case Direction.LEFT:
                        p.X -= bulletSpeed;
                        break;
                    case Direction.RIGHT:
                        p.X += bulletSpeed;
                        break;
                }

                //keep track of if we hit anything?
                bool madeCollision = false;

                //next, see if it hit an enemy
                foreach (Enemy e in enemies)
                {
                    if (e.ContainsPoint(p))
                    {
                        e.TakeHit(bulletDamage);
                        madeCollision = true;
                        break;
                    }
                }

                //next, see if it hit a wall
                if (!madeCollision && arena.IsInWall(p))
                    madeCollision = true;

                //if we didn't hit anything, keep the bullet going
                if (!madeCollision)
                    bulletPositions.Enqueue(p);
            }
        }

        /// <summary>
        /// Helper method for Update; this checks if the gun is ready to fire (in terms of
        /// reloading and max ammo) and if it sees anything.  If it has both, it fires a new
        /// bullet, which is just an entry in the bulletPositions queue.
        /// </summary>
        /// <param name="enemies"></param>
        private void fireIfPossible(IEnumerable<Enemy> enemies)
        {
            //first, check if the gun is loaded, and if not, spend your time on that
            if (framesSinceFiring < reloadFrames)
            {
                framesSinceFiring += 1;
                return;
            }

            //now check if there is too much ordinance already in motion
            if (bulletPositions.Count >= maxBullets)
            {
                return;
            }

            //all clear, so check if we have a target
            bool found = false;

            int xmin = position.X;
            int xmax = position.X + gunImageWidth;
            int ymin = position.Y;
            int ymax = position.Y + gunImageHeight;

            foreach (Enemy e in enemies)
            {
                switch (facing)
                {
                    case Direction.LEFT:
                        if (e.YCenter >= ymin && e.YCenter <= ymax && e.XCenter <= xmax)
                        {
                            found = true;
                        }

                        break;

                    case Direction.RIGHT:
                        if (e.YCenter >= ymin && e.YCenter <= ymax && e.XCenter >= xmin)
                        {
                            found = true;
                        }

                        break;

                    case Direction.UP:
                        if (e.XCenter >= xmin && e.XCenter <= xmax && e.YCenter <= ymin)
                        {
                            found = true;
                        }

                        break;

                    case Direction.DOWN:
                        if (e.XCenter >= xmin && e.XCenter <= xmax && e.YCenter >= ymax)
                        {
                            found = true;
                        }

                        break;
                }
            }

            //if there's something to shoot, shoot it
            if (found)
            {
                Point bulletPos;
                bulletPos.X = position.X;
                bulletPos.Y = position.Y;

                bulletPositions.Enqueue(bulletPos);

                framesSinceFiring = 0;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset)
        {
            //draw bullets first
            foreach (Point p in bulletPositions)
            {
                batch.Draw(
                    bulletTexture,
                    new Vector2(
                        p.X + xOffset - bulletImageWidth / 2,
                        p.Y + yOffset - bulletImageHeight / 2
                        ),
                    Color.White
                    );
            }

            //then the gun itself
            batch.Draw(
                myGunTexture,
                new Vector2(
                    position.X + xOffset - gunImageWidth / 2,
                    position.Y + yOffset - gunImageHeight / 2
                    ),
                Color.White
                );

        }
    }
}
