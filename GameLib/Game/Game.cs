using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public static class Game
    {
        public static SFS.Actor Player { get; private set; }
        private static Action<String> Output;
        
        public static void SwitchPlayerCharacter(SFS.Actor NewCharacter)
        {
            if (Player != null)
                Player.Output = null;

            NewCharacter.CommandHandler = SFS.Core.ParserCommandHandler;
            NewCharacter.Output = Output;
            NewCharacter.Listens = true;
            Player = NewCharacter;
        }
        
        public static void Begin(Action<String> Output)
        {
            Game.Output = Output;
            SFS.Core.Start();

            SwitchPlayerCharacter(SFS.Core.GetObject("Game.Player") as SFS.Actor);
            SFS.Core.MoveObject(Player, SFS.Core.GetObject("Game.Foyer") as SFS.Room);
            SFS.Core.SendMessage(Player, "Hurrying through the rainswept November night, you're glad to see the bright lights of the Opera House. It's surprising that there aren't more people about but, hey, what do you expect in a cheap demo game...?");

            Input("look");
        }

        public static void Input(String Command)
        {
            SFS.Core.EnqueuActorCommand(Player, Command);
            SFS.Core.ProcessCommands();
        }
    }
}