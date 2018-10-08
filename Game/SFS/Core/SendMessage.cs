using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace SFS
{
    public static partial class Core
    {
        internal static bool SilentFlag = false;
        internal static bool OutputQueryTriggered = false;

        /// <summary>
        /// Begin watching for output.
        /// </summary>
        public static void BeginOutputQuery()
        {
            OutputQueryTriggered = false;
        }

        /// <summary>
        /// Has there been any output since the last time BeginOutputQuery was called?
        /// </summary>
        /// <returns></returns>
        public static bool CheckOutputQuery()
        {
            return OutputQueryTriggered;
        }
    
        public static void SendMessage(Actor Actor, String Message, params Object[] MentionedObjects)
        {
            if (String.IsNullOrEmpty(Message)) return;
            if (Core.SilentFlag) return;
            Core.OutputQueryTriggered = true;

            if (Actor != null && Actor.Listens)
                Core.PendingMessages.Add(new PendingMessage(Actor, Core.FormatMessage(Actor, Message, MentionedObjects)));
        }

        public static void SendLocaleMessage(MudObject Object, String Message, params Object[] MentionedObjects)
        {
            if (String.IsNullOrEmpty(Message)) return;
            if (Core.SilentFlag) return;
            Core.OutputQueryTriggered = true;

            var locale = FindLocale(Object) as Container;
            if (locale != null)
                foreach (var actor in locale.EnumerateObjects().OfType<Actor>())
                    if (actor.Listens)
                        Core.PendingMessages.Add(new PendingMessage(actor, Core.FormatMessage(actor, Message, MentionedObjects)));
        }

        public static void SendExternalMessage(Actor Actor, String Message, params Object[] MentionedObjects)
        {
            if (String.IsNullOrEmpty(Message)) return;
            if (Core.SilentFlag) return;
            Core.OutputQueryTriggered = true;

            if (Actor == null) return;
            if (Actor.Location == null) return;

            foreach (var other in Actor.Location.EnumerateObjects().OfType<Actor>().Where(a => !Object.ReferenceEquals(a, Actor)))
                if (other.Listens)
                    Core.PendingMessages.Add(new PendingMessage(other, Core.FormatMessage(other, Message, MentionedObjects)));
        }
    }
}
