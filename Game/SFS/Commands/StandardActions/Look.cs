﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;
using static SFS.CommandFactory;
using static SFS.Core;

namespace SFS.Commands.StandardActions
{
    internal class Look
    {
        [AtStartup]
        public static void __()
        {
            Core.DefaultParser.AddCommand(
                Or(
                    KeyWord("LOOK"),
                    KeyWord("L")))
                .ID("StandardActions:Look")
                .Manual("Displays a description of your location, and lists what else is present there.")
                .ProceduralRule((match, actor) => Core.GlobalRules.ConsiderPerformRule("describe locale", actor, actor.Location));

            Core.StandardMessage("nowhere", "You aren't anywhere.");
            Core.StandardMessage("dark", "It's too dark to see.");
            Core.StandardMessage("also here", "Also here: <l0>.");
            Core.StandardMessage("on which", "(on which is <l0>)");
            Core.StandardMessage("obvious exits", "Obvious exits:");
            Core.StandardMessage("through", "through <the0>");
            Core.StandardMessage("to", "to <the0>");

            Core.GlobalRules.DeclarePerformRuleBook<Actor, MudObject>("describe in locale", "[Actor, Item] : Generate a locale description for the item.", "actor", "item");

            Core.GlobalRules.DeclarePerformRuleBook<Actor, Room>("describe locale", "[Actor, Room] : Generates a description of the locale.", "actor", "room");

            Core.GlobalRules.Perform<Actor, Room>("describe locale")
                .First
                .When((viewer, room) => room == null)
                .Do((viewer, room) =>
                {
                    SendMessage(viewer, "@nowhere");
                    return SFS.Rules.PerformResult.Stop;
                })
                .Name("Can't describe the locale if there isn't one rule.");

            Core.GlobalRules.Perform<Actor, Room>("describe locale")
                .First
                .Do((viewer, room) =>
                {
                    Core.GlobalRules.ConsiderPerformRule("update", room);
                    return SFS.Rules.PerformResult.Continue;
                })
                .Name("Update room lighting before generating description rule.");

            Core.GlobalRules.Perform<Actor, Room>("describe locale")
                .First
                .Do((viewer, room) =>
                {
                    if (!String.IsNullOrEmpty(room.Short))
                        SendMessage(viewer, room.Short);
                    return SFS.Rules.PerformResult.Continue;
                })
                .Name("Display room name rule.");

            Core.GlobalRules.Perform<Actor, Room>("describe locale")
                .First
                .When((viewer, room) => room.Light == LightingLevel.Dark)
                .Do((viewer, room) =>
                {
                    SendMessage(viewer, "@dark");
                    return SFS.Rules.PerformResult.Stop;
                })
                .Name("Can't see in darkness rule.");

            Core.GlobalRules.Perform<Actor, Room>("describe locale")
                .Do((viewer, room) =>
                {
                    Core.GlobalRules.ConsiderPerformRule("describe", viewer, room);
                    return SFS.Rules.PerformResult.Continue;
                })
                .Name("Include describe rules in locale description rule.");

            // Todo: THIS IS A FUCKING HACK.
            var describingLocale = false;

            Core.GlobalRules.Value<Actor, Container, String, String>("printed name")
                .When((viewer, container, article) => describingLocale && (container.LocationsSupported & RelativeLocations.On) == RelativeLocations.On)                    
                .Do((viewer, container, article) =>
                    {
                        var subObjects = new List<MudObject>(container.EnumerateObjects(RelativeLocations.On)
                        .Where(t => Core.GlobalRules.ConsiderCheckRule("should be listed?", viewer, t) == SFS.Rules.CheckResult.Allow));

                        if (subObjects.Count > 0)
                            return container.Short + " " + Core.FormatMessage(viewer, Core.GetMessage("on which"), subObjects);
                        else
                            return container.Short;
                    })
                    .Name("List contents of container after name when describing locale rule");

            Core.GlobalRules.DeclareCheckRuleBook<MudObject, MudObject>("should be listed in locale?", "[Viewer, Item] : When describing a room, or the contents of a container, should this item be listed?");

            Core.GlobalRules.Check<MudObject, MudObject>("should be listed in locale?")
                .When((viewer, item) => System.Object.ReferenceEquals(viewer, item))
                .Do((viewer, item) => SFS.Rules.CheckResult.Disallow)
                .Name("Don't list yourself rule.");

            Core.GlobalRules.Check<Actor, Scenery>("should be listed in locale?")
               .Do((viewer, item) => SFS.Rules.CheckResult.Disallow)
               .Name("Don't list scenery objects rule.");

            Core.GlobalRules.Check<Actor, Portal>("should be listed in locale?")
                .Do((viewer, item) => SFS.Rules.CheckResult.Disallow)
                .Name("Don't list portals rule.");

            Core.GlobalRules.Check<MudObject, MudObject>("should be listed in locale?")
               .Do((viewer, item) => SFS.Rules.CheckResult.Allow)
               .Name("List objects by default rule.");

            Core.GlobalRules.Perform<Actor, Room>("describe locale")
                .Do((viewer, room) =>
                {
                    var visibleThings = room.EnumerateObjects(RelativeLocations.Contents)
                        .Where(t => Core.GlobalRules.ConsiderCheckRule("should be listed in locale?", viewer, t) == SFS.Rules.CheckResult.Allow);

                    var normalContents = new List<MudObject>();

                    foreach (var thing in visibleThings)
                    {
                        Core.BeginOutputQuery();
                        Core.GlobalRules.ConsiderPerformRule("describe in locale", viewer, thing);
                        if (!Core.CheckOutputQuery()) normalContents.Add(thing);
                    }

                    if (normalContents.Count > 0)
                    {
                        describingLocale = true;
                        SendMessage(viewer, "@also here", normalContents);
                        describingLocale = false;
                    }

                    return SFS.Rules.PerformResult.Continue;
                })
                .Name("List contents of room rule.");

            Core.GlobalRules.Perform<Actor, Room>("describe locale")
                .Last
                .Do((viewer, room) =>
                {
                    if (room.EnumerateObjects().OfType<Portal>().Count() > 0)
                    {
                        SendMessage(viewer, "@obvious exits");

                        foreach (var link in room.EnumerateObjects<MudObject>().OfType<Portal>())
                        {
                            var builder = new StringBuilder();
                            builder.Append("  ^");
                            builder.Append(link.Direction.ToString());

                            if (!link.Anonymous)
                                builder.Append(" " + Core.FormatMessage(viewer, Core.GetMessage("through"), link));

                            var destinationRoom = MudObject.GetObject(link.Destination);
                            if (destinationRoom != null)
                                builder.Append(" " + Core.FormatMessage(viewer, Core.GetMessage("to"), destinationRoom));

                            SendMessage(viewer, builder.ToString());
                        }
                    }

                    return SFS.Rules.PerformResult.Continue;
                })
                .Name("List exits in locale description rule.");
        }
    }
}
