using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control 
{
    public class AddCombatAI : MonoBehaviour
    {
        PlayerController playerController;

        private void Awake()
        {
            playerController = gameObject.GetComponentInParent<PlayerController>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Enemy")) return;

            playerController.AddAI(other.gameObject.GetComponent<NPCController>());
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag("Enemy")) return;

            playerController.RemoveAI(other.gameObject.GetComponent<NPCController>());
        }
    }
}

