using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaidouCommon.Model;

namespace TaidouServer.DataBase.Utils
{
    class RoleUtil
    {
        public IList<Role> GetRoleByUser(User user)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    var ans = session.QueryOver<Role>().Where(x => x.user == user);
                    transaction.Commit();
                    return ans.List();
                }
            }
        }
        /// <summary>
        /// 传入的role要有user信息
        /// </summary>
        /// <param name="role"></param>
        public void AddRole(Role role)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Save(role);
                    transaction.Commit();
                }
            }
        }
        /// <summary>
        /// 完整的role信息
        /// </summary>
        /// <param name="role"></param>
        public void Delete(Role role)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Delete(role);
                    transaction.Commit();
                }
            }
        }

        public IList<Role> GetRoleByName(string name)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    var res= session.QueryOver<Role>().Where(x => x.name == name);
                    transaction.Commit();
                    return res.List();
                }
            }
        }

        public void UpdateRole(Role role)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    session.Update(role);
                    transaction.Commit();
                }
            }
        }
    }
}
