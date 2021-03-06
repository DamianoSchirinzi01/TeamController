using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearbyTargets : MonoBehaviour
{
    private SphereCollider targetTrigger;
    private CharacterState state;

    // Start is called before the first frame update
    void Awake()
    {
        targetTrigger = GetComponent<SphereCollider>();

        state = GetComponentInParent<CharacterState>();
    }

    private void Update()
    {
        if(state.memberID == 0)
        {
            targetTrigger.enabled = true;
        }
        else
        {
            targetTrigger.enabled = false;
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if(state.memberID == 0)
        {
            if (collision.GetComponent<Target>())
            {
                Target nearbyTarget = collision.GetComponent<Target>();

                TargetManager.instance.storeEnemyInList(nearbyTarget);
            }
        }     
    }

    void OnTriggerExit(Collider collision)
    {
        if(state.memberID == 0)
        {
            if (collision.GetComponent<Target>())
            {
                Target nearbyTarget = collision.GetComponent<Target>();

                TargetManager.instance.removeEnemyFromList(nearbyTarget);
            }
        }      
    }
}
