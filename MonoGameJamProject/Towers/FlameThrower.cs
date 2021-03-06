﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using MonoGame.Extended;

namespace MonoGameJamProject.Towers
{
    class FlameThrower : Tower
    {
        private const int amountOfFlamesPerCycle = 200;
        private List<Projectile> flameList;
        List<Point> damageTiles;
        public float BurnTime => 5;
        public FlameThrower(int iX, int iY, int iHotkeyNumber) : base(iX, iY, 1F, iHotkeyNumber)
        {
            towerColor = Color.OrangeRed;
            damageTiles = new List<Point>();
            flameList = new List<Projectile>();
            type = Utility.TowerType.FlameThrower;
            Damage = 2;
            minRange = 0;
            maxRange = 2;
            towerInfo = "Flame Turret\nMin. Range: " + minRange + "\nMax. Range: " + maxRange + "\nDamage: " + Damage + "\nLights minions on fire\nuse multiple to stack DMG";
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!disabled && attackTimer.IsExpired)
            {
                GenerateDamageTiles();
                CheckDamageTiles();
                GenerateFireEffect();
                attackTimer.Reset();
            }

            for (int x = flameList.Count - 1; x >= 0; x--)
            {
                if (flameList[x].MarkedForDeletion)
                    flameList.Remove(flameList[x]);
            }
            foreach (Projectile f in flameList)
            {
                f.Update(gameTime);
                // bullets start at the center, therefore an extra 0.5f is added to the range
                if (f.DistanceTravelled > maxRange + 0.5f)
                    f.MarkedForDeletion = true;
            }
        }
        public override void Draw(SpriteBatch s)
        {
            base.Draw(s);
        }

        public void DrawFireEffect(SpriteBatch s)
        {
            foreach (Projectile f in flameList)
                f.Draw(s);
        }

        private void GenerateFireEffect()
        {
            for (int i = 0; i <= amountOfFlamesPerCycle; i++)
            {
                Vector2 randomDirection = RandomDirectionGenerator();
                Projectile flame = new Projectile(new Vector2(this.X + 0.5f, this.Y + 0.5f), randomDirection, Color.Orange, 3.5F);
                flameList.Add(flame);
            }
        }

        private Vector2 RandomDirectionGenerator()
        {
            Vector2 randomDirection = new Vector2((float)Utility.random.NextDouble(), (float)Utility.random.NextDouble());
            int r = Utility.random.Next(0, 5);
            if (r == 2)
                randomDirection.X = -randomDirection.X;
            else if (r == 3)
                randomDirection.Y = -randomDirection.Y;
            else if (r == 4)
                randomDirection = -randomDirection;
            return randomDirection;
        }
        public void GenerateDamageTiles()
        {
            damageTiles.Clear();
            for(int i = -maxRange; i <= maxRange; i++)
            {
                for(int j = -maxRange; j <= maxRange; j++)
                {
                    damageTiles.Add(new Point(X + i, Y + j));
                }
            }
        }
        public void CheckDamageTiles()
        {
            foreach (Path p in Utility.board.Paths)
            {
                foreach(Minion m in p.MinionList)
                {
                    if (!RangeChecker(m.Position.X, m.Position.Y, maxRange))
                        continue;
                    foreach (Point point in damageTiles)
                    {
                        if (m.IsInTile(point.X, point.Y))
                        {
                            m.AddFireStack(this);
                        }
                    }
                }
            }
        }
    }
}
