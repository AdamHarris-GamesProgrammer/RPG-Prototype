using UnityEngine;
using UnityEngine.UI;

using RPG.Resources;
using RPG.UI;

[RequireComponent(typeof(Health))]
public class HealthBar : MonoBehaviour
{
    [SerializeField] Image healthBar = null;
    Health healthComponent;
    [SerializeField] GameObject GUI = null;

    private void Awake()
    {
        healthComponent = GetComponent<Health>();
        healthComponent.OnHealthChanged += UpdateBar;

        ShowHideUI ui = GameObject.FindGameObjectWithTag("InventoryCanvas").GetComponent<ShowHideUI>();
        ui.showUI += ShowBar;
        ui.closeUI += CloseBar;
    }

    void ShowBar()
    {
        GUI.SetActive(true);
    }
    void CloseBar()
    {
        GUI.SetActive(false);
    }

    public void UpdateBar()
    {
        if (healthComponent.isDead)
        {
            GUI.SetActive(false);
            return;
        }
        else
        {
            healthBar.fillAmount = healthComponent.GetHealthPercentage();
        }


    }
}
