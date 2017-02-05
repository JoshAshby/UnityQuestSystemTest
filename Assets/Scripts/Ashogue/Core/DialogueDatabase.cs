using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System;

namespace Ashogue
{
    namespace Data
    {
        [XmlRoot("DialogueCollection")]
        public class DialogueContainer
        {
            private Dictionary<string, IDialogue> _dialogues = new Dictionary<string, IDialogue>();
            [XmlIgnore]
            public Dictionary<string, IDialogue> Dialogues {
                get { return _dialogues; }
                set { _dialogues = value; }
            }

            [XmlArray("Dialogues")]
            [XmlArrayItem("Dialogue", typeof(Dialogue))]
            public IDialogue[] XmlDialogues
            {
                get { return Dialogues.Values.ToArray(); }
                set { Dialogues = value.ToDictionary(i => i.ID, i => i); }
            }

            public IDialogue AddDialogue(string ID = null)
            {
                if (String.IsNullOrEmpty(ID))
                    ID = Guid.NewGuid().ToString();

                IDialogue dialogue = new Dialogue { ID = ID };
                Dialogues.Add(ID, dialogue);

                return dialogue;
            }

            public void RemoveDialogue(string ID)
            {
                Dialogues.Remove(ID);
            }

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
                    return (DialogueContainer)serializer.Deserialize(stream);
                }
            }

            //Loads the xml directly from the given string. Useful in combination with www.text.
            public static DialogueContainer LoadFromText(string text)
            {
                var serializer = new XmlSerializer(typeof(DialogueContainer));
                return (DialogueContainer)serializer.Deserialize(new StringReader(text));
            }
        }
    }
}