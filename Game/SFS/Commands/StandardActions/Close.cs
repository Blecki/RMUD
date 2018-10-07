using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;
using SFS.Rules;

namespace SFS.Commands.StandardActions
{
	internal class OpenClose : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
            Parser.AddCommand(
                Sequence(
                    KeyWord("CLOSE"),
                    BestScore("SUBJECT",
                        MustMatch("@not here",
                            Object("SUBJECT", InScope, (actor, thing) =>
                                {
                                    if (Core.GlobalRules.ConsiderCheckRuleSilently("can close?", actor, thing) == CheckResult.Allow) return MatchPreference.Likely;
                                    return MatchPreference.Unlikely;
                                })))))
                .ID("StandardActions:Close")
                .Manual("Closes a thing.")
                .Check("can close?", "ACTOR", "SUBJECT")
                .BeforeActing()
                .Perform("close", "ACTOR", "SUBJECT")
                .AfterActing();
		}

        public static void AtStartup(SFS.SFSRuleEngine GlobalRules)
        {
            Core.StandardMessage("you close", "You close <the0>.");
            Core.StandardMessage("they close", "^<the0> closes <the1>.");

            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("can close?", "[Actor, Item] : Determine if the item can be closed.", "actor", "item");

            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("close", "[Actor, Item] : Handle the item being closed.", "actor", "item");

            GlobalRules.Check<Actor, MudObject>("can close?")
                .Do((a, b) =>
                {
                    MudObject.SendMessage(a, "@not openable");
                    return CheckResult.Disallow;
                })
                .Name("Default can't close unopenable things rule.");

            // Todo: Doors and containers need to implement these rules.

            GlobalRules.Perform<Actor, MudObject>("close").Do((actor, target) =>
            {
                MudObject.SendMessage(actor, "@you close", target);
                MudObject.SendExternalMessage(actor, "@they close", actor, target);
                return PerformResult.Continue;
            }).Name("Default close reporting rule.");

            GlobalRules.Check<Actor, MudObject>("can close?").First.Do((actor, item) => MudObject.CheckIsVisibleTo(actor, item)).Name("Item must be visible rule.");
        }
    }

    public static class CloseRuleFactoryExtensions
    {
        public static RuleBuilder<MudObject, MudObject, CheckResult> CheckCanClose(this MudObject ThisObject)
        {
            return ThisObject.Check<MudObject, MudObject>("can close?").ThisOnly(1);
        }

        public static RuleBuilder<MudObject, MudObject, PerformResult> PerformClose(this MudObject ThisObject)
        {
            return ThisObject.Perform<MudObject, MudObject>("close").ThisOnly(1);
        }
    }
}
