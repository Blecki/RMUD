using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SFS
{
    public static partial class Core
    {
        private static Random _Random;
        public static Random Random
        {
            get
            {
                if (_Random == null) _Random = new Random();
                return _Random;
            }
        }
  
        public static T ChooseAtRandom<T>(List<T> From)
        {
            return From[Core.Random.Next(From.Count)];
        }

        public static T ChooseAtRandom<T>(params T[] From)
        {
            return From[Core.Random.Next(From.Length)];
        }
    }
}
