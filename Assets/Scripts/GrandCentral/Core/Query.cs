using System;

namespace GrandCentral
{
    public class Query
    {
        public StateShard Context;

        protected RulesShard _databasePartition;
        protected Type _payloadType = typeof(object);

        public Query(RulesShard databasePartition)
        {
            Context = new StateShard();
            _databasePartition = databasePartition;
        }

        public static Query From(RulesShard databasePartition)
        {
            Query query = new Query(databasePartition);
            return query;
        }

        public Query Where(string key, string val)
        {
            Context.Add(key, new GenericData<string>(val));
            return this;
        }

        public Query Where(string key, int val)
        {
            Context.Add(key, new GenericData<int>(val));
            return this;
        }

        public object Select()
        {
            return _databasePartition.Query(this);
        }

        public TResult SelectAs<TResult>()
        {
            return (TResult)Select();
        }
    }
}