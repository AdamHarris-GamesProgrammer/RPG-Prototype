using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;

public class Pickup : MonoBehaviour
{
    [SerializeField] private Weapon pickup = null;
    [SerializeField] private float respawnTimer;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Fighter>().EquipWeapon(pickup);

            StartCoroutine(HideForSeconds(respawnTimer));
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
