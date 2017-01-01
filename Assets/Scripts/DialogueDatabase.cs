using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Ashogue
{
    public class Choice
    {
        [XmlAttribute("id")]
        public string ID;

        public string Text;
        public string NextNodeID;
    }

    public abstract class INode
    {
        [XmlAttribute("id")]
        public string ID;

        [XmlArray("Choices")]
        [XmlArrayItem("Choice")]
        public List<Choice> Choices;

        public XMLDictionary<string, string> Metadata;
    }

    public class TextNode : INode
    {
        public string Text;
    }

    public class Dialogue
    {
        [XmlAttribute("id")]
        public string ID;

        [XmlArray("Nodes")]
        [XmlArrayItem("TextNode", typeof(TextNode))]
        public List<INode> Nodes;

        public Dialogue()
        { }

        public Dialogue(string id = "")
        {
            this.ID = id;
        }
    }

    [XmlRoot("DialogueCollection")]
    public class DialogueContainer
    {
        [XmlArray("Dialogues")]
        [XmlArrayItem("Dialogue")]
        public List<Dialogue> Dialogues = new List<Dialogue>();

        // Copypasta from http://wiki.unity3d.com/index.php?title=Saving_and_Loading_Data:_XmlSerializer
        // var monsterCollection = MonsterContainer.Load(Path.Combine(Application.dataPath, "monsters.xml"));
        // monsterCollection.Save(Path.Combine(Application.persistentDataPath, "monsters.xml"));
        public void Save(string path)
        {
            var serializer = new XmlSerializer(typeof(DialogueContainer));
            using (var stream = new FileStream(path, FileMode.Create))
            {
                serializer.Serialize(stream, this);
            }
        }

        public static DialogueContainer Load(string path)
        {
            var serializer = new XmlSerializer(typeof(DialogueContainer));
            using (var stream = new FileStream(path, FileMode.Open))
            {
                return serializer.Deserialize(stream) as DialogueContainer;
            }
        }

        //Loads the xml directly from the given string. Useful in combination with www.text.
        public static DialogueContainer LoadFromText(string text)
        {
            var serializer = new XmlSerializer(typeof(DialogueContainer));
            return serializer.Deserialize(new StringReader(text)) as DialogueContainer;
        }
    }

    // copypasta from http://stackoverflow.com/a/12555098/3877528
    public class XMLDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            reader.ReadStartElement("item");

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();

                this.Add(key, value);

                reader.MoveToContent();
            }

            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

            writer.WriteStartElement("item");

            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();

                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }
}