using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace SFS
{
    public enum StartupFlags
    {
        Silent = 1,
        SearchDirectory = 2,
        NoLog = 8
    }

    public static partial class Core
    {
        /// <summary>
        /// Integrate a module into the running game by calling every static AtStartup method found on any type
        /// in the module's base namespace.
        /// </summary>
        /// <param name="Module"></param>
        private static void IntegrateModule(ModuleAssembly Module)
        {
            if (Module == null) throw new InvalidOperationException("Tried to load null module");
            if (Module.Assembly == null) throw new InvalidOperationException("Tried to load invalid module assembly - " + Module.FileName);

            DefaultParser.ModuleBeingInitialized = Module.FileName;

            foreach (var type in Module.Assembly.GetTypes())
                    foreach (var method in type.GetMethods())
                        if (method.IsStatic && method.Name == "AtStartup")
                        {
                            var methodParameters = method.GetParameters();

                            if (methodParameters.Length == 0)
                            {
                                try
                                {
                                    method.Invoke(null, null);
                                }
                                catch (Exception e)
                                {
                                    //LogWarning("Error while loading module " + Module.FileName + " : " + e.Message);
                                }
                            }
                            else if (methodParameters.Length == 1 && methodParameters[0].ParameterType == typeof(SFSRuleEngine))
                            {
                                try
                                {
                                    method.Invoke(null, new Object[] { GlobalRules });
                                }
                                catch (Exception e)
                                {
                                    //LogWarning("Error while loading module " + Module.FileName + " : " + e.Message);
                                }
                            }
                            else
                            {
                                //LogWarning("Error while loading module " + Module.FileName + " : AtStartup method had incompatible signature.");
                            }
                        }
        }

        /// <summary>
        /// Start the mud engine.
        /// </summary>
        /// <param name="Flags">Flags control engine functions</param>
        /// <param name="Database"></param>
        /// <param name="Assemblies">Modules to integrate</param>
        /// <returns></returns>
        public static bool Start(WorldDataService Database)
        {
            try
            {
                // Setup the rule engine and some basic rules.
                GlobalRules = new SFSRuleEngine();
                GlobalRules.DeclarePerformRuleBook("at startup", "[] : Considered when the engine is started.");

                DefaultParser = new CommandParser();

                IntegrateModule(new ModuleAssembly(Assembly.GetExecutingAssembly(), "Core.dll"));

                InitializeCommandProcessor();

                GlobalRules.FinalizeNewRules();

                Core.Database = Database;
                Database.Initialize();

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
