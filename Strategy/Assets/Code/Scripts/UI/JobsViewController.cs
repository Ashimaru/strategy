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

    private List<JobViewController> _createdQueueElements = new();
    private JobQueue _currentQueue;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void LoadQueue(JobQueue jobQueue)
    {
        if(_currentQueue != null)
        {
            _currentQueue.OnQueueChanged -= RefreshView;
        }

        if(jobQueue == null)
        {
            _currentQueue = null;
            gameObject.SetActive(false);
            return;
        }
        _currentQueue = jobQueue;
        _currentQueue.OnQueueChanged += RefreshView;
        CurrentJobView.CancelAction = _currentQueue.CancelJob;
        RefreshView();
    }

    private void RefreshView()
    {
        ClearCurrentItems();
        foreach (var job in _currentQueue.Jobs)
        {
            var go = Instantiate(JobQueueElementPrefab, JobQueueElementsParent);
            var controller = go.GetComponent<JobViewController>();
            controller.CancelJob = () => _currentQueue.CancelJob(job);
            controller.LoadJob(job);
            _createdQueueElements.Add(controller);
        }
        CurrentJobView.LoadJob(_currentQueue.CurrentJob);
    }


    private void ClearCurrentItems()
    {
        foreach (var jobView in _createdQueueElements)
        {
            DestroyImmediate(jobView.gameObject);
        }
        _createdQueueElements.Clear();
    }
}
