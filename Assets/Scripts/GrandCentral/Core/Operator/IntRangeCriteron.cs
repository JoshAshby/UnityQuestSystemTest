namespace GrandCentral
{
    namespace Operator
    {
        internal class IntRangeCriteron : ICriteron
        {
            public string FactKey { get; set; }
            public string AccessKey { get; set; }

            private int _lowCompareValue;
            private int _highCompareValue;

            public IntRangeCriteron(string fact, string key, int low, int high)
            {
                FactKey = fact;
                AccessKey = key;

                _lowCompareValue = low;
                _highCompareValue = high;
            }

            public bool Check(IGenericValue rawValue)
            {
                IGenericValue<int> val = rawValue.OfType<int>();

                if (val == null)
                    return false;

                return _lowCompareValue <= val.Value && val.Value <= _highCompareValue;
            }
        }

        internal class IntGteCriteron : ICriteron
        {
            public string FactKey { get; set; }
            public string AccessKey { get; set; }

            private int _compareValue;

            public IntGteCriteron(string fact, string key, int val)
            {
                FactKey = fact;
                AccessKey = key;

                _compareValue = val;
            }

            public bool Check(IGenericValue rawValue)
            {
                IGenericValue<int> val = rawValue.OfType<int>();

                if (val == null)
                    return false;

                return val.Value <= _compareValue;
            }
        }

        internal class IntGtCriteron : ICriteron
        {
            public string FactKey { get; set; }
            public string AccessKey { get; set; }

            private int _compareValue;

            public IntGtCriteron(string fact, string key, int val)
            {
                FactKey = fact;
                AccessKey = key;

                _compareValue = val;
            }

            public bool Check(IGenericValue rawValue)
            {
                IGenericValue<int> val = rawValue.OfType<int>();

                if (val == null)
                    return false;

                return val.Value < _compareValue;
            }
        }

        internal class IntLteCriteron : ICriteron
        {
            public string FactKey { get; set; }
            public string AccessKey { get; set; }

            private int _compareValue;

            public IntLteCriteron(string fact, string key, int val)
            {
                FactKey = fact;
                AccessKey = key;

                _compareValue = val;
            }

            public bool Check(IGenericValue rawValue)
            {
                IGenericValue<int> val = rawValue.OfType<int>();

                if (val == null)
                    return false;

                return _compareValue <= val.Value;
            }
        }

        internal class IntLtCriteron : ICriteron
        {
            public string FactKey { get; set; }
            public string AccessKey { get; set; }

            private int _compareValue;

            public IntLtCriteron(string fact, string key, int val)
            {
                FactKey = fact;
                AccessKey = key;

                _compareValue = val;
            }

            public bool Check(IGenericValue rawValue)
            {
                IGenericValue<int> val = rawValue.OfType<int>();

                if (val == null)
                    return false;

                return _compareValue < val.Value;
            }
        }
    }
}