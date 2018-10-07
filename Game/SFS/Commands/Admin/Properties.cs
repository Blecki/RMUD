using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;
using SFS.Rules;

namespace SFS.Commands.Debug
{
    internal class Properties : CommandFactory
    {
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    DebugOnly(),
                    KeyWord("!PROPERTIES")))
                .Manual("List all properties that have been registered.")
                .ProceduralRule((match, actor) =>
                {
                    foreach (var prop in PropertyManifest.GetAllPropertyInformation())
                    {
                        MudObject.SendMessage(actor, "<s0> (<s1>) : <s2>", prop.Key, prop.Value.Type.ToString(), prop.Value.Converter.ConvertToString(prop.Value.DefaultValue));
                    }

                    return PerformResult.Continue;
                });
        }
        
    }
}