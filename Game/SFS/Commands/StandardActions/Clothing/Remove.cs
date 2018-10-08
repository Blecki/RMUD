using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;
using SFS.Rules;

namespace SFS.Commands.StandardActions
{
	internal class Remove : CommandFactory
	{
        public override void Create(CommandParser Parser)
        {
            Parser.AddCommand(
                Sequence(
                    KeyWord("REMOVE"),
                    BestScore("OBJECT",
                        MustMatch("@clothing remove what",
                            Object("OBJECT", InScope, PreferWorn)))))
                .Manual("Expose your amazingly supple flesh.")
                .Check("can remove?", "ACTOR", "OBJECT")
                .BeforeActing()
                .Perform("remove", "ACTOR", "OBJECT")
                .AfterActing();
        }

        public static void AtStartup(SFS.SFSRuleEngine GlobalRules)
        {
            GlobalRules.DeclareCheckRuleBook<Actor, Clothing>("can remove?", "[Actor, Item] : Can the actor remove the item?", "actor", "item");
            GlobalRules.DeclarePerformRuleBook<Actor, Clothing>("remove", "[Actor, Item] : Handle the actor removing the item.", "actor", "item");

            GlobalRules.Check<Actor, Clothing>("can remove?")
                .When((a, b) => !a.Contains(b, RelativeLocations.Worn))
                .Do((actor, item) =>
                {
                    MudObject.SendMessage(actor, "@clothing not wearing");
                    return CheckResult.Disallow;
                });
           
            GlobalRules.Check<Actor, Clothing>("can remove?").Do((a, b) => CheckResult.Allow);

            GlobalRules.Perform<Actor, Clothing>("remove").Do((actor, target) =>
                {
                    MudObject.SendMessage(actor, "@clothing you remove", target);
                    MudObject.SendExternalMessage(actor, "@clothing they remove", actor, target);
                    MudObject.Move(target, actor, RelativeLocations.Held);
                    return PerformResult.Continue;
                });
        }
    }
}
