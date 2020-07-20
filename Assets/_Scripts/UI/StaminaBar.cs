using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] Image staminaBar = null;

    public void UpdateBar(float stamina, float maxStamina)
    {
        staminaBar.fillAmount = stamina / maxStamina;
    }
}
