﻿using System;
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
                        MustMatch("@clothing wear what",
                            Object("OBJECT", InScope, PreferHeld)))))
                .Manual("Cover your disgusting flesh.")
                .Check("can wear?", "ACTOR", "OBJECT")
                .BeforeActing()
                .Perform("wear", "ACTOR", "OBJECT")
                .AfterActing();

            Core.GlobalRules.DeclareCheckRuleBook<Actor, MudObject>("can wear?", "[Actor, Item] : Can the actor wear the item?", "actor", "item");
            Core.GlobalRules.DeclarePerformRuleBook<Actor, Clothing>("wear", "[Actor, Item] : Handle the actor wearing the item.", "actor", "item");

            Core.GlobalRules.Check<Actor, MudObject>("can wear?")
                .When((a, b) => !MudObject.ObjectContainsObject(a, b))
                .Do((actor, item) =>
                {
                    SendMessage(actor, "@dont have that");
                    return CheckResult.Disallow;
                });

            Core.GlobalRules.Check<Actor, Clothing>("can wear?")
                .When((a, b) => a.RelativeLocationOf(b) == RelativeLocations.Worn)
                .Do((a, b) =>
                {
                    SendMessage(a, "@clothing already wearing");
                    return CheckResult.Disallow;
                });

            Core.GlobalRules.Check<Actor, Clothing>("can wear?")
                .Do((a, b) => CheckResult.Allow)
                .Name("Default allow wear clothing rule");

            Core.GlobalRules.Check<Actor, MudObject>("can wear?")
                .Do((actor, item) =>
                {
                    SendMessage(actor, "@clothing cant wear");
                    return CheckResult.Disallow;
                })
                .Name("Can't wear unwearable things rule.");

            Core.GlobalRules.Perform<Actor, Clothing>("wear").Do((actor, target) =>
                {
                    SendMessage(actor, "@clothing you wear", target);
                    SendExternalMessage(actor, "@clothing they wear", actor, target);
                    MoveObject(target, actor, RelativeLocations.Worn);
                    return PerformResult.Continue;
                });
        }
    }
}
