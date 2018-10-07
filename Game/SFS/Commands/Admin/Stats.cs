using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;

namespace SFS.Commands.Debug
{
    internal class Stats : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    DebugOnly(),
                    KeyWord("!STATS"),
                    Optional(SingleWord("TYPE"))))
                .Manual("Displays various stats about the server. Type the command with no argument to get a list of available options.")
                .ProceduralRule((match, actor) =>
                {
                    if (!match.ContainsKey("TYPE"))
                    {
                        MudObject.SendMessage(actor, "Try one of these stats options");
                        Core.GlobalRules.ConsiderPerformRule("enumerate-stats", actor);
                    }
                    else
                        Core.GlobalRules.ConsiderPerformRule("stats", actor, match["TYPE"].ToString().ToUpper());
                    return SFS.Rules.PerformResult.Continue;
                });                
        }

        public static void AtStartup(SFSRuleEngine GlobalRules)
        {
            GlobalRules.DeclarePerformRuleBook<MudObject, String>("stats", "[Actor, Type] : Display engine stats.");
            
            GlobalRules.DeclarePerformRuleBook<MudObject>("enumerate-stats", "[Actor] : Display stats options.");
        }
    }
}