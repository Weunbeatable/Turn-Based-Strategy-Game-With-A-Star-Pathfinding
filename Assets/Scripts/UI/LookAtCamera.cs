using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private bool invert;
    private Transform cameraTransform;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (invert)
        {
            //direction to camera
            Vector3 directionToCamera = (cameraTransform.position - transform.position).normalized;
            transform.LookAt( transform.position + directionToCamera * -1);// look at camera and invert the direction to look the other way so we can see values properly
        }
        else
        {
            transform.LookAt(cameraTransform);
        }
        
    }
}
