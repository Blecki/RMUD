﻿using SFS;
using static SFS.Core;

namespace Game
{
    public class Cloak : SFS.Clothing
    {
        public override void Initialize()
        {
            /*
 The player wears a velvet cloak. The cloak can be hung or unhung.
Understand "dark" or "black" or "satin" as the cloak.
The description of the cloak is "A handsome cloak,
of velvet trimmed with satin, and slightly splattered with raindrops.
Its blackness is so deep that it almost seems to suck light from the room."
*/
            Layer = ClothingLayer.Outer;
            BodyPart = ClothingBodyPart.Cloak;
            Short = "velvet cloak";
            Nouns = new NounList();
            Nouns.Add("dark", "black", "satin", "velvet", "cloak");
            Long = "A handsome cloak, of velvet trimmed with satin, and slightly spattered with raindrops. Its blackness is so deep that it almost seems to suck light from the room.";

            //Carry out taking the cloak:
            //    now the bar is dark.
            Perform<MudObject, MudObject>("take")
                .Do((actor, thing) =>
                {
                    (GetObject("Game.Bar") as Room).AmbientLight = LightingLevel.Dark;
                    return SFS.Rules.PerformResult.Continue;
                });

            //Carry out putting the unhung cloak on something in the cloakroom:
            //    now the cloak is hung;
            //    increase score by 1.
            //Perform<MudObject, MudObject, MudObject, RelativeLocations>("put")
            //    .When((actor, item, container, relloc) => container is Hook)
            //    .Do((actor, item, container, relloc) => { isHung = true; return PerformResult.Continue; });

            //Carry out putting the cloak on something in the cloakroom:
            //    now the bar is lit.
            Perform<Actor, Cloak, MudObject, RelativeLocations>("put")
                .When((actor, item, container, relloc) => actor.Location is Cloakroom)
                .Do((actor, item, container, relloc) =>
                {
                    (GetObject("Game.Bar") as Room).AmbientLight = LightingLevel.Bright;
                    return SFS.Rules.PerformResult.Continue;
                });

            //Carry out dropping the cloak in the cloakroom:
            //    now the bar is lit.
            Perform<Actor, MudObject>("drop")
                .When((actor, item) => actor.Location is Cloakroom)
                .Do((actor, item) => 
                {
                    (GetObject("Game.Bar") as Room).AmbientLight = LightingLevel.Bright;
                    return SFS.Rules.PerformResult.Continue;
                });

            //Instead of dropping or putting the cloak on when the player is not in the cloakroom:
            //  say "This isn't the best place to leave a smart cloak lying around."
            // * Since putting checks dropping, we only need one rule.
            Check<Actor, MudObject>("can drop?")
                .When((actor, item) => !(actor.Location is Cloakroom))
                .Do((actor, item) =>
                {
                    SendMessage(actor, "This isn't the best place to leave a smart cloak lying around.");
                    return SFS.Rules.CheckResult.Disallow;
                });
        }
    }
}
