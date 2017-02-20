namespace GrandCentral
{
    internal class MatchCriteron<T> : ICriteron
    {
        public string AccessKey { get; set; }

        private T _compareValue;

        public MatchCriteron(string key, T val)
        {
            AccessKey = key;

            _compareValue = val;
        }

        public bool Check(Query query)
        {
            if (!query.Context.ContainsKey(AccessKey))
                return false;

            IGenericData<T> val = query.Context[AccessKey].OfType<T>();

            if (val == null)
                return false;

            return val.Value.Equals(_compareValue);
        }
    }
}