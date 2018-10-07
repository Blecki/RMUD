using SFS;

namespace Game
{
    public class Player : SFS.Actor
    {
        public override void Initialize()
        {
            Short = "you";
            Move(GetObject("Game.Cloak"), this, RelativeLocations.Worn);
        }
    }
}
