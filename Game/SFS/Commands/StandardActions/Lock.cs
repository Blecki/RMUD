﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;
using SFS.Rules;

namespace SFS.Commands.StandardActions
{
	internal class Lock : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
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
                .Perform("locked", "ACTOR", "SUBJECT", "KEY")
                .AfterActing();
        }

        public static void AtStartup(SFS.SFSRuleEngine GlobalRules)
        {
            PropertyManifest.RegisterProperty("lockable?", typeof(bool), false, new BoolSerializer());

            Core.StandardMessage("not lockable", "I don't think the concept of 'locked' applies to that.");
            Core.StandardMessage("you lock", "You lock <the0>.");
            Core.StandardMessage("they lock", "^<the0> locks <the1> with <the2>.");

            GlobalRules.DeclareCheckRuleBook<Actor, MudObject, MudObject>("can lock?", "[Actor, Item, Key] : Can the item be locked by the actor with the key?", "actor", "item", "key");
            
            GlobalRules.Check<Actor, MudObject, MudObject>("can lock?")
                .Do((actor, item, key) => MudObject.CheckIsVisibleTo(actor, item))
                .Name("Item must be visible to lock it.");

            GlobalRules.Check<Actor, MudObject, MudObject>("can lock?")
                .Do((actor, item, key) => MudObject.CheckIsHolding(actor, key))
                .Name("Key must be held rule.");

            // Todo: LockedDoor needs to implement this.
            GlobalRules.Check<Actor, MudObject, MudObject>("can lock?")
                .Do((a, b, c) =>
                {
                    MudObject.SendMessage(a, "@not lockable");
                    return SFS.Rules.CheckResult.Disallow;
                })
                .Name("Can't lock the unlockable rule.");

            GlobalRules.Check<Actor, MudObject, MudObject>("can lock?")
                .Do((a, b, c) => SFS.Rules.CheckResult.Allow)
                .Name("Default allow locking rule.");

            GlobalRules.DeclarePerformRuleBook<Actor, MudObject, MudObject>("locked", "[Actor, Item, Key] : Handle the actor locking the item with the key.", "actor", "item", "key");

            GlobalRules.Perform<Actor, MudObject, MudObject>("locked").Do((actor, target, key) =>
            {
                MudObject.SendMessage(actor, "@you lock", target);
                MudObject.SendExternalMessage(actor, "@they lock", actor, target, key);
                return SFS.Rules.PerformResult.Continue;
            });
        }
    }

    public static class LockExtensions
    {
        public static RuleBuilder<MudObject, MudObject, MudObject, CheckResult> CheckCanLock(this MudObject Object)
        {
            return Object.Check<MudObject, MudObject, MudObject>("can lock?").ThisOnly();
        }

        public static RuleBuilder<MudObject, MudObject, MudObject, PerformResult> PerformLocked(this MudObject Object)
        {
            return Object.Perform<MudObject, MudObject, MudObject>("locked");
        }
    }
}
