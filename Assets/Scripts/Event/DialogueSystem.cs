using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class DialogueSystem : MonoBehaviour
    {
        [Header("Dialogue Settings: ")]
        [SerializeField] private Text nameText;
        [SerializeField] private Text dialogueText;
        [SerializeField] private GameObject dialogueObject;
        [SerializeField] private float typingSpeed;

        [Header("Audio Dialogue: ")] 
        [SerializeField] private AudioSource dialogueSound = null;

        [Header("Other Event: ")] public bool otherEvent;
        
        private string[] _currentNameLines;
        private string[] _currentDialogueLines;
        private AudioClip[] _currentAudioClips;
        
        private int _currentLineIndex;
        private int _currentNameIndex;
        private bool _isTyping;
        private bool _isDone;

        [SerializeField] private DialogueBox currentDialogueBox;

        public void StartDialogue(DialogueBox dialogueBox)
        {
            var dialogueCount = dialogueBox.dialogueEntry.Length;
            currentDialogueBox = dialogueBox;

            _currentNameLines = new string[dialogueCount];
            _currentDialogueLines = new string[dialogueCount];
            _currentAudioClips = new AudioClip[dialogueCount];

            for (var i = 0; i < dialogueCount; i++)
            {
                _currentNameLines[i] = dialogueBox.dialogueEntry[i].speakerName;
                _currentDialogueLines[i] = dialogueBox.dialogueEntry[i].dialogueTextRef;
                _currentAudioClips[i] = dialogueBox.dialogueEntry[i].speakerVoice;
            }
    
            _currentLineIndex = 0;
            _currentNameIndex = 0;
            _isTyping = false;

            DisplayCurrentLine();
        }

        private void Update()
        {
            if (_isTyping)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                    SkipTyping();
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _currentLineIndex++;
                    _currentNameIndex++;
                    if (_currentLineIndex < _currentDialogueLines.Length)
                        DisplayCurrentLine();
                    else
                        EndDialogue();
                }
            }
        }

        #region Type Functions
        private void DisplayCurrentLine()
        {
            if (_currentAudioClips[_currentLineIndex] != null && dialogueSound != null)
            {
                dialogueSound.Stop();
                dialogueSound.clip = _currentAudioClips[_currentLineIndex];
                dialogueSound.Play();
            }

            nameText.text = "";
            nameText.text = _currentNameLines[_currentNameIndex];
            dialogueText.text = "";
            StartCoroutine(TypeDialogue(_currentDialogueLines[_currentLineIndex]));
        }

        private IEnumerator TypeDialogue(string dialogueLine)
        {
            _isTyping = true;
            if (dialogueSound != null)
                dialogueSound.Stop(); 

            foreach (var character in dialogueLine)
            {
                if (dialogueSound != null) dialogueSound.Play();
                dialogueText.text += character;
                yield return new WaitForSecondsRealtime(typingSpeed);
            }
            
            _isTyping = false;
        }
        #endregion

        #region Stop Typing Functions
        private void SkipTyping()
        {
            StopAllCoroutines();
            dialogueText.text = _currentDialogueLines[_currentLineIndex];
            _isTyping = false;
        }

        private void EndDialogue()
        {
            dialogueText.text = "";
            dialogueObject.SetActive(false);

            if (currentDialogueBox != null && currentDialogueBox.anotherEvent)
            {
                // Trigger the event in eventPlayer here if needed
                otherEvent = true;
                _isDone = true;
            }
            else if (currentDialogueBox != null && !currentDialogueBox.anotherEvent)
            {
                otherEvent = false;
                _isDone = true;
            }
                
        }
        #endregion

        public bool IsDone()
        {
            return _isDone;
        }

        public void SetBool(bool value) => value = IsDone();
    }
}
