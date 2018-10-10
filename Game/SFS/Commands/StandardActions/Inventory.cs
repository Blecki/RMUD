using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS;
using static SFS.CommandFactory;
using static SFS.Core;

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

            GlobalRules.DeclarePerformRuleBook<Actor>("inventory", "[Actor] : Describes a player's inventory to themselves.", "actor");

            GlobalRules.Perform<Actor>("inventory")
                .Do(a =>
                {
                    var heldObjects = a.GetContents(RelativeLocations.Held);
                    if (heldObjects.Count == 0) SendMessage(a, "@empty handed", a);
                    else
                    {
                        SendMessage(a, "You are carrying..");
                        foreach (var item in heldObjects)
                            SendMessage(a, "  <a0>", item);
                    }
                    return SFS.Rules.PerformResult.Continue;
                })
                .Name("List held items in inventory rule.");
        }
    }
}
