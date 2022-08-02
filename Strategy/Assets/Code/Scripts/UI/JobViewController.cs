using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JobViewController : MonoBehaviour
{
    public Action CancelJob { get; set; }

    [SerializeField]
    private TextMeshProUGUI _name;
    public void Cancel()
    {
        CancelJob();
    }

    public void LoadJob(Job job)
    {
        _name.text = job.Data.name;
    }
}
