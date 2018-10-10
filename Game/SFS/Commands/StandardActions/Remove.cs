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
                        MustMatch("I couldn't figure out what you're trying to remove.",
                            Object("OBJECT", InScope, PreferWorn)))))
                .Manual("Expose your amazingly supple flesh.")
                .Check("can remove?", "ACTOR", "OBJECT")
                .BeforeActing()
                .Perform("remove", "ACTOR", "OBJECT")
                .AfterActing();

            GlobalRules.DeclareCheckRuleBook<Actor, Clothing>("can remove?", "[Actor, Item] : Can the actor remove the item?", "actor", "item");
            GlobalRules.DeclarePerformRuleBook<Actor, Clothing>("remove", "[Actor, Item] : Handle the actor removing the item.", "actor", "item");

            GlobalRules.Check<Actor, Clothing>("can remove?")
                .When((a, b) => !a.Contains(b, RelativeLocations.Worn))
                .Do((actor, item) =>
                {
                    SendMessage(actor, "You aren't actually wearing that.");
                    return CheckResult.Disallow;
                });

            GlobalRules.Check<Actor, Clothing>("can remove?").Do((a, b) => CheckResult.Allow);

            GlobalRules.Perform<Actor, Clothing>("remove").Do((actor, target) =>
                {
                    SendMessage(actor, "You take off <the0>.", target);
                    SendExternalMessage(actor, "^<the0> takes off <the1>.", actor, target);
                    MoveObject(target, actor, RelativeLocations.Held);
                    return PerformResult.Continue;
                });
        }
    }
}
