using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class TeamController : MonoBehaviour
{
    [SerializeField] private int characterIndex;
    //Store character variables & components
    [SerializeField] private List<CharacterState> characterStates;
    [SerializeField] private Transform currentTeamLeader;

    [SerializeField] private Transform member1;
    [SerializeField] private Transform member2;

    private bool isLeaderSprinting;
    private bool hasLeaderMovedPastThreshold;

    void Start()
    {
        currentTeamLeader = characterStates[characterIndex].transform; //stores starting values
        member1 = characterStates[1].transform;
        member2 = characterStates[2].transform;
        toggleCharacterComponents(characterStates[characterIndex]); //Assigns current leader and AI

        Invoke("UpdateMembersOfLeader", .5f);

    }

    public GameObject returnCurrentTeamLeader()
    {
        GameObject thisCurrentLeader = currentTeamLeader.gameObject;
        return thisCurrentLeader;
    }
    
    public void OnSwapCharacter(InputAction.CallbackContext value) //Button press
    {
        if (value.started)
        {
            swapCharacter();
        }
    }

    void Update()
    {
        passInfoToMembers();
    }

    private void toggleCharacterComponents(CharacterState thisCharacter) //enables player components on the leader, enables AI components on the other members. Disables corresponding conponents
    {
        thisCharacter.toggleState(false); //Enable components on the newly selected character      
        thisCharacter.memberID = 0;

        if(thisCharacter == characterStates[0]) //If we are switching to character_1
        {
            member1 = characterStates[1].transform;
            characterStates[1].toggleState(true); //Set character 1 and 3 to AI state        
            characterStates[1].memberID = 1;

            member2 = characterStates[2].transform;
            characterStates[2].toggleState(true);
            characterStates[2].memberID = 2;
        }
        else if(thisCharacter == characterStates[1]) //If we are switching to character_2
        {
            member1 = characterStates[0].transform;
            characterStates[0].toggleState(true); //Set character 2 and 3 to AI state 
            characterStates[0].memberID = 1;

            member2 = characterStates[2].transform;
            characterStates[2].toggleState(true);
            characterStates[2].memberID = 2;

        }
        else if (thisCharacter == characterStates[2]) //If we are switching to character_3
        {
            member1 = characterStates[0].transform;
            characterStates[0].toggleState(true); //Set character 1 and 2 to AI state 
            characterStates[0].memberID = 1;

            member2 = characterStates[1].transform;
            characterStates[1].toggleState(true);
            characterStates[1].memberID = 2;
        }
    }

    private void swapCharacter()
    {
        if(characterIndex >= 2) //If the character index exceeds team value
        {
            characterIndex = 0; //Reset
        }
        else 
        {
            characterIndex++; //If it has not exceeded team member ammount, increment
        }

        toggleCharacterComponents(characterStates[characterIndex]); //Sets the current leader based on the character index
        currentTeamLeader = characterStates[characterIndex].transform;
        TargetManager.instance.clearList();
        TargetManager.instance.toggleLockOnUI(false);

        UpdateMembersOfLeader();
    }

    private void passInfoToMembers() //Gets information from leader and informs AI members what the leader is doing
    {
        PlayerMovement currentLeader = currentTeamLeader.GetComponent<PlayerMovement>(); //Gets the playerMovement class on the current leader
        CameraManager currentLeaderCamManager = currentTeamLeader.GetComponent<CameraManager>();

        if (currentLeader != null)
        {
            isLeaderSprinting = currentLeader.isSprinting; //Sets the local bool to true or false depending on the teamLeaders isSpritning bool
            hasLeaderMovedPastThreshold = currentLeader.movedBeyondThreshold; //Same as above relative to this bool

            TargetManager.instance.target = currentLeaderCamManager.CurrentTarget;
        }

        foreach (CharacterState state in characterStates) 
        {
            if (state.transform != currentTeamLeader) //Checks which of the members in the list are current AI/Not the team leader
            {
                TeamAIMovement thisAI = state.GetComponent<TeamAIMovement>();
                if (thisAI != null)
                {
                    thisAI.leaderSprinting = isLeaderSprinting; //Sets the bool within each respective TeamAIMovement class to true or false depending on the local bool variable "isLeaderSprinting"
                    thisAI.leaderMovedPastThreshold = hasLeaderMovedPastThreshold; //Same as above relative to this bool
                }

            }
        }
    }

    private void UpdateMembersOfLeader()
    {
        foreach (CharacterState state in characterStates) //Informs members of the new leader
        {
            state.updateLeader(currentTeamLeader);
        }
    }
}
