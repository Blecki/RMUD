using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SFS
{
    public class Room : Container
	{
        public RoomType RoomType = RoomType.Interior;
        public LightingLevel Light = LightingLevel.Dark;
        public LightingLevel AmbientLight = LightingLevel.Dark;

        public Room() : base(RelativeLocations.Contents, RelativeLocations.Contents)
        {
        }

        public void OpenLink(Direction Direction, String Destination, Portal Portal = null)
        {
            //if (RemoveAll(thing => thing.GetProperty<Direction>("link direction") == Direction && thing.GetProperty<bool>("portal?")) > 0)
            //    Core.LogWarning("Opened duplicate link in " + Path);

            if (Portal == null)
            {
                Portal = new Portal();
                Portal.Anonymous = true;
                Portal.Short = "link " + Direction + " to " + Destination;
            }

            Portal.Direction = Direction;
            Portal.Destination = Destination;
            Portal.Location = this;
            Add(Portal, RelativeLocations.Contents);
        }
    }
}
