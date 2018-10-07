﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;
using SFS.Rules;

namespace SFS.Commands.StandardActions
{
    internal class Unlock : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    KeyWord("UNLOCK"),
                    BestScore("ITEM",
                        MustMatch("@not here",
                            Object("ITEM", InScope))),
                    OptionalKeyWord("WITH"),
                    BestScore("KEY",
                        MustMatch("@not here",
                            Object("KEY", InScope, PreferHeld)))))
                .ID("StandardActions:Unlock")
                .Manual("Use the KEY to unlock the ITEM.")
                .Check("can lock?", "ACTOR", "ITEM", "KEY")
                .BeforeActing()
                .Perform("unlocked", "ACTOR", "ITEM", "KEY")
                .AfterActing();
        }

        public static void AtStartup(SFS.SFSRuleEngine GlobalRules)
        {
            Core.StandardMessage("you unlock", "You unlock <the0>.");
            Core.StandardMessage("they unlock", "^<the0> unlocks <the1> with <a2>.");

            GlobalRules.DeclarePerformRuleBook<Actor, MudObject, MudObject>("unlocked", "[Actor, Item, Key] : Handle the actor unlocking the item with the key.", "actor", "item", "key");

            GlobalRules.Perform<Actor, MudObject, MudObject>("unlocked").Do((actor, target, key) =>
            {
                MudObject.SendMessage(actor, "@you unlock", target);
                MudObject.SendExternalMessage(actor, "@they unlock", actor, target, key);
                return SFS.Rules.PerformResult.Continue;
            });
        }
    }

    public static class UnlockExtensions
    {
        public static RuleBuilder<MudObject, MudObject, MudObject, PerformResult> PerformUnlocked(this MudObject Object)
        {
            return Object.Perform<MudObject, MudObject, MudObject>("unlocked");
        }
    }
}
