using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static SFS.Core;

namespace SFS
{
    public class MudObjectRules 
    {
        [Initialize]
        public static void __()
        {
            GlobalRules.DeclareValueRuleBook<Actor, MudObject, String, String>("printed name", "[Viewer, Object, Article -> String] : Find the name that should be displayed for an object.", "actor", "item", "article");

            GlobalRules.Value<Actor, MudObject, String, String>("printed name")
               .Last
               .Do((viewer, thing, article) => (String.IsNullOrEmpty(article) ? (thing.Short) : (article + " " + thing.Short)))
               .Name("Default name of a thing.");
        }
    }
}
