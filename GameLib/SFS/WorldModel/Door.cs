using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS.Rules;
using static SFS.Core;

namespace SFS
{
    /// <summary>
    /// A basic door object. Doors are openable. When used as a portal, a door will automatically sync it's open state
    /// with the opposite side of the portal.
    /// </summary>
    public class Door : Portal
    {
        public bool Open = false;

        public Door()
        {
            Noun("DOOR");
            Noun("CLOSED").When(a => !Open);
            Noun("OPEN").When(a => Open);

            Short = "door";
            Long = "It's an ordinary door.";

            Check<Actor, Door>("can open?")
                .Last
                .Do((a, b) =>
                {
                    if (Open)
                    {
                        SendMessage(a, "@already open");
                        return CheckResult.Disallow;
                    }
                    return CheckResult.Allow;
                })
                .Name("Can open doors rule.");

            Check<Actor, Door>("can close?")
                .Last
                .Do((a, b) =>
                {
                    if (!Open)
                    {
                        SendMessage(a, "@already closed");
                        return CheckResult.Disallow;
                    }
                    return CheckResult.Allow;
                })
                .Name("Can close doors rule.");

            Perform<Actor, Door>("open").Do((a, b) =>
            {
                Open = true;

                // Doors are usually two-sided. If there is an opposite side, we need to open it and emit appropriate
                // messages.
                if (Portal.FindOppositeSide(this) is Door otherSide)
                {
                    otherSide.Open = true;

                    // This message is defined in the standard actions module. It is perhaps a bit coupled?
                    SendLocaleMessage(otherSide, "@they open", a, this);
                    Core.MarkLocaleForUpdate(otherSide);
                }

                return PerformResult.Continue;
            })
            .Name("Open a door rule");

            Perform<Actor, Door>("close").Do((a, b) =>
            {
                Open = false;

                // Doors are usually two-sided. If there is an opposite side, we need to close it and emit
                // appropriate messages.
                var otherSide = Portal.FindOppositeSide(this) as Door;
                if (otherSide != null)
                {
                    otherSide.Open = false;
                    SendLocaleMessage(otherSide, "@they close", a, this);
                    Core.MarkLocaleForUpdate(otherSide);
                }

                return PerformResult.Continue;
            })
            .Name("Close a door rule");
        }

        [Initialize]
        public static void __()
        {
            GlobalRules.Perform<Actor, Door>("describe")
                .Do((viewer, item) =>
                {
                    if (item.Open)
                        SendMessage(viewer, "(<the0 is open)", item);
                    else
                        SendMessage(viewer, "(<the0> is closed)", item);
                    return PerformResult.Continue;
                })
                .Name("Describe open or closed state rule.");
        }
    }
}
