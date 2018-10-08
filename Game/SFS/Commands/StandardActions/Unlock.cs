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
    internal class Unlock
    {
        [AtStartup]
        public static void __()
        {
            Core.DefaultParser.AddCommand(
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
                .Perform("unlock", "ACTOR", "ITEM", "KEY")
                .AfterActing();

            Core.StandardMessage("you unlock", "You unlock <the0>.");
            Core.StandardMessage("they unlock", "^<the0> unlocks <the1> with <a2>.");

            GlobalRules.DeclarePerformRuleBook<Actor, MudObject, MudObject>("unlock", "[Actor, Item, Key] : Handle the actor unlocking the item with the key.", "actor", "item", "key");

            GlobalRules.Perform<Actor, MudObject, MudObject>("unlock").Do((actor, target, key) =>
            {
                SendMessage(actor, "@you unlock", target);
                SendExternalMessage(actor, "@they unlock", actor, target, key);
                return SFS.Rules.PerformResult.Continue;
            });
        }
    }
}