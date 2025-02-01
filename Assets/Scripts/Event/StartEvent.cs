using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TextCore.Text;

namespace Events
{
    public class StartEvent : MonoBehaviour
    {

        [Header("Start Event Settings: ")]
        [SerializeField] private UnityEvent[] startEvent;
        [SerializeField] private DialogueSystem otherNewEvent;
        [SerializeField] private string eventId;
        [SerializeField] private bool isAutomatic;

        private bool _onlyOnce;

        // Start is called before the first frame update
        private void Start()
        {
            // ReSharper disable once InvertIf
            if (!PlayerPrefs.HasKey(eventId))
            {
                startEvent[0].Invoke();
                PlayerPrefs.SetInt(eventId, 1);
            }
        }

        private void Update()
        {
            if (otherNewEvent != null && otherNewEvent.otherEvent && !_onlyOnce)
            {
                _onlyOnce = true;
                // ReSharper disable once InvertIf
                if (!PlayerPrefs.HasKey(eventId + "_2"))
                {
                    startEvent[1].Invoke();
                    PlayerPrefs.SetInt(eventId + "_2", 1);
                }
            }

            switch (isAutomatic)
            {
                // Check if startEvent[2] should be activated
                case true when PlayerPrefs.HasKey(eventId) && startEvent.Length > 1 && !_onlyOnce:
                    _onlyOnce = true;
                    startEvent[2].Invoke();
                    break;
            }
        }

        public void ControlTime(int timeScale) => Time.timeScale = timeScale;
    }
}
