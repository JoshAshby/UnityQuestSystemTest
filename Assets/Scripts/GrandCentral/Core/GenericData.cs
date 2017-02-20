using System;

namespace GrandCentral
{
    public class GenericData<T> : IGenericData, IGenericData<T>
    {
        public GenericData(T val)
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

        object IGenericData.Value
        {
            get { return (object)Value; }
        }

        T IGenericData<T>.Value
        {
            get { return (T)Value; }
            set { Value = (object)value; }
        }

        public IGenericData<TResult> OfType<TResult>()
        {
            if (this is IGenericData<TResult>)
                return (IGenericData<TResult>)this;
            else
                return null;
        }
    }
}