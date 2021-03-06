using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStates : MonoBehaviour
{
    [SerializeField] private TeamController _teamController;

    public GameObject currentTeamLeader;
    public BaseEnemy thisEnemyStats;
    public CurrentEnemyState thisEnemyCurrentState;
    public EnemyType thisEnemyType;

    private void Awake()
    {
        currentTeamLeader = _teamController.returnCurrentTeamLeader();
    }

    private void Update()
    {
        currentTeamLeader = _teamController.returnCurrentTeamLeader();
    }
}

public enum CurrentEnemyState
{
    IDLE,
    ROAMING,
    CHASING,
    DEAD
}
public enum EnemyType
{
    FLYING,
    GROUNDED
}

