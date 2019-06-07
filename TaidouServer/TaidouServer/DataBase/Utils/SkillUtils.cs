using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaidouCommon.Model;

namespace TaidouServer.DataBase.Utils
{
    class SkillUtils
    {
        public IList<Skill> GetSkillByRole(Role role)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    var ans = session.QueryOver<Skill>().Where(x => x.role == role);
                    transaction.Commit();
                    return ans.List();
                }
            }
        }


        public void AddSkill(Skill skill)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Save(skill);
                    transaction.Commit();
                }
            }
        }

        public void UpdateSkill(Skill skill)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Update(skill);
                    transaction.Commit();
                }
            }
        }

        public IList<Skill> GetOneSkill(Role role,int pos)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    var ans = session.QueryOver<Skill>().Where(x => (x.role == role&&x.pos==pos));
                    transaction.Commit();
                    return ans.List();
                }
            }
        }
    }
}
