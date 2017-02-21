namespace GrandCentral
{
    namespace Operator
    {
        namespace Criterion
        {
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

        }
    }
}