using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Ashogue
{
    namespace Data
    {
        public abstract class XmlContainer<T>
        {
            // Copypasta from http://wiki.unity3d.com/index.php?title=Saving_and_Loading_Data:_XmlSerializer
            // var monsterCollection = MonsterContainer.Load(Path.Combine(Application.dataPath, "monsters.xml"));
            // monsterCollection.Save(Path.Combine(Application.persistentDataPath, "monsters.xml"));
            public void Save(string path)
            {
                var serializer = new XmlSerializer(typeof(T));
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    serializer.Serialize(stream, this);
                }
            }

            public static T Load(string path)
            {
                var serializer = new XmlSerializer(typeof(T));
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    return (T) serializer.Deserialize(stream);
                }
            }

            //Loads the xml directly from the given string. Useful in combination with www.text.
            public static T LoadFromText(string text)
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T) serializer.Deserialize(new StringReader(text));
            }
        }

        [XmlRoot("DialogueCollection")]
        public class DialogueContainer : XmlContainer<DialogueContainer>
        {
            [XmlIgnore]
            public Dictionary<string, Dialogue> Dialogues = new Dictionary<string, Dialogue>();

            [XmlArray("Dialogues")]
            [XmlArrayItem("Dialogue", typeof(Dialogue))]
            public Dialogue[] XmlDialogues
            {
                get { return Dialogues.Values.ToArray(); }
                set { Dialogues = value.ToDictionary(i => i.ID, i => i); }
            }
        }
    }
}