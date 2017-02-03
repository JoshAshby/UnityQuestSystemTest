using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace Ashogue
{
    namespace Data
    {
        public interface INode
        {
            string ID { get; set; }
            Vector2 Position { get; set; }

            string Name { get; set; }

            Dictionary<string, IMetadata> Metadata { get; set; }
            TMetadata AddMetadata<TMetadata>(string ID = null) where TMetadata : IMetadata, new();
            IMetadata AddMetadata(Type TMetadata, string ID = null);
            void RenameMetadata(string FromID, string ToID);
            void RemoveMetadata(string ID);
        }

        public interface IBranchedNode : INode
        {
            Dictionary<string, IBranch> Branches { get; set; }
            TBranch AddBranch<TBranch>(string ID = null) where TBranch : IBranch, new();
            IBranch AddBranch(Type TBranch, string ID = null);
            void RenameBranch(string FromID, string ToID);
            void RemoveBranch(string ID);
        }

        public abstract class ANode : INode
        {
            private Vector2 _position = new Vector2();
            public Vector2 Position
            {
                get { return _position; }
                set { _position = value; }
            }

            private string _id = Guid.NewGuid().ToString();
            [XmlAttribute("id")]
            public string ID
            {
                get { return _id; }
                set { _id = value; }
            }

            private string _name = "";
            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            private Dictionary<string, IMetadata> _metadata = new Dictionary<string, IMetadata>();
            [XmlIgnore]
            public virtual Dictionary<string, IMetadata> Metadata
            {
                get { return _metadata; }
                set { _metadata = value; }
            }

            [XmlArray("Metadata")]
            [XmlArrayItem("BoolMetadata", typeof(BoolMetadata))]
            [XmlArrayItem("FloatMetadata", typeof(FloatMetadata))]
            [XmlArrayItem("StringMetadata", typeof(StringMetadata))]
            public IMetadata[] XmlMetadata
            {
                get { return Metadata.Values.ToArray(); }
                set { Metadata = value.ToDictionary(i => i.ID, i => i); }
            }

            public virtual TMetadata AddMetadata<TMetadata>(string ID = null) where TMetadata : IMetadata, new()
            {
                if (String.IsNullOrEmpty(ID))
                    ID = Guid.NewGuid().ToString();

                TMetadata metadata = new TMetadata { ID = ID };
                Metadata.Add(ID, metadata);

                return metadata;
            }

            public virtual IMetadata AddMetadata(Type TMetadata, string ID = null)
            {
                if (String.IsNullOrEmpty(ID))
                    ID = Guid.NewGuid().ToString();

                IMetadata metadata = (IMetadata)Activator.CreateInstance(TMetadata);

                metadata.ID = ID;
                Metadata.Add(ID, metadata);

                return metadata;
            }

            public virtual void RenameMetadata(string fromID, string toID)
            {
                IMetadata metadata = Metadata[fromID];

                Metadata.Remove(fromID);
                metadata.ID = toID;
                Metadata.Add(toID, metadata);
            }

            public virtual void RemoveMetadata(string ID)
            {
                Metadata.Remove(ID);
            }
        }

        public abstract class ABranchedNode : ANode, IBranchedNode
        {
            private Dictionary<string, IBranch> _branches = new Dictionary<string, IBranch>();
            [XmlIgnore]
            public virtual Dictionary<string, IBranch> Branches
            {
                get { return _branches; }
                set { _branches = value; }
            }

            [XmlArray("Branches")]
            [XmlArrayItem("SimpleBranch", typeof(SimpleBranch))]
            public IBranch[] XmlBranches
            {
                get { return Branches.Values.ToArray(); }
                set { Branches = value.ToDictionary(i => i.ID, i => i); }
            }

            public virtual TBranch AddBranch<TBranch>(string ID = null) where TBranch : IBranch, new()
            {
                if (String.IsNullOrEmpty(ID))
                    ID = Guid.NewGuid().ToString();

                TBranch branch = new TBranch { ID = ID };
                Branches.Add(ID, branch);

                return branch;
            }

            public virtual IBranch AddBranch(Type TBranch, string ID = null)
            {
                if (String.IsNullOrEmpty(ID))
                    ID = Guid.NewGuid().ToString();

                IBranch Branch = (IBranch)Activator.CreateInstance(TBranch);

                Branch.ID = ID;
                Branches.Add(ID, Branch);

                return Branch;
            }

            public virtual void RenameBranch(string fromID, string toID)
            {
                IBranch Branch = Branches[fromID];

                Branches.Remove(fromID);
                Branch.ID = toID;
                Branches.Add(toID, Branch);
            }

            public virtual void RemoveBranch(string ID)
            {
                Branches.Remove(ID);
            }
        }

        public class TextNode : ABranchedNode
        {
            private string _text = "";
            public string Text { get { return _text; } set { _text = value; } }
        }

        public class WaitNode : ABranchedNode
        {
            private Dictionary<string, IBranch> _branches = new Dictionary<string, IBranch>
            {
                { "out", new SimpleBranch() }
            };
            [XmlIgnore]
            public override Dictionary<string, IBranch> Branches
            {
                get { return _branches; }
                set { _branches = value; }
            }

            private float _seconds = 0f;
            public float Seconds { get { return _seconds; } set { _seconds = value; } }
        }

        public class EventNode : ABranchedNode
        {
            private Dictionary<string, IBranch> _branches = new Dictionary<string, IBranch>
            {
                { "out", new SimpleBranch() }
            };
            [XmlIgnore]
            public override Dictionary<string, IBranch> Branches
            {
                get { return _branches; }
                set { _branches = value; }
            }

            private string _message = "";
            public string Message { get { return _message; } set { _message = value; } }
        }

        public class EndNode : ANode { }
    }
}