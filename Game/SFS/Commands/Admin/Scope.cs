using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;

namespace SFS.Commands.Debug
{
    internal class Scope : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    DebugOnly(),
                    KeyWord("!SCOPE")))
                .Manual("List all of the objects in scope")
                .ProceduralRule((match, actor) =>
                {
                    foreach (var thing in MudObject.EnumerateVisibleTree(MudObject.FindLocale(actor)))
                        MudObject.SendMessage(actor, thing.Short + " - " + thing.GetType().Name);
                    return SFS.Rules.PerformResult.Continue;
                }, "List all the damn things in scope rule.");
        }
    }
}
