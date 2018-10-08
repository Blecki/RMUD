using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;
using SFS.Rules;

namespace SFS.Commands.StandardActions
{
    internal class Open : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    KeyWord("OPEN"),
                    BestScore("SUBJECT",
                        MustMatch("@not here",
                            Object("SUBJECT", InScope, (actor, thing) =>
                            {
                                if (Core.GlobalRules.ConsiderCheckRuleSilently("can open?", actor, thing) == SFS.Rules.CheckResult.Allow) return MatchPreference.Likely;
                                return MatchPreference.Unlikely;
                            })))))
                .ID("StandardActions:Open")
                .Manual("Opens an openable thing.")
                .Check("can open?", "ACTOR", "SUBJECT")
                .BeforeActing()
                .Perform("open", "ACTOR", "SUBJECT")
                .AfterActing();
        }

        public static void AtStartup(SFS.SFSRuleEngine GlobalRules)
        {
            Core.StandardMessage("not openable", "I don't think the concept of 'open' applies to that.");
            Core.StandardMessage("you open", "You open <the0>.");
            Core.StandardMessage("they open", "^<the0> opens <the1>.");

            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can open?", "[Actor, Item] : Can the actor open the item?", "actor", "item");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("open", "[Actor, Item] : Handle the actor opening the item.", "actor", "item");

            GlobalRules.Check<Actor, MudObject>("can open?")
                .Do((a, b) =>
                {
                    MudObject.SendMessage(a, "@not openable");
                    return SFS.Rules.CheckResult.Disallow;
                })
                .Name("Can't open the unopenable rule.");

            GlobalRules.Perform<Actor, MudObject>("open").Do((actor, target) =>
            {
                MudObject.SendMessage(actor, "@you open", target);
                MudObject.SendExternalMessage(actor, "@they open", actor, target);
                return SFS.Rules.PerformResult.Continue;
            }).Name("Default report opening rule.");

            GlobalRules.Check<Actor, MudObject>("can open?").First.Do((actor, item) => MudObject.CheckIsVisibleTo(actor, item)).Name("Item must be visible rule.");
        }
    }
}