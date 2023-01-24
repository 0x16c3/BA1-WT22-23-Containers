public interface IDamageable
{
    int Health { get; }

    void Damage(int damage);
    void Heal(int heal);
}