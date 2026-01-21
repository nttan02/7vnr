using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Bullet bulletPrefab;
    public float bulletSpeed = 8f;
    public float spawnOffset = 0.5f;

    [Header("Multi Bullet Settings")]
    public int bulletCount = 5;
    public float delayBetweenBullets = 0.15f;

    [Header("Arc Pattern Settings")]
    public float travelDistance = 10f;
    public float minArcHeight = 0f;
    public float maxArcHeight = 3f;

    public bool randomPattern = false;

    public void Shoot()
    {
        StartCoroutine(ShootMultipleBullets());
    }
    private Transform FindNearestEnemy(Vector3 fromPos)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform nearest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(fromPos, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = enemy.transform;
            }
        }

        return nearest;
    }

    private IEnumerator ShootMultipleBullets()
    {
        Character character = Character.Instance;
        if (character == null)
        {
            Debug.LogError("Character.Instance is null!");
            yield break;
        }

        // int dir = character.dir;
        // Vector3 shootDir = new Vector3(dir, 0, 0);
        Vector3 characterPos = character.transform.position;

        Transform target = FindNearestEnemy(characterPos);

        Vector3 shootDir;
        if (target != null)
        {
            shootDir = (target.position - characterPos).normalized;
        }
        else
        {
            shootDir = new Vector3(character.dir, 0, 0);
        }


        for (int i = 0; i < bulletCount; i++)
        {
            Vector3 spawnPos = characterPos + shootDir * spawnOffset;
            spawnPos.y += spawnOffset;

            Bullet bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
            bullet.SetDirectionAndSpeed(shootDir, bulletSpeed);

            bullet.travelDistance = travelDistance;

            if (randomPattern)
            {
                bullet.arcHeight = Random.Range(minArcHeight, maxArcHeight);
            }
            else
            {
                float t = (float)i / Mathf.Max(1, bulletCount - 1);
                bullet.arcHeight = Mathf.Lerp(minArcHeight, maxArcHeight, t);
            }

            bullet.SetStartTime(i * delayBetweenBullets);
        }

        yield return null;
    }
}