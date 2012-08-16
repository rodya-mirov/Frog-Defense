using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Frog_Defense.Traps;
using Frog_Defense.Enemies;

namespace Frog_Defense.PlayerData
{
    enum Achievements
    {
        Victory=0, //win game
        TheseAreCheaper=1, //win without using cannons
        GenevaConvention=2, //win without using spikes or poison
        Miser=3, //have $5000
        MissingThePoint=5, //die without using any terrain modification
        StayAWhile=6, //fully slow a fast enemy with 5 fully upgraded spike traps
        WhyWontYouDie=7, //hit the same immune enemy with 5 fully upgraded dart traps
        AttackItsWeakPoint=8, //do >250 poison damage to the same tough enemy
        Luddite=9 //win without doing any upgrades
    };

    class AchievementTracker
    {
        private int numAchievements;
        
        private int currentPlayerMoney;
        private bool hasUsedTerrainModification, hasUsedCannons, hasUsedSpikesOrPoison, hasUsedUpgrades;

        private Dictionary<Achievements, bool> achieved;
        private string achievementString
        {
            get
            {
                string output = "";
                foreach (Achievements a in Enum.GetValues(typeof(Achievements)))
                {
                    if (achieved[a])
                        output += 'y';
                    else
                        output += 'n';
                }

                return output;
            }
        }

        public AchievementTracker()
        {
            hasUsedTerrainModification = false;
            hasUsedCannons = false;
            hasUsedSpikesOrPoison = false;
            hasUsedUpgrades = false;

            currentPlayerMoney = 0;

            numAchievements = Enum.GetValues(typeof(Achievements)).Length;

            loadAchieved();

            saveAchieved();
        }

        private void saveAchieved()
        {
            StreamWriter sw = new StreamWriter("PlayerData/Data.txt", false);
            sw.WriteLine(achievementString);
            sw.Close();
        }

        private void loadAchieved()
        {
            try
            {
                StreamReader sr = new StreamReader("PlayerData/Data.txt");

                string line = sr.ReadLine();

                sr.Close();

                Achievements[] achievements = (Achievements[])(Enum.GetValues(typeof(Achievements)));
                achieved = new Dictionary<Achievements,bool>();

                if (line.Length != numAchievements)
                    throw new Exception();

                for (int i = 0; i < numAchievements; i++)
                {
                    if (line[i] == 'y')
                        achieved[achievements[i]] = true;
                    else if (line[i] == 'n')
                        achieved[achievements[i]] = false;
                    else
                        throw new Exception();
                }
            }
            catch (Exception)
            {
                achieved = new Dictionary<Achievements, bool>();
                foreach (Achievements a in Enum.GetValues(typeof(Achievements)))
                    achieved[a] = false;
            }
        }

        public void ReportPlayerMoney(int money)
        {
            currentPlayerMoney = money;

            if (money >= 5000)
            {
                achieved[Achievements.Miser] = true;
                saveAchieved();
            }
        }


        public void ReportVictory()
        {
            achieved[Achievements.Victory] = true;
            saveAchieved();

            if (!hasUsedCannons)
            {
                achieved[Achievements.TheseAreCheaper] = true;
                saveAchieved();
            }

            if (!hasUsedSpikesOrPoison)
            {
                achieved[Achievements.GenevaConvention] = true;
                saveAchieved();
            }

            if (!hasUsedUpgrades)
            {
                achieved[Achievements.Luddite] = true;
                saveAchieved();
            }
        }

        public void ReportDeath()
        {
            if (!hasUsedTerrainModification)
            {
                achieved[Achievements.MissingThePoint] = true;
                saveAchieved();
            }
        }

        public void ReportTerrain()
        {
            hasUsedTerrainModification = true;
        }

        public void ReportTrap(Trap selectedTrap)
        {
            if (selectedTrap is CannonTrap)
            {
                hasUsedCannons = true;
            }
            else if (selectedTrap is DartTrap || selectedTrap is SpikeTrap)
            {
                hasUsedSpikesOrPoison = true;
            }
        }

        public void ReportUpgrade()
        {
            hasUsedUpgrades = true;
        }

