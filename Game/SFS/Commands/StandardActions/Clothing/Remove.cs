using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;
using SFS.Rules;
using static SFS.CommandFactory;
using static SFS.Core;

namespace SFS.Commands.StandardActions
{
	internal class Remove
	{
        [AtStartup]
        public static void __()
        {
            Core.DefaultParser.AddCommand(
                Sequence(
                    KeyWord("REMOVE"),
                    BestScore("OBJECT",
                        MustMatch("@clothing remove what",
                            Object("OBJECT", InScope, PreferWorn)))))
                .Manual("Expose your amazingly supple flesh.")
                .Check("can remove?", "ACTOR", "OBJECT")
                .BeforeActing()
                .Perform("remove", "ACTOR", "OBJECT")
                .AfterActing();

            Core.GlobalRules.DeclareCheckRuleBook<Actor, Clothing>("can remove?", "[Actor, Item] : Can the actor remove the item?", "actor", "item");
            Core.GlobalRules.DeclarePerformRuleBook<Actor, Clothing>("remove", "[Actor, Item] : Handle the actor removing the item.", "actor", "item");

            Core.GlobalRules.Check<Actor, Clothing>("can remove?")
                .When((a, b) => !a.Contains(b, RelativeLocations.Worn))
                .Do((actor, item) =>
                {
                    SendMessage(actor, "@clothing not wearing");
                    return CheckResult.Disallow;
                });

            Core.GlobalRules.Check<Actor, Clothing>("can remove?").Do((a, b) => CheckResult.Allow);

            Core.GlobalRules.Perform<Actor, Clothing>("remove").Do((actor, target) =>
                {
                    SendMessage(actor, "@clothing you remove", target);
                    SendExternalMessage(actor, "@clothing they remove", actor, target);
                    MoveObject(target, actor, RelativeLocations.Held);
                    return PerformResult.Continue;
                });
        }
    }
}
