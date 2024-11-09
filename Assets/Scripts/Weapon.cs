using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Weapon : MonoBehaviour//, IInteractable
{
    [SerializeField] protected string weaponName;
    [SerializeField] protected int ammo;
    [SerializeField] protected float range;
    [SerializeField] protected float fireRate;
    [SerializeField] protected float damage;
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Transform shootingPoint;
    
    public virtual void Fire()
    {
        Debug.Log("Weapon fired");
        Instantiate(bulletPrefab, transform.position, transform.rotation);
    }

    public void Interact()
    {
        Debug.Log("Picked Up " + weaponName);
        // Here, you could add logic for picking up the weapon, like adding it to the player's inventory
        // For example:
        // PlayerInventory.Instance.AddWeapon(this);

        gameObject.SetActive(false); // Disable the weapon in the scene after pickup
        UIManager.Instance.DisplayText("Picked up " + weaponName);
    }
   
    public string GetWeaponName()
    {
        return weaponName;
    }
}
