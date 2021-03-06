﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frog_Defense.Waves;
using Frog_Defense.Enemies;
using Frog_Defense.Traps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Frog_Defense.PlayerData;

namespace Frog_Defense
{
    class ArenaManager
    {
        private Point arenaTranslation;

        private ArenaMap arenaMap;
        public ArenaMap Map { get { return arenaMap; } }

        private WaveTracker waveTracker;
        public WaveTracker WaveTracker { get { return waveTracker; } }

        private ScrollPanel scrollPanel;
        public ScrollPanel ScrollPanel { get { return scrollPanel; } }

        private Queue<Enemy> enemies;
        private Queue<Enemy> enemiesToBeAdded;

        private Queue<Trap> floorTraps, wallTraps;
        private Queue<Trap> trapsToBeAdded;

        private GameUpdater env;
        public AchievementTracker AchievementTracker
        {
            get { return env.AchievementTracker; }
        }

        public PlayerHUD Player
        {
            get { return env.Player; }
        }

        private const int scrollPanelWidth = 10;

        private TrapType selectedTrapType;
        public TrapType SelectedTrapType
        {
            get { return selectedTrapType; }
            set { selectedTrapType = value; }
        }

        private int pixelWidth, pixelHeight;

        public ArenaManager(GameUpdater env, int pixelWidth, int pixelHeight)
        {
            this.env = env;
            this.selectedTrapType = TrapType.NoType;

            this.pixelWidth = pixelWidth;
            this.pixelHeight = pixelHeight;

            this.arenaTranslation = new Point(0, 0);

            this.scrollPanel = new ScrollPanel(this, scrollPanelWidth, pixelWidth, pixelHeight);
        }

        /// <summary>
        /// Won't allow the map to be dragged more than this many pixels away
        /// from the appropriate area
        /// </summary>
        private const int maxArenaScreenBuffer = 40;

        public void scrollMap(int xChange, int yChange)
        {
            arenaTranslation.X += xChange;
            arenaTranslation.Y += yChange;

            arenaTranslation.X = Math.Max(arenaTranslation.X, pixelWidth - arenaMap.PixelWidth - maxArenaScreenBuffer);
            arenaTranslation.X = Math.Min(arenaTranslation.X, maxArenaScreenBuffer);

            arenaTranslation.Y = Math.Max(arenaTranslation.Y, pixelHeight - arenaMap.PixelHeight - maxArenaScreenBuffer);
            arenaTranslation.Y = Math.Min(arenaTranslation.Y, maxArenaScreenBuffer);
        }

        public static void LoadContent()
        {
            ArenaMap.LoadContent();

            Enemy.LoadContent();

            Trap.LoadContent();
        }

        public void ResetGame()
        {
            arenaMap = ArenaMap.MakeMapFromTextFile("Maps/DefaultMap.txt", this);
            waveTracker = new WaveTracker(arenaMap, this);

            enemies = new Queue<Enemy>();
            enemiesToBeAdded = new Queue<Enemy>();

            floorTraps = new Queue<Trap>();
            wallTraps = new Queue<Trap>();
            trapsToBeAdded = new Queue<Trap>();

            framesOfWin = 0;
            arenaTranslation = new Point(0, 0);
            scrollMap(0, 0);
        }

        public void GetClicked()
        {
            arenaMap.GetLeftClicked();

            if (Player.DetailViewType != DetailViewType.TrapPreview)
            {
                if (mousedEnemy != null)
                {
                    selectedEnemyToShow = mousedEnemy;
                    Player.DetailViewType = DetailViewType.EnemyExisting;
                }
                else if (mousedTrap != null)
                {
                    selectedTrapToShow = mousedTrap;
                    Player.DetailViewType = DetailViewType.TrapExisting;
                }
            }
        }

        public void addTrap(Trap t)
        {
            trapsToBeAdded.Enqueue(t);
        }

        public void addEnemy(Enemy e)
        {
            enemiesToBeAdded.Enqueue(e);
        }

        public void Update(GameTime gameTime, bool paused)
        {
            //deal with the scrolling
            scrollPanel.Update();

            //next, add in all the queued up new friends :)
            foreach (Trap t in trapsToBeAdded)
            {
                if (t.LocationType == TrapLocationType.Floor)
                    floorTraps.Enqueue(t);
                else
                    wallTraps.Enqueue(t);
            }

            trapsToBeAdded.Clear();

            //most things won't happen during paused time
            if (!paused)
            {
                foreach (Enemy e in enemiesToBeAdded)
                    enemies.Enqueue(e);

                enemiesToBeAdded.Clear();

                //update things that do things
                updateEnemies();
                updateTraps();

                //prep for spawning
                waveTracker.Update();
            }

            detectWin();
        }

        private const int framesBeforeWin = 60;
        private int framesOfWin;

        private void detectWin()
        {
            if (enemies.Count == 0 && waveTracker.isClear())
            {
                framesOfWin++;
                if (framesOfWin >= framesBeforeWin)
                {
                    TDGame.MainGame.Win();
                }
            }
        }

