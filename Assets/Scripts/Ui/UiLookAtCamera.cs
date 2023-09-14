using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiLookAtCamera : MonoBehaviour
{
    private Transform _transform;
    private Camera _camera;
    
    private void Awake()
    {
        _transform = transform;
        _camera = Camera.main;
    }

    private void Update()
    {
        var cameraTransform = _camera.transform;
        _transform.LookAt(cameraTransform, cameraTransform.up);
        _transform.forward *= -1;
    }
}
