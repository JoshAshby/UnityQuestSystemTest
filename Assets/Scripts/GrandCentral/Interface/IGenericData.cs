using System;

namespace GrandCentral
{
    public interface IGenericData
    {
        Type Type { get; }
        object Value { get; }

        IGenericData<TResult> OfType<TResult>();
    }

    public interface IGenericData<T>
    {
        T Value { get; set; }
    }
}