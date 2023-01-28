using UnityEngine;

public interface IEvent
{
    Vector2Int Duration { get; set; }

    EventData Data { get; set; }
}