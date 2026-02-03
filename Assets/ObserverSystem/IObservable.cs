using UnityEngine;

public interface IObservable<T>
{
public void AddObserver(T observer);
public void RemoveObserver(T observer);
 
}
