using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISource
{
    public void AddObserver(IObserver _ob);
    public void RemoveObserver(IObserver _ob);

    public void SendSignal();
}
