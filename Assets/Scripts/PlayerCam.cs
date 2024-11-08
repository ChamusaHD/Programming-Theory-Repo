using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [SerializeField] private float senseX;
    [SerializeField] private float senseY;
    [SerializeField] private float interactRange = 5f;

    [SerializeField] private Transform orientation;

    private float xRotation;
    private float yRotation;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * senseX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * senseY;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -60f, 60f);
        
        transform.root.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    //public bool CheckForInteraction(IInteractable objectToInteract)
    //{
    //    RaycastHit hit;
    //    Debug.DrawRay(transform.position, transform.forward * interactRange, Color.red);
    //    if (Physics.Raycast(transform.position, transform.forward, out hit, interactRange))
    //    {
    //        // Check if the object hit has an IInteractable component
    //        IInteractable interactable = hit.collider.GetComponent<IInteractable>();
    //        if (interactable != null)
    //        {
    //            // Call Interact on the interactable object
    //            Debug.DrawRay(transform.position, transform.forward * interactRange, Color.green);
    //            interactable.Interact();
    //            return true;

    //        }
    //    }
    //}

}
