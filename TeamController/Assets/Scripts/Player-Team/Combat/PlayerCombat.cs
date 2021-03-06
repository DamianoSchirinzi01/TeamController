using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    private Target currentTarget;
    private CharacterState state;

    [Header("Combat values")]
    [SerializeField] private int shootChargeMultiplier;
    [SerializeField] private float shootVelocity;
    [SerializeField] private float attackTimerCheck;
    private float originalShootVelocity;
    public bool checkForComboPress;
    public bool[] isHitting = new bool[2];

    [Header("Character Weapons")]
    [SerializeField] private GameObject mageArcLightningBox;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject projectile;

    #region Input
    public bool attackComplete { get; set; }
    public bool isAttacking { get; set; }
    public bool isUsingAlternateAction { get; set; }
    #endregion

    #region Audio
    private bool soundSpawned;
    private AudioSource arcLightningTempSource;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        state = GetComponent<CharacterState>();
    }

    void Start()
    {
        originalShootVelocity = shootVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state.thisCharacterType)
        {
            case characterType.Knight:
                checkKnightAttackType();
                break;
            case characterType.Archer:
                checkArcherAttack();
                break;
            case characterType.Mage:
                checkMageAttackType();
                break;
            default:
                break;
        }

        checkAlternateActionState();
    }

    public void setNewTarget(Target _thisTarget)
    {
        currentTarget = _thisTarget;
        state.thisCamManger.CurrentTarget = currentTarget;
    }

    //Is this a heavy or light attack? Which type of character is calling the attack?
    private void checkKnightAttackType()
    {
        if (isAttacking) //Is true whilst button is held
        {
            attackTimerCheck -= Time.deltaTime; //If we are attacking decrease the timer

            if (attackTimerCheck <= 0) //If the button has been held for longer that the value in "attackTimerCheck"
            {
                knightAttack(true);

                isAttacking = false; //The attack is now complete and the player has to release the corresponding button to attack again.
            }
            else
            {
                knightAttack(false);
            }
        }
        else
        {
            attackTimerCheck = .4f; //reset the attack timer when character is not attacking.
        }
    }
    private void checkArcherAttack()
    {
        if (isAttacking) //Is true whilst button is held
        {
            archerFire();
        }
    }

    private void checkMageAttackType()
    {
        if (isUsingAlternateAction)
        {
            if (isAttacking)
            {
                mageArcFire(true);
            }
            else
            {
                mageArcFire(false);
            }
        }
        else
        {
            if (isAttacking) //Is true whilst button is held
            {
                attackTimerCheck -= Time.deltaTime; //If we are attacking decrease the timer

                if (attackTimerCheck <= 0) //If the button has been held for longer that the value in "attackTimerCheck"
                {
                    //Debug.Log("Heavy should run");
                    mageFire(true);
                }
                else
                {
                    //Debug.Log("Light should run");
                    //Light blast
                    mageFire(false);
                }
            }
            else
            {
                attackTimerCheck = .4f; //reset the attack timer when character is not attacking.
            }

            mageArcLightningBox.SetActive(false);
        }
    }
    private void checkAlternateActionState()
    {
        if (isUsingAlternateAction)
        {
            setAlternateActionStandard(true);
        }
        else
        {
            setAlternateActionStandard(false);
        }
    }   

    #region attacks (Melee attacks / Firing)
    private void knightAttack(bool _isHeavy)
    {
        if (_isHeavy) //Check if this should be a heavy attack
        {
            state.thisAnimHook.setBool(state.thisAnimHook.meleeLight_2, false); //Stop combo 2 if interupted by a heavy attack
            state.thisAnimHook.setBool(state.thisAnimHook.HeavyAttack, true); //Start heavy attack

        }
        else //If it is not heavy, it is a light attack
        {
            if (attackComplete == false) //We can only light attack if this current button press has been fully performed. i.e it has been pressed AND released
            {
                StartCoroutine(resetAttackValues());
                if (!isHitting[0] && !isHitting[1])
                {
                    //Debug.Log("Combo_1");
                    isHitting[0] = true;
                    state.thisAnimHook.setBool(state.thisAnimHook.meleeLight_1, true);
                }
                else if (isHitting[0] && checkForComboPress)
                {
                    //Debug.Log("Combo_2");

                    isHitting[1] = true;
                    state.thisAnimHook.setBool(state.thisAnimHook.meleeLight_1, false);
                    state.thisAnimHook.setBool(state.thisAnimHook.meleeLight_2, true);
                }
                attackComplete = true; //We can no longer use this light attack in this instance of button hold, stops this attack from being called every frame whilst button is being pressed     
            }
        }
    }

    private void archerFire()
    {
        if (isUsingAlternateAction)
        {
            if (attackComplete == false)
            {
                //Instantiate an arrow with velocity

                Vector3 shootDir = Vector3.zero;
                if (TargetManager.instance.isLockingOn)
                {
                    shootDir = (currentTarget.transform.position - shootPoint.position).normalized;
                }
                else
                {
                    shootDir = transform.forward;
                }

                SoundManager.Play3DSound(SoundManager.Sound.BowRelease, false, true, 2f, 1f, 50f, transform.position);
                GameObject spawnedArrow = Instantiate(projectile, shootPoint.position, Quaternion.LookRotation(shootDir, transform.up));
                spawnedArrow.GetComponent<Projectile>().Setup(state.thisCharacterType, shootDir, state.IsMoving(), TargetManager.instance.isLockingOn, shootVelocity);

                state.thisAnimHook.setBool(state.thisAnimHook.fire, true);

                attackComplete = true;
                shootVelocity = originalShootVelocity;
                isAttacking = false;
                StartCoroutine(resetAttackValues());
            }
        }
        else
        {
            //Debug.Log("Needs to aim first!");
            state.thisAnimHook.setBool(state.thisAnimHook.fire, false);
        }
    }

    private void mageArcFire(bool _isCasting)
    {
        if (_isCasting)
        {
            //Debug.Log("Casting lightning!");
            if(soundSpawned == false)
            {
                //arcLightningTempSource = SoundManager.Play3DSound(SoundManager.Sound.ArcLightning, true, false, 0f, .6f, 80f, transform.position);
                soundSpawned = true;
            }
            state.thisAnimHook.setBool(state.thisAnimHook.fire, true);
            mageArcLightningBox.SetActive(true);
        }
        else
        {
            //Destroy(arcLightningTempSource, .5f);
            mageArcLightningBox.SetActive(false);
            state.thisAnimHook.setBool(state.thisAnimHook.fire, false);
        }
    }

    private void mageFire(bool _isHeavy)
    {
        Vector3 shootDir = Vector3.zero;

        if (TargetManager.instance.isLockingOn)
        {
            shootDir = currentTarget.transform.position;
        }
        else
        {
            shootDir = transform.forward;
        }

        if (attackComplete == false)
        {
            if (_isHeavy)
            {
                //Debug.Log("Heavy blast!");
                state.thisAnimHook.setBool(state.thisAnimHook.HeavyAttack, true);

                GameObject spawnedProjectile = Instantiate(projectile, shootPoint.position, Quaternion.LookRotation(shootDir, transform.up));
                spawnedProjectile.GetComponent<Projectile>().Setup(state.thisCharacterType, shootDir, state.IsMoving(), TargetManager.instance.isLockingOn, 8);

                isAttacking = false;
            }
            else
            {
                //Debug.Log("LightBlast!");
                state.thisAnimHook.setBool(state.thisAnimHook.fire, true);

                GameObject spawnedProjectile = Instantiate(projectile, shootPoint.position, Quaternion.LookRotation(shootDir, transform.up));
                spawnedProjectile.GetComponent<Projectile>().Setup(state.thisCharacterType, shootDir, state.IsMoving(), TargetManager.instance.isLockingOn, 8);
            }

            attackComplete = true;
            StartCoroutine(resetAttackValues());
        }
        else
        {
            state.thisAnimHook.setBool(state.thisAnimHook.fire, false);
        }
    }
    #endregion

    #region alternates (Blocking / Aiming)
    private void setAlternateActionStandard(bool _using)
    {
        if (_using)
        {
            state.thisAnimHook.setBool(state.thisAnimHook.AlternateAction, _using);

            if (state.thisCharacterType == characterType.Archer)
            {
                if(soundSpawned == false)
                {
                    SoundManager.Play3DSound(SoundManager.Sound.BowReady, false, true, 2f, 1f, 80f, transform.position);
                    soundSpawned = true;
                }

                shootVelocity += shootChargeMultiplier * Time.deltaTime;
                if (shootVelocity >= 30f)
                {
                    isAttacking = true;
                    shootVelocity = originalShootVelocity;
                }
            }
        }
        else
        {
            state.thisAnimHook.setBool(state.thisAnimHook.AlternateAction, _using);
            soundSpawned = false;
            shootVelocity = originalShootVelocity;
        }
    }
    #endregion

    private IEnumerator resetAttackValues()
    {
        switch (state.thisCharacterType)
        {
            case characterType.Knight:

                yield return new WaitForSeconds(.15f); //We dont want the user mashing buttons and being able to use the second attack
                checkForComboPress = true; //We wait .15f until the player can hit again, if they do it before this, the attack wont be registered

                yield return new WaitForSeconds(.3f);
                state.thisAnimHook.setBool(state.thisAnimHook.meleeLight_1, false); //Reset first attack after .3f of it being called

                yield return new WaitForSeconds(.8f); //After this 1f mark, all combo bools will reset and the player will be able to do their initial attack again
                checkForComboPress = false;
                isHitting[0] = false;
                isHitting[1] = false;
                isHitting[2] = false;

                state.thisAnimHook.setBool(state.thisAnimHook.meleeLight_2, false); //Reset other attacks after .8f of the first attack being called, Any input just after or just before this wont count
                state.thisAnimHook.setBool(state.thisAnimHook.HeavyAttack, false);

                break;
            case characterType.Archer:
                yield return new WaitForSeconds(.3f);
                state.thisAnimHook.setBool(state.thisAnimHook.fire, false);

                yield return new WaitForSeconds(.45f);
                attackComplete = false;

                break;
            case characterType.Mage:

                yield return new WaitForSeconds(1.4f);
                attackComplete = false;

                state.thisAnimHook.setBool(state.thisAnimHook.HeavyAttack, false); //Start heavy attack

                break;
            default:
                break;
        }
    }

  

    //private void sortTargetList()
    //{
    //    //sorts objects by angle and stores it into the Sorted_List
    //    SortedTargetList = nearbyTargets.OrderBy(gameObjects =>
    //    {
    //        float angle = 0;
    //        //get vector from camera to target
    //        if (gameObjects != null)
    //        {
    //            Vector3 target_direction = gameObjects.transform.position - transform.position;
    //            var camera_forward = new Vector2(Camera.main.transform.forward.x, Camera.main.transform.forward.z);

    //            //convert target_direction into 2D Vector
    //            var target_dir = new Vector2(target_direction.x, target_direction.z);

    //            //get the angle between the two vectors 
    //            //angle = Vector2.Angle(camera_forward, target_dir);
    //        }
    //        //convert camera forward direction into 2D vector
    //        return angle;
    //    }).ToList(); //store the objects based off of the angle into the sorted List

    //    //copy objects into the main game_object list
    //    //remove objects that happen to die before selecting next target
    //    for (var i = 0; i < nearbyTargets.Count(); ++i)
    //    {
    //        //overwrite the candidate list with the sorted list
    //        nearbyTargets[i] = SortedTargetList[i];

    //        if(nearbyTargets.Count > 0)
    //        {
    //            if (!nearbyTargets[i].gameObject.activeInHierarchy)
    //            {
    //                nearbyTargets.RemoveAt(i);
    //                currentTargetIndex--;
    //            }
    //        }           
    //    }
    //    //Super cool thing to note,  "float angle = Vector2.Angle(camera_forward, target_dir);" sorts by abs(angle) aka unsigned so the first object you target is always the object you are most looking at
    //    //set target as the target you are most looking at
    //    //if I am targeting, there are candidate objects within my radius     
    //}

    //Sets/toggles current target
   
       


}