using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IRepository<Element>
{
    public void Add(Element element);
    public void Remove(Element element);    
    public List<Element> Find(Func<Element, bool> condition);
}


public class EntityRepository : MonoBehaviour,
                                IRepository<ArmyController>,
                                IRepository<Location>
                                
{
    private List<ArmyController> _armies = new();
    private List<Location> _locations = new();

    void Awake()
    {
        Systems.RegisterSystem<IRepository<ArmyController>>(this);
        Systems.RegisterSystem<IRepository<Location>>(this);
    }

    private void OnDestroy()
    {
        Systems.DeregisterSystem<IRepository<ArmyController>>(this);
        Systems.DeregisterSystem<IRepository<Location>>(this);
    }

    public void Add(ArmyController army)
    {
        _armies.Add(army);
    }
    public List<ArmyController> Find(Func<ArmyController, bool> condition)
    {
        return _armies.Where(condition).ToList();
    }

    public void Remove(ArmyController army)
    {
        _armies.Remove(army);
    }

    public void Add(Location element)
    {
        _locations.Add(element);
    }

    public void Remove(Location element)
    {
        _locations.Remove(element);
    }

    public List<Location> Find(Func<Location, bool> condition)
    {
        return _locations.Where(condition).ToList();
    }
}