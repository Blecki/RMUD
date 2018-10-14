using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;
using SFS.Rules;
using static SFS.Core;

namespace SFS.Commands.StandardActions
{
    internal static class DescribeRules
    {
        [Initialize]
        public static void __()
        {
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject>("describe", "[Actor, Item] : Generates descriptions of the item.", "actor", "item");

            GlobalRules.Perform<Actor, MudObject>("describe")
                .When((viewer, item) => !String.IsNullOrEmpty(item.Long))
                .Do((viewer, item) =>
                {
                    SendMessage(viewer, item.Long);
                    return PerformResult.Continue;
                })
                .Name("Basic description rule.");

            GlobalRules.Perform<Actor, Container>("describe")
                .When((viewer, item) => (item.SupportedLocations & RelativeLocations.On) == RelativeLocations.On)
                .Do((viewer, item) =>
                {
                    var contents = item.GetContents(RelativeLocations.On);
                    if (contents.Count() > 0)
                        SendMessage(viewer, "On <the0> is <l1>.", item, contents);
                    return PerformResult.Continue;
                })
                .Name("List things on container in description rule.");

            GlobalRules.Perform<Actor, OpenableContainer>("describe")
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
                        SendMessage(viewer, "In <the0> is <l1>.", item, contents);
                    return PerformResult.Continue;
                })
                .Name("List things in open container in description rule.");


            GlobalRules.Perform<Actor, Actor>("describe")
                .First
                .Do((viewer, actor) =>
                {
                    var heldItems = new List<MudObject>(actor.EnumerateObjects(RelativeLocations.Held));
                    if (heldItems.Count == 0)
                        SendMessage(viewer, "^<the0> is empty handed.", actor);
                    else
                        SendMessage(viewer, "^<the0> is holding <l1>.", actor, heldItems);

                    return PerformResult.Continue;
                })
                .ID("list-actor-held-items-rule")
                .Name("List held items when describing an actor rule.");
        }
    }
}
