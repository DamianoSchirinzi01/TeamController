using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scriptables", menuName = "ScriptableObject/Heroes")]
public class BaseCharacter : ScriptableObject
{
    public string characterName;

    [Header("Movement values")]
    public float sprintSpeed;
    public float walkSpeed;
    public float jumpHeight;
    public float speedGain;

    [Header("Combat values")]
    public float damage;
}
