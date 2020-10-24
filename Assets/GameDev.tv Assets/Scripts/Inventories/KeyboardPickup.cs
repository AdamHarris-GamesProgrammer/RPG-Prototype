using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    [RequireComponent(typeof(Pickup))]
    public class KeyboardPickup : MonoBehaviour, IInteractive
    {
        Pickup pickup;

        public void Interact()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                pickup.PickupItem();
            }
        }

        public void ShowUI(bool isActive)
        {

        }//TODO: Add ui prompt

        void Awake()
        {
            pickup = GetComponent<Pickup>();
        }
    }
}

