using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 cameraOffset = new Vector3(0f, 0f, -10f);
    public float zoomSpeed = 2f;
    public float minZoom = 5f;
    public float maxZoom = 10f;
    

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPos = target.position + cameraOffset;
            Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
            transform.position = smoothedPos;


            float zoomFactor = Mathf.Clamp(target.GetComponent<Rigidbody2D>().velocity.magnitude, 0f, 1f);
            float targetZoom = Mathf.Lerp(minZoom, maxZoom, zoomFactor);
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
        }
    }
}
