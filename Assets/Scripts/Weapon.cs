using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IInteractable
{
    [SerializeField] protected string weaponName;
    [SerializeField] protected int ammo;
    [SerializeField] protected float range;
    [SerializeField] protected float fireRate;
    [SerializeField] protected float damage;
    public virtual void Fire()
    {
        Debug.Log("Weapon fired");
    }

    public void Interact()
    {
        Debug.Log("Picked Up " + gameObject.name);
        // Here, you could add logic for picking up the weapon, like adding it to the player's inventory
        // For example:
        // PlayerInventory.Instance.AddWeapon(this);

        gameObject.SetActive(false); // Disable the weapon in the scene after pickup
    }
}
