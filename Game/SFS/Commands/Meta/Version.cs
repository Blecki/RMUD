using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static SFS.CommandFactory;

namespace SFS.Commands.Meta
{
	internal class Version
	{
        [AtStartup]
		public static void __()
		{
            Core.StandardMessage("version", "Build: Sharp Fiction System pre-alpha <s0>");
            Core.StandardMessage("commit", "Commit: <s0>");
            Core.StandardMessage("no commit", "Commit version not found.");

            Core.DefaultParser.AddCommand(
                Or(
                    KeyWord("VERSION"),
                    KeyWord("VER")))
                .Manual("Displays the version currently running.")
                .ProceduralRule((match, actor) =>
                {
                    var buildVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

                    MudObject.SendMessage(actor, "@version", buildVersion);

                    if (System.IO.File.Exists("version.txt"))
                        MudObject.SendMessage(actor, "@commit", System.IO.File.ReadAllText("version.txt"));
                    else
                        MudObject.SendMessage(actor, "@no commit");

                    return SFS.Rules.PerformResult.Continue;
                });
		}
	}
}
