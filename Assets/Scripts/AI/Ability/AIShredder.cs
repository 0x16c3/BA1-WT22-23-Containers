using UnityEngine;

using GD.MinMaxSlider;

public class AIShredder : MonoBehaviour, ICustomBehavior
{
    [MinMaxSlider(0f, 10f)]
    public Vector2 SearchDelay = new Vector2(3f, 5f);
    [MinMaxSlider(0f, 10f)]
    public Vector2 EatDelay = new Vector2(3f, 5f);
    public int Damage = 1;

    public bool WanderWhileUsingAbility => false;

    public bool Enabled
    {
        get => enabled;
        set => enabled = value;
    }

    float _nextSearch = -1f;

    public void Ability()
    {

    }
}
