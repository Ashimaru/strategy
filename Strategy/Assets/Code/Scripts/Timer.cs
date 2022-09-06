
using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public string Name;
    public float TotalTime { get => _totalTime; set
        {
            _totalTime = value;
            _timeLeft = value;
        }
    }

    public bool IsRepeating { get; set; }

    public Action OnTimeElapsed { get; set; }
    public Action<float, float> OnProgressUpdate;

    private float _totalTime;
    private float _timeLeft;

    bool _isActive = true;

    public void Cancel()
    {
        if (_isActive)
        {
            //Debug.Log($"Removing {Name} on {gameObject.name} with id {GetInstanceID()}");
            _isActive = false;
            DestroyImmediate(this);
        }

    }


    private void Update()
    {
        _timeLeft -= Time.deltaTime;
        OnProgressUpdate?.Invoke(_timeLeft / _totalTime, _timeLeft);
        if (_timeLeft > 0)
        {
            return;
        }

        if (_isActive)
        {
            OnTimeElapsed();
        }

        if(IsRepeating)
        {
            _timeLeft = _totalTime;
            return;
        }

        Cancel();
    }
}