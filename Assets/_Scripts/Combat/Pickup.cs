using System.Collections;
using UnityEngine;

using RPG.Combat;
using RPG.Control;
using RPG.Resources;

public class Pickup : MonoBehaviour, IInteractive
{
    [SerializeField] private WeaponConfig pickup = null;
    [SerializeField] private float healthToRestore = 0.0f;
    [SerializeField] private float respawnTimer = 5.0f;

    [SerializeField] GameObject interactImage = null;

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
            if(pickup == null)
            {
                FindObjectOfType<PlayerController>().gameObject.GetComponent<Health>().FillHealth(healthToRestore);
            }
            else
            {
                FindObjectOfType<PlayerController>().gameObject.GetComponent<Fighter>().EquipWeapon(pickup);
            }

            StartCoroutine(HideForSeconds(respawnTimer));
        }
    }

    public void ShowUI(bool isActive)
    {
        interactImage.SetActive(isActive);
    }
}