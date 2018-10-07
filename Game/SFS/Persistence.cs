using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace SFS
{
    public partial class MudObject
    {
        public static void PersistInstance(MudObject Object) { Core.Database.PersistInstance(Object); }
        public static void ForgetInstance(MudObject Object) { Core.Database.ForgetInstance(Object); }
    }
}