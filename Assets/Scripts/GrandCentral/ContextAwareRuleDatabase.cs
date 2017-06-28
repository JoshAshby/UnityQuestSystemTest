using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GrandCentral
{
    public class ContextAwareRuleDatabase
    {
        internal List<IEntry> Entries;

        public ContextAwareRuleDatabase()
        {
            Entries = new List<IEntry>();
        }

        public IEntry QueryFor(string line, FactDictionary context)
        {
            List<IEntry> entries = Entries.Where(ent =>
            {
                if (ent.Name != line)
                    return false;

                return ent.Check(context);
            }).ToList();

            IEntry entry;
            int length = entries.Count();

            if (length <= 1)
                entry = entries.FirstOrDefault();
            else
            {
                entries = entries.Where(ent => ent.Length == entries.First().Length).ToList();

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