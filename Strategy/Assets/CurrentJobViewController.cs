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
        _job = job;
        _name.text = job.Data.name;
    }

    public void CancelJob()
    {
        CancelAction.Invoke(_job);
    }
}
