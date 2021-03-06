using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Scriptables", menuName = "ScriptableObject/Enemies")]
public class BaseEnemy : ScriptableObject
{
    public float roamingSpeed;
    public float orbitSpeed;
    public float retreatingSpeed;

    public float health;
    public float damage;
}
