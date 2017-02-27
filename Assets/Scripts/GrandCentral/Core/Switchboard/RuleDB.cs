using System.Collections.Generic;
using System.Linq;
using GrandCentral.FileCabinet;
using UnityEngine;

namespace GrandCentral.Switchboard
{
    public class RuleDB
    {
        internal List<IEntry> Entries;

        public RuleDB()
        {
            Entries = new List<IEntry>();
        }

        public IEntry QueryFor(string line, FactShard context)
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

                entry = entries.ElementAtOrDefault(Random.Range(0, Entries.Count()));
            }

            if (entry == null)
                return null;

            entry.StateMutations.ForEach(x => x.Mutate());

            return entry;
        }
    }
}