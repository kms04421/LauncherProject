using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform target;


    private Vector2 _input ;

    [SerializeField] private MouseSensitibity mouseSensitivity;

    private CameraRotation _cameraRotation;

    [SerializeField] private CameraAngle cameraAngle;


    public void Look(InputAction.CallbackContext context)
    {
        _input = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        _cameraRotation.Yam += _input.x * mouseSensitivity.horizontal *  Time.deltaTime;
        _cameraRotation.Pitch += _input.y * -mouseSensitivity.vertical *  Time.deltaTime;
        _cameraRotation.Pitch = Mathf.Clamp(_cameraRotation.Pitch, cameraAngle.min, cameraAngle.max);
    }

    private void LateUpdate()
    {
        transform.eulerAngles = new Vector3(_cameraRotation.Pitch, _cameraRotation.Yam, 0.0f);
    }

    //private static int BoolToInt(bool b) => b ? 1 : -1;

}

[Serializable]
public struct MouseSensitibity
{
    public float horizontal;
    public float vertical;

}

[Serializable]
public struct CameraAngle
{
    public float min;
    public float max;
}

public struct CameraRotation
{
    public float Pitch;
    public float Yam;
}

