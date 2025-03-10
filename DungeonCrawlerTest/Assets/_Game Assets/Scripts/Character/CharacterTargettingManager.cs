using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterTargettingManager : MonoBehaviour
{
    public static CharacterTargettingManager Instance;
    private void Awake()
    {
        if (Instance)
            Destroy(gameObject);
        else
            Instance = this;
    }

    [SerializeField] private Damageable playerDamageable;

    public Damageable GetTargetForEnemy()
    {
        return playerDamageable;
    }
}