        /// <summary>
        /// Helper method for Update.  This updates all the enemies through cycling,
        /// and simply forgets to add them back in if the enemy is dead.  It also
        /// credits the player with the kill.
        /// </summary>
        private void updateEnemies()
        {
            int numEnemies = enemies.Count;

            for (int i = 0; i < numEnemies; i++)
            {
                Enemy enemy = enemies.Dequeue();

                enemy.Update();

                if (enemy.HasReachedGoal)
                {
                    Player.TakePlayerHit();
                }
                else if (enemy.IsAlive)
                {
                    enemies.Enqueue(enemy);
                }
                else
                {
                    Player.AddMoney(enemy.CashValue);
                }
            }
        }

        /// <summary>
        /// Just cycles through the traps and updates them
        /// </summary>
        private void updateTraps()
        {
            foreach (Trap t in floorTraps)
                t.Update(enemies);

            foreach (Trap t in wallTraps)
                t.Update(enemies);
        }

        public void DrawArena(GameTime gameTime, SpriteBatch batch, int arenaOffsetX, int arenaOffsetY, bool paused)
        {
            arenaMap.Draw(gameTime,
                batch,
                arenaOffsetX + arenaTranslation.X + scrollPanelWidth,
                arenaOffsetY + arenaTranslation.Y + scrollPanelWidth,
                paused);

            //...draw the floor traps...
            foreach (Trap t in floorTraps)
            {
                t.Draw(
                    gameTime,
                    batch,
                    arenaOffsetX + arenaTranslation.X + scrollPanelWidth,
                    arenaOffsetY + arenaTranslation.Y + scrollPanelWidth,
                    paused
                    );
            }

            //...draw the wall traps...
            foreach (Trap t in wallTraps)
            {
                t.Draw(
                    gameTime,
                    batch,
                    arenaOffsetX + arenaTranslation.X + scrollPanelWidth,
                    arenaOffsetY + arenaTranslation.Y + scrollPanelWidth,
                    paused
                    );
            }

            //...draw the enemies...
            foreach (Enemy e in enemies)
            {
                e.Draw(
                    gameTime,
                    batch,
                    arenaOffsetX + arenaTranslation.X + scrollPanelWidth,
                    arenaOffsetY + arenaTranslation.Y + scrollPanelWidth,
                    paused
                    );
            }

            //...and their health bars
            foreach (Enemy e in enemies)
            {
                e.DrawHealthBar(
                    gameTime,
                    batch,
                    arenaOffsetX + arenaTranslation.X + scrollPanelWidth,
                    arenaOffsetY + arenaTranslation.Y + scrollPanelWidth,
                    paused
                    );
            }

            //and finally, draw the scroll bars
            scrollPanel.Draw(gameTime, batch, arenaOffsetX, arenaOffsetY);
        }

        public void spawnEnemy(Enemy e)
        {
            arenaMap.putInSpawn(e);
            enemiesToBeAdded.Enqueue(e);
        }

        public void shiftObjects(int xChange, int yChange, int xSquareChange, int ySquareChange)
        {
            foreach (Enemy e in enemiesToBeAdded)
                e.shift(xChange, yChange, xSquareChange, ySquareChange);

            foreach (Enemy e in enemies)
                e.shift(xChange, yChange, xSquareChange, ySquareChange);

            foreach(Trap t in trapsToBeAdded)
                t.shift(xChange, yChange, xSquareChange, ySquareChange);

            foreach (Trap t in floorTraps)
                t.shift(xChange, yChange, xSquareChange, ySquareChange);

            foreach (Trap t in wallTraps)
                t.shift(xChange, yChange, xSquareChange, ySquareChange);
        }

        /// <summary>
        /// Marks the wall at (x, y) as destroyed and kills all related traps.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MineWall(int x, int y)
        {
            int n;

            n = trapsToBeAdded.Count;

            for (int i = 0; i < n; i++)
            {
                Trap t = trapsToBeAdded.Dequeue();
                if (!t.touchesWall(x, y))
                    trapsToBeAdded.Enqueue(t);
            }

            n = wallTraps.Count;

            for (int i = 0; i < n; i++)
            {
                Trap t = wallTraps.Dequeue();
                if (!t.touchesWall(x, y))
                    wallTraps.Enqueue(t);
            }
        }

        /// <summary>
        /// Returns an enumerable collection of all the points
        /// the current enemies are immediately moving toward.
        /// </summary>
        /// <param name="squareX"></param>
        /// <param name="squareY"></param>
        /// <returns></returns>
        public IEnumerable<Point> EnemyTargets()
        {
            List<Point> output = new List<Point>();
            foreach (Enemy e in enemies)
                output.Add(e.NextSquare);

            return output;
        }

