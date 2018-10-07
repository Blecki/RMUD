using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;

namespace SFS
{
    /// <summary>
    /// Represents an assembly that should be integrated at startup. Core.Start will search an assembly
    /// for every type that implements 'static void OnStartup(RuleEngine)' and try to call it.
    /// A module can use this behavior to implement registration of it's custom rules.
    /// </summary>
    public class ModuleAssembly
    {
        public Assembly Assembly;
        public String FileName;

        /// <summary>
        /// Construct a module assembly from an assembly. Automatically find and load the ModuleInfo type
        /// for the module assembly.
        /// </summary>
        /// <param name="Assembly"></param>
        /// <param name="FileName"></param>
        public ModuleAssembly(Assembly Assembly, String FileName)
        {
            this.Assembly = Assembly;
            this.FileName = FileName;
        }

        /// <summary>
        /// Construct a module assembly from an assembly on disc. Automatically find and load the ModuleInfo
        /// type.
        /// </summary>
        /// <param name="FileName">The assembly file to load</param>
        public ModuleAssembly(String FileName)
        {
            FileName = System.IO.Path.GetFullPath(FileName);
            this.FileName = FileName;

            Assembly = System.Reflection.Assembly.LoadFrom(FileName);
            if (Assembly == null) throw new InvalidOperationException("Could not load assembly " + FileName);
        }
    }
}
