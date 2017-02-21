using System;

namespace GrandCentral
{
    namespace Operator
    {
        public interface IQuery
        {
            StateShard Context { get; }

            IQuery Where(string key, string val);
            IQuery Where(string key, int val);

            string Select();
        }

        public class Query : IQuery
        {
            public StateShard Context { get; }

            protected RulesShard _databasePartition;
            protected Type _payloadType = typeof(object);

            public Query(RulesShard databasePartition)
            {
                Context = new StateShard();
                _databasePartition = databasePartition;
            }

            public static IQuery From(RulesShard databasePartition)
            {
                Query query = new Query(databasePartition);
                return query;
            }

            public IQuery Where(string key, string val)
            {
                Context.Add(key, new GenericValue<string>(val));
                return this;
            }

            public IQuery Where(string key, int val)
            {
                Context.Add(key, new GenericValue<int>(val));
                return this;
            }

            public string Select()
            {
                return _databasePartition.Query(this);
            }
        }
    }
}