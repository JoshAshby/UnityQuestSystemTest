using System;

namespace GrandCentral.Switchboard.Criterion
{
    public class ProcCriterion<T> : ICriterion
    {
        public string FactKey { get; set; }
        public string AccessKey { get; set; }

        private Func<T, bool> _compare;

        public ProcCriterion(string fact, string key, Func<T, bool> compare)
        {
            FactKey = fact;
            AccessKey = key;

            _compare = compare;
        }

        public ProcCriterion(string key, Func<T, bool> compare)
        {
            FactKey = "global";
            AccessKey = key;

            _compare = compare;
        }

        public override string ToString()
        {
            // TODO: ((Expression<Func<T, bool>>)_compare).ToString()
            return string.Format("{0}.{1}", FactKey, AccessKey);
        }

        public bool Check(object rawValue)
        {
            if (rawValue == null)
                rawValue = default(T);

            T typedValue = (T)rawValue;

            if (typedValue == null)
                return false;

            return _compare(typedValue);
        }
    }
}