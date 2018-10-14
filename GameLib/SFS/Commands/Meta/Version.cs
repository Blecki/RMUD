using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static SFS.CommandFactory;
using static SFS.Core;

namespace SFS.Commands.Meta
{
	internal class Version
	{
        [Initialize]
		public static void __()
		{
            Core.DefaultParser.AddCommand(
                Or(
                    KeyWord("VERSION"),
                    KeyWord("VER")))
                .Manual("Displays the version currently running.")
                .ProceduralRule((match, actor) =>
                {
                    var buildVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

                    SendMessage(actor, "Build: Sharp Fiction System pre-alpha <s0>", buildVersion);

                    if (System.IO.File.Exists("version.txt"))
                        SendMessage(actor, "Commit: <s0>", System.IO.File.ReadAllText("version.txt"));
                    else
                        SendMessage(actor, "Commit version not found.");

                    return SFS.Rules.PerformResult.Continue;
                });
		}
	}
}
