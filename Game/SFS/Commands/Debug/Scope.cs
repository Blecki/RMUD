using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;
using static SFS.CommandFactory;
using static SFS.Core;

namespace SFS.Commands.Debug
{
    internal class Scope
    {
        [AtStartup]
        public static void __()
        {
            Core.DefaultParser.AddCommand(
                Sequence(
                    DebugOnly(),
                    KeyWord("!SCOPE")))
                .Manual("List all of the objects in scope")
                .ProceduralRule((match, actor) =>
                {
                    foreach (var thing in MudObject.EnumerateVisibleTree(FindLocale(actor)))
                        SendMessage(actor, thing.Short + " - " + thing.GetType().Name);
                    return SFS.Rules.PerformResult.Continue;
                }, "List all the damn things in scope rule.");
        }
    }
}
