using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static SFS.Core;

namespace SFS
{
   	public partial class MudObject
    {
        public ObjectState State = ObjectState.Unitialized;
        public String Path;
        public Container Location;

        public String Short = "object";
        public String Long = "";
        public String Article = "a";
        public List<Noun> Nouns = new List<Noun>();

        public virtual void Initialize() { }

		public MudObject()
		{
		}

        public MudObject(String Short, String Long)
        {
            this.Short = Short;
            this.Long = Long;
            Noun(Short.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            var firstChar = Short.ToLower()[0];
            if (firstChar == 'a' || firstChar == 'e' || firstChar == 'i' || firstChar == 'o' || firstChar == 'u')
                Article = "an";
        }

        #region Nouns
        public Noun Noun(String N)
        {
            var r = new Noun(N);
            Nouns.Add(r);
            return r;
        }

        public Noun Noun(params String[] Ns)
        {
            var r = new Noun(Ns);
            Nouns.Add(r);
            return r;
        }

        public Noun Noun(IEnumerable<String> Ns)
        {
            var r = new Noun(Ns);
            Nouns.Add(r);
            return r;
        }
        #endregion
    }
}
