using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobsViewController : MonoBehaviour
{
    [SerializeField]
    private GameObject JobQueueElementPrefab;
    [SerializeField]
    private Transform JobQueueElementsParent;
    [SerializeField]
    private CurrentJobViewController CurrentJobView;

    private List<JobViewController> _createdQueueElements;
    private JobQueue _currentQueue;

    void Start()
    {
        CurrentJobView.CancelAction = _currentQueue.CancelJob;
    }    

    public void LoadJobs(JobQueue jobQueue)
    {
        ClearCurrentItems();

        foreach(var job in jobQueue.Jobs)
        {
            var go = Instantiate(JobQueueElementPrefab, JobQueueElementsParent);
            var controller = go.GetComponent<JobViewController>();
            controller.CancelJob = () => _currentQueue.CancelJob(job);
            controller.LoadJob(job);
        }


        _currentQueue = jobQueue;
        CurrentJobView.LoadJob(jobQueue.CurrentJob);
    }

    private void ClearCurrentItems()
    {
        foreach (var jobView in _createdQueueElements)
        {
            Destroy(jobView.gameObject);
        }
        _createdQueueElements.Clear();
    }
}
