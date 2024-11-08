using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Weapon
{
    public override void Fire()
    {
        Debug.Log("Pistol fired with damage: " + damage);
    }
}
