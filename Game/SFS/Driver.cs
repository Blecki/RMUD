using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFS.SinglePlayer
{
    public class Driver
    {
        public MudObject Player { get; private set; }
        public Action<String> Output;

        public Driver()
        {
        }

        public void SwitchPlayerCharacter(MudObject NewCharacter)
        {
            if (Player != null)
                Player.SetProperty("output", null);

            NewCharacter.SetProperty("command handler", Core.ParserCommandHandler);
            NewCharacter.SetProperty("output", Output); // There should be a perform rulebook for handling this.
            NewCharacter.SetProperty("listens?", true);
            Player = NewCharacter;
        }
        
        public bool Start(
            System.Reflection.Assembly DatabaseAssembly, 
            Action<String> Output)
        {
            this.Output = Output;

            if (SFS.Core.Start(new SFS.SinglePlayer.CompiledDatabase(DatabaseAssembly)))
            {
                SwitchPlayerCharacter(SFS.MudObject.GetObject("Game.Player"));
                Core.GlobalRules.ConsiderPerformRule("singleplayer game started", Player);

                return true;
            }

            return false;
        }

        public void Input(String Command)
        {
            SFS.Core.EnqueuActorCommand(Player, Command);
            SFS.Core.ProcessCommands();
        }
    }
}
