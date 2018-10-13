using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;
using SFS.Rules;
using static SFS.CommandFactory;
using static SFS.Core;

namespace SFS.Commands.StandardActions
{
	internal class Go
	{
		[AtStartup]
        public static void __()
        {
            Core.DefaultParser.AddCommand(
                FirstOf(
                    Sequence(
                        KeyWord("GO"),
                        MustMatch("What way was that?", Cardinal("DIRECTION"))),
                    Cardinal("DIRECTION")))
                .Manual("Move between rooms. 'Go' is optional, a raw cardinal works just as well.")
                .ProceduralRule((match, actor) =>
                {
                    var direction = match["DIRECTION"] as Direction?;
                    var link = actor.Location.EnumerateObjects().OfType<Portal>().FirstOrDefault(thing => thing.Direction == direction.Value);
                    match.Upsert("LINK", link);
                    return PerformResult.Continue;
                }, "lookup link rule")
                .ID("StandardActions:Go")
                .Check("can go?", "ACTOR", "LINK")
                .BeforeActing()
                .Perform("go", "ACTOR", "LINK")
                .AfterActing()
                .ProceduralRule((match, actor) =>
                {
                    Core.MarkLocaleForUpdate(actor);
                    Core.MarkLocaleForUpdate(match["LINK"] as MudObject);
                    return PerformResult.Continue;
                }, "Mark both sides of link for update rule");

            GlobalRules.DeclareCheckRuleBook<Actor, Portal>("can go?", "[Actor, Link] : Can the actor go through that link?", "actor", "link");

            GlobalRules.Check<Actor, Portal>("can go?")
                .When((actor, link) => link == null)
                .Do((actor, link) =>
                {
                    SendMessage(actor, "You can't go that way.");
                    return CheckResult.Disallow;
                })
                .Name("No link found rule.");

            GlobalRules.Check<Actor, Door>("can go?")
                .When((actor, link) => link != null && !link.Open)
                .Do((actor, link) =>
                {
                    SendMessage(actor, "[first opening <the0>]", link);
                    var tryOpen = Core.Try("StandardActions:Open", Core.ExecutingCommand.With("SUBJECT", link), actor);
                    if (tryOpen == PerformResult.Stop)
                        return CheckResult.Disallow;
                    return CheckResult.Continue;
                })
                .Name("Try opening a closed door first rule.");

            GlobalRules.Check<Actor, Portal>("can go?")
                .Do((actor, link) => CheckResult.Allow)
                .Name("Default can go rule.");

            GlobalRules.DeclarePerformRuleBook<Actor, Portal>("go", "[Actor, Link] : Handle the actor going through the link.", "actor", "link");

            GlobalRules.Perform<Actor, Portal>("go")
                .Do((actor, link) =>
                {
                    var direction = link.Direction;
                    SendMessage(actor, "You went <s0>.", direction.ToString().ToLower());
                    SendExternalMessage(actor, "^<the0> went <s1>.", actor, direction.ToString().ToLower());
                    return PerformResult.Continue;
                })
                .Name("Report leaving rule.");

            GlobalRules.Perform<Actor, Portal>("go")
                .Do((actor, link) =>
                {
                    var destination = MudObject.GetObject(link.Destination) as Container;
                    if (destination == null)
                    {
                        SendMessage(actor, "[Error: Link does not lead to a room.]");
                        return PerformResult.Stop;
                    }
                    MoveObject(actor, destination);
                    return PerformResult.Continue;
                })
                .Name("Move through the link rule.");

            GlobalRules.Perform<Actor, Portal>("go")
                .Do((actor, link) =>
                {
                    var direction = link.Direction;
                    var arriveMessage = Link.FromMessage(Link.Opposite(direction));
                    SendExternalMessage(actor, "^<the0> arrives <s1>.", actor, arriveMessage);
                    return PerformResult.Continue;
                })
                .Name("Report arrival rule.");

            GlobalRules.Perform<Actor, Portal>("go")
                .When((actor, link) => actor.Listens)
                .Do((actor, link) =>
                {
                    Core.EnqueuActorCommand(actor, "look", HelperExtensions.MakeDictionary("AUTO", true));
                    return PerformResult.Continue;
                })
                .Name("Players look after going rule.");
        }
    }
}
