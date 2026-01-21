using UnityEngine;
using UnityEngine.UI;

public class DamagePopup : MonoBehaviour
{
    public float moveUpSpeed = 40f;
    public float lifeTime = 0.8f;

    private Text text;
    private Color startColor;

    private void Awake()
    {
        text = GetComponent<Text>();
        startColor = text.color;
    }

    public void Setup(int damage)
    {
        text.text = damage.ToString();
        Invoke(nameof(DestroySelf), lifeTime);
    }

    private void Update()
    {
        transform.Translate(Vector3.up * moveUpSpeed * Time.deltaTime);

        startColor.a -= Time.deltaTime / lifeTime;
        text.color = startColor;
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
