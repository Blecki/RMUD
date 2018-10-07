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
            String BasePath, InstanceName;
            SplitObjectName(Path, out BasePath, out InstanceName);

            if (!String.IsNullOrEmpty(InstanceName))
            {
                MudObject activeInstance = null;
                if (ActiveInstances.TryGetValue(Path, out activeInstance))
                    return activeInstance;
                else
                    return CreateInstance(Path);
            }
            else
            {
                MudObject r = null;

                if (NamedObjects.ContainsKey(BasePath))
                    r = NamedObjects[BasePath];
                else
                {
                    var type = SourceAssembly.GetType(Path);
                    if (type == null) return null;
                    r = Activator.CreateInstance(type) as MudObject;
                    if (r != null)
                    {
                        r.Path = Path;
                        r.State = ObjectState.Unitialized;
                        NamedObjects.Upsert(BasePath, r);
                    }
                }

                if (r != null && r.State == ObjectState.Unitialized)
                    MudObject.InitializeObject(r);

                return r;
            }
        }

        override public SFS.MudObject ReloadObject(string Path)
        {
            return GetObject(Path);
        }
        
        override public void PersistInstance(SFS.MudObject Object)
        {
            if (Object.IsPersistent) return; //The object is already persistent.
            if (ActiveInstances.ContainsKey(Object.GetFullName()))
                throw new InvalidOperationException("An instance with this name is already persisted.");
            if (Object.IsNamedObject)
            {
                Object.IsPersistent = true;
                ActiveInstances.Upsert(Object.GetFullName(), Object);
                ReadPersistentObject(Object);
            }
            else
                throw new InvalidOperationException("Anonymous objects cannot be persisted.");

        }

        override public void ForgetInstance(SFS.MudObject Object)
        {
            var instanceName = Object.Path + "@" + Object.Instance;
            if (ActiveInstances.ContainsKey(instanceName))
                ActiveInstances.Remove(instanceName);
            Object.IsPersistent = false;
        }

        private void SavePersistentObject(MudObject Object)
        {
            //var filename = DynamicPath + Object.GetFullName() + ".txt";
            //Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filename));
            //var data = Core.SerializeObject(Object);
            //System.IO.File.WriteAllText(filename, data);
        }

        private void ReadPersistentObject(MudObject Object)
        {
            //var filename = DynamicPath + Object.GetFullName() + ".txt";
            //if (!System.IO.File.Exists(filename)) return;
            //var data = System.IO.File.ReadAllText(filename);
            //Core.DeserializeObject(Object, data);
        }

        override public Tuple<bool, string> LoadSourceFile(string Path)
        {
            return Tuple.Create(false, "The compiled database does not support this operation.");
        }

        override public int Save()
        {
            var counter = 0;
            foreach (var instance in ActiveInstances)
            {
                ++counter;
                SavePersistentObject(instance.Value);
            }
            return counter;
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
