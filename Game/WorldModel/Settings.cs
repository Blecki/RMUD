using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RMUD
{
	public class Settings : MudObject
	{
        public int AllowedCommandRate = 100; //How many milliseconds to allow between commands - default is to not limit very much.

        //How many milliseconds should a command be allowed to run before it is aborted?
        //Aborting a player's command is always reported as a critical error, however this
        //  helps guard against infinite loops in the database source that could lock up
        //  the server.
        public int CommandTimeOut = 10000;

        public int AFKTime = 1000 * 60 * 5; //Go AFK after five minutes of inactivity.

        public int MaximumChatChannelLogSize = 1000;
        public int HeartbeatInterval = 1000; //Heartbeat every second
        public TimeSpan ClockAdvanceRate = TimeSpan.FromSeconds(10);
	}
}
