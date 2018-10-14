using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SFS
{
    // Todo: Convert all nouns to all caps.

    public class Noun
    {
        public List<String> Values;
        public Func<MudObject, bool> WhenClause = null;

        public bool Match(String Word, MudObject Actor)
        {
            if (!Values.Contains(Word)) return false;
            if (WhenClause != null) return WhenClause(Actor);
            return true;
        }

        public Noun(IEnumerable<String> Values)
        {
            this.Values = new List<string>();
            this.Values.AddRange(Values);
        }

        public Noun(params String[] Values)
        {
            this.Values = new List<string>();
            this.Values.AddRange(Values);
        }

        public Noun(String Value)
        {
            this.Values = new List<string>();
            Values.Add(Value);
        }

        public Noun When(Func<MudObject, bool> WhenClause)
        {
            if (this.WhenClause != null)
            {
                var oldClause = this.WhenClause;
                this.WhenClause = (obj) => oldClause.Invoke(obj) && WhenClause(obj);
            }
            else
                this.WhenClause = WhenClause;
            return this;
        }
    }
}
