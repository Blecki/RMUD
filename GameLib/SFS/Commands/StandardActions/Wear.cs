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
	internal class Wear
	{
        [AtStartup]
        public static void __()
        {
            Core.DefaultParser.AddCommand(
                Sequence(
                    KeyWord("WEAR"),
                    BestScore("OBJECT",
                        MustMatch("I couldn't figure out what you're trying to wear.",
                            Object("OBJECT", InScope, PreferHeld)))))
                .Manual("Cover your disgusting flesh.")
                .Check("can wear?", "ACTOR", "OBJECT")
                .BeforeActing()
                .Perform("wear", "ACTOR", "OBJECT")
                .AfterActing();

            GlobalRules.DeclareCheckRuleBook<Actor, MudObject>("can wear?", "[Actor, Item] : Can the actor wear the item?", "actor", "item");
            GlobalRules.DeclarePerformRuleBook<Actor, Clothing>("wear", "[Actor, Item] : Handle the actor wearing the item.", "actor", "item");

            GlobalRules.Check<Actor, MudObject>("can wear?")
                .When((a, b) => !ObjectContainsObject(a, b))
                .Do((actor, item) =>
                {
                    SendMessage(actor, "You don't have that.");
                    return CheckResult.Disallow;
                });

            GlobalRules.Check<Actor, Clothing>("can wear?")
                .When((a, b) => a.RelativeLocationOf(b) == RelativeLocations.Worn)
                .Do((a, b) =>
                {
                    SendMessage(a, "You're already wearing that.");
                    return CheckResult.Disallow;
                });

            GlobalRules.Check<Actor, Clothing>("can wear?")
                .Do((a, b) => CheckResult.Allow)
                .Name("Default allow wear clothing rule");

            GlobalRules.Check<Actor, MudObject>("can wear?")
                .Do((actor, item) =>
                {
                    SendMessage(actor, "That isn't something that can be worn.");
                    return CheckResult.Disallow;
                })
                .Name("Can't wear unwearable things rule.");

            GlobalRules.Perform<Actor, Clothing>("wear").Do((actor, target) =>
                {
                    SendMessage(actor, "You don <the0>.", target);
                    SendExternalMessage(actor, "^<the0> dons <a1>.", actor, target);
                    MoveObject(target, actor, RelativeLocations.Worn);
                    return PerformResult.Continue;
                });
        }
    }
}
