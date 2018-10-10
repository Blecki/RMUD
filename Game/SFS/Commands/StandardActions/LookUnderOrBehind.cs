using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;
using static SFS.CommandFactory;
using static SFS.Core;

namespace SFS.Commands.StandardActions
{
    internal class LookUnderOrBehind
    {
        [AtStartup]
        public static void __()
        {
            Core.DefaultParser.AddCommand(
                Sequence(
                    Or(
                        KeyWord("LOOK"),
                        KeyWord("L")),
                    RelativeLocation("RELLOC"),
                    Object("OBJECT", InScope)))
                .ID("StandardActions:LookRelloc")
                .Manual("Lists object that are in, on, under, or behind the object specified.")
                .Check("can look relloc?", "ACTOR", "OBJECT", "RELLOC")
                .Perform("look relloc", "ACTOR", "OBJECT", "RELLOC");

            GlobalRules.DeclareCheckRuleBook<Actor, Container, RelativeLocations>("can look relloc?", "[Actor, Item, Relative Location] : Can the actor look in/on/under/behind the item?", "actor", "item", "relloc");

            GlobalRules.Check<Actor, Container, RelativeLocations>("can look relloc?")
                .Do((actor, item, relloc) => CheckIsVisibleTo(actor, item))
                .Name("Container must be visible rule.");

            GlobalRules.Check<Actor, Container, RelativeLocations>("can look relloc?")
                .When((actor, item, relloc) => (item.SupportedLocations & relloc) != relloc)
                .Do((actor, item, relloc) =>
                {
                    SendMessage(actor, "You can't look <s0> that.", Relloc.GetRelativeLocationName(relloc));
                    return SFS.Rules.CheckResult.Disallow;
                })
                .Name("Container must support relloc rule.");

            GlobalRules.Check<Actor, OpenableContainer, RelativeLocations>("can look relloc?")
                .When((actor, item, relloc) => (relloc == RelativeLocations.In) && !item.Open)
                .Do((actor, item, relloc) =>
                {
                        SendMessage(actor, "^<the0> is closed.", item);
                        return SFS.Rules.CheckResult.Disallow;
                })
                .Name("Container must be open to look in rule.");

            GlobalRules.Check<Actor, Container, RelativeLocations>("can look relloc?")
                .Do((actor, item, relloc) => SFS.Rules.CheckResult.Allow)
                .Name("Default allow looking relloc rule.");

            GlobalRules.DeclarePerformRuleBook<Actor, Container, RelativeLocations>("look relloc", "[Actor, Item, Relative Location] : Handle the actor looking on/under/in/behind the item.", "actor", "item", "relloc");

            GlobalRules.Perform<Actor, Container, RelativeLocations>("look relloc")
                .Do((actor, item, relloc) =>
                {
                    var contents = new List<MudObject>(item.EnumerateObjects(relloc));

                    if (contents.Count > 0)
                    {
                        SendMessage(actor, "^<s0) <the1> is..", Relloc.GetRelativeLocationName(relloc), item);
                        foreach (var thing in contents)
                            SendMessage(actor, "  <a0>", thing);
                    }
                    else
                        SendMessage(actor, "There is nothing <s0> <the1>.", Relloc.GetRelativeLocationName(relloc), item);

                    return SFS.Rules.PerformResult.Continue;
                })
                .Name("List contents in relative location rule.");
        }
    }
}