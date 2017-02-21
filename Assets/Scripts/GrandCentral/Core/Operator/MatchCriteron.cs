namespace GrandCentral
{
    namespace Operator
    {
        internal class MatchCriteron<T> : ICriteron
        {
            public string FactKey { get; set; }
            public string AccessKey { get; set; }

            private T _compareValue;

            public MatchCriteron(string fact, string key, T val)
            {
                FactKey = fact;
                AccessKey = key;

                _compareValue = val;
            }

            public bool Check(IGenericValue rawValue)
            {
                IGenericValue<T> typedValue = rawValue.OfType<T>();

                if (typedValue == null)
                    return false;

                return typedValue.Value.Equals(_compareValue);
            }
        }
    }
}