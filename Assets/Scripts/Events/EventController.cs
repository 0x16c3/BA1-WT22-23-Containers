using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GD.MinMaxSlider;

[System.Serializable]
public class EventData
{
    public GameObject Prefab;
    public float StartsAt;
    public float EndsAt;

    public float Duration => EndsAt - StartsAt;
}

public class EventController : MonoBehaviour
{
    [Header("General Settings")]
    public float RoundTime = 420;
    public static float RoundTimeStatic = 420;
    public static List<float> EventTimeStatic;
    public int MaxEvents = 8;
    public int GracePeriod = 30;

    [MinMaxSlider(0f, 420f)]
    public Vector2 TimeBetweenEvents = new Vector2(30f, 60f);

    public List<GameObject> EventPrefabs = new List<GameObject>();

    [Header("Event Info")]
    public List<EventData> RemainingEvents = new List<EventData>();

    float _enabledAt = -1f;
    int _eventCount = 0;

    List<EventData> _eventList = new List<EventData>();
    List<float> _eventTimes = new List<float>();

    private void Awake()
    {
        RoundTimeStatic = RoundTime;
    }
    void OnEnable()
    {
        _enabledAt = Time.time + 0.1f; // Add a small delay to make sure everything is initialized
        _eventCount = Random.Range(3, MaxEvents > 0 ? MaxEvents : 3);

        for (int i = 0; i < _eventCount; i++)
        {
            EventData prevEvent = _eventList.Count > 0 ? _eventList.Last() : null;

            var eventData = new EventData();
            eventData.Prefab = EventPrefabs[Random.Range(0, EventPrefabs.Count)];

            if (prevEvent != null)
                eventData.StartsAt = prevEvent.StartsAt /* used to be prevEvent.EndsAt */ + Random.Range(TimeBetweenEvents.x, TimeBetweenEvents.y);
            else
                eventData.StartsAt = _enabledAt + GracePeriod + Random.Range(TimeBetweenEvents.x, TimeBetweenEvents.y);

            // Get IEvent interface
            var iEvent = eventData.Prefab.GetComponent<IEvent>();
            if (iEvent == null)
            {
                Debug.LogError($"Event prefab {eventData.Prefab.name} does not implement IEvent interface!");
                return;
            }

            eventData.EndsAt = eventData.StartsAt + Random.Range(iEvent.Duration.x, iEvent.Duration.y);

            _eventList.Add(eventData);
        }

        _eventTimes = _eventList.Select(e => e.StartsAt).ToList();
        EventTimeStatic = _eventTimes;
    }

    public EventData GetEventAt(float time = -1f)
    {
        if (time == -1f)
            time = Time.time;

        return _eventList.FirstOrDefault(e => time >= e.StartsAt && time <= e.EndsAt);
    }

    void Update()
    {

        if (_eventTimes.Count > 0 && Time.time >= _eventTimes[0])
        {
            // Find event that starts at this time
            var eventData = _eventList.FirstOrDefault(e => e.StartsAt == _eventTimes[0]);
            if (eventData.Duration <= 0)
            {
                Debug.LogError("Event not found!");
                return;
            }

            // Remove event from lists
            _eventTimes.RemoveAt(0);
            _eventList.Remove(eventData);

            // GameObject with IEvent interface
            var eventObj = Instantiate(eventData.Prefab, transform);
            eventObj.transform.localPosition = Vector3.zero;

            var iEvent = eventObj.GetComponent<IEvent>();
            if (iEvent == null)
            {
                Debug.LogError($"Event prefab {eventData.Prefab.name} does not implement IEvent interface!");
                return;
            }

            iEvent.Data = eventData;
        }

        RemainingEvents = _eventList;
    }
}
