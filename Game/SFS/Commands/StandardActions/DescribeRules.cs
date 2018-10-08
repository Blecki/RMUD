using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;
using SFS.Rules;

namespace SFS.Commands.StandardActions
{
    internal static class DescribeRules
    {
        [AtStartup]
        public static void __()
        {
            Core.StandardMessage("is open", "^<the0> is open.");
            Core.StandardMessage("is closed", "^<the0> is closed.");
            Core.StandardMessage("describe on", "On <the0> is <l1>.");
            Core.StandardMessage("describe in", "In <the0> is <l1>.");
            Core.StandardMessage("empty handed", "^<the0> is empty handed.");
            Core.StandardMessage("holding", "^<the0> is holding <l1>.");

            Core.GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("describe", "[Actor, Item] : Generates descriptions of the item.", "actor", "item");

            Core.GlobalRules.Perform<Actor, MudObject>("describe")
                .When((viewer, item) => !String.IsNullOrEmpty(item.Long))
                .Do((viewer, item) =>
                {
                    MudObject.SendMessage(viewer, item.Long);
                    return PerformResult.Continue;
                })
                .Name("Basic description rule.");

            //Todo: Containers/Doors implement this rule.
            //GlobalRules.Perform<MudObject, MudObject>("describe")
            //    .When((viewer, item) => item.GetProperty<bool>("openable?"))
            //    .Do((viewer, item) =>
            //    {
            //        if (item.GetProperty<bool>("open?"))
            //            MudObject.SendMessage(viewer, "@is open", item);
            //        else
            //            MudObject.SendMessage(viewer, "@is closed", item);
            //        return PerformResult.Continue;
            //    })
            //    .Name("Describe open or closed state rule.");

            Core.GlobalRules.Perform<Actor, Container>("describe")
                .When((viewer, item) => (item.LocationsSupported & RelativeLocations.On) == RelativeLocations.On)
                .Do((viewer, item) =>
                {
                    var contents = item.GetContents(RelativeLocations.On);
                    if (contents.Count() > 0)
                        MudObject.SendMessage(viewer, "@describe on", item, contents);
                    return PerformResult.Continue;
                })
                .Name("List things on container in description rule.");

            Core.GlobalRules.Perform<Actor, OpenableContainer>("describe")
                .When((viewer, item) =>
                    {
                        if (!item.Open) return false;
                        if (item.EnumerateObjects(RelativeLocations.In).Count() == 0) return false;
                        return true;
                    })
                .Do((viewer, item) =>
                {
                    var contents = item.GetContents(RelativeLocations.In);
                    if (contents.Count() > 0)
                        MudObject.SendMessage(viewer, "@describe in", item, contents);
                    return PerformResult.Continue;
                })
                .Name("List things in open container in description rule.");


            Core.GlobalRules.Perform<Actor, Actor>("describe")
                .First
                .Do((viewer, actor) =>
                {
                    var heldItems = new List<MudObject>(actor.EnumerateObjects(RelativeLocations.Held));
                    if (heldItems.Count == 0)
                        MudObject.SendMessage(viewer, "@empty handed", actor);
                    else
                        MudObject.SendMessage(viewer, "@holding", actor, heldItems);

                    return PerformResult.Continue;
                })
                .ID("list-actor-held-items-rule")
                .Name("List held items when describing an actor rule.");
        }
    }
}
