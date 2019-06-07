using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaidouCommon.Model;

namespace TaidouServer.DataBase.Mapping
{
    class SkillMap:ClassMap<Skill>
    {
        public SkillMap()
        {
            Id(x => x.id).Column("id");
            Map(x => x.pos).Column("pos");
            References(x => x.role).Column("roleid");
            Map(x => x.level).Column("level");
        }
    }
}
