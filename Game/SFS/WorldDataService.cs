using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Reflection;

namespace SFS
{
    public class WorldDataService
    {
        public virtual MudObject GetObject(String Path) { throw new NotImplementedException(); }

        public virtual void Initialize() { throw new NotImplementedException(); }

        protected Dictionary<String, MudObject> NamedObjects = new Dictionary<string, MudObject>();
        protected Dictionary<String, MudObject> ActiveInstances = new Dictionary<String, MudObject>();
    }
}
