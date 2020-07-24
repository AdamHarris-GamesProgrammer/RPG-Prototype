using RPG.Control;
using UnityEngine;

public class EnableUIOnTrigger : MonoBehaviour
{
    bool inRange = false;


    PlayerController player = null;

    IInteractive interactable;

    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
    }


    private void Start()
    {
        interactable = GetComponent<IInteractive>();
        if (interactable == null)
        {
            Debug.LogError(gameObject.name + " is missing a component that implements IInteractive");
        }
    }


    private void Update()
    {
        if (inRange)
        {
            if (player.InCombat()) return;
            interactable.Interact();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (player.InCombat()) return;
        if (other.gameObject.CompareTag("Player"))
        {
            inRange = true;
            interactable.ShowUI(inRange);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (player.InCombat()) return;
        if (other.gameObject.CompareTag("Player"))
        {
            inRange = false;
            interactable.ShowUI(inRange);
        }
    }
}
