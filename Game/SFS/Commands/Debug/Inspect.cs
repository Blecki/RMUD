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
                    SendMessage(actor, "Path: <s0>", target.Path);
                    if (target.Location == null)
                        SendMessage(actor, "Location: NOWHERE");
                    else
                        SendMessage(actor, "Location: <s0>", target.Location.Path);
                    SendMessage(actor, "*** DYNAMIC PROPERTIES ***");

                    //Todo: Reimplement using reflection.
                    //foreach (var property in target.Properties)
                    //{
                    //    var info = PropertyManifest.GetPropertyInformation(property.Key);
                    //    SendMessage(actor, "<s0>: <s1>", property.Key, info.Converter.ConvertToString(property.Value));
                    //}

                    SendMessage(actor, "*** END OF LISTING ***");

                    return SFS.Rules.PerformResult.Continue;
                }, "List all the damn things rule.");
        }
	}
}
