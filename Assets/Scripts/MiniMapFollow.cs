using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    public float zOffset = -10f;

    Camera miniCam;

    float halfWidth;
    float halfHeight;

    void Start()
    {
        miniCam = GetComponent<Camera>();

        halfHeight = miniCam.orthographicSize;
        halfWidth = halfHeight * miniCam.aspect;
    }

    void LateUpdate()
    {
        if (CameraFollowBounds.Instance == null)
            return;
        float minX = CameraFollowBounds.Instance.minX;
        float maxX = CameraFollowBounds.Instance.maxX;
        float minY = CameraFollowBounds.Instance.minY;
        float maxY = CameraFollowBounds.Instance.maxY;

        Transform player = CameraFollowBounds.Instance.target;

        if (player == null) return;

        float clampX = Mathf.Clamp(player.position.x, minX + halfWidth, maxX - halfWidth);
        float clampY = Mathf.Clamp(player.position.y + 3f, minY + halfHeight, maxY - halfHeight);

        transform.position = new Vector3(clampX, clampY, zOffset);
    }
}
