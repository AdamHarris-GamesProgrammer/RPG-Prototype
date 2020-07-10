using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using RPG.Core;

[RequireComponent(typeof(Health))]
public class HealthBar : MonoBehaviour
{
    [SerializeField] Image healthBar = null;
    Health healthComponent;
    [SerializeField] GameObject GUI = null;

    private void Awake()
    {
        healthComponent = GetComponent<Health>();
    }

    void Update()
    {
        if (healthComponent.isDead)
        {
            GUI.SetActive(false);
            return;
        }

        healthBar.fillAmount = healthComponent.healthPoints / healthComponent.totalHealthPoints;

    }
}
