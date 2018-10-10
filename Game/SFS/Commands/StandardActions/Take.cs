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
    internal class Take
    {
        [AtStartup]
        public static void __()
        {
            Core.DefaultParser.AddCommand(
                Sequence(
                    Or(
                        KeyWord("GET"),
                        KeyWord("TAKE")),
                    BestScore("SUBJECT",
                        MustMatch("I don't see that here.",
                            Object("SUBJECT", InScope, (actor, item) =>
                            {
                                if (GlobalRules.ConsiderCheckRuleSilently("can take?", actor, item) != CheckResult.Allow)
                                    return MatchPreference.Unlikely;
                                return MatchPreference.Plausible;
                            })))))
                .ID("StandardActions:Take")
                .Manual("Takes an item and adds it to your inventory.")
                .Check("can take?", "ACTOR", "SUBJECT")
                .BeforeActing()
                .Perform("take", "ACTOR", "SUBJECT")
                .AfterActing()
                .MarkLocaleForUpdate();

            GlobalRules.DeclareCheckRuleBook<Actor, MudObject>("can take?", "[Actor, Item] : Can the actor take the item?", "actor", "item");
            GlobalRules.DeclarePerformRuleBook<Actor, MudObject>("take", "[Actor, Item] : Handle the actor taking the item.", "actor", "item");

            GlobalRules.Check<Actor, MudObject>("can take?")
                .Do((actor, item) => CheckIsVisibleTo(actor, item))
                .Name("Item must be visible to take rule.");

            GlobalRules.Check<Actor, MudObject>("can take?")
                .When((actor, item) => actor.Contains(item, RelativeLocations.Held))
                .Do((actor, item) =>
                {
                    SendMessage(actor, "You already have that.");
                    return CheckResult.Disallow;
                })
                .Name("Can't take what you're already holding rule.");

            GlobalRules.Check<Actor, MudObject>("can take?")
                .Last
                .Do((a, t) => CheckResult.Allow)
                .Name("Default allow taking rule.");

            GlobalRules.Perform<Actor, MudObject>("take")
                .Do((actor, target) =>
                {
                    SendMessage(actor, "You take <the0>.", target);
                    SendExternalMessage(actor, "^<the0> takes <the1>.", actor, target);
                    MoveObject(target, actor);
                    return PerformResult.Continue;
                })
                .Name("Default handle taken rule.");

            GlobalRules.Check<Actor, Actor>("can take?")
                .First
                .Do((actor, thing) =>
                {
                    SendMessage(actor, "I don't think <the1> would appreciate that.");
                    return CheckResult.Disallow;
                })
                .Name("Can't take people rule.");

            GlobalRules.Check<Actor, Portal>("can take?")
                .First
                .Do((actor, thing) =>
                {
                    SendMessage(actor, "You can't take that.");
                    return CheckResult.Disallow;
                });

            GlobalRules.Check<Actor, Scenery>("can take?")
                .First
                .Do((actor, thing) =>
                {
                    SendMessage(actor, "You can't take that.");
                    return CheckResult.Disallow;
                })
                .Name("Can't take scenery rule.");
        }
    }
}
