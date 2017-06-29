using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GrandCentral
{
    public class RuleDatabase
    {
        internal List<IEntry> Entries;

        public RuleDatabase()
        {
            // If speed becomes an issue, this could be broken up
            // into a dictionary of entry lists
            Entries = new List<IEntry>();
        }

        public IEntry QueryFor(string line, FactDictionary context, FactDatabase FactDatabase)
        {
            List<IEntry> entries = Entries.Where(ent =>
            {
                if (ent.Name != line)
                    return false;

                return ent.Check(context, FactDatabase);
            }).ToList();

            IEntry entry;
            int length = entries.Count();

            if (length <= 1)
                entry = entries.FirstOrDefault();
            else
            {
                entries = entries
                          .Where(ent => ent.Length == entries.First().Length)
                          .ToList();

                int count = entries.Count();
                int index = Random.Range(0, count);

                entry = entries.ElementAtOrDefault(index);
            }

            if (entry == null)
                return null;

            entry.StateMutations.ForEach(x => x.Mutate());

            return entry;
        }
    }
}