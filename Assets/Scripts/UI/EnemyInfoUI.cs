using UnityEngine;
using UnityEngine.UI;

public class EnemyInfoUI : MonoBehaviour
{
    public Text nameText;
    public Text hpText;

    public Image hpFillImage;
    private float displayFill;
    private float targetFill;

    private float displayHp;
    private float targetHp;
    private float displayMaxHp;
    private float smoothSpeed = 10f;

    private static EnemyInfoUI instance;
    private Enemy currentEnemy;

    public static void Show(Enemy enemy)
    {
        if (instance == null)
        {
            instance = FindObjectOfType<EnemyInfoUI>(includeInactive: true);
            if (instance == null) return;
        }
        instance.ShowInfo(enemy);
    }

    public void ShowInfo(Enemy enemy)
    {
        if (currentEnemy != null)
            currentEnemy.OnHpChanged -= UpdateHp;

        currentEnemy = enemy;
        if (currentEnemy == null) return;

        currentEnemy.OnHpChanged += UpdateHp;

        nameText.text = enemy.data.name;

        displayHp = enemy.hp;
        targetHp = enemy.hp;
        displayMaxHp = enemy.maxHp;

        displayFill = (float)enemy.hp / enemy.maxHp;
        targetFill = displayFill;
        hpFillImage.fillAmount = displayFill;

        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (currentEnemy == null) return;

        if (currentEnemy.IsDead)
        {
            HideInfo();
            return;
        }

        displayHp = Mathf.Lerp(displayHp, targetHp, Time.deltaTime * smoothSpeed);
        displayFill = Mathf.Lerp(displayFill, targetFill, Time.deltaTime * smoothSpeed);

        hpText.text = $"HP: {Mathf.CeilToInt(displayHp)}/{displayMaxHp}";
        hpFillImage.fillAmount = displayFill;
    }

    private void UpdateHp(int currentHp, int maxHp)
    {
        targetHp = currentHp;
        displayMaxHp = maxHp;
        targetFill = (float)currentHp / maxHp;
    }

    public void HideInfo()
    {
        if (currentEnemy != null)
            currentEnemy.OnHpChanged -= UpdateHp;

        currentEnemy = null;
        gameObject.SetActive(false);
    }
}
