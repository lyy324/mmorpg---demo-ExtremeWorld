using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Data
{
    public enum NpcType
    {
        None = 0,
        Functional = 1,
        Task,
    }

    public enum NpcFunction
    {
        None = 0,
        InvokeShop = 1,
        InvokeInstance = 2,
    }

    public class NpcDefine
    {

        public int ID { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        
        public NpcType Type { get; set; }

        public NpcFunction Function { get; set; }

        public int Param { get; set; }
    }
}
