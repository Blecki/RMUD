using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SFS
{    
    public class Actor : Container
    {
        public bool Preserve = false;
        public Gender Gender = Gender.Female;
        public ClientCommandHandler CommandHandler = null;
        public Action<String> Output = null;
        public bool Listens = false;

        public Actor() : base(RelativeLocations.Held | RelativeLocations.Worn, RelativeLocations.Held)
        {
            Noun("MAN").When(a => Gender == Gender.Male);
            Noun("WOMAN").When(a => Gender == Gender.Female);
        }
    }
}
