using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Combat;
using RPG.Control;

public class Pickup : MonoBehaviour, IInteractive
{
    [SerializeField] private Weapon pickup = null;
    [SerializeField] private float respawnTimer;

    [SerializeField] GameObject interactImage;

    private IEnumerator HideForSeconds(float seconds)
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(seconds);

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    //IInteractive interface implementation
    public void Interact()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            FindObjectOfType<PlayerController>().gameObject.GetComponent<Fighter>().EquipWeapon(pickup);

            StartCoroutine(HideForSeconds(respawnTimer));
        }
    }

    public void ShowUI(bool isActive)
    {
        interactImage.SetActive(isActive);
    }
}