using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{   
    private EnemyStates state;

    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private List<MonoBehaviour> componentsToDisable;

    [SerializeField] private List<Dissolve> dissolveComponents;

    private float currentHealth;
    private bool isDead;

    private void Awake()
    {
        state = GetComponent<EnemyStates>();
    }

    private void Start()
    {
        currentHealth = state.thisEnemyStats.health;
       
    }

    private void Update()
    {  
        checkDeathState();
    }

    public void takeDamage(float _dmg)
    {
        Debug.Log("You hit " + _dmg);

        currentHealth -= _dmg;
    }

    private void checkDeathState()
    {
        if (currentHealth <= 0 && isDead == false)
        {
            startDeath();
        }
    }

    private void startDeath()
    {
        TargetManager.instance.removeEnemyFromList(GetComponent<Target>());

        foreach (Dissolve _dissolve in dissolveComponents)
        {
            _dissolve.swapMaterials();
        }

        EffectsController.instance.startParticles(deathParticles);

        disableComponents();

        isDead = true;
    }   

    private void disableComponents()
    {
        NavMeshAgent thisAgent = GetComponent<NavMeshAgent>();
        
        foreach(MonoBehaviour i in componentsToDisable)
        {
            i.enabled = false;
        }

        thisAgent.enabled = false;

        StartCoroutine(Death());
    }

    private IEnumerator Death()
    {
        yield return new WaitForSeconds(.5f);

        yield return new WaitForSeconds(1.3f);
        EffectsController.instance.stopParticles(deathParticles);
        Destroy(gameObject, 1f);
    }
}
