using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;
using UnityEngine.InputSystem;
public class CharacterState : MonoBehaviour
{
    public int memberID;
    private currentControlType thisCurrentControl;
    public characterType thisCharacterType;
    public BaseCharacter thisCharacterStats;

    [Header("References")]
    public CameraManager thisCamManger;
    public ReadInput thisReadInput;
    public AnimHook thisAnimHook;
    public NearbyTargets thisNearbyTargets;
    public TeamPositions thisNavTarget;
    public CharacterController thisController;
    public PlayerMovement thisPlayerMovement;
    public PlayerCombat thisPlayerCombat;
    public TeamAIMovement thisTeamMovement;
    public NavMeshAgent thisAgent;

    public currentControlType thisControlType
    {
        get
        {
            return thisCurrentControl;
        }
    }

    #region getter and setters
    private bool isMoving;
    public bool IsMoving()
    {
        if (thisCurrentControl == currentControlType.Player_Driven)
        {
            isMoving = thisPlayerMovement.isMoving;
        }
        else
        {
            return false;
        }

        return isMoving;
    }

    private bool sprinting;
    public bool isThisCharacterSprinting()
    {
        if (thisControlType == currentControlType.Player_Driven)
        {
            sprinting = thisPlayerMovement.isSprinting;
        }
        else
        {
            return false;
        }

        return sprinting;
    }

    private bool usingAlternateAction;
    public bool isThisCharacterUsingAltAction()
    {
        if(thisCurrentControl == currentControlType.Player_Driven)
        {
            usingAlternateAction = thisPlayerCombat.isUsingAlternateAction;
        }
        else
        {
            return false;
        }

        return usingAlternateAction;
    }
    #endregion

    public void toggleState(bool switchToAI) //Toggles character state between AI controlled/player controlled
    {
        if (switchToAI) //If this is AI
        {
            thisCamManger.ThisIsLeader = false;

            thisCurrentControl = currentControlType.AI_Driven; //Current state is AI_Driven
            thisController.enabled = false; //Disable components which relate to player controlled movement
            thisPlayerMovement.enabled = false;
            thisPlayerCombat.enabled = false;
            thisReadInput.enabled = false;

            thisTeamMovement.enabled = true; //Active AI components
            thisTeamMovement.AI_Active = true; 
            thisAgent.enabled = true;
        }
        else
        {
            thisCamManger.ThisIsLeader = true;

            thisCurrentControl = currentControlType.Player_Driven; //Current state is Player_Driven
            thisAgent.enabled = false; //Disable AI components
            thisTeamMovement.enabled = false;
            thisTeamMovement.AI_Active = false;

            thisReadInput.enabled = true;
            thisController.enabled = true; //Activate components related to player movement
            thisPlayerMovement.enabled = true;
            thisPlayerCombat.enabled = true;
        }
    }

    public void updateLeader(Transform currentLeader) //Assigns current Leaders to member 1 and 2
    {
        thisNavTarget = currentLeader.GetComponentInChildren<TeamPositions>(); //Stores the current navTarget of the current leader
        TargetManager.instance.resetLockOn();

        switch (memberID) //Depending on this characters member ID
        {
            //Assing the correct position
            case 0: //member ID 0 is always the player so assigning the targetPosition does not matter.
                thisTeamMovement.setPosition(thisNavTarget.pos1);
                break;
            case 1:
                thisTeamMovement.setPosition(thisNavTarget.pos1); //Member 1 will go to position 1
                break;
            case 2:
                thisTeamMovement.setPosition(thisNavTarget.pos2); //Member 2 will go to position 2
                break;
        }
    }
}

public enum currentControlType
{
    Player_Driven,
    AI_Driven
}

public enum characterType
{
    Knight,
    Archer,
    Mage
}
