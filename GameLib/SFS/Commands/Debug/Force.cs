using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;
using static SFS.CommandFactory;
using static SFS.Core;

namespace SFS.Commands.Debug
{
	internal class Force
	{
        [Initialize]
        public static void __()
        {
            Core.DefaultParser.AddCommand(
                Sequence(
                    DebugOnly(),
                    KeyWord("!FORCE"),
                    MustMatch("Whom do you wish to command?",
                        FirstOf(
                            Object("OBJECT", InScope),
                            Path("PATH"))),
                    Rest("RAW-COMMAND")))
                .Manual("An administrative command that allows you to execute a command as if you were another actor or player. The other entity will see all output from the command, and rules restricting their access to the command are considered.")
                .ProceduralRule((match, actor) =>
                    {
                        if (match.ContainsKey("PATH"))
                        {
                            var target = GetObject(match["PATH"].ToString());
                            if (target == null)
                            {
                                SendMessage(actor, "I can't find whomever it is you want to submit to your foolish whims.");
                                return SFS.Rules.PerformResult.Stop;
                            }
                            match.Upsert("OBJECT", target);
                        }
                        return SFS.Rules.PerformResult.Continue;
                    }, "Convert path to object rule.")
                .ProceduralRule((match, actor) =>
                {
                    var target = match["OBJECT"] as Actor;

                    if (target == null)
                    {
                        SendMessage(actor, "The target is not an actor.");
                        return SFS.Rules.PerformResult.Stop;
                    }
                    
                    var command = match["RAW-COMMAND"].ToString();
                    var matchedCommand = Core.DefaultParser.ParseCommand(new PendingCommand { RawCommand = command, Actor = target });

                    if (matchedCommand != null)
                    {
                        if (matchedCommand.Matches.Count > 1)
                            SendMessage(actor, "The command was ambigious.");
                        else
                        {
                            SendMessage(actor, "Enacting your will.");
                            Core.ProcessPlayerCommand(matchedCommand.Command, matchedCommand.Matches[0], target);
                        }
                    }
                    else
                        SendMessage(actor, "The command did not match.");

                    return SFS.Rules.PerformResult.Continue;
                });
        }
	}
}
