using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;

namespace SFS.Commands.Debug
{
    internal class Dump : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    DebugOnly(),
                    KeyWord("!DUMP"),
                    MustMatch("It helps if you supply a path.",
                        Path("TARGET"))))
                .Manual("Display the source of a database object.")
                .ProceduralRule((match, actor) =>
                {
                    var target = match["TARGET"].ToString();
                        var source = Core.Database.LoadSourceFile(target);
                        if (!source.Item1)
                            MudObject.SendMessage(actor, "Could not display source: " + source.Item2);
                        else
                            MudObject.SendMessage(actor, "Source of " + target + "\n" + source.Item2);
                        return SFS.Rules.PerformResult.Continue;
                });
        }
    }
}