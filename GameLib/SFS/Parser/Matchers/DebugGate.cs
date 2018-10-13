using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SFS
{
    public partial class CommandFactory
    {
        public static CommandTokenMatcher DebugOnly()
        {
            return new DebugGate();
        }
    }

    internal class DebugGate : CommandTokenMatcher
    {
		internal DebugGate()
		{

        }

        public List<PossibleMatch> Match(PossibleMatch State, MatchContext Context)
        {
            var R = new List<PossibleMatch>();

#if DEBUG
            R.Add(State);
#endif

            return R;
        }

        public String FindFirstKeyWord() { return null; }
		public String Emit() { return "<Only available in debug builds>"; }
    }
}
