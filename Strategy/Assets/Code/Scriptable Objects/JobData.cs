using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "Job", menuName = "Game/Job")]
public class JobData : ScriptableObject
{
    [field: SerializeField] public float Duration { get; private set; } = 1000;
}