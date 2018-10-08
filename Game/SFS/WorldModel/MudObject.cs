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
        public NounList Nouns = null;

        public virtual void Initialize() { }

		public MudObject()
		{
		    State = ObjectState.Alive;
            Nouns = new NounList();
		}

        public MudObject(String Short, String Long)
        {
            this.Short = Short;
            this.Long = Long;
            this.Nouns = new NounList(Short.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));

            var firstChar = Short.ToLower()[0];
            if (firstChar == 'a' || firstChar == 'e' || firstChar == 'i' || firstChar == 'o' || firstChar == 'u')
                Article = "an";

            State = ObjectState.Alive;
        }

        public static MudObject GetObject(String Path)
        {
            return Core.Database.GetObject(Path);
        }

        public static MudObject InitializeObject(MudObject Object)
        {
            Object.Initialize();
            Object.State = ObjectState.Alive;
            GlobalRules.ConsiderPerformRule("update", Object);
            return Object;
        }

        public static T GetObject<T>(String Path) where T: MudObject
        {
            return GetObject(Path) as T;
        }
    }
}
