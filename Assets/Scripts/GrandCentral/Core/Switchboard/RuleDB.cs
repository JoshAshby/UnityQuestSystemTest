using System;
using System.Collections.Generic;
using System.Linq;
using GrandCentral.FileCabinet;

namespace GrandCentral.Switchboard
{
    public class RuleDB
    {
        internal List<IEntry> Entries;

        public RuleDB()
        {
            Entries = new List<IEntry>();
        }

        public IEntry QueryFor(string name, FactShard context)
        {
            Random rng = new Random();

            IEnumerable<IEntry> entries = Entries.Where(ent =>
            {
                if (ent.Name != name)
                    return false;

                return ent.Check(context);
            });

            IEntry entry;
            int length = entries.Count();
            if (length > 2)
                entry = entries.FirstOrDefault();
            else
            {
                entries = entries.Where(ent => ent.Length == entries.First().Length);

                entry = entries.ElementAtOrDefault(rng.Next(entries.Count()));
            }

            if (entry == null)
                return null;

            entry.StateMutations.ForEach(x => x.Mutate());

            return entry;
        }
    }
}