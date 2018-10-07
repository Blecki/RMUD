using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public static class Game
    {
        public static SFS.SinglePlayer.Driver Driver { get; set; }
        internal static SFS.Actor Player { get { return Driver.Player; } }

        public static void SwitchPlayerCharacter(SFS.Actor NewCharacter)
        {
            Driver.SwitchPlayerCharacter(NewCharacter);
        }

        public static void Begin(Action<String> Output)
        {
            Driver = new SFS.SinglePlayer.Driver();
            Driver.Start(System.Reflection.Assembly.GetExecutingAssembly(), Output);

            SwitchPlayerCharacter(SFS.MudObject.GetObject("Game.Player") as SFS.Actor);
            SFS.MudObject.Move(Player, SFS.MudObject.GetObject("Game.Foyer") as SFS.Room);
            SFS.MudObject.SendMessage(Player, "Hurrying through the rainswept November night, you're glad to see the bright lights of the Opera House. It's surprising that there aren't more people about but, hey, what do you expect in a cheap demo game...?");

            Driver.Input("look");
        }

        public static void Input(String Command)
        {
            Driver.Input(Command);
        }
    }
}