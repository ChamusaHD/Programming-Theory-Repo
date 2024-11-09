using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionArea : MonoBehaviour, IInteractable
{
    [SerializeField] private Weapon associatedWeapon;
    [SerializeField] private bool hasInteracted = false;
    [SerializeField] private Transform orientation;
    [SerializeField] private PlayerController playerController;

    private void Awake()
    {
        associatedWeapon = GetComponentInChildren<Weapon>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (hasInteracted) return;

        PlayerInteraction playerInteractor = other.GetComponent<PlayerInteraction>();
        if ((other.gameObject.tag == "Player"))
        {
            UIManager.Instance.DisplayText("Press " + playerInteractor.interactKey.ToString() + " to interact");
            playerInteractor.canInteract = true;
            //player.ItemToEquip(interactableItem);
            playerInteractor.ObjectToInteract(this, associatedWeapon);
        }       
    }

    private void OnTriggerExit(Collider other)
    {
        if((other.gameObject.tag == "Player"))
        {
            StartCoroutine(UIManager.Instance.TextFadeOut());
            PlayerInteraction player = other.GetComponent<PlayerInteraction>();
            if (player != null)
            {
                player.canInteract = false;
                //player.ItemToEquip(null);  // Clear interactable item
                player.ObjectToInteract(null, null);
            }
        }    
    }
    public void Interact()
    {
        if (hasInteracted) return;
        hasInteracted = true;
        UIManager.Instance.DisplayText("Picked up " + associatedWeapon.GetWeaponName());
        UIManager.Instance.DisplayWeaponEquipedText("Weapon Equiped: " + associatedWeapon.GetWeaponName());
        StartCoroutine(UIManager.Instance.TextFadeOut());
       // associatedWeapon.gameObject.SetActive(false);
        //if(playerController.GetCurrentWeaponEquiped() != null)
        //{
        //    playerController.SetWeaponToNull();
        //    playerController.SetWeaponEquiped(associatedWeapon);
        //}
        //else
        //{
        //    playerController.SetWeaponEquiped(associatedWeapon);
        //}
        associatedWeapon.gameObject.transform.parent = orientation;  
        associatedWeapon.gameObject.transform.localPosition = new Vector3(0.4f, -1.5f, .3f); // temporary code
        associatedWeapon.gameObject.transform.rotation = orientation.rotation;  
        associatedWeapon.GetComponent<Gun_rotation>().enabled = false;
        Debug.Log("Picked Up " + associatedWeapon.GetWeaponName());
        // Additional interaction logic for the item (e.g., giving player the weapon)
    }
    public Weapon GetInteractableItem()
    {
        return associatedWeapon;
    }
    public void SetInteractableObject()
    {
        associatedWeapon = null;
    }
}
