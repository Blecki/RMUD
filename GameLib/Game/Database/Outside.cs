using SFS;

namespace Game
{
    public class Outside : SFS.Room
    {
        public override void Initialize()
        {
            RoomType = RoomType.Exterior;
            Short = "Outside the Opera House";
        }
    }
}