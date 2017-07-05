using System;

namespace GrandCentral.Criterion
{
    public class ProcCriterion<T> : ICriterion
    {
        public string Hint { get; set; }
        public string FactKey { get; set; }

        private Func<T, bool> _compare;

        public ProcCriterion(string fact, string key, Func<T, bool> compare)
        {
            Hint = fact;
            FactKey = key;

            _compare = compare;
        }

        public ProcCriterion(string key, Func<T, bool> compare)
        {
            Hint = "global";
            FactKey = key;

            _compare = compare;
        }

        public override string ToString()
        {
            // TODO: ((Expression<Func<T, bool>>)_compare).ToString()
            return string.Format("{0}.{1}", Hint, FactKey);
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