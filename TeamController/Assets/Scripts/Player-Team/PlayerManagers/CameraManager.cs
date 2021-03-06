using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook mainCam;
    [SerializeField] private CinemachineVirtualCamera aimCam;
    private FreeLookAddOn mainCamAddOn;

    void Awake()
    {
        mainCamAddOn = mainCam.GetComponent<FreeLookAddOn>();
    }

    private Target currentTarget;
    public Target CurrentTarget
    {
        get { return currentTarget; }
        set { currentTarget = value; }
    }

    private bool thisIsLeader;
    public bool ThisIsLeader
    {
        get { return thisIsLeader; }
        set { thisIsLeader = value; }
    } 

    public void toggleMainCam(bool _on)
    {
        if (_on)
        {
            mainCam.m_Priority = 20;
        }
        else
        {
            mainCam.m_Priority = 8;
        }
    } 

    private void Update()
    {
        if (ThisIsLeader)
        {
            mainCamAddOn.IsLeaderCam = true;
            if (TargetManager.instance.isLockingOn)
            {
                aimCam.m_Priority = 25;
                aimCam.LookAt = CurrentTarget.transform;
                if(aimCam.LookAt == null)
                {
                    TargetManager.instance.isLockingOn = false;
                }
                toggleMainCam(false);
            }
            else
            {
                toggleMainCam(true);
                aimCam.m_Priority = 8;
                aimCam.LookAt = null;
            }
        }
        else
        {
            aimCam.m_Priority = 8;
            //Debug.Log(transform.name + " is a leader is " + isLeaderCam);
            mainCamAddOn.IsLeaderCam = false;

            toggleMainCam(false);
        }
    }
}
