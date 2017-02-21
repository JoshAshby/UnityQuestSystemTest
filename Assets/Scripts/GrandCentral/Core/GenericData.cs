using System;

namespace GrandCentral
{
    public class GenericValue<T> : IGenericValue, IGenericValue<T>
    {
        public GenericValue(T val)
        {
            Value = val;
        }

        public Type Type { get { return typeof(T); } }
        private T _value = default(T);

        public object Value
        {
            get { return (object)_value; }
            set { _value = (T)value; }
        }

        object IGenericValue.Value
        {
            get { return (object)_value; }
        }

        T IGenericValue<T>.Value
        {
            get { return (T)_value; }
            set { _value = (T)value; }
        }

        public IGenericValue<TResult> OfType<TResult>()
        {
            if (this is IGenericValue<TResult>)
                return (IGenericValue<TResult>)this;
            else
                return null;
        }
    }
}