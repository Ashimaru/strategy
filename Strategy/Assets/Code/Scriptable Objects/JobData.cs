using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Job", menuName = "Game/Job")]
public class JobData : ScriptableObject
{
    [SerializeField]
    private float _duration = 1;
    [SerializeField]
    private int _jobCost = 5;
    public float Duration { get => _duration; }
    public int JobCost { get => _jobCost; }
}