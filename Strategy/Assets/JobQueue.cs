using System;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class Job
{
    public Job(JobData jobData, Action onJobDone, Action onJobCancelled)
    {
        Data = jobData;
        OnJobDone = onJobDone;
        OnJobCancelled = onJobCancelled;
    }
    public JobData Data { get; private set; }
    public Action OnJobDone { get; private set; }
    public Action OnJobCancelled { get; private set; }
}

public class JobQueue : MonoBehaviour
{
    public List<Job> Jobs { get; private set; } = new();
    public Job CurrentJob { get; private set; }

    [SerializeField]
    private int _maxSize = 10;

    private Timer _currentTimer;
    void Start()
    {
        var data = ScriptableObject.CreateInstance<JobData>();
        data.name = "Dummy Job";
       
        AddToQueue(new Job(data, 
            () => Debug.Log("JOB DONE!"),
            () => Debug.Log("JOB CANCELLED")));
    }
    

    public void AddToQueue(Job job)
    {
        if(Jobs.Count >= _maxSize)
        {
            return;
        }

        if(Jobs == null)
        {
            StartJob(job);
            return;
        }

        Jobs.Add(job);
    }

    public void CancelJob(Job job)
    {
        if(CurrentJob == job)
        {
            Destroy(_currentTimer);
            CurrentJob.OnJobCancelled();
        }

        StartNextJobFromQueue();
    }

    private void StartNextJobFromQueue()
    {
        if(Jobs.Count == 0)
        {
            return;
        }
        var newJob = Jobs[0];
        Jobs.RemoveAt(0);
        StartJob(newJob);
    }

    private void StartJob(Job job)
    {
        CurrentJob = job;
       _currentTimer =  Utils.CreateTimer(gameObject, job.Data.Duration, OnJobCompleted);
    }

    private void OnJobCompleted()
    {
        CurrentJob.OnJobDone();
        StartNextJobFromQueue();
    }

}
