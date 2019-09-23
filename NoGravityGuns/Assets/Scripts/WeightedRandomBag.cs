using System;
using System.Collections.Generic;
/*
class WeightedRandomBag<T>
{

    public struct Entry
    {
        public double accumulatedWeight;
        public T item;
    }

    public List<Entry> entries = new List<Entry>();
    private double accumulatedWeight;
    private Random rand = new Random();

    public void AddEntry(T item, double weight)
    {
        accumulatedWeight += weight;
        entries.Add(new Entry { item = item, accumulatedWeight = accumulatedWeight });
    }

    public void EditEntry(T item, double newWeight)
    {

        accumulatedWeight += newWeight;

        entries.Remove(item);

        entries.Add(new Entry { item = item, accumulatedWeight = accumulatedWeight });
    }

    public T GetRandom()
    {
        double r = rand.NextDouble() * accumulatedWeight;

        foreach (Entry entry in entries)
        {
            if (entry.accumulatedWeight >= r)
            {
                return entry.item;
            }
        }
        return default(T); //should only happen when there are no entries
    }
}
*/