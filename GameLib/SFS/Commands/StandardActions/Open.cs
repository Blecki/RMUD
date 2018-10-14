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
    internal class Open
    {
        [Initialize]
        public static void __()
        {
            Core.DefaultParser.AddCommand(
                Sequence(
                    KeyWord("OPEN"),
                    BestScore("SUBJECT",
                        MustMatch("I don't see that here.",
                            Object("SUBJECT", InScope, (actor, thing) =>
                            {
                                if (GlobalRules.ConsiderCheckRuleSilently("can open?", actor, thing) == SFS.Rules.CheckResult.Allow) return MatchPreference.Likely;
                                return MatchPreference.Unlikely;
                            })))))
                .ID("StandardActions:Open")
                .Manual("Opens an openable thing.")
                .Check("can open?", "ACTOR", "SUBJECT")
                .BeforeActing()
                .Perform("open", "ACTOR", "SUBJECT")
                .AfterActing();

            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can open?", "[Actor, Item] : Can the actor open the item?", "actor", "item");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("open", "[Actor, Item] : Handle the actor opening the item.", "actor", "item");

            GlobalRules.Check<Actor, MudObject>("can open?")
                .Do((a, b) =>
                {
                    SendMessage(a, "I don't think the concept of 'open' applies to that.");
                    return SFS.Rules.CheckResult.Disallow;
                })
                .Name("Can't open the unopenable rule.");

            GlobalRules.Perform<Actor, MudObject>("open").Do((actor, target) =>
            {
                SendMessage(actor, "You open <the0>.", target);
                SendExternalMessage(actor, "@they open", actor, target);
                return SFS.Rules.PerformResult.Continue;
            }).Name("Default report opening rule.");

            GlobalRules.Check<Actor, MudObject>("can open?").First.Do((actor, item) => CheckIsVisibleTo(actor, item)).Name("Item must be visible rule.");
        }
    }
}