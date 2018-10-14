using SFS;
using static SFS.Core;

namespace Game
{

    public class Foyer : SFS.Room
    {

        public override void Initialize()
        {
            /*
             * Foyer of the Opera House is a room.  "You are standing in a spacious hall,
splendidly decorated in red and gold, with glittering chandeliers overhead.
The entrance from the street is to the north, and there are doorways south and west."

Instead of going north in the Foyer, say "You've only just arrived, and besides,
the weather outside seems to be getting worse."

             */
            Short = "Foyer of the Opera House";
            Long = "You are standing in a spacious hall, splendidly decorated in red and gold, with glittering chandeliers overhead.";
            AmbientLight = LightingLevel.Bright;

            OpenLink(Direction.NORTH, "Game.Outside");
            OpenLink(Direction.SOUTH, "Game.Bar");
            OpenLink(Direction.WEST, "Game.Cloakroom", new Door());

            Check<Actor, Portal>("can go?")
               .First
               .When((actor, link) => link != null && link.Location is Foyer && link.Direction == Direction.NORTH)
               .Do((actor, link) =>
               {
                   SendMessage(actor, "You've only just arrived, and besides, the weather outside seems to be getting worse.");
                   return SFS.Rules.CheckResult.Disallow;
               });
        }
    }

   
}