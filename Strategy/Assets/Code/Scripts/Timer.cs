
using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float TimeLeft;
    public Action OnTimeElapsed;

    public void Cancel()
    {
        Destroy(this);
    }


    private void Update()
    {
        TimeLeft -= Time.deltaTime;
        if(TimeLeft <= 0)
        {
            OnTimeElapsed();
            Destroy(this);
        }
    }
}