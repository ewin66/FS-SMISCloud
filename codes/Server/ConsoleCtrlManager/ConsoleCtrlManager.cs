using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Filter;

namespace FS.SMIS_Cloud.Services.ConsoleCtrlManager
{
    public class ConsoleCtrlManager
    {
        #region member

        private static readonly ILog Log = LogManager.GetLogger("ConsoleCtrlManager");

        private static string Banner =
            @"++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                             ______    ______
                            |  ____|  |  ___ |
                            | |____   | |___
                            |  ____|  | ___  | 
                            | |         ___| |
                            |_|       |______|  

                                                        http://www.anxinyun.cn
++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++";

        private const string Tip = "fs%>:";

        public delegate void EventHandler();

        private delegate void ControlHandler(CtrlType sig);

        public delegate string CmdHandler(string[] args);

        private EventHandler _processExit;
        private Dictionary<string, CmdHandler> _onCmdExecute;
        private bool _cmdState = false;
        private Level _logLevel = Level.Off;

        private delegate bool SysEventHandler(CtrlType sig);

        private static SysEventHandler _sysHandler;

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(SysEventHandler handler, bool add);

        #endregion

        private enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private ConsoleCtrlManager()
        {
            Init();
        }

        private void Init()
        {
            _onCmdExecute = new Dictionary<string, CmdHandler>();
            _sysHandler += new SysEventHandler(SysCtrlHandler);
            SetConsoleCtrlHandler(_sysHandler, true);
            InitLogLevel();
            this.RegCmd("help", OnCmdHelp);
            this.RegCmd("exit", OnCmdExit);
        }

        #region innercmd

        private string OnCmdHelp(string[] args)
        {
            var keys = _onCmdExecute.Keys;
            return string.Join("\r\n", keys.ToArray());
        }

        private string OnCmdExit(string[] args)
        {
            _cmdState = false;
            OpenConsoleAppender();
            return "Exit FS Command Shell.";
        }

        #endregion

        private bool SysCtrlHandler(CtrlType sig)
        {
            switch (sig)
            {
                case CtrlType.CTRL_CLOSE_EVENT:
                case CtrlType.CTRL_BREAK_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                    this.OnExit();
                    break;
                case CtrlType.CTRL_C_EVENT:
                    _cmdState = true;
                    this.OnCmd();
                    return true;
                default:
                    break;
            }
            return true;
        }

        #region log

        private void InitLogLevel()
        {
            var appenders = LogManager.GetRepository().GetAppenders();
            foreach (var appender in appenders)
            {
                if (appender is ConsoleAppender || appender is ColoredConsoleAppender)
                {
                    _logLevel = ((LevelRangeFilter) (((AppenderSkeleton) appender).FilterHead)).LevelMin;
                }
            }
        }

        private void OpenConsoleAppender()
        {
            var appenders = LogManager.GetRepository().GetAppenders();
            foreach (var appender in appenders)
            {
                if (appender is ConsoleAppender || appender is ColoredConsoleAppender)
                {
                    ((AppenderSkeleton) appender).Threshold = _logLevel;
                }
            }
        }

        private void ShutConsoleAppender()
        {
            var appenders = LogManager.GetRepository().GetAppenders();
            foreach (var appender in appenders)
            {
                if (appender is ConsoleAppender || appender is ColoredConsoleAppender)
                {
                    ((AppenderSkeleton) appender).Threshold = Level.Off;
                }
            }
        }

        #endregion

        #region singleinstance

        public static ConsoleCtrlManager Instance
        {
            get { return Nested.instance; }
        }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly ConsoleCtrlManager instance = new ConsoleCtrlManager();
        }

        #endregion

        #region exitevent

        public event EventHandler Exit
        {
            add
            {
                if (value != null)
                {
                    lock (this)
                        _processExit += value;
                }
            }
            remove
            {
                lock (this)
                    _processExit -= value;
            }
        }

        public bool OnExit()
        {
            if (_processExit != null)
            {
                lock (this)
                {
                    _processExit();
                }
            }
            return true;
        }

        #endregion

        #region registercmd

        public void RegCmd(string cmd, CmdHandler handler)
        {
            if (!string.IsNullOrEmpty(cmd) && handler != null)
            {
                if (_onCmdExecute.ContainsKey(cmd))
                {
                    _onCmdExecute[cmd] = handler;
                }
                else
                {
                    _onCmdExecute.Add(cmd, handler);
                }
            }
        }


        public void OnCmd()
        {
            ShutConsoleAppender();
            Console.WriteLine(Banner);
            Console.Write(Tip);
            while (_cmdState)
            {
                var cmd = Convert.ToString(Console.ReadLine());
                Console.WriteLine();
                Log.Info(cmd);
                CmdHandler handler = null;
                string[] param = new string[] {};
                if (!string.IsNullOrEmpty(cmd) && cmd.Contains(" "))
                {
                    var regex = new Regex(@"((""((?<token>.*?)(?<!\\)"")|(?<token>[\w]+))(\s)*)", RegexOptions.None);
                    var tmp = (from Match m in regex.Matches(cmd)
                        where m.Groups["token"].Success
                        select m.Groups["token"].Value).ToArray();
                    if (tmp.Length > 1)
                    {
                        param = new string[tmp.Length - 1];
                        Array.Copy(tmp, 1, param, 0, param.Length);
                    }
                    handler = _onCmdExecute.Where(m => m.Key == tmp[0]).Select(m => m.Value).FirstOrDefault();
                }
                else
                {
                    handler = _onCmdExecute.Where(m => m.Key == cmd).Select(m => m.Value).FirstOrDefault();
                }
                var result = handler != null ? handler(param) : "Not Found Handler !";
                Console.WriteLine(result);
                Console.WriteLine();
                if (_cmdState)
                {
                    Console.Write(Tip);
                }
                Log.Info(result);
            }
        }

        #endregion
    }
}
