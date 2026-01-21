using UnityEngine;
using UnityEngine.UI;

public class CharacterInforUI : MonoBehaviour
{
    public Text PowerText;
    public Text Hptext;
    public Text Mptext;
    public Image hpFillImage;
    private float displayFill;
    private float targetFill;
    public Image mpFillImage;
    private float displayMpFill;
    private float targetMpFill;
    private float smoothSpeed = 10f;
    public static CharacterInforUI instance;
    public Character currentCharacter;

    private void Awake()
    {
        instance = this;
    }
    public static void Show(Character character)
    {
        if (instance == null)
        {
            Debug.LogError("CharacterInforUI not found!");
            return;
        }

        instance.ShowInfo(character);
    }

    public void ShowInfo(Character character)
    {
        if (currentCharacter != null)
            currentCharacter.OnStatChanged -= UpdateUI;

        currentCharacter = character;
        currentCharacter.OnStatChanged += UpdateUI;

        displayFill = (float)character.hp / character.hpMax;
        targetFill = displayFill;
        hpFillImage.fillAmount = displayFill;

        displayMpFill = (float)character.mp / character.mpMax;
        targetMpFill = displayMpFill;
        mpFillImage.fillAmount = displayMpFill;

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (currentCharacter == null) return;
        PowerText.text = $"Sức Mạnh: {currentCharacter.power.ToString("N0")}";
        displayFill = Mathf.Lerp(displayFill, targetFill, Time.deltaTime * smoothSpeed);
        Hptext.text = $"HP: {currentCharacter.hp.ToString("N0")}/{currentCharacter.hpMax.ToString("N0")}";
        displayMpFill = Mathf.Lerp(displayMpFill, targetMpFill, Time.deltaTime * smoothSpeed);
        Mptext.text = $"MP: {currentCharacter.mp.ToString("N0")}/{currentCharacter.mpMax}";
    }
}