﻿using System.Collections;
using UnityEngine;

using RPG.Combat;
using RPG.Control;

public class Pickup : MonoBehaviour, IInteractive
{
    [SerializeField] private Weapon pickup = null;
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
            FindObjectOfType<PlayerController>().gameObject.GetComponent<Fighter>().EquipWeapon(pickup);

            StartCoroutine(HideForSeconds(respawnTimer));
        }
    }

    public void ShowUI(bool isActive)
    {
        interactImage.SetActive(isActive);
    }
}