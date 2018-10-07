using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFS.Rules;

namespace SFS
{
    public static class RoomLightingRules 
    {
        public static LightingLevel AmbientExteriorLightingLevel = LightingLevel.Bright;

        public static void AtStartup(SFSRuleEngine GlobalRules)
        {
            GlobalRules.DeclareValueRuleBook<MudObject, LightingLevel>("light level", "[item] -> LightingLevel, How much light does the item emit?", "item");

            GlobalRules.Value<MudObject, LightingLevel>("light level")
                .Do(item => LightingLevel.Dark)
                .Name("Items emit no light by default rule.");

            GlobalRules.Perform<MudObject>("update")
                .When(room => room.GetProperty<RoomType>("room type") != RoomType.NotARoom)
                .Do(room =>
                {
                    var light = LightingLevel.Dark;
                    var roomType = room.GetProperty<RoomType>("room type");

                    if (roomType == SFS.RoomType.Exterior)
                        light = AmbientExteriorLightingLevel;

                    foreach (var item in MudObject.EnumerateVisibleTree(room))
                    {
                        var lightingLevel = GlobalRules.ConsiderValueRule<LightingLevel>("light level", item);
                        if (lightingLevel > light) light = lightingLevel;
                    }

                    var ambient = room.GetProperty<LightingLevel>("ambient light");
                    if (ambient > light) light = ambient;

                    room.SetProperty("light", light);

                    return PerformResult.Continue;
                })
                .Name("Update room lighting rule.");
        }

        public static RuleBuilder<MudObject, LightingLevel> ValueLightingLevel(this MudObject Object)
        {
            return Object.Value<MudObject, LightingLevel>("light level").ThisOnly();
        }

    }
}
