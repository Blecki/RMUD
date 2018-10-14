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
        [Initialize]
        public static void __()
        {
            Core.DefaultParser.AddCommand(
                Sequence(
                    KeyWord("UNLOCK"),
                    BestScore("ITEM",
                        MustMatch("I don't see that here.",
                            Object("ITEM", InScope))),
                    OptionalKeyWord("WITH"),
                    BestScore("KEY",
                        MustMatch("I don't see that here.",
                            Object("KEY", InScope, PreferHeld)))))
                .ID("StandardActions:Unlock")
                .Manual("Use the KEY to unlock the ITEM.")
                .Check("can lock?", "ACTOR", "ITEM", "KEY") // We use the same rules as locking under the assumption that keys work for both actions.
                .BeforeActing()
                .Perform("unlock", "ACTOR", "ITEM", "KEY")
                .AfterActing();

            // Todo: Provide a key that matches if they leave it out of the command.

            GlobalRules.DeclarePerformRuleBook<Actor, MudObject, MudObject>("unlock", "[Actor, Item, Key] : Handle the actor unlocking the item with the key.", "actor", "item", "key");

            GlobalRules.Perform<Actor, MudObject, MudObject>("unlock").Do((actor, target, key) =>
            {
                SendMessage(actor, "You unlock <the0>.", target);
                SendExternalMessage(actor, "^<the0> unlocks <the1> with <a2>.", actor, target, key);
                return SFS.Rules.PerformResult.Continue;
            });
        }
    }
}