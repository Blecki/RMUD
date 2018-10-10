﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SFS
{
    public class Container : MudObject
    {
        public Dictionary<RelativeLocations, List<MudObject>> Lists { get; set; }

        public RelativeLocations SupportedLocations = RelativeLocations.None;
        public RelativeLocations DefaultLocation = RelativeLocations.None;

        public Container(RelativeLocations Locations, RelativeLocations Default)
        {
            this.SupportedLocations = Locations;
            this.DefaultLocation = Default;
            this.Lists = new Dictionary<RelativeLocations, List<MudObject>>();
        }

        public void Remove(MudObject Object)
        {
            if (Lists != null) foreach (var list in Lists)
            {
                if (list.Value.Remove(Object))
                    Object.Location = null;
            }
        }

        public int RemoveAll(Predicate<MudObject> Func)
        {
            var r = 0;
            if (Lists != null) foreach (var list in Lists)
                r += list.Value.RemoveAll(Func);
            return r;
        }

        public void Add(MudObject Object, RelativeLocations Locations)
        {
            if (Lists == null) return;

            if (Locations == RelativeLocations.Default) Locations = DefaultLocation;

            if ((SupportedLocations & Locations) == Locations)
            {
                if (!Lists.ContainsKey(Locations)) Lists.Add(Locations, new List<MudObject>());
                Lists[Locations].Add(Object);
            }
        }

        public IEnumerable<MudObject> EnumerateObjects()
        {
            if (Lists != null) foreach (var list in Lists)
                foreach (var item in list.Value)
                    yield return item;
        }

        public IEnumerable<Tuple<MudObject, RelativeLocations>> EnumerateObjectsAndRelloc()
        {
            if (Lists != null) foreach (var list in Lists)
                foreach (var item in list.Value)
                    yield return Tuple.Create(item, list.Key);
        }

        public IEnumerable<T> EnumerateObjects<T>() where T : MudObject
        {
            if (Lists != null) foreach (var list in Lists)
                foreach (var item in list.Value)
                    if (item is T) yield return item as T;
        }

        public IEnumerable<MudObject> EnumerateObjects(RelativeLocations Locations)
        {
            if (Lists != null) foreach (var list in Lists)
                if ((list.Key & Locations) == list.Key)
                    foreach (var item in list.Value)
                        yield return item;
        }

        public IEnumerable<T> EnumerateObjects<T>(RelativeLocations Locations) where T : MudObject
        {
            if (Lists != null) foreach (var list in Lists)
                if ((list.Key & Locations) == list.Key)
                    foreach (var item in list.Value)
                        if (item is T) yield return item as T;
        }

        public List<MudObject> GetContents(RelativeLocations Locations)
        {
            return new List<MudObject>(EnumerateObjects(Locations));
        }

        public bool Contains(MudObject Object, RelativeLocations Locations)
        {
            if (Locations == RelativeLocations.Default) Locations = DefaultLocation;

            if (Lists != null)
                if (Lists.ContainsKey(Locations))
                    return Lists[Locations].Contains(Object);
            return false;
        }

        public RelativeLocations RelativeLocationOf(MudObject Object)
        {
            if (Lists != null)
                foreach (var list in Lists)
                    if (list.Value.Contains(Object))
                        return list.Key;
            return RelativeLocations.None;
        }
    }
}
