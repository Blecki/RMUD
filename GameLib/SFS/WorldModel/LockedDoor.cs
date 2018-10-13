using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS.Rules;
using static SFS.Core;

namespace SFS
{
    /// <summary>
    /// This is a fancy door - it can be locked!
    /// </summary>
	public class LockedDoor : Door
	{
        [AtStartup]
        public static void __x()
        {
            GlobalRules.DeclareCheckRuleBook<Actor, MudObject, MudObject>("can lock with?", "[Actor, Item, Key] : Can this key be used to lock this thing?", "actor", "item", "key");

            GlobalRules.Check<Actor, MudObject, MudObject>("can lock with?")
                .Do((actor, item, key) =>
                {
                    SendMessage(actor, "@wrong key");
                    return CheckResult.Disallow;
                })
                .Name("default that's not the right key rule.");
        }

        public bool Locked = true;

		public LockedDoor()
		{
			Locked = true;

            Check<Actor, LockedDoor, MudObject>("can lock?").Do((actor, door, key) =>
                {
                    if (Open)
                    {
                        SendMessage(actor, "@close it first");
                        return CheckResult.Disallow;
                    }

                    return GlobalRules.ConsiderCheckRule("can lock with?", actor, door, key);
                })
                .Name("can lock? invokes can lock with? rule");

            Perform<Actor, LockedDoor, MudObject>("lock").Do((a, b, c) =>
                {
                    Locked = true;

                    if (Portal.FindOppositeSide(this) is LockedDoor otherSide)
                        otherSide.Locked = true;

                    return PerformResult.Continue;
                })
                .Name("lock it and sync sides rule");

             Perform<Actor, LockedDoor, MudObject>("unlock").Do((a,b,c) =>
                {
                    Locked = false;

                    if (Portal.FindOppositeSide(this) is LockedDoor otherSide)
                        otherSide.Locked = false;

                    return PerformResult.Continue;
                });

             Check<Actor, LockedDoor>("can open?")
                 .First
                 .When((a, b) => Locked)
                 .Do((a, b) =>
                 {
                     SendMessage(a, "@error locked");
                     return CheckResult.Disallow;
                 })
                 .Name("Can't open locked door rule.");

             Perform<Actor, LockedDoor>("close")
                 .Do((a, b) => { Locked = false; return PerformResult.Continue; });
        }
        
	}
}
