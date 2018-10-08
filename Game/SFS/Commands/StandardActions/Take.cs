using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;
using SFS.Rules;
using static SFS.CommandFactory;

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
                        MustMatch("@not here",
                            Object("SUBJECT", InScope, (actor, item) =>
                            {
                                if (Core.GlobalRules.ConsiderCheckRuleSilently("can take?", actor, item) != CheckResult.Allow)
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

            Core.StandardMessage("you take", "You take <the0>.");
            Core.StandardMessage("they take", "^<the0> takes <the1>.");
            Core.StandardMessage("cant take people", "You can't take people.");
            Core.StandardMessage("cant take portals", "You can't take portals.");
            Core.StandardMessage("cant take scenery", "That's a terrible idea.");

            Core.GlobalRules.DeclareCheckRuleBook<Actor, MudObject>("can take?", "[Actor, Item] : Can the actor take the item?", "actor", "item");
            Core.GlobalRules.DeclarePerformRuleBook<Actor, MudObject>("take", "[Actor, Item] : Handle the actor taking the item.", "actor", "item");

            Core.GlobalRules.Check<Actor, MudObject>("can take?")
                .Do((actor, item) => MudObject.CheckIsVisibleTo(actor, item))
                .Name("Item must be visible to take rule.");

            Core.GlobalRules.Check<Actor, MudObject>("can take?")
                .When((actor, item) => actor.Contains(item, RelativeLocations.Held))
                .Do((actor, item) =>
                {
                    MudObject.SendMessage(actor, "@already have that");
                    return CheckResult.Disallow;
                })
                .Name("Can't take what you're already holding rule.");

            Core.GlobalRules.Check<Actor, MudObject>("can take?")
                .Last
                .Do((a, t) => CheckResult.Allow)
                .Name("Default allow taking rule.");

            Core.GlobalRules.Perform<Actor, MudObject>("take")
                .Do((actor, target) =>
                {
                    MudObject.SendMessage(actor, "@you take", target);
                    MudObject.SendExternalMessage(actor, "@they take", actor, target);
                    MudObject.Move(target, actor);
                    return PerformResult.Continue;
                })
                .Name("Default handle taken rule.");

            Core.GlobalRules.Check<Actor, Actor>("can take?")
                .First
                .Do((actor, thing) =>
                {
                    MudObject.SendMessage(actor, "@cant take people");
                    return CheckResult.Disallow;
                })
                .Name("Can't take people rule.");

            Core.GlobalRules.Check<Actor, Portal>("can take?")
                .First
                .Do((actor, thing) =>
                {
                    MudObject.SendMessage(actor, "@cant take portals");
                    return CheckResult.Disallow;
                });

            Core.GlobalRules.Check<Actor, Scenery>("can take?")
                .First
                .Do((actor, thing) =>
                {
                    MudObject.SendMessage(actor, "@cant take scenery");
                    return CheckResult.Disallow;
                })
                .Name("Can't take scenery rule.");
        }
    }
}
