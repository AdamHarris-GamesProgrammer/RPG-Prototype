using UnityEngine;

public class EnableUIOnTrigger : MonoBehaviour
{
    bool inRange = false;

    IInteractive interactable;

    private void Start()
    {
        interactable = GetComponent<IInteractive>();
        if(interactable == null)
        {
            Debug.LogError(gameObject.name + " is missing a component that implements IInteractive");
        }
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
