using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamage
{
    //void takeDamage(int dmgIn);
    void takeDamage(int dmgIn, bool wasCritical, float criticalMult);
    void pushObject(Vector3 pushDir);
}