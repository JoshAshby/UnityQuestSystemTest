namespace GrandCentral
{
    namespace Operator
    {
        internal class IntRangeCriterion : ICriterion
        {
            public string FactKey { get; set; }
            public string AccessKey { get; set; }

            private int _lowCompareValue;
            private int _highCompareValue;

            public IntRangeCriterion(string fact, string key, int low, int high)
            {
                FactKey = fact;
                AccessKey = key;

                _lowCompareValue = low;
                _highCompareValue = high;
            }

            public override string ToString()
            {
                return string.Format(" {0} <= {1}.{2} <= {3}", _lowCompareValue.ToString(), FactKey, AccessKey, _highCompareValue.ToString());
            }

            public bool Check(object rawValue)
            {
                int val = (int)rawValue;

                return _lowCompareValue <= val && val <= _highCompareValue;
            }
        }

        internal class IntGteCriteron : ICriterion
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

            public override string ToString()
            {
                return string.Format("{0}.{1} >= {2}", FactKey, AccessKey, _compareValue.ToString());
            }

            public bool Check(object rawValue)
            {
                int val = (int)rawValue;

                return val >= _compareValue;
            }
        }

        internal class IntGtCriteron : ICriterion
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

            public override string ToString()
            {
                return string.Format("{0}.{1} > {2}", FactKey, AccessKey, _compareValue.ToString());
            }

            public bool Check(object rawValue)
            {
                int val = (int)rawValue;

                return val > _compareValue;
            }
        }

        internal class IntLteCriteron : ICriterion
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

            public override string ToString()
            {
                return string.Format("{0}.{1} <= {2}", FactKey, AccessKey, _compareValue.ToString());
            }

            public bool Check(object rawValue)
            {
                int val = (int)rawValue;

                return val <= _compareValue;
            }
        }

        internal class IntLtCriteron : ICriterion
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

            public override string ToString()
            {
                return string.Format("{0}.{1} < {2}", FactKey, AccessKey, _compareValue.ToString());
            }

            public bool Check(object rawValue)
            {
                int val = (int)rawValue;

                return val < _compareValue;
            }
        }
    }
}