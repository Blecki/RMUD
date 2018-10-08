using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFS;
using SFS.Rules;
using static SFS.Core;

namespace SFS.Commands.StandardActions
{
    public class ClothingRules 
    {
        [AtStartup]
        public static void __()
        {
            GlobalRules.Perform<Actor>("inventory")
                .Do(a =>
                {
                    var wornObjects = a.GetContents(RelativeLocations.Worn);
                    if (wornObjects.Count == 0) SendMessage(a, "@nude");
                    else
                    {
                        SendMessage(a, "@clothing wearing");
                        foreach (var item in wornObjects)
                            SendMessage(a, "  <a0>", item);
                    }
                    return PerformResult.Continue;
                })
                .Name("List worn items in inventory rule.");

            GlobalRules.Check<Actor, Clothing>("can wear?")
                .Do((actor, item) =>
                {
                    foreach (Clothing wornItem in actor.EnumerateObjects(RelativeLocations.Worn).OfType<Clothing>())
                        if (wornItem.Layer == item.Layer && wornItem.BodyPart == item.BodyPart)
                        {
                            SendMessage(actor, "@clothing remove first", wornItem);
                            return CheckResult.Disallow;
                        }
                    return CheckResult.Continue;
                })
                .Name("Check clothing layering before wearing rule.");

            GlobalRules.Check<Actor, Clothing>("can remove?")
                .Do((actor, item) =>
                {
                    foreach (var wornItem in actor.EnumerateObjects(RelativeLocations.Worn).OfType<Clothing>())
                        if (wornItem.Layer < item.Layer && wornItem.BodyPart == item.BodyPart)
                        {
                            SendMessage(actor, "@clothing remove first", wornItem);
                            return CheckResult.Disallow;
                        }
                    return CheckResult.Allow;
                })
                .Name("Can't remove items under other items rule.");


            GlobalRules.Perform<Actor, Actor>("describe")
                .First
                .Do((viewer, actor) =>
                {
                    var wornItems = actor.GetContents(RelativeLocations.Worn);
                    if (wornItems.Count == 0)
                        SendMessage(viewer, "@clothing they are nude", actor);
                    else
                        SendMessage(viewer, "@clothing they are wearing", actor, wornItems);
                    return PerformResult.Continue;
                })
                .Name("List worn items when describing an actor rule.");
        }
    }
}
