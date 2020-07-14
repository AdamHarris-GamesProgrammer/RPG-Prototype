using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Control;

public class Pickup : MonoBehaviour
{
    [SerializeField] private Weapon pickup = null;
    [SerializeField] private float respawnTimer;

    bool playerInRange = false;

    private void Update()
    {
        if(playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            playerInRange = false;
            Debug.Log(gameObject.name + " is calling fighter.equipweapon(" + pickup.name + ")");
            FindObjectOfType<PlayerController>().gameObject.GetComponent<Fighter>().EquipWeapon(pickup);

            StartCoroutine(HideForSeconds(respawnTimer));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
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