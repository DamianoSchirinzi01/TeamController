using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TeamAIMovement : MonoBehaviour
{
    private CharacterState state;
    public bool AI_Active;
    [SerializeField] private NavMeshAgent agent;

    [SerializeField] private float targetSpeed;
    [SerializeField] private float currentSpeed;
    [SerializeField] Transform targetPos;
    [SerializeField] private float followThreshold;

    public bool leaderSprinting { private get; set; }  
    public bool leaderMovedPastThreshold { private get; set; }

    private void Awake()
    {
        state = GetComponent<CharacterState>();
    }

    private void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {     
        float distanceToLeader = Vector3.Distance(transform.position, targetPos.position);

        if (AI_Active)
        {
            if (agent.velocity.magnitude >= .1f)
            {
                state.thisAnimHook.setSpeed(agent.speed);

                if (leaderSprinting) //Checks whether leader is sprinting and sets the agents speed accordingly, information is passed by the TeamController class
                {
                    targetSpeed = state.thisCharacterStats.sprintSpeed;

                    float newSpeed = Utils.lerpValue(currentSpeed, targetSpeed, state.thisCharacterStats.speedGain * Time.deltaTime, true);
                    currentSpeed = newSpeed;

                    agent.speed = currentSpeed;
                }
                else
                {
                    targetSpeed = state.thisCharacterStats.walkSpeed;

                    float newSpeed = Utils.lerpValue(currentSpeed, targetSpeed, state.thisCharacterStats.speedGain * Time.deltaTime, true);
                    currentSpeed = newSpeed;
                    agent.speed = currentSpeed;

                }
            }
            else
            {
                targetSpeed = 0;

                float newSpeed = Utils.lerpValue(currentSpeed, targetSpeed, state.thisCharacterStats.speedGain * Time.deltaTime, false);
                currentSpeed = newSpeed;
            }

            state.thisAnimHook.setSpeed(currentSpeed);
           
            if (leaderMovedPastThreshold) //Only moves if player has moved beyond the leaders threshold
            {
                agent.SetDestination(targetPos.position); //Moves agent to new position
            }
        }
    }

    public void setPosition(Transform newPos) //Stores this character new position
    {
        targetPos = newPos;
    }

    public bool isLeaderSprinting(bool _leaderSprinting)
    {
        return _leaderSprinting;
    }

    private void OnEnable()
    {
        state.thisAnimHook.resetAnimations();
    }

    private void OnDisable()
    {
    }
}
