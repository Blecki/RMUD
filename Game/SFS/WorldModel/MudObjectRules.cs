using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SFS
{
    public class MudObjectRules 
    {
        [AtStartup]
        public static void __()
        {
            Core.GlobalRules.DeclareValueRuleBook<Actor, MudObject, String, String>("printed name", "[Viewer, Object, Article -> String] : Find the name that should be displayed for an object.", "actor", "item", "article");

            Core.GlobalRules.Value<Actor, MudObject, String, String>("printed name")
               .Last
               .Do((viewer, thing, article) => (String.IsNullOrEmpty(article) ? (thing.Short) : (article + " " + thing.Short)))
               .Name("Default name of a thing.");
        }
    }
}
