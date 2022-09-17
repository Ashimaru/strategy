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
                                IRepository<ArmyController>
                                
{
   private List<ArmyController> armies = new();

    void Awake()
    {
        Systems.RegisterSystem<IRepository<ArmyController>>(this);
    }

    private void OnDestroy()
    {
        Systems.DeregisterSystem<IRepository<ArmyController>>(this);
    }

    public void Add(ArmyController army)
    {
        armies.Add(army);
    }
    public List<ArmyController> Find(Func<ArmyController, bool> condition)
    {
        return armies.Where(condition).ToList();
    }

    public void Remove(ArmyController army)
    {
        armies.Remove(army);
    }
}