using SFS;

namespace Game
{
    public class Outside : SFS.MudObject
    {
        public override void Initialize()
        {
            Room(RoomType.Exterior);

            SetProperty("short", "Outside the Opera House");
        }
    }
}