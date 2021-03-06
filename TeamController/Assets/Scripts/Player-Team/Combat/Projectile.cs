using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private EnemyHealth detectedEnemy;

    private characterType thisCharacterType;
    private bool wasLockedOn;
    private Vector3 ShootDir;
    private float velocity;
    private Vector3 movementOffset;
    [SerializeField] private Rigidbody thisRB;
    [SerializeField] private GameObject explosionSubsitute;

    Cinemachine.CinemachineImpulseSource shakeSource;

    public void Setup(characterType _thisCharacterType, Vector3 _shootDir , bool _wasMoving, bool _wasLockedOn, float _velocity)
    {
        shakeSource = GetComponent<Cinemachine.CinemachineImpulseSource>();

        thisCharacterType = _thisCharacterType;
        wasLockedOn = _wasLockedOn;
        this.movementOffset = new Vector3(Random.Range(-.15f, .15f), Random.Range(-.15f, .15f), Random.Range(-.15f, .15f));
        this.velocity = _velocity;


        if (!_wasMoving)
        {
            this.ShootDir = _shootDir;
        }
        else
        {
            this.ShootDir = _shootDir + movementOffset;
        }
        Quaternion rotation = Quaternion.LookRotation(ShootDir, Vector3.up);
        transform.localRotation = rotation;
    }

    private IEnumerator releaseVelocity()
    {
        yield return new WaitForSeconds(3f);
        velocity = 20f;
    }

    // Update is called once per frame
    void Update()
    {
        if (thisCharacterType == characterType.Archer)
        {
            transform.position += ShootDir * velocity * Time.deltaTime;
        }
        else if(thisCharacterType == characterType.Mage)
        {

            if (wasLockedOn)
            {
                transform.position = Vector3.Lerp(transform.position, ShootDir, .8f * Time.deltaTime);
            }
            else
            {
                transform.position += ShootDir * 8f * Time.deltaTime;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        shakeSource.GenerateImpulse();

        if (!collision.gameObject.GetComponent<Projectile>())
        {
            if (thisCharacterType == characterType.Archer)
            {
                SoundManager.Play3DSound(SoundManager.Sound.BowImpactHard, false, true, 2f, .3f, 100f, collision.transform.position);
                thisRB.isKinematic = true;
                velocity = 0;
                transform.parent = collision.transform;

                if (collision.gameObject.CompareTag("Enemy"))
                {
                    SoundManager.Play3DSound(SoundManager.Sound.BowImpactSoft, false, true, 2f, .3f, 100f, collision.transform.position);
                    callDamageEnemy(collision);
                }
            }
            else if (thisCharacterType == characterType.Mage)
            {
                SoundManager.Play3DSound(SoundManager.Sound.MagicExplosion, false, true, 2f, .3f, 100f, collision.transform.position);

                if (collision.gameObject.CompareTag("Enemy"))
                {
                    callDamageEnemy(collision);
                }

                Instantiate(explosionSubsitute, transform.position, Quaternion.identity);
                Destroy(this.gameObject);
            }           
        }     
    }

    private void callDamageEnemy(Collision _thisCollision)
    {
        detectedEnemy = _thisCollision.gameObject.GetComponent<EnemyHealth>();
        detectedEnemy.takeDamage(100f);
    }

}
