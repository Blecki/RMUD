using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SFS
{
    public class OpenableContainer : Container
    {
        public bool Open = true;

        public OpenableContainer(RelativeLocations Locations, RelativeLocations Default) : base(Locations, Default)
        {
        }
    }
}
