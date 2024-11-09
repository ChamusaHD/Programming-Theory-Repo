using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public bool canInteract { get; set; } //ENCAPSULATION!!!
    [SerializeField] private Weapon itemToEquip;
    [SerializeField] private IInteractable objectToInteract;
    [SerializeField] public KeyCode interactKey { get; private set; } //ENCAPSULATION!!!

    [SerializeField] private PlayerController playerController;

    void Start()
    {
        canInteract = false;
        interactKey = KeyCode.E;
    }
    void Update()
    {
        if (Input.GetKeyDown(interactKey) && canInteract)
        {
            playerController.SetWeaponEquiped(itemToEquip);
            objectToInteract.Interact();
            //itemToEquip.Interact();
           // UIManager.Instance.TextFadeOut();
        }
    }
    //public void ItemToEquip(IInteractable item)
    //{
    //    itemToEquip = item;
    //}
    public void ObjectToInteract(IInteractable _object, Weapon _itemToEquip)
    {
        objectToInteract = _object;
        itemToEquip = _itemToEquip;
    }
}
