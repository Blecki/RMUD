using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFS.SinglePlayer
{
    public class CompiledDatabase : SFS.WorldDataService
    {
        System.Reflection.Assembly SourceAssembly;

        public CompiledDatabase(System.Reflection.Assembly SourceAssembly)
        {
            this.SourceAssembly = SourceAssembly;
        }

        override public SFS.MudObject GetObject(string Path)
        {
            MudObject r = null;

            if (NamedObjects.ContainsKey(Path))
                r = NamedObjects[Path];
            else
            {
                var type = SourceAssembly.GetType(Path);
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

        override public void Initialize()
        {
            Core.SettingsObject = new Settings();
            var settings = GetObject("settings") as Settings;
            if (settings == null) { }// Core.LogError("No settings object found in database. Using default settings.");
            else Core.SettingsObject = settings;
            NamedObjects.Clear();           
        }
    }
}
