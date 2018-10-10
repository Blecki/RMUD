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
    internal class Lock
    {
        [AtStartup]
        public static void __()
        {
            Core.DefaultParser.AddCommand(
                BestScore("KEY",
                    BestScore("SUBJECT",
                        Sequence(
                            KeyWord("LOCK"),
                            MustMatch("I don't see that here.",
                                Object("SUBJECT", InScope)),
                            OptionalKeyWord("WITH"),
                            MustMatch("I don't see that here.",
                                Object("KEY", InScope, PreferHeld))))))
                .ID("StandardActions:Lock")
                .Manual("Lock the subject with a key.")
                .Check("can lock?", "ACTOR", "SUBJECT", "KEY")
                .BeforeActing()
                .Perform("lock", "ACTOR", "SUBJECT", "KEY")
                .AfterActing();
            
            GlobalRules.DeclareCheckRuleBook<Actor, MudObject, MudObject>("can lock?", "[Actor, Item, Key] : Can the item be locked by the actor with the key?", "actor", "item", "key");

            GlobalRules.Check<Actor, MudObject, MudObject>("can lock?")
                .Do((actor, item, key) => CheckIsVisibleTo(actor, item))
                .Name("Item must be visible to lock it.");

            GlobalRules.Check<Actor, MudObject, MudObject>("can lock?")
                .Do((actor, item, key) => CheckIsHolding(actor, key))
                .Name("Key must be held rule.");

            GlobalRules.Check<Actor, MudObject, MudObject>("can lock?")
                .Do((a, b, c) =>
                {
                    SendMessage(a, "I don't think the concept of 'locked' applies to that.");
                    return SFS.Rules.CheckResult.Disallow;
                })
                .Name("Can't lock the unlockable rule.");

            GlobalRules.DeclarePerformRuleBook<Actor, MudObject, MudObject>("lock", "[Actor, Item, Key] : Handle the actor locking the item with the key.", "actor", "item", "key");

            GlobalRules.Perform<Actor, MudObject, MudObject>("lock").Do((actor, target, key) =>
            {
                SendMessage(actor, "You lock <the0>.", target);
                SendExternalMessage(actor, "^<the0> locks <the1>.", actor, target, key);
                return SFS.Rules.PerformResult.Continue;
            });
        }
    }
}
