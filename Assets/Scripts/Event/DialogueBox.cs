using UnityEngine;

namespace UI
{
    [CreateAssetMenu(fileName = "New DialogueData", menuName = "Dialogue System/Dialogue Data")]
    public class DialogueBox : ScriptableObject
    {
        [System.Serializable]
        public struct DialogueData
        {
            [Header("Dialogue Settings: ")]
            public AudioClip speakerVoice;
            public string speakerName;
            [TextArea(0, 100)] public string dialogueTextRef; 
        }

        public DialogueData[] dialogueEntry;
        [Header("Other Event:")] public bool anotherEvent;
    }
}
