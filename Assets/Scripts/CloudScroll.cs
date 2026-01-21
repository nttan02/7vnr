using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudScroll : MonoBehaviour
{
    public float scrollSpeed = 0.5f;
    private Material mat;
    private Vector2 offset;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        offset = mat.mainTextureOffset;
    }

    void Update()
    {
        offset.x += scrollSpeed * Time.deltaTime;

        if (offset.x > 1f)
            offset.x -= 1f;
        else if (offset.x < 0f)
            offset.x += 1f;

        mat.mainTextureOffset = offset;
    }
}
