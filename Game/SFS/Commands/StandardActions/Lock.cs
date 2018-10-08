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
                            MustMatch("@not here",
                                Object("SUBJECT", InScope)),
                            OptionalKeyWord("WITH"),
                            MustMatch("@not here",
                                Object("KEY", InScope, PreferHeld))))))
                .ID("StandardActions:Lock")
                .Manual("Lock the subject with a key.")
                .Check("can lock?", "ACTOR", "SUBJECT", "KEY")
                .BeforeActing()
                .Perform("lock", "ACTOR", "SUBJECT", "KEY")
                .AfterActing();

            Core.StandardMessage("not lockable", "I don't think the concept of 'locked' applies to that.");
            Core.StandardMessage("you lock", "You lock <the0>.");
            Core.StandardMessage("they lock", "^<the0> locks <the1> with <the2>.");

            Core.GlobalRules.DeclareCheckRuleBook<Actor, MudObject, MudObject>("can lock?", "[Actor, Item, Key] : Can the item be locked by the actor with the key?", "actor", "item", "key");

            Core.GlobalRules.Check<Actor, MudObject, MudObject>("can lock?")
                .Do((actor, item, key) => MudObject.CheckIsVisibleTo(actor, item))
                .Name("Item must be visible to lock it.");

            Core.GlobalRules.Check<Actor, MudObject, MudObject>("can lock?")
                .Do((actor, item, key) => MudObject.CheckIsHolding(actor, key))
                .Name("Key must be held rule.");

            // Todo: LockedDoor needs to implement this.
            Core.GlobalRules.Check<Actor, MudObject, MudObject>("can lock?")
                .Do((a, b, c) =>
                {
                    SendMessage(a, "@not lockable");
                    return SFS.Rules.CheckResult.Disallow;
                })
                .Name("Can't lock the unlockable rule.");

            Core.GlobalRules.Check<Actor, MudObject, MudObject>("can lock?")
                .Do((a, b, c) => SFS.Rules.CheckResult.Allow)
                .Name("Default allow locking rule.");

            Core.GlobalRules.DeclarePerformRuleBook<Actor, MudObject, MudObject>("lock", "[Actor, Item, Key] : Handle the actor locking the item with the key.", "actor", "item", "key");

            Core.GlobalRules.Perform<Actor, MudObject, MudObject>("lock").Do((actor, target, key) =>
            {
                SendMessage(actor, "@you lock", target);
                SendExternalMessage(actor, "@they lock", actor, target, key);
                return SFS.Rules.PerformResult.Continue;
            });
        }
    }
}
