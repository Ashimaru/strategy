using UnityEngine;

public enum IncreaseAmount
{
    Small = 10,
    Medium = 30,
    Large = 80
}

public enum HeatLevel
{
    Low,
    Medium,
    High
}

public class RegionHeatManager : MonoBehaviour
{
    [SerializeField]
    private int _heat = 0;

    public int Heat { get { return _heat; } }
    private Timer _heatDecreaseTimer = null;

    public HeatLevel GetHeatLevel()
    {
        if(_heat <= 30)
        {
            return HeatLevel.Low;
        }

        if(_heat <= 80)
        {
            return HeatLevel.Medium;
        }

        return HeatLevel.High;
    }

    public void IncreaseHeat(IncreaseAmount amount)
    {
        if(_heat == 0)
        {
            Debug.Assert(_heatDecreaseTimer == null);
            _heatDecreaseTimer = Utils.CreateRepeatingTimer(gameObject, 20F, () => DecreaseHeat(5), $"{gameObject.name} heat decrease");
        }

        _heat += (int)amount;
        if(_heat >= 100)
        {
            _heat = 100;
            Debug.Log("ALARM!!! HEAT CRITICAL - NOT IMPLEMENTED YET");
            return;
        }
    }


    private void DecreaseHeat(int amount)
    {
        _heat -= amount;
        if(_heat > 0)
        {
            return;
        }

        _heatDecreaseTimer.Cancel();
        _heatDecreaseTimer = null;
        _heat = 0;
    }
}
