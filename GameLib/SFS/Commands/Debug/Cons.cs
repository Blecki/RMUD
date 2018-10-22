﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;
using static SFS.CommandFactory;
using static SFS.Core;

namespace SFS.Commands.Debug
{
	internal class Cons
	{
        [Initialize]
		public static void __()
		{
            Core.DefaultParser.AddCommand(
                Sequence(
                    DebugOnly(),
                    KeyWord("!CONS"),
                    Optional(KeyWord("LOCAL"), "LOCAL")))
                .ID("Meta:Cons")
                .Manual("Scan all defined commands for ommissions, then scan loaded objects for omissions.")
                .ProceduralRule((match, actor) =>
                {
                    var localScan = false;
                    if (match.ContainsKey("LOCAL"))
                        localScan = (match["LOCAL"] as bool?).Value;

                    var resultsFound = 0;

                    SendMessage(actor, "Results of consistency check:");

                    if (!localScan)
                        foreach (var command in Core.DefaultParser.EnumerateCommands())
                        {
                            if (String.IsNullOrEmpty(command.GetID())) 
                            {
                                resultsFound += 1;
                                SendMessage(actor, "Command has no ID set: " + command.ManualName);
                            }
                        }

                    if (resultsFound == 0)
                        SendMessage(actor, "I found nothing wrong.");


                    return SFS.Rules.PerformResult.Continue;
                });
               
		}
	}
}