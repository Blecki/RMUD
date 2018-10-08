using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;
using static SFS.CommandFactory;
using static SFS.Core;

namespace SFS.Commands.Debug
{
	internal class Inspect
	{
        [AtStartup]
        public static void __()
        {
            Core.DefaultParser.AddCommand(
                Sequence(
                    DebugOnly(),
                    KeyWord("!INSPECT"),
                    MustMatch("I don't see that here.",
                        Or(
                            Object("OBJECT", InScope),
                            KeyWord("HERE")))))
                .Manual("Take a peek at the internal workings of any mud object.")
                .ProceduralRule((match, actor) =>
                    {
                        if (!match.ContainsKey("OBJECT"))
                            match.Upsert("OBJECT", actor.Location);
                        return SFS.Rules.PerformResult.Continue;
                    }, "Convert locale option to standard form rule.")
                .ProceduralRule((match, actor) =>
                {
                    var target = match["OBJECT"] as MudObject;

                    SendMessage(actor, "*** INSPECT LISTING ***");

                    foreach (var field in target.GetType().GetFields())
                    {
                        var value = field.GetValue(target);
                        SendMessage(actor, "<s0> <s1>: <s2>", field.Name, field.FieldType, value == null ? "NULL" : value.ToString());
                    }

                    SendMessage(actor, "*** END OF LISTING ***");

                    return SFS.Rules.PerformResult.Continue;
                }, "List all the damn things rule.");
        }
	}
}
