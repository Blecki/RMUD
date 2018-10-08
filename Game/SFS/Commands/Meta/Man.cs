using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static SFS.CommandFactory;
using static SFS.Core;

namespace SFS.Commands.Meta
{
	internal class Man
	{
        [AtStartup]
        public static void __()
		{
            Core.StandardMessage("help topics", "Available help topics");
            Core.StandardMessage("no help topic", "There is no help available for that topic.");

            Core.DefaultParser.AddCommand(
                Sequence(
                    Or(
                        KeyWord("HELP"),
                        KeyWord("MAN"),
                        KeyWord("?")),
                    Optional(Rest("TOPIC"))))
                .Manual("This is the command you typed to get this message.")
                .ProceduralRule((match, actor) =>
                {
                    if (!match.ContainsKey("TOPIC"))
                    {
                        SendMessage(actor, "@help topics");
                        var line = "";
                        foreach (var manPage in ManPages.Pages.Select(p => p.Name).Distinct().OrderBy(s => s))
                        {
                            line += manPage;
                            if (line.Length < 20) line += new String(' ', 20 - line.Length);
                            else if (line.Length < 40) line += new String(' ', 40 - line.Length);
                            else
                            {
                                SendMessage(actor, line);
                                line = "";
                            }
                        }
                    }
                    else
                    {
                        var manPageName = match["TOPIC"].ToString().ToUpper();
                        var pages = new List<ManPage>(ManPages.Pages.Where(p => p.Name == manPageName));
                        if (pages.Count > 0)
                            foreach (var manPage in pages)
                                manPage.SendManPage(actor);
                        else
                            SendMessage(actor, "@no help topic");

                    }
                    return SFS.Rules.PerformResult.Continue;
                });
               
		}
	}
}
