using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MonoGameJamProject
{
    /// <summary>
    /// Goal: Base class for the game's pathways.
    ///       Steps:
    ///         1) Pick a tile on the edges.
    ///         2) Make a list of all the surrounding tiles.
    ///         3) Eliminate tiles that have a path of an other color.
    ///         4) Pick one of the available tiles based on:
    ///             a) If path is short, bias towards the center.
    ///             b) If path is long, bias towars the edges.
    ///             c) Also bias for longer paths if the mode is easy.
    /// </summary>
    class Path
    {
        public enum Animation { spawn, despawn, none }
        public List<Tile> pathway;
        public List<Minion> MinionList;
        private Spawner spawner;
        private CoolDownTimer spawnSequenceTimer;
        private CoolDownTimer minionSpawner;
        private int pathsShown;
        public Animation Sequence
        {
            get; set;
        }
        private bool _done;
        public bool Done
        {
            get => _done;
        }

        public Path()
        {
            pathway = new List<Tile>();
            MinionList = new List<Minion>();
            spawnSequenceTimer = new CoolDownTimer(0.2f);
            spawnSequenceTimer.Reset();
            pathsShown = 0;
            Sequence = Animation.spawn;
            minionSpawner = new CoolDownTimer(10f);
            minionSpawner.Reset();
            spawner = new Spawner();
            _done = false;
        }

        public void Add(Tile p)
        {
            pathway.Add(p);
        }
        public Tile First()
        {
            return pathway.First();
        }
        public Tile Last()
        {
            return pathway.Last();
        }
        public int Count()
        {
            return pathway.Count;
        }
        public bool Contains(Tile tile)
        {
            if (Sequence == Animation.spawn) {
                for (int i = 0; i < pathsShown; i++)
                {
                    if (pathway[i] == tile) {
                        return true;
                    }
                }
            } else if (Sequence == Animation.despawn) {
                for (int i = pathsShown; i < pathway.Count; i++)
                {
                    if (pathway[i] == tile) {
                        return true;
                    }
                }
            } else {
                return pathway.Contains(tile);
            }

            return false;
        }
        public void Despawn()
        {
            spawnSequenceTimer.Reset();
            pathsShown = 0;
            Sequence = Animation.despawn;
        }
        public void AddMinion(Minion m)
        {
            m.FollowPath(this);
            MinionList.Add(m);
        }
        public void Update(GameTime gameTime)
        {
            spawner.Update(gameTime, this);
            minionSpawner.Update(gameTime);
            if (minionSpawner.IsExpired && spawner.IsActive)
            {
                spawner.IsActive = false;
            }
            if (!spawner.IsActive && MinionList.Count == 0 && Sequence != Animation.despawn)
            {
                Despawn();
            }
            if (Sequence == Animation.spawn)
            {
                spawnSequenceTimer.Update(gameTime);
                if (spawnSequenceTimer.IsExpired)
                {
                    pathsShown++;
                    Utility.assetManager.PlaySFX("Robot_Servo_006", 0.1f);
                    spawnSequenceTimer.Reset();
                    if (pathsShown >= pathway.Count)
                    {
                        Sequence = Animation.none;
                    }
                }
            } else if (Sequence == Animation.despawn)
            {
                spawnSequenceTimer.Update(gameTime);
                if (spawnSequenceTimer.IsExpired)
                {
                    pathsShown++;
                    Utility.assetManager.PlaySFX("Robot_Servo_006", 0.1f);
                    spawnSequenceTimer.Reset();
                    if (pathsShown >= pathway.Count)
                    {
                        Sequence = Animation.none;
                        _done = true;
                    }
                }
            }
            for(int i = MinionList.Count - 1; i >= 0; i--)
            {
                MinionList[i].Update(gameTime);
                if (MinionList[i].dead)
                {
                    MinionList.Remove(MinionList[i]);
                    Utility.totalNumberOfKills++;
                    Utility.assetManager.PlaySFX("Death");
                }
                else if (!MinionList[i].IsMoving)
                {
                    MinionList.Remove(MinionList[i]);
                    Utility.numberOfLives--;
                    Utility.assetManager.PlaySFX("BREAK_HIT_Celery_2");
                }

            }
        }

        public void Draw(SpriteBatch s)
        {
            if (Sequence == Animation.spawn)
            {
                for (int i = 0; i < pathsShown; i++)
                {
                    DrawPathTile(s, i);
                }
            } else if (Sequence == Animation.despawn)
            {
                for (int i = pathsShown; i < pathway.Count; i++)
                {
                    DrawPathTile(s, i);
                }
            } else
            {
                for (int i = 0; i < pathway.Count; i++) 
                {
                    DrawPathTile(s, i);
                }
            }
        }
        public void DrawLine(SpriteBatch s)
        {
            if (Sequence == Animation.spawn)
            {
                for (int i = 1; i < pathsShown; i++)
                {
                    DrawPathLine(s, i);
                }
            } else if (Sequence == Animation.despawn)
            {
                for (int i = pathsShown + 1; i < pathway.Count; i++)
                {
                    DrawPathLine(s, i);
                }
            } else
            {
                for (int i = 1; i < pathway.Count; i++)
                {
                    DrawPathLine(s, i);
                }
            }
        }
        public void DrawMinions(SpriteBatch s)
        {
            foreach (Minion m in MinionList)
                m.Draw(s);
        }
        private void DrawPathTile(SpriteBatch s, int i)
        {
            s.FillRectangle(new Rectangle(Utility.GameToScreen(pathway[i].X), Utility.GameToScreen(pathway[i].Y), Utility.board.GridSize, Utility.board.GridSize), new Color(19, 59, 131));
        }
        private void DrawPathLine(SpriteBatch s, int i)
        {
            float x1 = Utility.GameToScreen(pathway[i].X + 0.5f);
            float y1 = Utility.GameToScreen(pathway[i].Y + 0.5f);
            float x2 = Utility.GameToScreen(pathway[i - 1].X + 0.5f);
            float y2 = Utility.GameToScreen(pathway[i - 1].Y + 0.5f);

            s.DrawLine(x1, y1, x2, y2, Color.Black, Utility.board.GridSize * 0.15f);
            s.DrawLine(x1, y1, x2, y2, Color.White, Utility.board.GridSize * 0.1f);
        }
    }
}
