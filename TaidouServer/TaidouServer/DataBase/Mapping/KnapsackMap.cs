using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaidouCommon.Model;

namespace TaidouServer.DataBase.Mapping
{
    class KnapsackMap:ClassMap<Knapsack>
    {
        public KnapsackMap()
        {
            Id(x => x.id).Column("id");
            Map(x => x.toolid).Column("toolid");
            Map(x => x.count).Column("count");
            Map(x => x.level).Column("level");
            Map(x => x.hpadd).Column("hpadd");
            Map(x => x.poweradd).Column("poweradd");
            References(x => x.role).Column("roleid");
            Map(x => x.isdressd).Column("isdressed");
        }
    }
}
