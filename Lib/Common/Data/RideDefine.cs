using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Data
{
    public class RideDefine
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public string Resource { get; set; }
        public string Icon { get; set; }
        // [已删除] public int Speed { get; set; } — JSON中无此字段
    }
}
