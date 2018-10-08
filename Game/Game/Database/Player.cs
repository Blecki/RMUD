using SFS;
using static SFS.Core;

namespace Game
{
    public class Player : SFS.Actor
    {
        public override void Initialize()
        {
            Short = "you";
            MoveObject(GetObject("Game.Cloak"), this, RelativeLocations.Worn);
        }
    }
}
