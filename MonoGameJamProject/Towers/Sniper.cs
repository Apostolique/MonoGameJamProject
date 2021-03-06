﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MonoGameJamProject.Towers
{
    class Sniper : Tower
    {
        Minion targetedMinion;
        public Sniper(int iX, int iY, int iHotkeyNumber) : base(iX, iY, 9F, iHotkeyNumber)
        {
            towerColor = Color.LimeGreen;
            type = Utility.TowerType.Sniper;
            minRange = 2;
            maxRange = Math.Max(Utility.board.FullWidth, Utility.board.FullHeight);
            Damage = 250;
            towerInfo = "Sniper Tower\nMin. Range: " + minRange + "\nMax. Range: " + maxRange + "\nDamage: " + Damage + "\nSingle target\nhigh damage tower";
        }
        public override void Update(GameTime gameTime)
        {
            if (!disabled)
            {
                attackTimer.Update(gameTime);
                TargetMinion();
                if (targetedMinion != null)
                {
                    if (attackTimer.IsExpired)
                    {
                        targetedMinion.TakeDamage(Damage);
                        Utility.assetManager.PlaySFX("sniper_shot", 0.75f);
                        attackTimer.Reset();
                    }
                }
            }
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch s)
        {
            base.Draw(s);
            if (!disabled && !(targetedMinion == null))
                s.DrawLine(Utility.GameToScreen(this.X) + Utility.board.GridSize / 2, Utility.GameToScreen(this.Y) + Utility.board.GridSize / 2, Utility.GameToScreen(targetedMinion.Position.X), Utility.GameToScreen(targetedMinion.Position.Y), Color.Red, 2f);
        }
        private void TargetMinion()
        {
            targetedMinion = null;
            foreach (Path p in Utility.board.Paths)
            {
                foreach (Minion m in p.MinionList)
                {
                    if (RangeChecker(m.Position.X, m.Position.Y, MinimumRange))
                        continue;
                    else if (targetedMinion == null)
                        targetedMinion = m;
                    else if (m.maxHP > targetedMinion.maxHP)
                    {
                        if (m.maxHP == targetedMinion.maxHP && m.DistanceTraveled > targetedMinion.DistanceTraveled)
                            targetedMinion = m;
                        else if (m.maxHP != targetedMinion.maxHP)
                            targetedMinion = m;
                    }
                }
            }
        }
    }
}
