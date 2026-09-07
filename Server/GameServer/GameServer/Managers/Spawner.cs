using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using Common.Data;
using GameServer.Models;

namespace GameServer.Managers
{
    class Spawner
    {
        private Map Map;
        public SpawnRuleDefine Define { get; set; }
        private SpawnPointDefine spawnPoint = null;
        //刷新时间
        private float SpawnTime = 0;
        //消失时间
        private float UnSpawnTime = 0;
        private bool spawned = false;

        public Spawner(SpawnRuleDefine define, Map map)
        {
            this.Define = define;
            this.Map = map;

            if (DataManager.Instance.SpawnPoints.ContainsKey(this.Map.ID))
            {
                if (DataManager.Instance.SpawnPoints[this.Map.ID].ContainsKey(this.Define.SpawnPoint))
                {
                    spawnPoint = DataManager.Instance.SpawnPoints[this.Map.ID][this.Define.SpawnPoint];
                }
            }
            else
            {
                Log.ErrorFormat("SpawnRule {0} SpawnPoint {1} not existed", this.Define.ID, this.Define.SpawnPoint);
            }
        }

        internal void Update()
        {
            //每一帧检测是否可以刷
            if (this.CanSpawn())
            {
                this.Spawn();
            }
        }

        //刷怪
        private void Spawn()
        {
            this.spawned = true;
            Log.InfoFormat("Map:{0}  Spawn:{1} Mon:{2} Lv:{3} At Point {4}", this.Define.MapID, this.Define.ID, this.Define.SpawnMonID, this.Define.SpawnLevel, this.Define.SpawnPoint);
            this.Map.MonsterManager.Create(this.Define.SpawnMonID, this.Define.SpawnLevel, this.spawnPoint.Position, this.spawnPoint.Direction);
        }

        bool CanSpawn()
        {
            if (this.spawned)
            {
                return false;
            }
            if (this.UnSpawnTime + this.Define.SpawnPeriod > Time.time)
            {
                return false;
            }
            return true;
        }
    }
}
