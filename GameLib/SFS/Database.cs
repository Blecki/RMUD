using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFS
{
    public class Database
    {
        private Dictionary<String, MudObject> NamedObjects = new Dictionary<string, MudObject>();

        public SFS.MudObject GetObject(string Path)
        {
            MudObject r = null;

            if (NamedObjects.ContainsKey(Path))
                r = NamedObjects[Path];
            else
            {
                var type = System.Reflection.Assembly.GetExecutingAssembly().GetType(Path);
                if (type == null) return null;
                r = Activator.CreateInstance(type) as MudObject;
                if (r != null)
                {
                    r.Path = Path;
                    r.State = ObjectState.Unitialized;
                    NamedObjects.Upsert(Path, r);
                }
            }

            if (r != null && r.State == ObjectState.Unitialized)
                MudObject.InitializeObject(r);

            return r;
        }
    }
}