        /// <summary>
        /// Determines whether a wall at the specified SQUARE
        /// coordinates conflicts with any existing enemies;
        /// returns false in this case (since we can't build a
        /// wall there).
        /// </summary>
        /// <param name="squareX"></param>
        /// <param name="squareY"></param>
        /// <returns></returns>
        public bool CanBuildWall(int squareX, int squareY)
        {
            foreach (Enemy e in enemiesToBeAdded)
            {
                if (e.isInvolvedWithSquare(squareX, squareY))
                    return false;
            }

            foreach (Enemy e in enemies)
            {
                if (e.isInvolvedWithSquare(squareX, squareY))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Destroys any traps on the floor of the space
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ClearFloorTraps(int x, int y)
        {
            int n;
            Trap t;

            n = trapsToBeAdded.Count;

            for (int i = 0; i < n; i++)
            {
                t = trapsToBeAdded.Dequeue();
                if (!t.isOnSquare(x, y))
                    trapsToBeAdded.Enqueue(t);
            }

            n = floorTraps.Count;

            for (int i = 0; i < n; i++)
            {
                t = floorTraps.Dequeue();
                if (!t.isOnSquare(x, y))
                    floorTraps.Enqueue(t);
            }
        }

        public void updateMousePosition(int mouseX, int mouseY, bool mouseInArena)
        {
            arenaMap.updateMousePosition(
                mouseX - scrollPanelWidth - arenaTranslation.X,
                mouseY - scrollPanelWidth - arenaTranslation.Y,
                mouseInArena
                );

            pickMousedOverTarget(
                mouseX - scrollPanelWidth - arenaTranslation.X,
                mouseY - scrollPanelWidth - arenaTranslation.Y
                );

            scrollPanel.updateMousePosition(mouseX, mouseY);
        }

        private Enemy mousedEnemy, selectedEnemyToShow;
        private Trap mousedTrap, selectedTrapToShow;
        private const float mouseSquareThreshold = 100f;

        public Enemy DetailEnemy { get { return selectedEnemyToShow; } }
        public Trap DetailTrap { get { return selectedTrapToShow; } }

        /// <summary>
        /// Determines the closest enemy/trap, if there is one close
        /// enough, to the given coordinates.  Coordinates are assumed
        /// to be relativized to the arena.
        /// </summary>
        /// <param name="mx"></param>
        /// <param name="my"></param>
        private void pickMousedOverTarget(int mx, int my)
        {
            //ignore all traps/enemies outside the threshold
            float bestSquareDistance = mouseSquareThreshold;
            float possibleSquareDistance;

            mousedTrap = null;
            mousedEnemy = null;

            foreach (Trap t in floorTraps)
            {
                possibleSquareDistance = (mx - t.VisualXCenter) * (mx - t.VisualXCenter) + (my - t.VisualYCenter) * (my - t.VisualYCenter);
                if (possibleSquareDistance < bestSquareDistance)
                {
                    mousedTrap = t;
                    mousedEnemy = null;
                    bestSquareDistance = possibleSquareDistance;
                }
            }

            foreach (Trap t in wallTraps)
            {
                possibleSquareDistance = (mx - t.VisualXCenter) * (mx - t.VisualXCenter) + (my - t.VisualYCenter) * (my - t.VisualYCenter);
                if (possibleSquareDistance < bestSquareDistance)
                {
                    mousedTrap = t;
                    mousedEnemy = null;
                    bestSquareDistance = possibleSquareDistance;
                }
            }

            foreach (Enemy e in enemies)
            {
                possibleSquareDistance = (mx - e.VisualXCenter) * (mx - e.VisualXCenter) + (my - e.VisualYCenter) * (my - e.VisualYCenter);
                if (possibleSquareDistance < bestSquareDistance)
                {
                    mousedTrap = null;
                    mousedEnemy = e;
                    bestSquareDistance = possibleSquareDistance;
                }
            }
        }

        public void RightClick(bool mouseRightIsDown)
        {
            arenaMap.RightClick(mouseRightIsDown);
        }

        /// <summary>
        /// If there is a selected trap, it sells it and gives the money to the player.
        /// If not, does nothing.
        /// </summary>
        public void SellTrap()
        {
            if (DetailTrap != null)
            {
                int n;

                n = floorTraps.Count;
                for (int i = 0; i < n; i++)
                {
                    Trap t = floorTraps.Dequeue();
                    if (t != DetailTrap)
                    {
                        floorTraps.Enqueue(t);
                    }
                    else
                    {
                        arenaMap.ClearTrap(t);
                    }
                }

                n = wallTraps.Count;
                for (int i = 0; i < n; i++)
                {
                    Trap t = wallTraps.Dequeue();
                    if (t != DetailTrap)
                    {
                        wallTraps.Enqueue(t);
                    }
                    else
                    {
                        arenaMap.ClearTrap(t);
                    }
                }

                Player.AddMoney(DetailTrap.SellPrice);

                selectedTrapToShow = null;
            }
        }

        public void UpgradeTrap()
        {
            if (DetailTrap != null && DetailTrap.CanUpgrade && env.Player.Money >= DetailTrap.UpgradeCost)
            {
                env.Player.Spend(DetailTrap.UpgradeCost);
                DetailTrap.Upgrade();
            }
        }
    }
}
