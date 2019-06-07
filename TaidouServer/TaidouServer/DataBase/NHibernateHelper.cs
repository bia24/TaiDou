using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaidouServer.DataBase
{
    class NHibernateHelper
    {
        private static ISessionFactory sessionFactory = null;

        private static void InitializeSession()
        {
            //Standard静态方法返回一个MySQLConfiguration对象实例，
            //调用实体里的ConnectionString方法对它进行各项内容的配置,
            //上述方法参数是action<MySQLConnectionBuilder>的委托,
            //输入一个MySQLConnectionBuilder，并调用封装好的各个方法对builder赋值
            //ConnectionString返回构造好的MySQLConfiguration。
            MySQLConfiguration config = MySQLConfiguration.Standard.ConnectionString(
                db => db.Server("localhost").Database("taidou")
                    .Username("root").Password("123456"));
            
            //利用上面的数据库信息开始配置Nhibernate
            //应用mappings进行映射，将当前类所在的程序集中的所有映射添加到配置映射配置中
            //使用fluentConfiguration构造Isessionfactory
            sessionFactory =
                Fluently.Configure().Database(config).Mappings(x => x.FluentMappings.AddFromAssemblyOf<NHibernateHelper>())
                .BuildSessionFactory();


        }
        //单例的get方法，私有是因为外界无法获得factory，只能获得session
        private static ISessionFactory SessionFactory
        {
            get
            {
                if (sessionFactory == null)
                {
                    InitializeSession();
                }
                return sessionFactory;
            }
        }
        //外界获取session的接口
        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}
