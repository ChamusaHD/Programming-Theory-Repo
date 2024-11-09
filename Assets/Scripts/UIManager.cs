using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private TextMeshProUGUI textToDisplay;
    [SerializeField] private TextMeshProUGUI weponEquipedText;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI weponInfoText;
    [SerializeField] private TextMeshProUGUI rangeText;
    [SerializeField] private TextMeshProUGUI fireRateText;
    [SerializeField] private TextMeshProUGUI damageText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        textToDisplay.gameObject.SetActive(false);
    }
    public void DisplayWeaponEquipedText(string text)
    {
        weponEquipedText.text = text;
    }
    public void DisplayWeaponInfo(string weaponName, int ammo, float range, float fireRate, float damage)
    {
        weaponNameText.text = "Weapon: " + weaponName;
        ammoText.text = "Ammo: " + ammo;
        rangeText.text = "Range: " + range.ToString("F1");  // Limit to 1 decimal place
        fireRateText.text = "Fire Rate: " + fireRate.ToString("F1");
        damageText.text = "Damage: " + damage.ToString("F1");
    }

    public void DisplayText(string text)
    {
        textToDisplay.text = text;
        textToDisplay.color = new Color(1f, 1f, 1f, 1f); // reset alpha
        textToDisplay.gameObject.SetActive(true);
        //StartCoroutine(TextFadeOut());
    }
    public IEnumerator TextFadeOut()
    {
        float alpha = 1f;

        while (alpha > 0f)
        {
            alpha -= Time.deltaTime;
            textToDisplay.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        textToDisplay.text = "";
        textToDisplay.gameObject.SetActive(false);
    }
}
