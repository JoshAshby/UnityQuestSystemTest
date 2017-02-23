namespace GrandCentral.Switchboard.Criterion
{
    public class MatchCriterion<T> : ICriterion
    {
        public string FactKey { get; set; }
        public string AccessKey { get; set; }

        private T _compareValue;

        public MatchCriterion(string fact, string key, T val)
        {
            FactKey = fact;
            AccessKey = key;

            _compareValue = val;
        }

        public MatchCriterion(string key, T val)
        {
            FactKey = "global";
            AccessKey = key;

            _compareValue = val;
        }

        public override string ToString()
        {
            return string.Format("{0}.{1} == {2}", FactKey, AccessKey, _compareValue.ToString());
        }

        public bool Check(object rawValue)
        {
            if (rawValue == null)
                rawValue = default(T);

            T typedValue = (T)rawValue;

            if (typedValue == null)
                return false;

            return typedValue.Equals(_compareValue);
        }
    }
}