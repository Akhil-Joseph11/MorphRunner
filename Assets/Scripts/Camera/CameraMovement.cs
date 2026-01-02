using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform player; 
    public float fixedScreenY = -4f;

    void LateUpdate()
    {
        if (!player) return;

        float camY = player.position.y - fixedScreenY;
        transform.position = new Vector3(transform.position.x, camY, transform.position.z);
    }
}
