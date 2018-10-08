using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;
using static SFS.CommandFactory;

namespace SFS.Commands.StandardActions
{
    internal class Inventory
    {
        [AtStartup]
        public static void __()
        {
            Core.DefaultParser.AddCommand(
                Or(
                    KeyWord("INVENTORY"),
                    KeyWord("INV"),
                    KeyWord("I")))
                .ID("StandardActions:Inventory")
                .Manual("Displays what you are wearing and carrying.")
                .Perform("inventory", "ACTOR");

            Core.StandardMessage("carrying", "You are carrying..");

            Core.GlobalRules.DeclarePerformRuleBook<Actor>("inventory", "[Actor] : Describes a player's inventory to themselves.", "actor");

            Core.GlobalRules.Perform<Actor>("inventory")
                .Do(a =>
                {
                    var heldObjects = a.GetContents(RelativeLocations.Held);
                    if (heldObjects.Count == 0) MudObject.SendMessage(a, "@empty handed", a);
                    else
                    {
                        MudObject.SendMessage(a, "@carrying");
                        foreach (var item in heldObjects)
                            MudObject.SendMessage(a, "  <a0>", item);
                    }
                    return SFS.Rules.PerformResult.Continue;
                })
                .Name("List held items in inventory rule.");
        }
    }
}
