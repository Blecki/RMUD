using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SFS.Commands.StandardActions.Clothing
{
    public enum ClothingLayer
    {
        Under = 0,
        Outer = 1,
        Assecories = 2,
        Over = 3
    }

    public enum ClothingBodyPart
    {
        Feet,
        Legs,
        Torso,
        Hands,
        Neck,
        Head,
        Wrist,
        Fingers,
        Ears,
        Face,
        Cloak,
    }

    public static class Factory// : SFS.MudObject
    {
        public static SFS.MudObject Create(String Short, ClothingLayer Layer, ClothingBodyPart BodyPart)
        {
            var r = new SFS.MudObject(Short, "This is a generic " + Short + ". Layer: " + Layer + " BodyPart: " + BodyPart);
            r.SetProperty("clothing layer", Layer);
            r.SetProperty("clothing part", BodyPart);
            r.SetProperty("wearable?", true);
            return r;
        }

        public static void AtStartup(SFS.SFSRuleEngine GlobalRules)
        {
            SFS.PropertyManifest.RegisterProperty("clothing layer", typeof(ClothingLayer), ClothingLayer.Outer, new SFS.EnumSerializer<ClothingLayer>());
            SFS.PropertyManifest.RegisterProperty("clothing part", typeof(ClothingBodyPart), ClothingBodyPart.Cloak, new SFS.EnumSerializer<ClothingBodyPart>());
            SFS.PropertyManifest.RegisterProperty("wearable?", typeof(bool), false, new SFS.BoolSerializer());
        }

        public static void Clothing(this SFS.MudObject MudObject, ClothingLayer Layer, ClothingBodyPart BodyPart)
        {
            MudObject.SetProperty("clothing layer", Layer);
            MudObject.SetProperty("clothing part", BodyPart);
            MudObject.SetProperty("wearable?", true);
        }
    }
}
