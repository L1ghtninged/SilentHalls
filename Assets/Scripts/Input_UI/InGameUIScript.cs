using UnityEngine;
using UnityEngine.UI;

public class InGameUIScript : MonoBehaviour
{
    private StatScript playerStats;

    public Slider healthSlider;
    public Slider manaSlider;
    public Slider expSlider;
    public Slider staminaSlider;

    void Start()
    {
        playerStats = FindObjectOfType<StatScript>();
        
        playerStats.Health.OnValueChanged += UpdateHealthBar;
        playerStats.Mana.OnValueChanged += UpdateManaBar;
        playerStats.Stamina.OnValueChanged += UpdateStaminaBar;
        playerStats.Experience.OnValueChanged += UpdateExpBar;

        healthSlider.value = healthSlider.maxValue;
        manaSlider.value = manaSlider.maxValue;
        staminaSlider.value = staminaSlider.maxValue;
        expSlider.value = 0;
    }

    void UpdateHealthBar(int current, int max)
    {
        healthSlider.maxValue = max;
        healthSlider.value = current;
    }

    void UpdateManaBar(int current, int max)
    {
        manaSlider.maxValue = max;
        manaSlider.value = current;
    }

    void UpdateStaminaBar(int current, int max)
    {
        staminaSlider.maxValue = max;
        staminaSlider.value = current;
    }

    void UpdateExpBar(int current, int max)
    {
        expSlider.maxValue = max;
        expSlider.value = current;
    }
}