using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;
using static SFS.CommandFactory;
using static SFS.Core;

namespace SFS.Commands.Debug
{
    internal class Set
    {
        [AtStartup]
        public static void __()
        { 
            Core.DefaultParser.AddCommand(
                Sequence(
                    DebugOnly(),
                    KeyWord("!SET"),
                    MustMatch("I don't see that here.",
                        Object("OBJECT", InScope)),
                    SingleWord("PROPERTY"),
                    Rest("VALUE")))
                .Manual("Set the value of a property on an object.")
                .ProceduralRule((match, actor) =>
                {
                    //var _object = match["OBJECT"] as MudObject;
                    //var property_name = match["PROPERTY"].ToString();
                    //var stringValue = match["VALUE"].ToString();

                    //var propertyInfo = PropertyManifest.GetPropertyInformation(property_name);

                    //if (propertyInfo == null)
                    //{
                    //    SendMessage(actor, "That property does not exist.");
                    //    return SFS.Rules.PerformResult.Stop;
                    //}

                    //var realValue = propertyInfo.Converter.ConvertFromString(stringValue);
                    // Todo: Implement using reflection.
                    //_object.SetProperty(property_name, realValue);
                    
                    SendMessage(actor, "Property set.");
                    return SFS.Rules.PerformResult.Continue;
                });
        }
    }
}