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
    internal class Drop
    {
        [AtStartup]
        public static void __()
        {
            Core.DefaultParser.AddCommand(
                Sequence(
                    KeyWord("DROP"),
                    BestScore("SUBJECT",
                        MustMatch("@dont have that",
                            Object("SUBJECT", InScope, PreferHeld)))))
                .ID("StandardActions:Drop")
                .Manual("Drop a held item. This can also be used to remove and drop a worn item.")
                .Check("can drop?", "ACTOR", "SUBJECT")
                .BeforeActing()
                .Perform("drop", "ACTOR", "SUBJECT")
                .AfterActing();

            Core.StandardMessage("you drop", "You drop <the0>.");
            Core.StandardMessage("they drop", "^<the0> drops <a1>.");

            GlobalRules.DeclareCheckRuleBook<Actor, MudObject>("can drop?", "[Actor, Item] : Determine if the item can be dropped.", "actor", "item");
            GlobalRules.DeclarePerformRuleBook<Actor, MudObject>("drop", "[Actor, Item] : Handle an item being dropped.", "actor", "item");

            GlobalRules.Check<Actor, MudObject>("can drop?")
                .First
                .When((actor, item) => !ObjectContainsObject(actor, item))
                .Do((actor, item) =>
                {
                    SendMessage(actor, "@dont have that");
                    return CheckResult.Disallow;
                })
                .Name("Must be holding it to drop it rule.");

            GlobalRules.Check<Actor, MudObject>("can drop?")
                .First
                .When((actor, item) => actor.Contains(item, RelativeLocations.Worn))
                .Do((actor, item) =>
                {
                    if (GlobalRules.ConsiderCheckRule("can remove?", actor, item) == CheckResult.Allow)
                    {
                        GlobalRules.ConsiderPerformRule("remove", actor, item);
                        return CheckResult.Continue;
                    }
                    return CheckResult.Disallow;
                })
                .Name("Dropping worn items follows remove rules rule.");

            GlobalRules.Check<Actor, MudObject>("can drop?").Do((a, b) => CheckResult.Allow).Name("Default can drop anything rule.");

            GlobalRules.Perform<Actor, MudObject>("drop").Do((actor, target) =>
            {
                SendMessage(actor, "@you drop", target);
                SendExternalMessage(actor, "@they drop", actor, target);
                MoveObject(target, actor.Location);
                return PerformResult.Continue;
            }).Name("Default drop handler rule.");
        }
    }
}