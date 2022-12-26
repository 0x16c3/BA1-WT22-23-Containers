using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagable
{
    void Damage(float damage, float frequency, GameObject damageEffect);
}
