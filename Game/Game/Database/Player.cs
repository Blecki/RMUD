using SFS;

namespace Game
{
    public class Player : SFS.MudObject
    {
        public override void Initialize()
        {
            Actor();

            SetProperty("short", "you");
            Move(GetObject("Game.Cloak"), this, RelativeLocations.Worn);
        }
    }
}
