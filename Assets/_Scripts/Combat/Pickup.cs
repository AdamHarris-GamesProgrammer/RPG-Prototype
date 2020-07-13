using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;

public class Pickup : MonoBehaviour
{
    [SerializeField] private Weapon pickup = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Fighter>().EquipWeapon(pickup);

            Destroy(this.gameObject);
        }
    }
}
