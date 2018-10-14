using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;
using static SFS.CommandFactory;
using static SFS.Core;

namespace SFS.Commands.Meta
{
	internal class Alias
	{
        //Todo: PERSIST
        internal static Dictionary<String, String> Aliases = new Dictionary<string, string>();

        [Initialize]
        public static void __()
        {
            Core.DefaultParser.AddCommand(
                Sequence(
                    KeyWord("ALIAS"),
                    SingleWord("NAME"),
                    Rest("RAW-COMMAND")))
                .Manual("Create an alias for another command, or a series of them.")
                .ProceduralRule((match, actor) =>
                {
                    Aliases.Add(match["NAME"].ToString().ToUpper(), match["RAW-COMMAND"].ToString());
                    SendMessage(actor, "Alias added.");
                    return SFS.Rules.PerformResult.Continue;
                });

            Core.DefaultParser.AddCommand(
                Generic((match, context) =>
                {   
                    var r = new List<PossibleMatch>();
                    if (Aliases.ContainsKey(match.Next.Value.ToUpper()))
                        r.Add(match.AdvanceWith("ALIAS", Aliases[match.Next.Value.ToUpper()]));
                    return r;
                }, "<ALIAS NAME>"))
                .Manual("Execute an alias.")
                .ProceduralRule((match, actor) =>
                {
                    var commands = match["ALIAS"].ToString().Split(';');
                    foreach (var command in commands)
                        EnqueuActorCommand(actor, command);
                    return SFS.Rules.PerformResult.Continue;
                });
        }
	}
}
