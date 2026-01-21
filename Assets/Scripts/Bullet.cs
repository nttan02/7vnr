using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private Rigidbody2D rb;

    [Header("Arc Settings")]
    public float arcHeight = 2f;
    public float travelDistance = 10f;

    private float timeAlive = 0f;
    private float startTimeOffset = 0f;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float journeyProgress = 0f;
    private bool isMoving = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.isKinematic = true;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
    }

    public void SetDirectionAndSpeed(Vector3 dir, float spd)
    {
        if (dir.sqrMagnitude < 0.01f) // Threshold nhỏ hơn
        {
            direction = Vector3.right;
        }
        else
        {
            direction = dir.normalized;
        }
        speed = spd;
    }

    public void SetStartTime(float offset)
    {
        startTimeOffset = offset;
        timeAlive = -offset;
    }

    private void Update()
    {
        timeAlive += Time.deltaTime;

        if (timeAlive >= 3f)
        {
            Destroy(gameObject);
        }

        if (timeAlive < 0f)
        {
            return;
        }

        if (!isMoving)
        {
            startPosition = transform.position;
            targetPosition = startPosition + direction * travelDistance;
            isMoving = true;
        }

        journeyProgress += (speed / travelDistance) * Time.deltaTime;

        if (journeyProgress >= 1f)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 linearPosition = Vector3.Lerp(startPosition, targetPosition, journeyProgress);

        float arcOffset = -4 * arcHeight * Mathf.Pow(journeyProgress - 0.5f, 2) + arcHeight;

        Vector3 newPosition = linearPosition;
        newPosition.y += arcOffset;

        if (rb != null)
        {
            rb.MovePosition(newPosition);
        }
        else
        {
            transform.position = newPosition;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}