using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using log4net;
using log4net.Config;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaidouCommon;
using TaidouServer.Handler;
using TaidouCommon.Model;

namespace TaidouServer
{
    class TaidouApplication : ApplicationBase
    {
        #region Constants and Fields
        //单例
        private static TaidouApplication instance;
        //持有log对象,log在SetLoggerFactory后应该会更新，不然无法解释
        public static readonly ILogger log = ExitGames.Logging.LogManager.GetCurrentClassLogger();
        //handler存储字典
        private Dictionary<OperationCode, HandlerBase> handlers = new Dictionary<OperationCode, HandlerBase>();
        //副本组队需要的人数
        public static int RAIDNEEDPLAYER = 2; 
        #endregion

        #region Properties
        public static new TaidouApplication Instance { get { return instance; } }
        //副本组队队列，int为副本id,队列参加副本组队的peer
        public Dictionary<int, Queue<TaidouPeer>> raidTeam = new Dictionary<int, Queue<TaidouPeer>>();
        //副本同步字典，通过副本队长名称唯一确定
        public Dictionary<string, List<SynPlayer>> serverPlayers = new Dictionary<string, List<SynPlayer>>();
        //副本peer对象存储字典，通过副本队长名字唯一确定
        public Dictionary<string, List<TaidouPeer>>peerTeams = new Dictionary<string, List<TaidouPeer>>();

        #endregion

        #region Method
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            return new TaidouPeer(initRequest);
        }

        protected override void Setup()
        {
            //单例赋值
            instance = this;
            //注册handler
            RegistHandlers();
            //创建日志实例工厂
            ExitGames.Logging.LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
            //设置属性值：应用程序日志路径- 应用的根路径(deploy)/log
            GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(this.ApplicationRootPath, "log");
            //设置属性值：日志名称
            GlobalContext.Properties["LogFileName"] = "Taidou" + this.ApplicationName;
            //读取路径为deploy/项目/bin下的config文件（文件拷贝模板至bin下，删除模板类相关信息）
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(this.BinaryPath, "log4net.config")));

            log.Debug("==========TaidouApplication v4.5 Setup complete.==========");

        }

        protected override void TearDown()
        {
            log.Debug("==========TaidouApplication v4.5 TearDown finish.==========");
        }
        //注册handler,避免重复new
        private void RegistHandlers()
        {
            handlers.Add(OperationCode.Login, new LoginHandler());
            handlers.Add(OperationCode.GetServerList, new ServerHandler());
            handlers.Add(OperationCode.Register, new RegisterHandler());
            handlers.Add(OperationCode.Role, new RoleHandler());
            handlers.Add(OperationCode.Task, new TaskHandler());
            handlers.Add(OperationCode.Knapsack, new KnapsackHandler());
            handlers.Add(OperationCode.Skill, new SkillHandler());
            handlers.Add(OperationCode.Team, new TeamHandler());
            handlers.Add(OperationCode.Sync, new SyncPlayerHandler());
            handlers.Add(OperationCode.EnemySync, new EnemyHandler());
            handlers.Add(OperationCode.ExitTeam, new ExitTeamHandler());
        }

        #endregion

        #region Public Method
        public HandlerBase GetHandler(OperationCode code)
        {
            HandlerBase handler = null;
            if (handlers.TryGetValue(code, out handler))
            {
                return handler;
            }
            log.Debug("********* No exits handler:" + code.ToString() + " *********");
            return handler;
        }
        #endregion
    }
}
