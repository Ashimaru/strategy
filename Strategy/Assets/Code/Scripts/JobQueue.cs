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

    public Job(JobData jobData, Action onJobDone) : this(jobData, onJobDone, () => { })
    {
    }

    public JobData Data { get; private set; }
    public Action OnJobDone { get; private set; }
    public Action OnJobCancelled { get; private set; }

    public delegate void JobProgressUpdate(float progress, float timeLeft);
    public JobProgressUpdate OnProgressUpdate { get; set; } = delegate { };

}

public class JobQueue : MonoBehaviour
{
    public List<Job> Jobs { get; private set; } = new();
    public Job CurrentJob { get; private set; }

    public delegate void QueueChangedCallback();
    public QueueChangedCallback OnQueueChanged { get; set; }

    [SerializeField]
    private int _maxSize = 10;

    private Timer _currentTimer;
    private static int id = 0;

    void Update()
    {

        if (Input.GetKeyUp(KeyCode.K))
        {
            var data = ScriptableObject.CreateInstance<JobData>();
            data.name = "Dummy Job " + ++id;

            AddToQueue(new Job(data,
                () => Debug.Log(data.name + " DONE!"),
                () => Debug.Log(data.name + " CANCELLED")));
        }
    }


    public void AddToQueue(Job job)
    {
        if (Jobs.Count >= _maxSize)
        {
            return;
        }

        if (CurrentJob == null)
        {
            StartJob(job);
            return;
        }

        Jobs.Add(job);
        if (OnQueueChanged != null)
        {
            OnQueueChanged();
        }
    }

    public void CancelJob(Job job)
    {
        if (CurrentJob == job)
        {
            Destroy(_currentTimer);
            CurrentJob.OnJobCancelled();
            CurrentJob = null;
            StartNextJobFromQueue();
            OnQueueChanged();
            return;
        }

        if (Jobs.Remove(job) && OnQueueChanged != null)
        {
            job.OnJobCancelled();
            OnQueueChanged();
        }

    }

    private void StartNextJobFromQueue()
    {
        if (Jobs.Count == 0)
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
        _currentTimer = Utils.CreateTimer(gameObject, 
                                job.Data.Duration, 
                                OnJobCompleted, 
                                $"Job Queue: waiting until {job.Data.name} is complete", 
                                (progress, timeLeft) => job.OnProgressUpdate(progress, timeLeft));
        OnQueueChanged?.Invoke();
    }

    private void OnJobCompleted()
    {
        CurrentJob.OnJobDone();
        CurrentJob = null;
        OnQueueChanged?.Invoke();

        StartNextJobFromQueue();
    }

}
