using System;

namespace GrandCentral
{
    namespace Operator
    {
        public interface IQuery
        {
            StateShard Context { get; }
            string Segment { get; }

            IQuery Where(string key, string val);
            IQuery Where(string key, int val);

            string Select();
        }
    }
}