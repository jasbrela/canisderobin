using System.Collections;
using InteractableObjects;
using TMPro;
using UnityEngine;

namespace Managers
{
    public class AlarmManager : MonoBehaviour
    {
        [SerializeField] private Sprite activatedSprite;
        [SerializeField] private Sprite deactivatedSprite;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private GameObject alarmObject;
        [SerializeField] private float timerIncrease;
    
        private Alarm _currentAlarm;
        private float _timer;
        private bool _inGracePeriod;
        private Coroutine _alarmCoroutine;
    
        void Awake()
        {
            EventHandler.Instance.ListenToOnAlarmIsDeactivated(ResetAlarm);
        }
    
        private void ResetAlarm(Alarm newAlarm)
        {
            if (_currentAlarm != null)
            {
                _currentAlarm.ActivateAlarm();
                _currentAlarm.SetSprite(activatedSprite);
            }
        
            if (_alarmCoroutine != null) StopCoroutine(_alarmCoroutine);
        
            _currentAlarm = newAlarm;
            _timer = _currentAlarm.GetGracePeriod();
            _currentAlarm.SetSprite(deactivatedSprite);
        
            timerText.text = FormatTime(0);
        
            _alarmCoroutine = StartCoroutine(AlarmTimer());
            alarmObject.SetActive(true);
        }
    
        private IEnumerator AlarmTimer()
        {
            _inGracePeriod = true;
            while (_inGracePeriod)
            {
                yield return new WaitForSecondsRealtime(timerIncrease);
                _timer -= timerIncrease;
                if (_timer < 0f)
                {
                    timerText.text = FormatTime(0);
                    StopAlarm();
                }
                else
                {
                    timerText.text = FormatTime(_timer);
                }
            }
        }

        private string FormatTime( float time )
        {
            int minutes = (int) time / 60 ;
            int seconds = (int) time - 60 * minutes;
            int milliseconds = (int) (10 * (time - minutes * 60 - seconds));
            return $"{minutes:0}:{seconds:00}.{milliseconds:0}";
        }
    
        private void StopAlarm()
        {
            StopCoroutine(_alarmCoroutine);
            alarmObject.SetActive(false);
        }
    }
}
