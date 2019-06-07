using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaidouCommon.Model;

namespace TaidouServer.DataBase.Mapping
{
    class RoleMap:ClassMap<Role>
    {
        public RoleMap()
        {
            Id(x => x.id).Column("id");
            //外键映射
            References(x => x.user).Column("userid");
            Map(x => x.name).Column("name");
            Map(x => x.isman).Column("isman");
            Map(x => x.level).Column("level");
            Map(x => x.power).Column("power");
            Map(x => x.exp).Column("exp");
            Map(x => x.diamond).Column("diamond");
            Map(x => x.gold).Column("gold");
            Map(x => x.physical).Column("physical");
            Map(x => x.energy).Column("energy");
        }
           
    }
}
