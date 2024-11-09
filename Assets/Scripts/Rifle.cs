using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : Weapon
{
    public override void Fire()
    {
        base.Fire();
        Debug.Log("Rifle fired with damage: " + damage);
    }
}
