using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace SFS
{
    [System.AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    sealed class AtStartupAttribute : Attribute
    {
    }

    public static partial class Core
    {
        /// <summary>
        /// Start the mud engine.
        /// </summary>
        /// <param name="Database"></param>
        /// <returns></returns>
        public static bool Start()
        {
            try
            {
                // Setup the rule engine and some basic rules.
                GlobalRules = new SFSRuleEngine();
                GlobalRules.DeclarePerformRuleBook("at startup", "[] : Considered when the engine is started.");

                DefaultParser = new CommandParser();

                foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
                    foreach (var method in type.GetMethods())
                    {
                        var atStartup = method.GetCustomAttribute<AtStartupAttribute>();
                        if (atStartup != null)
                            method.Invoke(null, null);
                    }

                InitializeCommandProcessor();
                GlobalRules.FinalizeNewRules();

                Database = new CompiledDatabase();

                GlobalRules.ConsiderPerformRule("at startup");
            }
            catch (Exception e)
            {
                //LogError("Failed to start mud engine.");
                //LogError(e.Message);
                //LogError(e.StackTrace);
                throw;
            }
            return true;
        }
    }
}
