using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IFocusableTile
{
    abstract void OnFocusAcquired();
    abstract void OnFocusLost();
}

