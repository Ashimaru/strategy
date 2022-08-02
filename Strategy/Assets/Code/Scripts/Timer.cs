
using System;
using UnityEngine;

public class Timer : MonoBehaviour
{

    public float TotalTime { get => _totalTime; set
        {
            _totalTime = value;
            _timeLeft = value;
        }
    }
    public Action OnTimeElapsed { get; set; }

    public Action<float, float> OnProgressUpdate;

    private float _totalTime;
    private float _timeLeft;

    public void Cancel()
    {
        Destroy(this);
    }


    private void Update()
    {
        _timeLeft -= Time.deltaTime;
        if(_timeLeft <= 0)
        {
            OnTimeElapsed();
            Destroy(this);
        }

        OnProgressUpdate?.Invoke(_timeLeft / _totalTime, _timeLeft);
    }
}