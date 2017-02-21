namespace GrandCentral
{
    namespace Operator
    {
        internal interface ICriterion
        {
            string FactKey { get; }
            string AccessKey { get; }

            bool Check(object value);
        }
    }
}