using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace RMUD
{
    public class PendingCommand
    {
        public MudObject Actor;
        public String RawCommand;
        internal Action ProcessingCompleteCallback;
        internal Dictionary<String, Object> PreSettings;
    }

    public static partial class Core
    {
        private static LinkedList<PendingCommand> PendingCommands = new LinkedList<PendingCommand>();
        private static PendingCommand NextCommand;

        //The client command handler can set this flag when it wants the command timeout to be ignored.
        public static bool CommandTimeoutEnabled = true;

        public static ParserCommandHandler ParserCommandHandler;
        public static CommandParser DefaultParser;

        public static void EnqueuActorCommand(MudObject Actor, String RawCommand, Dictionary<String, Object> MatchPreSettings = null)
        {
            PendingCommands.AddLast(new PendingCommand { Actor = Actor, RawCommand = RawCommand, PreSettings = MatchPreSettings });
        }

        public static void EnqueuActorCommand(MudObject Actor, String RawCommand, Action ProcessingCompleteCallback)
        {
            PendingCommands.AddLast(new PendingCommand { Actor = Actor, RawCommand = RawCommand, ProcessingCompleteCallback = ProcessingCompleteCallback });
        }

        public static void EnqueuActorCommand(PendingCommand Command)
        {
            PendingCommands.AddLast(Command);
        }

        internal static void DiscoverCommandFactories(ModuleAssembly In, CommandParser AddTo)
        {
            foreach (var type in In.Assembly.GetTypes())
            {
                AddTo.ModuleBeingInitialized = In.FileName;
                if (type.IsSubclassOf(typeof(CommandFactory)))
                    CommandFactory.CreateCommandFactory(type).Create(AddTo);
            }
        }

        private static void InitializeCommandProcessor()
        {
            DiscoverCommandFactories(new ModuleAssembly(Assembly.GetExecutingAssembly(), ""), DefaultParser);
            ParserCommandHandler = new ParserCommandHandler();
        }
        
        /// <summary>
        /// Process commands in a single thread, until there are no more queued commands.
        /// Heartbeat is called between every command.
        /// </summary>
        public static void ProcessCommands()
        {
            while (PendingCommands.Count > 0 && !ShuttingDown)
            {
                GlobalRules.ConsiderPerformRule("heartbeat");
               
                PendingCommand PendingCommand = null;

                try
                {
                    PendingCommand = PendingCommands.FirstOrDefault();
                    if (PendingCommand != null)
                        PendingCommands.Remove(PendingCommand);
                }
                catch (Exception e)
                {
                    LogCommandError(e);
                    PendingCommand = null;
                }

                if (PendingCommand != null)
                {
                    NextCommand = PendingCommand;

                    //Reset flags that the last command may have changed
                    CommandTimeoutEnabled = true;
                    SilentFlag = false;
                    GlobalRules.LogRules(null);

                    try
                    {
                        var handler = NextCommand.Actor.GetProperty<ClientCommandHandler>("command handler");
                        if (handler != null)
                            handler.HandleCommand(NextCommand);
                    }
                    catch (Exception e)
                    {
                        LogCommandError(e);
                        Core.ClearPendingMessages();
                    }
                    if (PendingCommand.ProcessingCompleteCallback != null)
                        PendingCommand.ProcessingCompleteCallback();

                }
            }
        }
    }
}
