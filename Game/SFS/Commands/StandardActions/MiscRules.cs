using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFS;

namespace SFS.Commands.StandardActions
{
    public static class MiscRules
    {
        public static void AtStartup(SFSRuleEngine GlobalRules)
        {
            // This rule is never called in a single player context.
            GlobalRules.Perform<MudObject>("player joined")
                .Last
                .Do((actor) =>
                {
                    Core.EnqueuActorCommand(actor, "look");
                    return SFS.Rules.PerformResult.Continue;
                })
                .Name("New players look rule.");
        }
    }
}
