using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class TargetManager : MonoBehaviour
{
    public static TargetManager instance;
    [SerializeField] private TeamController teamControl;
    [SerializeField] private CharacterState currentLeaderState;

    private Vector2 lockOnIconPos;
    [SerializeField] private Camera mainCam;
    [SerializeField] private Image lockOnIcon;
    [SerializeField] private TextMeshProUGUI targetDistanceText;

    public bool isLockingOn;
    public Vector2 mouseValue { get; set; }

    private bool targetSwitchSet;
    public List<Target> nearbyEnemies;
    private int currentTargetIndex;    

    public Target target;  

    void Awake()
    {
        instance = this;

        teamControl = GetComponent<TeamController>();
    }

    void Start()
    {
        lockOnIcon.enabled = false;
        targetDistanceText.enabled = false;
    }

    //Toggle Lock on Icon UI
    public void toggleLockOnUI(bool _toggleOn)
    {
        if (!_toggleOn)
        {
            lockOnIcon.enabled = false;
            targetDistanceText.enabled = false;
        }
        else
        {
            lockOnIcon.enabled = true;
            targetDistanceText.enabled = true;

            //lockOnIcon.color = target.TargetColor;           
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentLeaderState = teamControl.returnCurrentTeamLeader().GetComponent<CharacterState>();

        if (isLockingOn)
        {
            if(target == null)
            {
                onTargetLost();
            }

            //Positions Icon UI over the target positon realtive to its screen to world space
            lockOnIconPos = mainCam.WorldToScreenPoint(target.transform.position);
            lockOnIcon.transform.position = lockOnIconPos;

            if (target.NeedDistanceText)
            {
                targetDistanceText.text = Mathf.RoundToInt(target.GetDistanceFromCamera(mainCam.transform.position)).ToString() + " m";
            }
            else
            {
                targetDistanceText.text = "";
            }
        }

        CheckEnemyListState();
        OnAllTargetsRemoved();

        toggleBetweenTargets();
        setCorrectCameraTarget();
    }

    private IEnumerator delayTargetSwitch()
    {
        yield return new WaitForSeconds(.12f);
        targetSwitchSet = false;
    }

    public void storeEnemyInList(Target _thisTarget)
    {
        nearbyEnemies.Add(_thisTarget);

        //sortTargetList();
    }

    public void removeEnemyFromList(Target _thisTarget)
    {
        nearbyEnemies.Remove(_thisTarget);

        if (currentTargetIndex == 0 && nearbyEnemies.Count > 0)
        {
            currentTargetIndex = nearbyEnemies.Count - 1;
        }
        else
        {
            if (nearbyEnemies.Count > 0)
            {
                currentTargetIndex--;
            }
        }       
    }
    private void onTargetLost()
    {
        target = nearbyEnemies[Random.Range(0, nearbyEnemies.Count)];
        setCorrectCameraTarget();
        currentLeaderState.thisPlayerCombat.setNewTarget(target);
    }

    public void clearList()
    {
        currentLeaderState.thisPlayerCombat.setNewTarget(null);
        nearbyEnemies.Clear();
    }

    private void CheckEnemyListState()
    {
        if (nearbyEnemies.Count > 0)
        {
            target = nearbyEnemies[currentTargetIndex];
        }
        else
        {
            target = null;
        }
    }

    private void OnAllTargetsRemoved()
    {
        if (isLockingOn && nearbyEnemies.Count == 0)
        {
            target = null;
            isLockingOn = false;
            resetLockOn();
            TargetManager.instance.toggleLockOnUI(false);

            return;
        }
    }

    private void setCorrectCameraTarget()
    {
        //Checks if the correct target is set on the cameraManager
        if (currentLeaderState.thisCamManger.CurrentTarget != target)
        {
            currentLeaderState.thisPlayerCombat.setNewTarget(nearbyEnemies[currentTargetIndex]);
        }
    }

    private void toggleBetweenTargets()
    {            
        Vector2 normalizedValue = mouseValue;

        if (isLockingOn)
        {
            if (targetSwitchSet == false)
            {
                if (normalizedValue.x >= 10f)
                {
                    if (currentTargetIndex == nearbyEnemies.Count - 1)
                    {
                        currentTargetIndex = 0;
                    }
                    else
                    {
                        currentTargetIndex++;
                    }
                }
                else if (normalizedValue.x <= -10f)
                {
                    if (currentTargetIndex == 0)
                    {
                        currentTargetIndex = nearbyEnemies.Count - 1;
                    }
                    else
                    {
                        currentTargetIndex--;
                    }
                }

                StartCoroutine(delayTargetSwitch());

                targetSwitchSet = true;
            }            
        }
    }

    public void resetLockOn()
    {
        isLockingOn = false;
        currentLeaderState.thisCamManger.CurrentTarget = null;
    }
}
