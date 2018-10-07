using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloakOfDarkness
{
    public static class Game
    {
        public static RMUD.SinglePlayer.Driver Driver { get; set; }
        internal static RMUD.MudObject Player { get { return Driver.Player; } }

        public static void SwitchPlayerCharacter(RMUD.MudObject NewCharacter)
        {
            Driver.SwitchPlayerCharacter(NewCharacter);
        }

        public static void AtStartup(RMUD.RuleEngine GlobalRules)
        {
            GlobalRules.Perform<Player>("singleplayer game started")
                .First
                .Do((actor) =>
                {
                    SwitchPlayerCharacter(RMUD.MudObject.GetObject("CloakOfDarkness.Player"));
                    RMUD.MudObject.Move(Player, RMUD.MudObject.GetObject("CloakOfDarkness.Foyer"));
                    RMUD.MudObject.SendMessage(Player, "Hurrying through the rainswept November night, you're glad to see the bright lights of the Opera House. It's surprising that there aren't more people about but, hey, what do you expect in a cheap demo game...?");
                    Driver.Input("look");
                    //RMUD.Core.EnqueuActorCommand(Player, "look");
        
                    return SharpRuleEngine.PerformResult.Stop;
                });
        }
    }
}