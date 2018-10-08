using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;
using static SFS.CommandFactory;
using static SFS.Core;

namespace SFS.Commands.StandardActions
{
	internal class Pull
	{
        [AtStartup]
        public static void __()
        {
            Core.DefaultParser.AddCommand(
                Sequence(
                    KeyWord("PULL"),
                    BestScore("SUBJECT",
                        MustMatch("@not here",
                            Object("SUBJECT", InScope, (actor, item) =>
                            {
                                if (GlobalRules.ConsiderCheckRuleSilently("can pull?", actor, item) != SFS.Rules.CheckResult.Allow)
                                    return MatchPreference.Unlikely;
                                return MatchPreference.Plausible;
                            })))))
                .ID("StandardActions:Pull")
                .Manual("Pull an item. By default, this does nothing.")
                .Check("can pull?", "ACTOR", "SUBJECT")
                .BeforeActing()
                .Perform("pull", "ACTOR", "SUBJECT")
                .AfterActing()
                .MarkLocaleForUpdate();

            GlobalRules.DeclareCheckRuleBook<Actor, MudObject>("can pull?", "[Actor, Item] : Can the actor pull the item?", "actor", "item");
            GlobalRules.DeclarePerformRuleBook<Actor, MudObject>("pull", "[Actor, Item] : Handle the actor pulling the item.", "actor", "item");

            GlobalRules.Check<Actor, MudObject>("can pull?")
                .Do((actor, item) => CheckIsVisibleTo(actor, item))
                .Name("Item must be visible to pull rule.");

            GlobalRules.Check<Actor, MudObject>("can pull?")
                .Last
                .Do((a, t) => 
                    {
                        SendMessage(a, "@does nothing");
                        return SFS.Rules.CheckResult.Disallow;
                    })
                .Name("Default disallow pulling rule.");

            GlobalRules.Perform<Actor, MudObject>("pull")
                .Do((actor, target) =>
                {
                    SendMessage(actor, "@nothing happens");
                    return SFS.Rules.PerformResult.Continue;
                })
                .Name("Default handle pulling rule.");

            GlobalRules.Check<Actor, Actor>("can pull?")
                .First
                .Do((actor, thing) =>
                {
                    SendMessage(actor, "@unappreciated", thing);
                    return SFS.Rules.CheckResult.Disallow;
                })
                .Name("Can't pull people rule.");
        }
    }
}
