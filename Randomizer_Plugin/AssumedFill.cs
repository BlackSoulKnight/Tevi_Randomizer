using System;
using System.Collections.Generic;


namespace TeviRandomizer
{
    class AssumedFill
    {
        public static Dictionary<string, int> debugItems;
        public static HashSet<Randomizer.Location> debugLocation;
        public static List<Randomizer.Location> Search(Randomizer.Area start, Dictionary<string, int> items)
        {
            List<Randomizer.Location> R = new();
            Queue<Randomizer.Area> Queue = new();
            Queue.Enqueue(start);
            HashSet<Randomizer.Area> visited = new();
            visited.Add(start);
            while (Queue.Count > 0)
            {
                var r = Queue.Dequeue();
                foreach (var edge in r.Connections)
                {
                    if (!visited.Contains(edge.to) && edge.checkEntrance(items))
                    {
                        Queue.Enqueue(edge.to);
                        visited.Add(edge.to);
                    }
                }
                foreach (var loc in r.Locations)
                {
                    if (loc.isReachAble(items))
                        R.Add(loc);
                }
            }
            return R;
        }
        public static List<Randomizer.Location> AssumedSearch(Randomizer.Area start, Dictionary<string, int> items = null)
        {
            if (items is null)
                items = new Dictionary<string, int>();
            else
                items = new(items);
            HashSet<Randomizer.Location> visited = new();
            List<Randomizer.Location> R = new();
            Queue<Randomizer.Location> newLocations = new();
            int prev = -1;
            do
            {
                prev = R.Count;
                newLocations = new(Search(start, items));
                while (newLocations.Count > 0)
                {
                    var loc = newLocations.Dequeue();
                    if (visited.Contains(loc))
                        continue;
                    if (loc.isReachAble(items))
                    {
                        if (String.IsNullOrEmpty(loc.newItem))
                            R.Add(loc);
                        else if (items.ContainsKey(loc.newItem))
                            items[loc.newItem] += 1;
                        else 
                            items[loc.newItem] = 1;
                        prev = -1;
                        visited.Add(loc);
                    }
                }
            } while (R.Count != prev);
            debugItems = items;
            debugLocation = visited;
            return R;
        }
        public static List<List<Randomizer.Location>> SphereSearch(Randomizer.Area start)
        {
            List<List<Randomizer.Location>> returnVal = new();
            HashSet<Randomizer.Location> visited = new();
            Dictionary<string, int> items = new();
            Queue<Randomizer.Location> newLocations = new();

            int prev = -1;
            do
            {
                prev = visited.Count;
                newLocations = new(Search(start, items));
                List<Randomizer.Location> sphereItems = new();
                while (newLocations.Count > 0)
                {
                    var loc = newLocations.Dequeue();
                    if (visited.Contains(loc))
                        continue;
                    if (loc.isReachAble(items))
                    {
                        if(items.ContainsKey(loc.newItem))
                            items[loc.newItem] += 1;
                        else
                            items[loc.newItem] = 1;
                        sphereItems.Add(loc);
                        prev = -1;
                        visited.Add(loc);
                    }
                }
                returnVal.Add(sphereItems);
            } while (visited.Count != prev);
            return returnVal;
        }
    }
}
