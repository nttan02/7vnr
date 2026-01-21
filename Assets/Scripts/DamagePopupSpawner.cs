using UnityEngine;

public class DamagePopupSpawner : MonoBehaviour
{
    public static DamagePopupSpawner Instance;

    public GameObject damagePopupPrefab;
    public Canvas uiCanvas;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowDamage(int damage, Vector3 worldPos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        GameObject popup = Instantiate(damagePopupPrefab, uiCanvas.transform);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            uiCanvas.transform as RectTransform,
            screenPos,
            uiCanvas.worldCamera,
            out Vector2 localPos
        );

        popup.GetComponent<RectTransform>().localPosition = localPos;

        popup.GetComponent<DamagePopup>().Setup(damage);
    }

}
