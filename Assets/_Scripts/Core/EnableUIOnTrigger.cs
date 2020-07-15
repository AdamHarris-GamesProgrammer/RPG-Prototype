using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableUIOnTrigger : MonoBehaviour
{
    bool inRange = false;

    IInteractive interactable;

    private void Start()
    {
        interactable = GetComponent<IInteractive>();
    }


    private void Update()
    {
        if (inRange)
        {
            interactable.Interact();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inRange = true;
            interactable.ShowUI(inRange);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inRange = false;
            interactable.ShowUI(inRange);
        }
    }
}
