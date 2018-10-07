using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;

namespace SFS.Commands.Debug
{
	internal class DumpMessages : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
            Parser.AddCommand(
                KeyWord("!DUMPMESSAGES"))
                .Manual("Dump defined messages to messages.txt")
                .ProceduralRule((match, actor) =>
                {
                    var builder = new StringBuilder();
                    Core.DumpMessagesForCustomization(builder);
                    System.IO.File.WriteAllText("messages.txt", builder.ToString());
                    MudObject.SendMessage(actor, "Messages dumped to messages.txt.");
                    return SFS.Rules.PerformResult.Continue;
                });
		}
	}
}
