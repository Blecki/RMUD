using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SFS
{
    public class Scenery : MudObject
	{
        public Scenery(String Description, params String[] Nouns)
		{
            Long = Description;
            this.Nouns = new NounList();
			foreach (var noun in Nouns)
				this.Nouns.Add(noun.ToUpper());
		}
    }
}
