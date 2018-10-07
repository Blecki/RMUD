using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD
{
    public static partial class Core
    {
        internal static Mutex DatabaseLock = new Mutex();
        public static bool ShuttingDown { get; private set; }
        public static Settings SettingsObject;
        public static String DatabasePath;
        public static WorldDataService Database;
        public static RuleEngine GlobalRules;
        public static Action OnShutDown = null;
        private static StartupFlags Flags;

        public static bool Silent { get { return (Flags & StartupFlags.Silent) == StartupFlags.Silent; } }
        public static bool NoLog { get { return (Flags & StartupFlags.NoLog) == StartupFlags.NoLog; } }

        public static void Shutdown()
        {
            ShuttingDown = true;
        }
    }
}
