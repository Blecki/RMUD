﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD.Commands
{
	internal class Open : CommandFactory, DeclaresRules
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    KeyWord("OPEN"),
                    BestScore("SUBJECT",
                        MustMatch("I don't see that here.",
                            Object("SUBJECT", InScope, (actor, thing) =>
                            {
                                if (GlobalRules.ConsiderCheckRuleSilently("can open?", actor, thing) == CheckResult.Allow) return MatchPreference.Likely;
                                return MatchPreference.Unlikely;
                            })))))
                .Manual("Opens an openable thing.")
                .Check("can open?", "ACTOR", "SUBJECT")
                .BeforeActing()
                .Perform("opened", "ACTOR", "SUBJECT")
                .AfterActing();
        }

        public void InitializeRules()
        {
            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can open?", "[Actor, Item] : Can the actor open the item?");

            GlobalRules.DeclareValueRuleBook<MudObject, bool>("openable?", "[Item -> bool] : Is the item openable?");
            
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("opened", "[Actor, Item] : Handle the actor opening the item.");

            GlobalRules.DeclareValueRuleBook<MudObject, bool>("open?", "[Item -> bool] : Is the item open?");

            GlobalRules.Check<MudObject, MudObject>("can open?")
                .When((actor, item) => !GlobalRules.ConsiderValueRule<bool>("openable?", item))
                .Do((a, b) =>
                {
                    MudObject.SendMessage(a, "I don't think the concept of 'open' applies to that.");
                    return CheckResult.Disallow;
                })
                .Name("Can't open the unopenable rule.");

            GlobalRules.Check<MudObject, MudObject>("can open?")
                .Do((a, b) => CheckResult.Allow)
                .Name("Default go ahead and open it rule.");

            GlobalRules.Value<MudObject, bool>("open?").Do(a => true).Name("Things open by default rule.");

            GlobalRules.Value<MudObject, bool>("openable?").Do(a => false).Name("Things unopenable by default rule.");

            GlobalRules.Perform<MudObject, MudObject>("opened").Do((actor, target) =>
            {
                MudObject.SendMessage(actor, "You open <the0>.", target);
                MudObject.SendExternalMessage(actor, "^<a0> opens <a1>.", actor, target);
                return PerformResult.Continue;
            }).Name("Default report opening rule.");

            GlobalRules.Check<MudObject, MudObject>("can open?").First.Do((actor, item) => MudObject.CheckIsVisibleTo(actor, item)).Name("Item must be visible rule.");
        }
    }
}
