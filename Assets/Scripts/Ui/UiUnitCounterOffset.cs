using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiUnitCounterOffset : MonoBehaviour
{
    [SerializeField] private Transform _parentTransform;
    [SerializeField] private Vector3 _offset;

    private Camera _camera;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
        _camera = Camera.main;
    }

    private void Update()
    {
        var newPosition = _parentTransform.position;
        newPosition += _offset.x * _parentTransform.forward;
        newPosition += _offset.y * Vector3.up;

        var directionToCamera = _camera.transform.position - newPosition;
        directionToCamera.y = 0;
        directionToCamera.x = 0;
        directionToCamera.Normalize();
            
        newPosition += _offset.z * directionToCamera;

        _transform.position = newPosition;
    }
}
