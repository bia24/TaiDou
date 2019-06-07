using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaidouCommon.Model;

namespace TaidouServer.DataBase.Utils
{
    class KnapsackUtil
    {
        public IList<Knapsack> GetKnapsackByRole(Role role)
        {
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    var ans = session.QueryOver<Knapsack>().Where(x=>x.role==role);
                    transaction.Commit();
                    return ans.List();
                }
            }
        }
        public void DeleteAllKnapsackByRole(Role role)
        {
            IList<Knapsack> res = null;
            //查找所有背包信息集合
            using (ISession session = NHibernateHelper.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    var ans = session.QueryOver<Knapsack>().Where(x => x.role == role);
                    transaction.Commit();
                    res = ans.List();
                }
            }
            //删除
            if (res.Count == 0) return;
            else
            {
                foreach(var t in res)
                {
                    using (ISession session = NHibernateHelper.OpenSession())
                    {
                        using (ITransaction transaction = session.BeginTransaction())
                        {
                            session.Delete(t);
                            transaction.Commit();
                        }
                    }
                }
            }
        }

        public void AddKnapsack(List<Knapsack> knaps)
        {
            foreach(var t in knaps)
            {
                using (ISession session = NHibernateHelper.OpenSession())
                {
                    using (ITransaction transaction = session.BeginTransaction())
                    {
                        session.Save(t);
                        transaction.Commit();
                    }
                }
            }
        }
    }
}
