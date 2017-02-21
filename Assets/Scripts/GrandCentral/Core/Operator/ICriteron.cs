namespace GrandCentral
{
    namespace Operator
    {
        internal interface ICriteron
        {
            string FactKey { get; }
            string AccessKey { get; }

            bool Check(IGenericValue value);
        }
    }
}