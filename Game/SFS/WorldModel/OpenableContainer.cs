using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static SFS.Core;
using SFS.Rules;

namespace SFS
{
    public class OpenableContainer : Container
    {
        public bool Open = true;

        public OpenableContainer(RelativeLocations Locations, RelativeLocations Default) : base(Locations, Default)
        {
            Check<Actor, OpenableContainer>("can open?")
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
                .Name("Can open openable containers rule.");

            Check<Actor, OpenableContainer>("can close?")
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
                .Name("Can close openable containers rule.");

            Perform<Actor, OpenableContainer>("open").Do((a, b) =>
            {
                Open = true;
                return PerformResult.Continue;
            })
            .Name("Open an openable container rule");

            Perform<Actor, OpenableContainer>("close").Do((a, b) =>
            {
                Open = false;
                return PerformResult.Continue;
            })
            .Name("Close an openable container rule");
        }
    }
}
