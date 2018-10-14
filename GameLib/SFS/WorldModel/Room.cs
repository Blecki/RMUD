using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static SFS.Core;

namespace SFS
{
    public class Room : Container
	{
        [Initialize]
        public static void __room()
        {
            GlobalRules.Value<Actor, Room, String, String>("printed name")
               .First
               .Do((viewer, room, article) => room.Short)
               .Name("Rooms are proper-named rule.");
        }

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
