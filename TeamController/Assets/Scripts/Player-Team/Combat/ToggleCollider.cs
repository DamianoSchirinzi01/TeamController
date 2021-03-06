using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleCollider : MonoBehaviour
{
    [SerializeField] private Collider colliderToToggle;
   
    public void toggleColliderOn()
    {
        setRandomSwipeClip();

        colliderToToggle.enabled = true;
    }

    public void toggleColliderOff()    {

        colliderToToggle.enabled = false;
    }

    private void setRandomSwipeClip()
    {
        int randomSoundSelected = Random.Range(0, 2);

        switch (randomSoundSelected)
        {
            case 0:
                SoundManager.Play3DSound(SoundManager.Sound.SwordSwipe_1, false, true, 2f, .7f, 80f, transform.position);
                break;
            case 1:
                SoundManager.Play3DSound(SoundManager.Sound.SwordSwipe_2, false, true, 2f, .7f, 80f, transform.position);
                break;
            case 2:
                SoundManager.Play3DSound(SoundManager.Sound.SwordSwipe_3, false, true, 2f, .7f, 80f, transform.position);
                break;
        }
    }
}
