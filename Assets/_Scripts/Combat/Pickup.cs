using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Combat;
using RPG.Control;

public class Pickup : MonoBehaviour
{
    [SerializeField] private Weapon pickup = null;
    [SerializeField] private float respawnTimer;

    [SerializeField] GameObject interactImage;

    bool playerInRange = false;

    private void Awake()
    {
    }

    private void Update()
    {
        if(playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            playerInRange = false;
            interactImage.SetActive(false);
            FindObjectOfType<PlayerController>().gameObject.GetComponent<Fighter>().EquipWeapon(pickup);

            StartCoroutine(HideForSeconds(respawnTimer));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
            interactImage.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
            interactImage.SetActive(false);
        }
    }

    private IEnumerator HideForSeconds(float seconds)
    {
        GetComponent<Collider>().enabled = false;
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(seconds);

        GetComponent<Collider>().enabled = true;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }

    }
}