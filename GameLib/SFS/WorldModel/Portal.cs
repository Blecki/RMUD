using System;
using System.Linq;
using static SFS.Core;

namespace SFS
{
    /// <summary>
    /// A link between two rooms.
    /// </summary>
    public class Portal : MudObject
    {
        public bool Anonymous = false;
        public Direction Direction;
        public String Destination;

        /// <summary>
        /// Given a portal, find the opposite side. Portals are used to connect two rooms. The opposite side of 
        /// the portal is assumed to be the portal in the linked room that faces the opposite direction. For example,
        /// if portal A is in room 1, faces west, and is linked to room 2, the opposite side would be the portal
        /// in room 2 that faces east. It does not actually check to see if the opposite side it finds is linked
        /// to the correct room.
        /// </summary>
        /// <param name="Portal"></param>
        /// <returns></returns>
        public static MudObject FindOppositeSide(Portal Portal)
        {
            if (!(GetObject(Portal.Destination) is Room destination))
                return null; // Link is malformed in some way.

            var oppositeDirection = Link.Opposite(Portal.Direction);
            return destination.EnumerateObjects().FirstOrDefault(p =>  p is Portal && (p as Portal).Direction == oppositeDirection);
        }
    }
}
