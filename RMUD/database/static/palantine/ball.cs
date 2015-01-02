﻿public class ball : RMUD.MudObject, RMUD.DeclaresRules
{
    public ball() : base("ball", "This is a small dirty ball.") { }


    public void InitializeRules()
    {
        RMUD.Core.DefaultParser.AddCommand(RMUD.CommandFactory.KeyWord("BOUNCE"))
            .ProceduralRule((match, actor) =>
            {
                SendMessage(actor, "Database defined commands appear to work.");
                return RMUD.PerformResult.Continue;
            });
    }
}