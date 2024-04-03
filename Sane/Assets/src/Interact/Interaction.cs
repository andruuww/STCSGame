using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IInteractable
{
    public void Interact();
}
public class Interaction : MonoBehaviour
{
    public Transform InteractorSource;
    public float InteractRange;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray r = new Ray(InteractorSourceSrouce.position, InteractorSource.forward);
            if (Physics.Raycast(r, out RaycastHit hitInfo, InteractorSourceRange))
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteratctable interactObj))
                {
                    interactObj.Obj.Interact();
                }    
            }
        }
    }
}
