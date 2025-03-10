using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    protected Character _character;
    public bool IsAlive => _character && _character.IsAlive;
    protected AttackerCharacter _attacker;

    protected bool _isInitialized;

    [SerializeField] private Color redColor = Color.red;
    
    [SerializeField] private bool throwBack;

    [SerializeField] private DamageNumbersPro.DamageNumber damageNumber;

    [SerializeField] private int coinGainPrize = 5;
    [SerializeField, Range(0f, 1f)] private float coinDropPossibility = 0.5f;

    [SerializeField] protected bool applyTriggerDamage = true;

    [SerializeField] protected bool isBig;
    
    protected virtual void ManualStart() { }

    IEnumerator Start()
    {
        ManualStart();
        
        var renderes = GetComponentsInChildren<Renderer>(true);
        foreach (Renderer r in renderes)
        {
            r.material.color = redColor;
        }
        
        while (GameManager.Instance.GameState != GameState.Gameplay)
        {
            yield return null;
        }
        
        _character = GetComponent<Character>();
        _character.IsEnemy = true;
        _character.InitializeInGame();
        _character.InitializeForInGameMove();
        _character.MarkAsInAttackMode();
        
        _attacker = _character.AttackerCharacter;
        if (_attacker)
        {
            
            // _attacker.SetTargetDetectLayer(targetLayer);
            //
            // var bounds = _attacker.TargetDetectBounds;
            // Vector3 center = bounds.center;
            // center.z = +3f;
            // bounds.center = center;
            // _attacker.SetTargetDetectBounds(bounds);
        }

        // if (PlayerCharactersMovementController.Instance.MovementStyle == MovementStyle.OnlyHorizontal 
        //     || GameManager.Instance.levelManager.currentLevel.IsOpenWorldLevel)
        //     _character.AnimatorHandler.SetBooleanVariable("IsRunning", !isBig && (!_attacker || !_attacker.IsRanger));
        
        _isInitialized = true;
    }

    public virtual void Died()
    {
        if (GameManager.Instance.GameState == GameState.Gameplay
            && Random.Range(0f, 1f) <= coinDropPossibility)
        {
            InGameCurrencyGainManager.Instance.SpawnAndGainCurrency(transform.position, coinGainPrize);
        }
        if (throwBack)
        {
            var t = transform;
            t.DOKill();
            t.DOJump(t.position - t.forward * 1.25f, 0.5f, 1, 0.25f);
        }
        transform.DOMoveY(transform.position.y - 3f, 1f).SetDelay(2f)
            .OnComplete(() =>
            {
                Destroy(gameObject, 10f);
            });
    }

    public void KillForcefully()
    {
        if (_character)
            _character.KillForcefully();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (applyTriggerDamage && _character && _character.IsAlive && col && col.TryGetComponent(out Character character) && character.IsAlive && !character.IsEnemy)
        {
            float dmg = _character.AttackerCharacter ? _character.AttackerCharacter.DamagePower : 1;
            if (damageNumber)
                damageNumber.Spawn(character.transform.position + Vector3.up, dmg);
            character.GetHit(dmg);


            if (_character.IsAlive)
                _character.GetHit(dmg);
        }       
    }
}
