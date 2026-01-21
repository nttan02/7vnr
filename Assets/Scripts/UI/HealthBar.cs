using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private float min;
    [SerializeField] private Transform bar;
    private float ratePercent;
    private Enemy target;
    private Vector3 offset;

    void Awake()
    {
        ratePercent = min;
    }

    void LateUpdate()
    {
        if (target == null || target.IsDead)
        {
            ClearTarget();
            return;
        }

        transform.position = target.transform.position + offset;
    }


    public void UpdateFill(long currentHp, long hpMax)
    {
        float value = Mathf.Clamp01((float)currentHp / hpMax);
        ratePercent = -((value - 1) * min);
        bar.transform.localPosition = new Vector2(ratePercent, 0);
    }
    private void OnHpChanged(int currentHp, int maxHp)
    {
        UpdateFill(currentHp, maxHp);
    }

    public void SetTarget(Enemy enemy)
    {
        if (target != null)
            target.OnHpChanged -= OnHpChanged;

        target = enemy;

        if (target == null) return;

        target.OnHpChanged += OnHpChanged;

        transform.SetParent(null);

        offset = new Vector3(0, enemy.GetTopCollider(), 0);
        transform.position = enemy.transform.position + offset;

        gameObject.SetActive(true);

        UpdateFill(enemy.hp, enemy.maxHp);
    }


    public void ClearTarget()
    {
        if (target != null)
            target.OnHpChanged -= OnHpChanged;

        target = null;
        gameObject.SetActive(false);
    }

}