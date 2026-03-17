using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private static TimeManager _instance;
    public static TimeManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = UnityEngine.Object.FindFirstObjectByType<TimeManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("TimeManager");
                    _instance = go.AddComponent<TimeManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    private HashSet<object> pauseRequesters = new HashSet<object>();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void RequestPause(object requester)
    {
        if (requester == null) return;
        
        pauseRequesters.Add(requester);
        UpdateTimeScale();
    }

    public void RequestResume(object requester)
    {
        if (requester == null) return;

        pauseRequesters.Remove(requester);
        UpdateTimeScale();
    }

    private void UpdateTimeScale()
    {
        Time.timeScale = pauseRequesters.Count > 0 ? 0f : 1f;
        // Debug.Log($"TimeManager: Pause Count = {pauseRequesters.Count}, Scale = {Time.timeScale}");
    }
}
