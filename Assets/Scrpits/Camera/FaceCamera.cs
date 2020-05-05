using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform cameraPosition;

    private void Start()
    {
        cameraPosition = CameraController.cameraInstance.transform;
    }

    private void FixedUpdate()
    {
        this.transform.LookAt(cameraPosition);
        this.transform.Rotate(new Vector3(0, 180, 0), Space.Self);
    }
}
