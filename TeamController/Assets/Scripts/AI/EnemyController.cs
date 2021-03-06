using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private EnemyStates state;
    private NavMeshAgent agent;

    [SerializeField] private GameObject target;

    [SerializeField] private LineRenderer laser;
    [SerializeField] private GameObject laserBeamGO;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float laserLength;
    [SerializeField] private Vector3 yOffset;

    [SerializeField] private Vector3 roamingPos;
    [SerializeField] private float roamingTimer;
    [SerializeField] private float roamingSearchRadius;
    [SerializeField] private float moveModifier;
    [SerializeField] private float detectionThreshold;
    [SerializeField] private float stopThreshold;
    [SerializeField] private float retreatThreshold;

    private bool orbitPositive;
    [SerializeField] private LayerMask whatIsWalkable;

    private void Awake()
    {
        state = GetComponent<EnemyStates>();
        agent = GetComponent<NavMeshAgent>();

        target = state.currentTeamLeader;
    }

    private void Start()
    {
        orbitPositive = (Random.Range(0,2) == 1);

        roamingPos = RandomNavmeshLocation(transform.position, roamingSearchRadius);
    }

    // Update is called once per frame
    void Update()
    {
        setTarget();

        float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
        var playerOffset = target.transform.position - transform.position;
        var direction = Vector3.zero;

        setOrbitDir(direction, playerOffset);
        

        switch (state.thisEnemyCurrentState)
        {
            case CurrentEnemyState.IDLE:
                //Play animation
                break;
            case CurrentEnemyState.ROAMING:

                roam(distanceToTarget);

                break;
            case CurrentEnemyState.CHASING:
                
                transform.LookAt(target.transform);
                chase(distanceToTarget, direction, playerOffset);

                break;            
            case CurrentEnemyState.DEAD:
                //Play animation
                break;
            default:
                break;
        }
    }

    #region state actions
    private void roam(float _distanceToTarget) {

        roamingTimer += Time.deltaTime;

        if (roamingTimer > 5f)
        {
            roamingPos = RandomNavmeshLocation(transform.position, roamingSearchRadius);

            roamingTimer = 0f;
        }

        if(_distanceToTarget <= detectionThreshold)
        {
            state.thisEnemyCurrentState = CurrentEnemyState.CHASING;
        }

        agent.SetDestination(roamingPos);
    }

    private void setOrbitDir(Vector3 _direction, Vector3 _playerOffset)
    {
        if (orbitPositive)
        {
            _direction = Vector3.Cross(_playerOffset, Vector2.up);
        }
        else
        {
            _direction = Vector3.Cross(_playerOffset, Vector2.down);
        }
    }

    private void chase(float _distanceToTarget, Vector3 _direction, Vector3 _playerOffset)
    {
        laser.enabled = false;

        if (_distanceToTarget >= stopThreshold) //5
        {
            disableLaser();

            agent.speed = state.thisEnemyStats.orbitSpeed;
            agent.SetDestination(target.transform.position + _direction);
        }
        else if (_distanceToTarget < stopThreshold && _distanceToTarget > retreatThreshold)
        {
            enableLaser();
            updateLaser();
        }
        else
        {
            disableLaser();

            Vector3 retreatPos = _playerOffset.normalized * -10;
            agent.speed = state.thisEnemyStats.retreatingSpeed;
            agent.SetDestination(retreatPos);
        }
    }
    #endregion

    #region FloaterCombat
    private void enableLaser()
    {
        laser.enabled = true;
        laserBeamGO.SetActive(true);
    }

    private void updateLaser()
    {
        RaycastHit hit;

        Physics.Raycast(firePoint.position, transform.TransformDirection(Vector3.forward), out hit, laserLength);        

        laser.SetPosition(0, firePoint.position);
        laser.SetPosition(1, hit.point + yOffset);       
    }

    private void disableLaser()
    {
        laser.enabled = false;
        laserBeamGO.SetActive(false);
    }
    #endregion

    private void setTarget()
    {
        target = state.currentTeamLeader;
    }

    public Vector3 RandomNavmeshLocation(Vector3 origin ,float radius)
    {
        Vector3 randomPos = Random.insideUnitSphere * radius + origin;

        NavMeshHit hit; // NavMesh Sampling Info Container

        // from randomPos find a nearest point on NavMesh surface in range of maxDistance
        NavMesh.SamplePosition(randomPos, out hit, radius, NavMesh.AllAreas);

        return hit.position;
    }
}
