using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;
using SFS.Rules;

namespace SFS.Commands.StandardActions
{
	internal class Go : CommandFactory
	{
		public override void Create(CommandParser Parser)
		{
            Parser.AddCommand(
                FirstOf(
                    Sequence(
                        KeyWord("GO"),
                        MustMatch("@unmatched cardinal", Cardinal("DIRECTION"))),
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
		}

        public static void AtStartup(SFS.SFSRuleEngine GlobalRules)
        {
            Core.StandardMessage("unmatched cardinal", "What way was that?");
            Core.StandardMessage("go to null link", "You can't go that way.");
            Core.StandardMessage("go to closed door", "The door is closed.");
            Core.StandardMessage("you went", "You went <s0>.");
            Core.StandardMessage("they went", "^<the0> went <s1>.");
            Core.StandardMessage("bad link", "Error - Link does not lead to a room.");
            Core.StandardMessage("they arrive", "^<the0> arrives <s1>.");
            Core.StandardMessage("first opening", "[first opening <the0>]");

            GlobalRules.DeclareCheckRuleBook<Actor, Portal>("can go?", "[Actor, Link] : Can the actor go through that link?", "actor", "link");

            GlobalRules.Check<Actor, Portal>("can go?")
                .When((actor, link) => link == null)
                .Do((actor, link) =>
                {
                    MudObject.SendMessage(actor, "@go to null link");
                    return CheckResult.Disallow;
                })
                .Name("No link found rule.");

            GlobalRules.Check<Actor, Door>("can go?")
                .When((actor, link) => link != null && !link.Open)
                .Do((actor, link) =>
                {
                    MudObject.SendMessage(actor, "@first opening", link);
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
                    MudObject.SendMessage(actor, "@you went", direction.ToString().ToLower());
                    MudObject.SendExternalMessage(actor, "@they went", actor, direction.ToString().ToLower());
                    return PerformResult.Continue;
                })
                .Name("Report leaving rule.");

            GlobalRules.Perform<Actor, Portal>("go")
                .Do((actor, link) =>
                {
                    var destination = MudObject.GetObject(link.Destination) as Container;
                    if (destination == null)
                    {
                        MudObject.SendMessage(actor, "@bad link");
                        return PerformResult.Stop;
                    }
                    MudObject.Move(actor, destination);
                    return PerformResult.Continue;
                })
                .Name("Move through the link rule.");

            GlobalRules.Perform<Actor, Portal>("go")
                .Do((actor, link) =>
                {
                    var direction = link.Direction;
                    var arriveMessage = Link.FromMessage(Link.Opposite(direction));
                    MudObject.SendExternalMessage(actor, "@they arrive", actor, arriveMessage);
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
