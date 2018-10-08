using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace SFS
{
    public class PendingCommand
    {
        public Actor Actor;
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

        public static void EnqueuActorCommand(Actor Actor, String RawCommand, Dictionary<String, Object> MatchPreSettings = null)
        {
            PendingCommands.AddLast(new PendingCommand { Actor = Actor, RawCommand = RawCommand, PreSettings = MatchPreSettings });
        }

        public static void EnqueuActorCommand(Actor Actor, String RawCommand, Action ProcessingCompleteCallback)
        {
            PendingCommands.AddLast(new PendingCommand { Actor = Actor, RawCommand = RawCommand, ProcessingCompleteCallback = ProcessingCompleteCallback });
        }

        public static void EnqueuActorCommand(PendingCommand Command)
        {
            PendingCommands.AddLast(Command);
        }

        private static void InitializeCommandProcessor()
        {
            ParserCommandHandler = new ParserCommandHandler();
        }
        
        /// <summary>
        /// Process commands in a single thread, until there are no more queued commands.
        /// Heartbeat is called between every command.
        /// </summary>
        public static void ProcessCommands()
        {
            while (PendingCommands.Count > 0)
            {
                PendingCommand PendingCommand = null;

                try
                {
                    PendingCommand = PendingCommands.FirstOrDefault();
                    if (PendingCommand != null)
                        PendingCommands.Remove(PendingCommand);
                }
                catch (Exception e)
                {
                    __ShowCommandException(PendingCommand.Actor, e);
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
                        NextCommand.Actor.CommandHandler?.HandleCommand(NextCommand);
                    }
                    catch (Exception e)
                    {
                        Core.ClearPendingMessages();
                        __ShowCommandException(PendingCommand.Actor, e);
                    }

                    if (PendingCommand.ProcessingCompleteCallback != null)
                        PendingCommand.ProcessingCompleteCallback();
                }
            }
        }

        private static void __ShowCommandException(Actor Actor, Exception e)
        {
#if DEBUG
            MudObject.SendMessage(Actor, String.Format("{0:MM/dd/yy HH:mm:ss} -- Error while handling command.", DateTime.Now));
            MudObject.SendMessage(Actor, e.Message);
            MudObject.SendMessage(Actor, e.StackTrace);
            if (e.InnerException != null)
                __ShowCommandException(Actor, e.InnerException);
            SendPendingMessages();
#endif
        }
    }
}
