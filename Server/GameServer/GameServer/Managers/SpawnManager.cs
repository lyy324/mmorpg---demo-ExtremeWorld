using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GameServer.Models;

namespace GameServer.Managers
{
    class SpawnManager
    {
        private List<Spawner> Rules = new List<Spawner>();
        private Map Map;

        //生成刷怪器，一个刷怪器对应一条刷怪规则
        public void Init(Map map)
        {
            this.Map = map;
            if (DataManager.Instance.SpawnRules.ContainsKey(map.Define.ID))
            {
                foreach (var define in DataManager.Instance.SpawnRules[map.Define.ID].Values)
                {
                    this.Rules.Add(new Spawner(define, this.Map));
                }
            }
        }

        public void Update()
        {
            if (Rules.Count == 0)
            {
                return;
            }
            for (int i = 0; i < this.Rules.Count; i++)
            {
                this.Rules[i].Update();
            }
        }
    }
}
