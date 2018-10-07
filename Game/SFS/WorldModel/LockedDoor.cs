using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS.Rules;

namespace SFS
{
    /// <summary>
    /// This is a fancy door - it can be locked!
    /// 
    /// TODO: "IsMatchingKey" should be replaced with a rule.
    /// TODO: "Locked" should be a property.
    /// TODO: Sync locked state with opposite side of portal
    /// </summary>
	public class LockedDoor : Door
	{
        public Func<MudObject, bool> IsMatchingKey;

        public bool Locked = true;

		public LockedDoor()
		{
			Locked = true;

            Check<Actor, LockedDoor, MudObject>("can lock?").Do((actor, door, key) =>
                {
                    if (Open) {
                        MudObject.SendMessage(actor, "@close it first");
                        return CheckResult.Disallow;
                    }

                    if (!IsMatchingKey(key))
                    {
                        MudObject.SendMessage(actor, "@wrong key");
                        return CheckResult.Disallow;
                    }

                    return CheckResult.Allow;
                });

            Perform<Actor, LockedDoor, MudObject>("locked").Do((a,b,c) =>
                {
                    Locked = true;
                    return PerformResult.Continue;
                });

             Perform<Actor, LockedDoor, MudObject>("unlocked").Do((a,b,c) =>
                {
                    Locked = false;
                    return PerformResult.Continue;
                });

             Check<Actor, LockedDoor>("can open?")
                 .First
                 .When((a, b) => Locked)
                 .Do((a, b) =>
                 {
                     MudObject.SendMessage(a, "@error locked");
                     return CheckResult.Disallow;
                 })
                 .Name("Can't open locked door rule.");

             Perform<Actor, LockedDoor>("close")
                 .Do((a, b) => { Locked = false; return PerformResult.Continue; });
        }
        
	}
}
