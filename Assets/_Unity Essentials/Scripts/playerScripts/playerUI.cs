using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class playerUI : MonoBehaviour
{
    public Slider healthBar;
    public Slider dashBar;
    public TMP_Text healthNum;
    public TMP_Text ammoText;

    public void SetUIHealth(int current, int max)
    {
        healthBar.maxValue = max;
        healthBar.value = current;
        healthNum.text = current.ToString() + "/" + max.ToString();
    }

    public void SetUIDash(int current, int max)
    {
        dashBar.maxValue = max;
        dashBar.value = current;
    }

    public void SetAmmo(int current, int max)
    {
        ammoText.text = $"{current}/{max}";
    }
}