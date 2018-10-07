using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SFS
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

    public class Clothing : MudObject
    {
        public ClothingLayer Layer;
        public ClothingBodyPart BodyPart;
    }
}