        public void ReportTotalPoisonDamage(float totalPoisonDamage)
        {
            if (totalPoisonDamage >= 250)
            {
                achieved[Achievements.AttackItsWeakPoint] = true;
                saveAchieved();
            }
        }

        private Dictionary<Enemy, HashSet<SpikeTrap>> fastSpikes;

        public void ReportFastSpike(Enemies.QuickEnemy enemy, SpikeTrap trap)
        {
            //only one achievement here
            if (achieved[Achievements.StayAWhile])
                return;

            //only interested in fully upgraded traps
            if (trap.CanUpgrade)
                return;

            if (fastSpikes == null)
                fastSpikes = new Dictionary<Enemy,HashSet<SpikeTrap>>();

            if (fastSpikes.ContainsKey(enemy))
            {
                fastSpikes[enemy].Add(trap);
                if (fastSpikes[enemy].Count >= 5)
                {
                    fastSpikes = null;
                    achieved[Achievements.StayAWhile] = true;
                    saveAchieved();
                }
            }
            else
            {
                fastSpikes[enemy] = new HashSet<SpikeTrap>();
                fastSpikes[enemy].Add(trap);
            }
        }

        private Dictionary<Enemy, int> poisonCounts;

        public void ReportPoisonImmune(Enemies.ImmuneEnemy immuneEnemy, Trap poisonTrap)
        {
            //there's only one achievement that matters here
            if (achieved[Achievements.WhyWontYouDie])
                return;

            //we only care about fully upgraded traps
            if (poisonTrap.CanUpgrade)
                return;

            if (poisonCounts == null)
                poisonCounts = new Dictionary<Enemy, int>();

            if (poisonCounts.ContainsKey(immuneEnemy))
            {
                poisonCounts[immuneEnemy]++;
                if (poisonCounts[immuneEnemy] >= 5)
                {
                    achieved[Achievements.WhyWontYouDie] = true;
                    saveAchieved();
                    poisonCounts = null;
                }
            }
            else
            {
                poisonCounts[immuneEnemy] = 1;
            }
        }

        public string SummaryString
        {
            get
            {
                string output = "";
                int i = 1;
                foreach(Achievements a in Enum.GetValues(typeof(Achievements)))
                {
                    output += i.ToString("00") + ": ";

                    if (!achieved[a])
                        output += "[Locked] ";

                    output += AchievementName(a) + "\n       " + Conditions(a);

                    i++;

                    if (i <= numAchievements)
                        output += "\n";
                }

                return output;
            }
        }

        public static String AchievementName(Achievements a)
        {
            switch (a)
            {
                case Achievements.Victory:
                    return "Victory!";

                case Achievements.WhyWontYouDie:
                    return "Why Won't You Die?!";

                case Achievements.TheseAreCheaper:
                    return "These Are Cheaper";

                case Achievements.StayAWhile:
                    return "Stay A While";

                case Achievements.MissingThePoint:
                    return "Missing The Point";

                case Achievements.Luddite:
                    return "Luddite";

                case Achievements.GenevaConvention:
                    return "Geneva Convention";

                case Achievements.Miser:
                    return "Miser";

                case Achievements.AttackItsWeakPoint:
                    return "Attack Its Weak Point";

                default:
                    throw new NotImplementedException();
            }
        }

        public static String Conditions(Achievements a)
        {
            switch (a)
            {
                case Achievements.AttackItsWeakPoint:
                    return "Deal 250 poison damage to a single armored enemy.";

                case Achievements.Miser:
                    return "Save up $5000.";

                case Achievements.GenevaConvention:
                    return "Win the game without using poison or spikes.";

                case Achievements.Luddite:
                    return "Win the game without using any upgrades.";

                case Achievements.MissingThePoint:
                    return "Die without using any terrain modification.";

                case Achievements.StayAWhile:
                    return "Slow a fast enemy with 5 different fully upgraded spike traps.";

                case Achievements.TheseAreCheaper:
                    return "Win the game without using any cannons.";

                case Achievements.Victory:
                    return "Win the game.";

                case Achievements.WhyWontYouDie:
                    return "Hit the same poison-immune enemy 5 times with a fully upgraded poison trap.";

                default:
                    throw new NotImplementedException();
            }
        }

        public bool HasAchieved(Achievements ach)
        {
            return achieved[ach];
        }
    }
}
