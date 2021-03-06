using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectHit : MonoBehaviour
{
    Cinemachine.CinemachineImpulseSource shakeSource;

    private EnemyHealth detectedEnemy;
    public bool isLighting;

    private void Start()
    {
        shakeSource = GetComponent<Cinemachine.CinemachineImpulseSource>();
    }
    private void OnTriggerEnter(Collider collision)
    {
        if(isLighting == false)
        {
            shakeSource.GenerateImpulse();

            if (collision.CompareTag("Enemy"))
            {
                SoundManager.Play3DSound(SoundManager.Sound.SwordImpact, false, true, 2f, .7f, 80f, collision.transform.position);
                detectedEnemy = collision.GetComponent<EnemyHealth>();
                detectedEnemy.takeDamage(100f);
            }
        }       
    }

    private void OnTriggerStay(Collider collision)
    {
        if(isLighting == true)
        {
            shakeSource.GenerateImpulse();

            if (collision.CompareTag("Enemy"))
            {
                detectedEnemy = collision.GetComponent<EnemyHealth>();
                detectedEnemy.takeDamage(.9f);
            }
        }        
    }
}
