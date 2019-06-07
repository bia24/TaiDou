using System;
using System.Collections.Generic;
using System.Text;

namespace TaidouCommon.Model
{
    public class User
    {
        public virtual int id { get; set; }
        public virtual string username { get; set; }
        public virtual string password { get; set; }

    }
}
