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

        public class Query : IQuery
        {
            public StateShard Context { get; }
            public string Segment { get; }

            protected RulesShard _databasePartition;
            protected Type _payloadType = typeof(object);

            public Query(RulesShard databasePartition, string name)
            {
                Context = new StateShard();
                _databasePartition = databasePartition;
                Segment = name;
            }

            public static IQuery From(RulesShard databasePartition, string name)
            {
                Query query = new Query(databasePartition, name);
                return query;
            }

            public IQuery Where(string key, string val)
            {
                Context.Add(key, val);
                return this;
            }

            public IQuery Where(string key, int val)
            {
                Context.Add(key, val);
                return this;
            }

            public string Select()
            {
                return _databasePartition.Query(this);
            }
        }
    }
}