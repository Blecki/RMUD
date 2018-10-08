using SFS;
using static SFS.Core;

namespace Game
{

    public class Bar : SFS.Room
    {
        public override void Initialize()
        {
            /*
             The Bar is south of the Foyer. The printed name of the bar is "Foyer Bar".
The Bar is dark.  "The bar, much rougher than you'd have guessed
after the opulence of the foyer to the north, is completely empty.
There seems to be some sort of message scrawled in the sawdust on the floor."
             */

            Short = "Foyer Bar";
            Long = "The bar, much rougher than you'd have guessed after the opulence of the foyer to the north, is completely empty. There seems to be some sort of message scrawled in the sawdust on the floor.";
            
            OpenLink(Direction.NORTH, "Game.Foyer");

            // The scrawled message is scenery in the Bar. Understand "floor" or "sawdust" as the message.

            var message = new Scenery("", "message", "floor", "sawdust", "scrawled");
            Add(message, RelativeLocations.Contents);

            //Neatness is a kind of value. The neatnesses are neat, scuffed, and trampled. The message has a neatness. The message is neat.

            bool messageScuffed = false;

            /*
Instead of examining the message:
    increase score by 1;
    say "The message, neatly marked in the sawdust, reads...";
    end the game in victory.

Instead of examining the trampled message:
    say "The message has been carelessly trampled, making it difficult to read.
    You can just distinguish the words...";
    end the game saying "You have lost".
             */
            message.Perform<Actor, MudObject>("describe")
                .Do((actor, item) =>
                {
                    if (messageScuffed)
                    {
                        SendMessage(actor, "The message has been carelessly trampled, making it difficult to read. You can just distinguish the words...");
                        SendMessage(actor, "YOU HAVE LOST.");
                    }
                    else
                    {
                        SendMessage(actor, "The message, neatly marked in the sawdust, reads...");
                        SendMessage(actor, "YOU HAVE WON!");
                    }
                    return SFS.Rules.PerformResult.Stop;
                });

            /*
             Instead of doing something other than going in the bar when in darkness:
    if the message is not trampled, change the neatness of the message
    to the neatness after the neatness of the message;
    say "In the dark? You could easily disturb something."
             */
            Perform<PossibleMatch, Actor>("before acting")
                .When((match, actor) => Light == LightingLevel.Dark)
                .Do((match, actor) =>
                {
                    if (match.TypedValue<CommandEntry>("COMMAND").IsNamed("GO"))
                        return SFS.Rules.PerformResult.Continue;
                    messageScuffed = true;
                    SendMessage(actor, "In the dark? You could easily disturb something.");
                    return SFS.Rules.PerformResult.Stop;
                });

            /*
             Instead of going nowhere from the bar when in darkness:
    now the message is trampled;
    say "Blundering around in the dark isn't a good idea!"
             */
            Perform<PossibleMatch, Actor>("before command")
                .When((match, actor) => Light == LightingLevel.Dark
                    && match.TypedValue<CommandEntry>("COMMAND").IsNamed("GO")
                    && (match.ValueOrDefault("DIRECTION") as Direction?).Value != Direction.NORTH)
                .Do((match, actor) =>
                {
                    messageScuffed = true;
                    SendMessage(actor, "Blundering around in the dark isn't a good idea!");
                    return SFS.Rules.PerformResult.Stop;
                });
        }
    }

   
}