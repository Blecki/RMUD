using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace SFS
{
    internal struct PendingMessage
    {
        public Actor Destination;
        public String Message;

        public PendingMessage(Actor Destination, String Message)
        {
            this.Destination = Destination;
            this.Message = Message;
        }
    }

    public static partial class Core
    {
		internal static List<PendingMessage> PendingMessages = new List<PendingMessage>();
                
        /// <summary>
        /// Send all messages currently in the pending message queue to their intended ricipients.
        /// </summary>
        public static void SendPendingMessages()
        {
            foreach (var message in PendingMessages)
            {
                message.Destination.Output?.Invoke(message.Message + "\r\n");
            }
            PendingMessages.Clear();
        }

        /// <summary>
        /// Clear the pending message queue, discarding any pending messages.
        /// </summary>
		public static void ClearPendingMessages()
		{
			PendingMessages.Clear();
		}

    }
}
