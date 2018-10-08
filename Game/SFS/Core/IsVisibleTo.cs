using SFS.Rules;

namespace SFS
{
    public static partial class Core
    {
        /// <summary>
        /// Determine is an object is visible to another object. This is essentially a comparison of the
        ///  locale of the two objects. However, some special cases must be accounted for.
        ///  a) A closed container is visible to it's contents, despite having different locales, 
        ///     and the reverse.
        /// </summary>
        /// <param name="Actor">The reference point object</param>
        /// <param name="Object">The object to be tested</param>
        /// <returns>True if the reference point object can 'see' the tested object, false otherwise.</returns>
        public static bool IsVisibleTo(Actor Actor, MudObject Object)
        {
            var actorLocale = MudObject.FindLocale(Actor);
            if (actorLocale == null) return false;
            
            var objectLocale = MudObject.FindLocale(Object);
            return System.Object.ReferenceEquals(actorLocale, objectLocale)
                || System.Object.ReferenceEquals(actorLocale, Object)
                || System.Object.ReferenceEquals(objectLocale, Actor);
        }

        /// <summary>
        /// Encapsulates the VisibleTo relation for easy use by check rules.
        /// </summary>
        /// <param name="Actor">Reference point object</param>
        /// <param name="Item">Object to be tested</param>
        /// <returns></returns>
        public static CheckResult CheckIsVisibleTo(Actor Actor, MudObject Item)
        {
            if (!IsVisibleTo(Actor, Item))
            {
                SendMessage(Actor, "@gone");
                return CheckResult.Disallow;
            }
            return CheckResult.Continue;
        }

        /// <summary>
        /// Encapsulates the containment relation for easy use by check rules.
        /// Note that this relation is stricter than the VisibleTo relation.
        /// </summary>
        /// <param name="Actor">Reference point object</param>
        /// <param name="Item">Object to be tested</param>
        /// <returns></returns>
        public static CheckResult CheckIsHolding(Actor Actor, MudObject Item)
        {
            if (!ObjectContainsObject(Actor, Item))
            {
                SendMessage(Actor, "@dont have that");
                return CheckResult.Disallow;
            }
            return CheckResult.Continue;
        }
    }
}
