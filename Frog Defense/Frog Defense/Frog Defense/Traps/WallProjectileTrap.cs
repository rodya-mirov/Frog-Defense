﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frog_Defense.Enemies;

namespace Frog_Defense.Traps
{
    abstract class WallProjectileTrap : WallTrap
    {
        //location parameters
        protected Point position;

        //contains subobjects which are bullets
        protected Queue<Point> projectilePositions;

        //parameters for the value of the gun
        protected virtual int maxBullets
        {
            get { return 2000; }
        }
        protected virtual int bulletSpeed
        {
            get { return 2; }
        }
        protected abstract float ProjectileDamage { get; }
        protected virtual int reloadFrames
        {
            get { return 15; }
        }
        protected int framesSinceFiring = 0;

        //texture stuff
        protected Texture2D mainTexture;

        protected abstract Texture2D UpTexture { get; }
        protected abstract Texture2D DownTexture { get; }
        protected abstract Texture2D LeftTexture { get; }
        protected abstract Texture2D RightTexture { get; }

        protected abstract int imageWidth { get; }
        protected abstract int imageHeight { get; }

        protected abstract Texture2D BulletTexture { get; }
        protected abstract int BulletImageWidth { get; }
        protected abstract int BulletImageHeight { get; }

        protected WallProjectileTrap(Arena arena, GameUpdater env, int centerX, int centerY, Direction facing)
            : base(arena, env, facing)
        {
            position = new Point(centerX, centerY);

            switch (facing)
            {
                case Direction.UP:
                    mainTexture = UpTexture;
                    break;

                case Direction.DOWN:
                    mainTexture = DownTexture;
                    break;

                case Direction.LEFT:
                    mainTexture = LeftTexture;
                    break;

                case Direction.RIGHT:
                    mainTexture = RightTexture;
                    break;

                default:
                    throw new NotImplementedException();
            }

            projectilePositions = new Queue<Point>();
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
            updateProjectiles(enemies);
        }

        private void updateProjectiles(IEnumerable<Enemy> enemies)
        {
            int numBullets = projectilePositions.Count;
            for (int i = 0; i < numBullets; i++)
            {
                Point p = projectilePositions.Dequeue();

                //first, move the bullet
                p = moveProjectilePoint(p);

                //keep track of if we hit anything, and therefore should stop
                bool shouldStop = false;

                //next, see if it hit an enemy
                foreach (Enemy e in enemies)
                {
                    if (e.ContainsPoint(p))
                    {
                        shouldStop = hitEnemy(e);
                        if (shouldStop)
                            break;
                    }
                }

                //next, see if it hit a wall
                if (!shouldStop && arena.IsInWall(p))
                {
                    shouldStop = true;
                }

                //if we didn't hit anything, keep the bullet going
                if (!shouldStop)
                    projectilePositions.Enqueue(p);
            }
        }

        /// <summary>
        /// Helper for the UpdateProjectiles method.  Returns true
        /// if the bullet should stop after hitting the enemy.
        /// </summary>
        /// <param name="madeCollision"></param>
        /// <param name="e"></param>
        /// <returns>Whether the bullet should stop</returns>
        protected virtual bool hitEnemy(Enemy e)
        {
            e.TakeHit(ProjectileDamage);
            return true;
        }

        private Point moveProjectilePoint(Point p)
        {
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
            return p;
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
            if (projectilePositions.Count >= maxBullets)
            {
                return;
            }

            //all clear, so check if we have a target
            bool found = false;

            int xmin = position.X;
            int xmax = position.X + imageWidth;
            int ymin = position.Y;
            int ymax = position.Y + imageHeight;

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

                projectilePositions.Enqueue(bulletPos);

                framesSinceFiring = 0;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch batch, int xOffset, int yOffset, bool paused)
        {
            //draw bullets first
            foreach (Point p in projectilePositions)
            {
                batch.Draw(
                    BulletTexture,
                    new Vector2(
                        p.X + xOffset - BulletImageWidth / 2,
                        p.Y + yOffset - BulletImageHeight / 2
                        ),
                    Color.White
                    );
            }

            //then the gun itself
            batch.Draw(
                mainTexture,
                new Vector2(
                    position.X + xOffset - imageWidth / 2,
                    position.Y + yOffset - imageHeight / 2
                    ),
                Color.White
                );

        }
    }
}