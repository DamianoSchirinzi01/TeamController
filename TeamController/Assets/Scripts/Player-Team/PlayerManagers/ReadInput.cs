using UnityEngine;
using DG.Tweening;

public class ReadInput : MonoBehaviour
{
    private PlayerActions thisPlayerInput;

    private CharacterState state;
    private PlayerMovement thisPlayerMovement;
    private PlayerCombat thisPlayerCombat;

    // Start is called before the first frame update
    void Awake()
    {
        thisPlayerInput = new PlayerActions();

        state = GetComponent<CharacterState>();
        thisPlayerMovement = GetComponent<PlayerMovement>();
        thisPlayerCombat = GetComponent<PlayerCombat>();        
    }

    private void Start()
    {
        thisPlayerInput.PlayerControls.Jump.performed += value => readJumpInput();

        thisPlayerInput.PlayerControls.Sprint.performed += value => readSprintPressed();
        thisPlayerInput.PlayerControls.Sprint.canceled += value => readSprintReleased();

        thisPlayerInput.PlayerControls.Attack.performed += value => onFirePressed();
        thisPlayerInput.PlayerControls.Attack.canceled += value => onFireReleased();

        thisPlayerInput.PlayerControls.AlternateActionPressed.performed += value => onAlternateActionPressed();
        thisPlayerInput.PlayerControls.AlternateActionPressed.canceled += value => onAlternateActionReleased();

        thisPlayerInput.PlayerControls.LockOn.performed += value => onLockOn();
    }

    // Update is called once per frame
    void Update()
    {
        thisPlayerMovement.direction = thisPlayerInput.PlayerControls.Movement.ReadValue<Vector2>();

        TargetManager.instance.mouseValue = thisPlayerInput.PlayerControls.Look.ReadValue<Vector2>(); //Mouse movement is stored to check for mouse swipes
    }

    private void readJumpInput()
    {
        thisPlayerMovement.jumped = true;
    }

    private void readSprintPressed()
    {
        thisPlayerMovement.isSprinting = true;
    }

    private void readSprintReleased()
    {
        thisPlayerMovement.isSprinting = false;
    }
    private void onFirePressed()
    {
        thisPlayerCombat.isAttacking = true;
    }

    private void onFireReleased()
    {
        thisPlayerCombat.isAttacking = false; //If released the character is no longer attacking

        if (state.thisCharacterType == characterType.Knight) {
            thisPlayerCombat.attackComplete = false;
        }
    }

    private void onAlternateActionPressed()
    {
        thisPlayerCombat.isUsingAlternateAction = true;

        DOVirtual.Float(EffectsController.instance.usingAlternateVolume.weight, 1, 1.5f, EffectsController.instance.setAlternateFXvolume);
    }

    private void onAlternateActionReleased()
    {
        thisPlayerCombat.isUsingAlternateAction = false;

        DOVirtual.Float(EffectsController.instance.usingAlternateVolume.weight, 0, 1.5f, EffectsController.instance.setAlternateFXvolume);
    }

    private void onLockOn()
    {
       if (TargetManager.instance.nearbyEnemies.Count > 0)
        {
            if (TargetManager.instance.isLockingOn == false)
            {
                TargetManager.instance.isLockingOn = true;

                state.thisPlayerCombat.setNewTarget(TargetManager.instance.target);

                TargetManager.instance.toggleLockOnUI(true);
            }
            else
            {
                TargetManager.instance.resetLockOn();
                TargetManager.instance.toggleLockOnUI(false);
                state.thisPlayerCombat.setNewTarget(null);

            }
        }
    }

    private void OnEnable()
    {
        thisPlayerInput.Enable();
    }

    private void OnDisable()
    {
        thisPlayerInput.Disable();
    }
}
