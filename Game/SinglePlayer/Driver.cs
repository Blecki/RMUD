using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMUD.SinglePlayer
{
    public class Driver
    {
        public MudObject Player { get; private set; }
        public Action<String> Output;
        
        public bool IsRunning
        {
            get
            {
                return !Core.ShuttingDown;
            }
        }

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

            if (RMUD.Core.Start(StartupFlags.Silent,
                "database/",
                new RMUD.SinglePlayer.CompiledDatabase(DatabaseAssembly)))
            {
                SwitchPlayerCharacter(RMUD.MudObject.GetObject("CloakOfDarkness.Player"));

                Core.GlobalRules.ConsiderPerformRule("singleplayer game started", Player);

                return true;
            }

            return false;
        }

        public void Input(String Command)
        {
            RMUD.Core.EnqueuActorCommand(Player, Command);
            RMUD.Core.ProcessCommands();
        }
    }
}
