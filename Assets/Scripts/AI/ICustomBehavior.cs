public interface ICustomBehavior
{
    bool Enabled { get; set; }
    bool WanderWhileUsingAbility { get; }

    void Ability();
}