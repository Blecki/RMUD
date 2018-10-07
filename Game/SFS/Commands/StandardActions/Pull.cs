using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;

namespace SFS.Commands.StandardActions
{
	internal class Pull : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
            Parser.AddCommand(
                Sequence(
                    KeyWord("PULL"),
                    BestScore("SUBJECT",
                        MustMatch("@not here",
                            Object("SUBJECT", InScope, (actor, item) =>
                            {
                                if (Core.GlobalRules.ConsiderCheckRuleSilently("can pull?", actor, item) != SFS.Rules.CheckResult.Allow)
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
		}

        public static void AtStartup(SFSRuleEngine GlobalRules)
        {

            GlobalRules.DeclareCheckRuleBook<Actor, MudObject>("can pull?", "[Actor, Item] : Can the actor pull the item?", "actor", "item");
            GlobalRules.DeclarePerformRuleBook<Actor, MudObject>("pull", "[Actor, Item] : Handle the actor pulling the item.", "actor", "item");

            GlobalRules.Check<Actor, MudObject>("can pull?")
                .Do((actor, item) => MudObject.CheckIsVisibleTo(actor, item))
                .Name("Item must be visible to pull rule.");

            GlobalRules.Check<Actor, MudObject>("can pull?")
                .Last
                .Do((a, t) => 
                    {
                        MudObject.SendMessage(a, "@does nothing");
                        return SFS.Rules.CheckResult.Disallow;
                    })
                .Name("Default disallow pulling rule.");

            GlobalRules.Perform<Actor, MudObject>("pull")
                .Do((actor, target) =>
                {
                    MudObject.SendMessage(actor, "@nothing happens");
                    return SFS.Rules.PerformResult.Continue;
                })
                .Name("Default handle pulling rule.");

            GlobalRules.Check<Actor, Actor>("can pull?")
                .First
                .Do((actor, thing) =>
                {
                    MudObject.SendMessage(actor, "@unappreciated", thing);
                    return SFS.Rules.CheckResult.Disallow;
                })
                .Name("Can't pull people rule.");
        }
    }
}
