using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class FreeLookAddOn : MonoBehaviour
{
    private bool isLeaderCam;
    public bool isLockedOn;

    public bool IsLeaderCam
    {
        get
        {
            return isLeaderCam;
        }
        set
        {
            isLeaderCam = value;
        }
    }

    [Range(0f, .5f)] public float LookSpeedX = .07f;
    [Range(0f, .5f)] public float LookSpeedY = .1f;

    public bool InvertY = false;
    private CinemachineFreeLook _freeLookComponent;

    public void Start()
    {
        _freeLookComponent = GetComponent<CinemachineFreeLook>();
    }

    // Update the look movement each time the event is trigger
    public void OnLook(InputAction.CallbackContext context)
    {
        if (IsLeaderCam)
        {
            if (!isLockedOn)
            {
                //Normalize the vector to have an uniform vector in whichever form it came from (I.E Gamepad, mouse, etc)
                Vector2 lookMovement = context.ReadValue<Vector2>().normalized;
                lookMovement.y = InvertY ? -lookMovement.y : lookMovement.y;

                // This is because X axis is only contains between -180 and 180 instead of 0 and 1 like the Y axis
                lookMovement.x = lookMovement.x * 180f;

                //Ajust axis values using look speed and Time.deltaTime so the look doesn't go faster if there is more FPS
                _freeLookComponent.m_XAxis.Value += lookMovement.x * LookSpeedX * Time.fixedDeltaTime;
                _freeLookComponent.m_YAxis.Value += lookMovement.y * LookSpeedY * Time.fixedDeltaTime;
            }
            else
            {
                _freeLookComponent.m_XAxis.Value = 100f;
                _freeLookComponent.m_YAxis.Value = 0f;
            }
        }       
    }
}

