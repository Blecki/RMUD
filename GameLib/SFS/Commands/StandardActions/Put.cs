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
	internal class Put
	{
        [Initialize]
        public static void __()
        {
            Core.DefaultParser.AddCommand(
                Sequence(
                    KeyWord("PUT"),
                    BestScore("SUBJECT",
                        MustMatch("You don't have that.",
                            Object("SUBJECT", InScope, PreferHeld))),
                    Optional(RelativeLocation("RELLOC")),
                    BestScore("OBJECT",
                        MustMatch("I don't see that here.",
                            Object("OBJECT", InScope, (actor, thing) =>
                            {
                                //Prefer objects that are actually containers. No means curently to prefer
                                //objects that actually support the relloc we matched previously.
                                if (thing is Container) return MatchPreference.Likely;
                                return MatchPreference.Plausible;
                            })))))
                .ID("StandardActions:Put")
                .Manual("This commands allows you to put things on other things. While dropping just deposits the object into your current location, putting is much more specific.")
                .ProceduralRule((match, actor) =>
                {
                    if (!match.ContainsKey("RELLOC"))
                    {
                        if (match["OBJECT"] is Container)
                            match.Upsert("RELLOC", (match["OBJECT"] as Container).DefaultLocation);
                        else
                            match.Upsert("RELLOC", RelativeLocations.On);
                    }
                    return SFS.Rules.PerformResult.Continue;
                }, "Supply default for optional relloc procedural rule.")
                .Check("can put?", "ACTOR", "SUBJECT", "OBJECT", "RELLOC")
                .BeforeActing()
                .Perform("put", "ACTOR", "SUBJECT", "OBJECT", "RELLOC")
                .AfterActing()
                .MarkLocaleForUpdate();

            GlobalRules.DeclareCheckRuleBook<MudObject, MudObject, MudObject, RelativeLocations>("can put?", "[Actor, Item, Container, Location] : Determine if the actor can put the item in or on or under the container.", "actor", "item", "container", "relloc");
            GlobalRules.DeclarePerformRuleBook<MudObject, MudObject, MudObject, RelativeLocations>("put", "[Actor, Item, Container, Location] : Handle an actor putting the item in or on or under the container.", "actor", "item", "container", "relloc");

            GlobalRules.Check<MudObject, MudObject, MudObject, RelativeLocations>("can put?")
                .Last
                .Do((a, b, c, d) => CheckResult.Allow)
                .Name("Allow putting as default rule.");

            GlobalRules.Check<Actor, MudObject, MudObject, RelativeLocations>("can put?")
                .Do((actor, item, container, relloc) =>
                {
                    if (!(container is Container))
                    {
                        SendMessage(actor, "You can't put things <s0> that.", Relloc.GetRelativeLocationName(relloc));
                        return CheckResult.Disallow;
                    }
                    return CheckResult.Continue;
                })
                .Name("Can't put things in things that aren't containers rule.");

            GlobalRules.Check<MudObject, MudObject, MudObject, RelativeLocations>("can put?")
                .Do((actor, item, container, relloc) =>
                {
                    if (GlobalRules.ConsiderCheckRule("can drop?", actor, item) != CheckResult.Allow)
                        return CheckResult.Disallow;
                    return CheckResult.Continue;
                })
                .Name("Putting is dropping rule.");

            GlobalRules.Perform<Actor, MudObject, Container, RelativeLocations>("put")
                .Do((actor, item, container, relloc) =>
                {
                    SendMessage(actor, "You put <the0> <s1> <the2>.", item, Relloc.GetRelativeLocationName(relloc), container);
                    SendExternalMessage(actor, "^<the0> puts <the1> <s2> <the3>.", actor, item, Relloc.GetRelativeLocationName(relloc), container);
                    MoveObject(item, container, relloc);
                    return PerformResult.Continue;
                })
                .Name("Default putting things in things handler.");

            GlobalRules.Check<Actor, MudObject, Container, RelativeLocations>("can put?")
                .Do((actor, item, container, relloc) =>
                {
                    if ((container.SupportedLocations & relloc) != relloc)
                    {
                        SendMessage(actor, "@cant put relloc", Relloc.GetRelativeLocationName(relloc));
                        return CheckResult.Disallow;
                    }
                    return CheckResult.Continue;
                })
                .Name("Check supported locations before putting rule.");

            GlobalRules.Check<Actor, MudObject, OpenableContainer, RelativeLocations>("can put?")
                .Do((actor, item, container, relloc) =>
                {
                    if (relloc == RelativeLocations.In && !container.Open)
                    {
                        SendMessage(actor, "^<the0> is closed.", container);
                        return CheckResult.Disallow;
                    }

                    return CheckResult.Continue;
                })
                .Name("Can't put things in closed container rule.");

            GlobalRules.Check<Actor, MudObject, MudObject, RelativeLocations>("can put?")
                .First
                .Do((actor, item, container, relloc) => CheckIsVisibleTo(actor, container))
                .Name("Container must be visible rule.");

            GlobalRules.Check<Actor, MudObject, MudObject, RelativeLocations>("can put?")
                .First
                .Do((actor, item, container, relloc) => CheckIsHolding(actor, item))
                .Name("Must be holding item rule.");
        }
    }
}
