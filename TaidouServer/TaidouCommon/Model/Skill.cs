using System;
using System.Collections.Generic;
using System.Text;

namespace TaidouCommon.Model
{
    public class Skill
    {
        public virtual int id { get; set; }
        public virtual int pos { get; set; }
        public virtual Role role { get; set; }
        public virtual int level { get; set; }

    }
}
