namespace GrandCentral.Switchboard.Criterion
{
    internal interface ICriterion
    {
        string FactKey { get; }
        string AccessKey { get; }

        bool Check(object value);
    }
}