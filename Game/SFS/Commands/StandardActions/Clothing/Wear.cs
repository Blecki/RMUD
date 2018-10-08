using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;
using SFS.Rules;

namespace SFS.Commands.StandardActions
{
	internal class Wear : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    KeyWord("WEAR"),
                    BestScore("OBJECT",
                        MustMatch("@clothing wear what",
                            Object("OBJECT", InScope, PreferHeld)))))
                .Manual("Cover your disgusting flesh.")
                .Check("can wear?", "ACTOR", "OBJECT")
                .BeforeActing()
                .Perform("wear", "ACTOR", "OBJECT")
                .AfterActing();
        }

        public static void AtStartup(SFS.SFSRuleEngine GlobalRules)
        {
            GlobalRules.DeclareCheckRuleBook<Actor, MudObject>("can wear?", "[Actor, Item] : Can the actor wear the item?", "actor", "item");
            GlobalRules.DeclarePerformRuleBook<Actor, Clothing>("wear", "[Actor, Item] : Handle the actor wearing the item.", "actor", "item");

            GlobalRules.Check<Actor, MudObject>("can wear?")
                .When((a, b) => !MudObject.ObjectContainsObject(a, b))
                .Do((actor, item) =>
                {
                    MudObject.SendMessage(actor, "@dont have that");
                    return CheckResult.Disallow;
                });

            GlobalRules.Check<Actor, Clothing>("can wear?")
                .When((a, b) => a.RelativeLocationOf(b) == RelativeLocations.Worn)
                .Do((a, b) =>
                {
                    MudObject.SendMessage(a, "@clothing already wearing");
                    return CheckResult.Disallow;
                });

            GlobalRules.Check<Actor, Clothing>("can wear?")
                .Do((a, b) => CheckResult.Allow)
                .Name("Default allow wear clothing rule");

            GlobalRules.Check<Actor, MudObject>("can wear?")
                .Do((actor, item) =>
                {
                    MudObject.SendMessage(actor, "@clothing cant wear");
                    return CheckResult.Disallow;
                })
                .Name("Can't wear unwearable things rule.");

            GlobalRules.Perform<Actor, Clothing>("wear").Do((actor, target) =>
                {
                    MudObject.SendMessage(actor, "@clothing you wear", target);
                    MudObject.SendExternalMessage(actor, "@clothing they wear", actor, target);
                    MudObject.Move(target, actor, RelativeLocations.Worn);
                    return PerformResult.Continue;
                });
        }
    }
}
