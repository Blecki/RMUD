﻿using SFS;

namespace Game
{
    public class Cloakroom : SFS.Room
    {
        public override void Initialize()
        {
            /*
             The Cloakroom is west of the Foyer.
"The walls of this small room were clearly once lined with hooks, though now only one remains.
The exit is a door to the east."

In the Cloakroom is a supporter called the small brass hook.
The hook is scenery. Understand "peg" as the hook.

[Inform will automatically understand any words in the object definition
("small", "brass", and "hook", in this case), but we can add extra synonyms
with this sort of Understand command.]

The description of the hook is "It's just a small brass hook,
[if something is on the hook]with [a list of things on the hook]
hanging on it[otherwise]screwed to the wall[end if]."

[This description is general enough that, if we were to add other hangable items
to the game, they would automatically be described correctly as well.]
             */

            Short = "Cloakroom";
            Long = "The walls of this small room were clearly once lined with hooks, though now only one remains.";

            OpenLink(Direction.EAST, "Game.Foyer");

            Move(MudObject.GetObject("Game.Hook"), this);

            AmbientLight = LightingLevel.Bright;
        }
    }

    public class Hook : Container
    {
        public Hook() : base(RelativeLocations.On, RelativeLocations.On) { }

        public override void Initialize()
        {
            Short = "small brass hook";
            Nouns = new NounList();
            Nouns.Add("small", "brass", "hook", "peg");
            Long = "It's just a small brass hook.";

            Check<Actor, MudObject>("can take?")
                .Do((actor, item) =>
                {
                    SendMessage(actor, "It's screwed into the wall.");
                    return SFS.Rules.CheckResult.Disallow;
                });
        }
    }   
}