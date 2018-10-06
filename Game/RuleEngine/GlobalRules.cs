using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace SharpRuleEngine
{
    public class RuleAssembly
    {
        public Assembly Assembly;
        public String BaseNameSpace = "Don't load me.";

        public RuleAssembly(Assembly Assembly, String BaseNameSpace)
        {
            this.Assembly = Assembly;
            this.BaseNameSpace = BaseNameSpace;
        }
    }

    public static class GlobalRules
    {
        internal static RuleEngine RuleEngine;
      
        public static void InitializeRuleEngine(params RuleAssembly[] Assemblies)
        {
            RuleEngine = new RuleEngine(NewRuleQueueingMode.QueueNewRules);

            foreach (var assembly in Assemblies)
            {
                foreach (var type in assembly.Assembly.GetTypes())
                    if (type.FullName.StartsWith(assembly.BaseNameSpace))
                        foreach (var method in type.GetMethods())
                            if (method.IsStatic && method.Name == "DeclareRules")
                                try { method.Invoke(null, new Object[] { RuleEngine }); }
                                catch (Exception e) { }
            }

            RuleEngine.FinalizeNewRules();
        }
    }
}
