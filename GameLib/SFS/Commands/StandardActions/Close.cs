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
        [Initialize]
        public static void __()
        {
            Core.DefaultParser.AddCommand(
                Sequence(
                    KeyWord("CLOSE"),
                    BestScore("SUBJECT",
                        MustMatch("I don't see that here.",
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

            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can close?", "[Actor, Item] : Determine if the item can be closed.", "actor", "item");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("close", "[Actor, Item] : Handle the item being closed.", "actor", "item");

            GlobalRules.Check<Actor, MudObject>("can close?")
                .Do((a, b) =>
                {
                    SendMessage(a, "I don't think the concept of 'open' applies to that.");
                    return CheckResult.Disallow;
                })
                .Name("Default can't close unopenable things rule.");

            GlobalRules.Perform<Actor, MudObject>("close").Do((actor, target) =>
            {
                SendMessage(actor, "You close <the0>.", target);
                SendExternalMessage(actor, "^<the0> closes <the1>.", actor, target);
                return PerformResult.Continue;
            }).Name("Default close reporting rule.");

            GlobalRules.Check<Actor, MudObject>("can close?").First.Do((actor, item) => CheckIsVisibleTo(actor, item)).Name("Item must be visible rule.");
        }
    }
}
