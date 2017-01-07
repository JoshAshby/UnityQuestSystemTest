using System;
using System.Xml;
using System.Xml.Serialization;

namespace Ashogue
{
    namespace Data
    {
        public interface IMetadata
        {
            string ID { get; set; }

            Type Type { get; }
            object Value { get; }

            IMetadata<TResult> OfType<TResult>();
        }

        public interface IMetadata<T>
        {
            string ID { get; set; }

            T Value { get; set; }
        }

        public abstract class AMetadata<T> : IMetadata, IMetadata<T>
        {
            private string _id = Guid.NewGuid().ToString();
            [XmlAttribute("id")]
            public string ID
            {
                get { return _id; }
                set { _id = value; }
            }

            public Type Type { get { return typeof(T); } }
            private T _value;
            public object Value
            {
                get { return (object)_value; }
                set { _value = (T)value; }
            }

            object IMetadata.Value
            {
                get { return (object)Value; }
            }

            T IMetadata<T>.Value
            {
                get { return (T)Value; }
                set { Value = (object)value; }
            }

            public IMetadata<TResult> OfType<TResult>()
            {
                if (this is IMetadata<TResult>)
                    return (IMetadata<TResult>)this;
                else
                    return null;
            }
        }

        public class BoolMetadata : AMetadata<bool> { }
        public class FloatMetadata : AMetadata<float> { }
        public class StringMetadata : AMetadata<string> { }
    }
}