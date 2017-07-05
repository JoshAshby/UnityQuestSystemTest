namespace GrandCentral.Criterion
{
    public interface ICriterion
    {
        string Hint { get; }
        string FactKey { get; }

        bool Check(object value);
    }
}