using RMUD;

namespace CloakOfDarkness
{
    public class Player : RMUD.MudObject
    {
        public override void Initialize()
        {
            Actor();

            SetProperty("short", "you");
            Move(GetObject("CloakOfDarkness.Cloak"), this, RelativeLocations.Worn);
        }
    }
}
