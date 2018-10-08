using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFS;
using SFS.Rules;

namespace SFS.Commands.StandardActions
{
    public class ClothingRules 
    {
        [AtStartup]
        public static void __()
        {
            Core.GlobalRules.Perform<Actor>("inventory")
                .Do(a =>
                {
                    var wornObjects = a.GetContents(RelativeLocations.Worn);
                    if (wornObjects.Count == 0) MudObject.SendMessage(a, "@nude");
                    else
                    {
                        MudObject.SendMessage(a, "@clothing wearing");
                        foreach (var item in wornObjects)
                            MudObject.SendMessage(a, "  <a0>", item);
                    }
                    return PerformResult.Continue;
                })
                .Name("List worn items in inventory rule.");

            Core.GlobalRules.Check<Actor, Clothing>("can wear?")
                .Do((actor, item) =>
                {
                    foreach (Clothing wornItem in actor.EnumerateObjects(RelativeLocations.Worn).OfType<Clothing>())
                        if (wornItem.Layer == item.Layer && wornItem.BodyPart == item.BodyPart)
                        {
                            MudObject.SendMessage(actor, "@clothing remove first", wornItem);
                            return CheckResult.Disallow;
                        }
                    return CheckResult.Continue;
                })
                .Name("Check clothing layering before wearing rule.");

            Core.GlobalRules.Check<Actor, Clothing>("can remove?")
                .Do((actor, item) =>
                {
                    foreach (var wornItem in actor.EnumerateObjects(RelativeLocations.Worn).OfType<Clothing>())
                        if (wornItem.Layer < item.Layer && wornItem.BodyPart == item.BodyPart)
                        {
                            MudObject.SendMessage(actor, "@clothing remove first", wornItem);
                            return CheckResult.Disallow;
                        }
                    return CheckResult.Allow;
                })
                .Name("Can't remove items under other items rule.");


            Core.GlobalRules.Perform<Actor, Actor>("describe")
                .First
                .Do((viewer, actor) =>
                {
                    var wornItems = actor.GetContents(RelativeLocations.Worn);
                    if (wornItems.Count == 0)
                        MudObject.SendMessage(viewer, "@clothing they are nude", actor);
                    else
                        MudObject.SendMessage(viewer, "@clothing they are wearing", actor, wornItems);
                    return PerformResult.Continue;
                })
                .Name("List worn items when describing an actor rule.");
        }
    }
}
