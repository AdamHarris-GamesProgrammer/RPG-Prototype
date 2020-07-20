using UnityEngine;
using UnityEngine.UI;

using RPG.Resources;

[RequireComponent(typeof(Health))]
public class HealthBar : MonoBehaviour
{
    [SerializeField] Image healthBar = null;
    Health healthComponent;
    [SerializeField] GameObject GUI = null;

    private void Awake()
    {
        healthComponent = GetComponent<Health>();
        healthComponent.onHealthChanged += UpdateBar;
    }

    public void UpdateBar()
    {
        if (healthComponent.isDead)
        {
            GUI.SetActive(false);
            return;
        }

        healthBar.fillAmount = healthComponent.GetHealthPercentage();

    }
}
