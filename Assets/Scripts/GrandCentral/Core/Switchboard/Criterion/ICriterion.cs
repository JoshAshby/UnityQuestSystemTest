namespace GrandCentral.Switchboard.Criterion
{
    public interface ICriterion
    {
        string FactKey { get; }
        string AccessKey { get; }

        bool Check(object value);
    }
}