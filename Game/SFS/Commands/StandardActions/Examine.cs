using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;
using static SFS.CommandFactory;
using static SFS.Core;

namespace SFS.Commands.StandardActions
{
    internal class Examine
    {
        [AtStartup]
        public static void __()
        {
            Core.DefaultParser.AddCommand(Or(KeyWord("EXAMINE"), KeyWord("X")))
                .ID("StandardActions:ExamineArea")
                .Manual("Take a detailed look at your surroundings.")
                .Perform("examine", "ACTOR");

            Core.DefaultParser.AddCommand(
                Sequence(
                    Or(
                        Or(KeyWord("EXAMINE"), KeyWord("X")),
                        Sequence(
                            Or(KeyWord("LOOK"), KeyWord("L")),
                            KeyWord("AT"))),
                    MustMatch("@dont see that", Object("OBJECT", InScope))))
                .ID("StandardActions:ExamineThing")
                .Manual("Take a close look at an object.")
                .Check("can examine?", "ACTOR", "OBJECT")
                .Perform("describe", "ACTOR", "OBJECT");

            Core.StandardMessage("dont see that", "I don't see that here.");

            Core.GlobalRules.DeclareCheckRuleBook<Actor, MudObject>("can examine?", "[Actor, Item] : Can the viewer examine the item?", "actor", "item");

            Core.GlobalRules.Check<Actor, MudObject>("can examine?")
                .First
                .Do((viewer, item) => MudObject.CheckIsVisibleTo(viewer, item))
                .Name("Can't examine what isn't here rule.");

            Core.GlobalRules.Check<Actor, MudObject>("can examine?")
                .Last
                .Do((viewer, item) => SFS.Rules.CheckResult.Allow)
                .Name("Default can examine everything rule.");

            Core.GlobalRules.DeclarePerformRuleBook<Actor>("examine", "[Actor] -> Take a close look at the actor's surroundings.");

            Core.GlobalRules.Perform<Actor>("examine")
                .Do((actor) =>
                {
                    SendMessage(actor, "A detailed account of all objects present.");
                    if (actor.Location != null)
                        foreach (var item in actor.Location.EnumerateObjects().Where(i => !System.Object.ReferenceEquals(i, actor)))
                        {
                            SendMessage(actor, "<a0>", item);
                        }
                    return SFS.Rules.PerformResult.Continue;
                });
        }
    }
}
