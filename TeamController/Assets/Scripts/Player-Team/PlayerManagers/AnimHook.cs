using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AnimHook : MonoBehaviour
{
    [Header("References")]
    private CharacterState state;
    [SerializeField] private Animator thisAnim;

    [Header("Runtime Information")]
    private Vector3 playerLookAtPos;

    [Header("ParameterNames")]
    public string meleeLight_1 = "LightAttack_1";
    public string meleeLight_2 = "LightAttack_2";
    public string HeavyAttack = "HeavyAttack_1";
    public string fire = "Fire";

    public string AlternateAction = "AlternateAction";
    public string moveSpeed = "MoveSpeed";
    public string jumped = "jumped";
    public string isGrounded = "isGrounded";
    public string isFalling = "isFalling";
    private string reset = "reset";

    private void Awake()
    {
        state = GetComponentInParent<CharacterState>();
    }
    public Vector3 PlayerLookAtPos
    {
        get { return PlayerLookAtPos; }
        set { playerLookAtPos = value; }
    }

    public void setSpeed(float _speed)
    {
        thisAnim.SetFloat(moveSpeed, _speed);
    } 

    public void setBool(string _parameterName ,bool _alternateActionState)
    {
        thisAnim.SetBool(_parameterName, _alternateActionState);
    }

    void OnAnimatorIK(int layerIndex) //Call back for animators Inverse kinematics
    {
        if(state.memberID == 0)
        {
            thisAnim.SetLookAtWeight(1);
            thisAnim.SetLookAtPosition(playerLookAtPos);
        }
        else
        {
            thisAnim.SetLookAtPosition(transform.forward * 5f);
        }
    }

    public void resetAnimations()
    {
        setBool(AlternateAction, false);
        setBool(meleeLight_1, false);
        setBool(meleeLight_2, false);
        setBool(HeavyAttack, false);

        DOVirtual.Float(thisAnim.GetFloat(moveSpeed), 0, 4f, setSpeed);

        setBool(isFalling, false);
        setBool(isGrounded, true);
        setBool(jumped, false);
        setBool(reset, true);

        StartCoroutine(toggleReset());
    }
    
    private IEnumerator toggleReset()
    {
        yield return new WaitForSeconds(.5f);
        setBool(reset, false);
    }
}
