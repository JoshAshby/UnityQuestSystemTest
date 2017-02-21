namespace GrandCentral
{
    namespace Operator
    {
        namespace Criterion
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
        }
    }
}