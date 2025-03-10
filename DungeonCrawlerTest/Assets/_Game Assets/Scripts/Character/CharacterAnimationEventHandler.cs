using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationEventHandler : MonoBehaviour
{
    private AttackerCharacter _attacker;
    
    void Start()
    {
        _attacker = GetComponentInParent<AttackerCharacter>();
    }

    void OnAttacked()
    {
        if (_attacker)
            _attacker.DoAttack();
    }
}
