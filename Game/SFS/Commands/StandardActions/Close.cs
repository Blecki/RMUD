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
	internal class OpenClose
	{
        [AtStartup]
        public static void __()
        {
            Core.DefaultParser.AddCommand(
                Sequence(
                    KeyWord("CLOSE"),
                    BestScore("SUBJECT",
                        MustMatch("@not here",
                            Object("SUBJECT", InScope, (actor, thing) =>
                            {
                                if (GlobalRules.ConsiderCheckRuleSilently("can close?", actor, thing) == CheckResult.Allow) return MatchPreference.Likely;
                                return MatchPreference.Unlikely;
                            })))))
                .ID("StandardActions:Close")
                .Manual("Closes a thing.")
                .Check("can close?", "ACTOR", "SUBJECT")
                .BeforeActing()
                .Perform("close", "ACTOR", "SUBJECT")
                .AfterActing();

            Core.StandardMessage("you close", "You close <the0>.");
            Core.StandardMessage("they close", "^<the0> closes <the1>.");

            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can close?", "[Actor, Item] : Determine if the item can be closed.", "actor", "item");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("close", "[Actor, Item] : Handle the item being closed.", "actor", "item");

            GlobalRules.Check<Actor, MudObject>("can close?")
                .Do((a, b) =>
                {
                    SendMessage(a, "@not openable");
                    return CheckResult.Disallow;
                })
                .Name("Default can't close unopenable things rule.");

            GlobalRules.Perform<Actor, MudObject>("close").Do((actor, target) =>
            {
                SendMessage(actor, "@you close", target);
                SendExternalMessage(actor, "@they close", actor, target);
                return PerformResult.Continue;
            }).Name("Default close reporting rule.");

            GlobalRules.Check<Actor, MudObject>("can close?").First.Do((actor, item) => CheckIsVisibleTo(actor, item)).Name("Item must be visible rule.");
        }
    }
}
