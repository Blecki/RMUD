using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SFS
{
   	public partial class MudObject : SFS.Rules.RuleObject
    {
        public override SFS.Rules.RuleEngine GlobalRules { get { return Core.GlobalRules; } }

        public ObjectState State = ObjectState.Unitialized; 
		public String Path { get; set; }
		public String Instance { get; set; }
        public bool IsNamedObject { get { return Path != null; } }
        public bool IsAnonymousObject { get { return Path == null; } }
        public bool IsInstance { get { return IsNamedObject && Instance != null; } }
        public String GetFullName() { return Path + "@" + Instance; }
        public bool IsPersistent { get; set; }
        public Container Location { get; set; }
        public override SFS.Rules.RuleObject LinkedRuleSource { get { return Location; } }

        public String Short = "object";
        public String Long = "";
        public String Article = "a";
        public NounList Nouns = null;
        public bool Preserve = false;

        public virtual void Initialize() { }

        public override string ToString()
        {
            if (String.IsNullOrEmpty(Path)) return this.GetType().Name;
            else return Path;
        }

		public MudObject()
		{
		    State = ObjectState.Alive;
            Nouns = new NounList();
            IsPersistent = false;
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
            IsPersistent = false;

        }

        public static MudObject GetObject(String Path)
        {
            return Core.Database.GetObject(Path);
        }

        public static MudObject InitializeObject(MudObject Object)
        {
            Object.Initialize();
            Object.State = ObjectState.Alive;
            Core.GlobalRules.ConsiderPerformRule("update", Object);
            return Object;
        }

        public static T GetObject<T>(String Path) where T: MudObject
        {
            return GetObject(Path) as T;
        }
    }
}
