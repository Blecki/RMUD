using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SFS.Core;
using SFS.Rules;

namespace SFS
{
    /// <summary>
    /// Anything that might supply rules for consideration must implement RuleSource. Any object implementing RuleSource
    /// passed as an argument to a consider or value rule function will have it's rules considered when executing the
    /// rulebook.
    /// </summary>
    public partial class MudObject
    {
        public RuleSet Rules { get; private set; }

        public PerformResult ConsiderPerformRule(String Name, params Object[] Arguments)
        {
            return GlobalRules.ConsiderPerformRule(Name, Arguments);
        }

        public CheckResult ConsiderCheckRule(String Name, params Object[] Arguments)
        {
            return GlobalRules.ConsiderCheckRule(Name, Arguments);
        }

        public RT ConsiderValueRule<RT>(String Name, params Object[] Arguments)
        {
            return GlobalRules.ConsiderValueRule<RT>(Name, Arguments);
        }

        public void DeleteRule(String RuleBookName, String RuleID)
        {
            if (Rules != null) Rules.DeleteRule(RuleBookName, RuleID);
        }

        public void DeleteAllRules(String RuleID)
        {
            if (Rules != null) Rules.DeleteAll(RuleID);
        }
    }
}
