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

        [AtStartup]
        public static void __()
        {
            Core.GlobalRules.DeclareValueRuleBook<MudObject, LightingLevel>("light level", "[item] -> LightingLevel, How much light does the item emit?", "item");

            Core.GlobalRules.Value<MudObject, LightingLevel>("light level")
                .Do(item => LightingLevel.Dark)
                .Name("Items emit no light by default rule.");

            Core.GlobalRules.Perform<Room>("update")
                .Do(room =>
                {
                    var light = LightingLevel.Dark;
                    var roomType = room.RoomType;

                    if (roomType == SFS.RoomType.Exterior)
                        light = AmbientExteriorLightingLevel;

                    foreach (var item in MudObject.EnumerateVisibleTree(room))
                    {
                        var lightingLevel = Core.GlobalRules.ConsiderValueRule<LightingLevel>("light level", item);
                        if (lightingLevel > light) light = lightingLevel;
                    }

                    var ambient = room.AmbientLight;
                    if (ambient > light) light = ambient;

                    room.Light = light;

                    return PerformResult.Continue;
                })
                .Name("Update room lighting rule.");
        }
    }
}
