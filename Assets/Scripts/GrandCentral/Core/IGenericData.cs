using System;

namespace GrandCentral
{
    public interface IGenericValue
    {
        Type Type { get; }
        object Value { get; }

        IGenericValue<TResult> OfType<TResult>();
    }

    public interface IGenericValue<T>
    {
        T Value { get; set; }
    }
}