namespace GrandCentral
{
    internal class IntRangeCriteron : ICriteron
    {
        public string AccessKey { get; set; }

        private int _lowCompareValue;
        private int _highCompareValue;

        public IntRangeCriteron(string key, int low, int high)
        {
            AccessKey = key;

            _lowCompareValue = low;
            _highCompareValue = high;
        }

        public bool Check(Query query)
        {
            if (!query.Context.ContainsKey(AccessKey))
                return false;

            IGenericData<int> val = query.Context[AccessKey].OfType<int>();

            if (val == null)
                return false;

            return _lowCompareValue <= val.Value && val.Value <= _highCompareValue;
        }
    }
}