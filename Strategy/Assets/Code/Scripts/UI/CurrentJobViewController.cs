using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrentJobViewController : MonoBehaviour
{
    public Action<Job> CancelAction;

    [SerializeField]
    private TextMeshProUGUI _name;
    [SerializeField]
    private TextMeshProUGUI _timeLeft;
    [SerializeField]
    private Slider _progress;

    private Job _job;
    public void LoadJob(Job job)
    {
        _timeLeft.text = "0";
        _progress.value = 0;
        if (job == null)
        {
            _name.text = "Idle";
            return;
        }
        _job = job;
        _timeLeft.text = "0";
        _job.OnProgressUpdate += UpdateProgress;
        _name.text = job.Data.name;
    }

    public void CancelJob()
    {

        CancelAction.Invoke(_job);
    }

    private void UpdateProgress(float progress, float timeLeft)
    {
        _progress.value = progress;
        _timeLeft.text = Mathf.CeilToInt(timeLeft).ToString();
    }
}
