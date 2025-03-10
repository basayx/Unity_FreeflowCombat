using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class AttackerCharacter : MonoBehaviour
{
    private int _level = 0;
    
    [SerializeField] private ParticleSystem upgradeParticle;
    [SerializeField] private ParticleSystem feverParticle;
    
    private Character _character;
    private Transform _characterTransform;
    
    private Weapon _weapon;
    public int InGameWeaponLevel { get; private set; }
    
    private Damageable _currentTarget;
    public Damageable CurrentTarget => _currentTarget;
    private float _passedTimeWithoutTarget;
    
    private bool _isAttacking;

    [SerializeField] private bool isRanger;
    public bool IsRanger => isRanger;
    [SerializeField] private bool canApplyDamage = true;
    
    [Header("Stats")]
    [SerializeField] IncrementalFloatData followDistanceData;
    [SerializeField] IncrementalFloatData attackDistanceData;
    [SerializeField] IncrementalFloatData damagePowerData;
    [SerializeField] private IncrementalFloatData attackSpeedData;
    [SerializeField] private IncrementalFloatData maxHealthData;
    [SerializeField] private Vector2 maxHealthMultiplierRange = new Vector2(1f, 1f);

    private float _followDistance;
    public float FollowDistance => _followDistance;
    private float _attackDistance;
    public float AttackDistance => _attackDistance;
    private float _damagePower;
    public float DamagePower => _damagePower + (_damagePower * GainedBonusDamagePercent);
    private float _attackSpeed;
    public float AttackSpeed => (_attackSpeed + (_attackSpeed * GainedBonusFireRatePercent));
    
    public float GainedBonusDamagePercent { get; set; } = 0f;
    public float GainedBonusFireRatePercent { get; set; } = 0f;
    
    public Vector3 TargetFollowPos => _currentTarget.GetFollowPos(_character);
    public float DistanceBetweenTarget => _currentTarget ? Vector3.Distance(_characterTransform.position, TargetFollowPos) : 0f;
    public bool IsInFollowRange => DistanceBetweenTarget <= FollowDistance;
    public bool IsInAttackRange => DistanceBetweenTarget <= AttackDistance;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    [SerializeField] private float rangeDivisionForJoystickMovement = 2;

    private bool _isDetectedCloseTarget;
    public bool IsDetectedCloseTarget => _isDetectedCloseTarget;

    [SerializeField] private float closeAttackDistance = 5f;

    [SerializeField] private bool directApplyFirstAttack = false;
    [SerializeField] private bool killForcefullyAfterFirstAttack = false;

    private GameObject _nextEraViewObject;
    
    public void Initialize(Character character)
    {
        _character = character;
        _characterTransform = _character.transform;

        if (!_character.IsEnemy)
            PlayUpgradeParticle();
        
        _weapon = _character.GetComponentInChildren<Weapon>();
        _weapon.Initialize(_character);

        RefreshStats();
    }

    public void RefreshStats()
    {
        _followDistance = followDistanceData.CalculateFloat(_level);
        _attackDistance = attackDistanceData.CalculateFloat(_level);
        _damagePower = damagePowerData.CalculateFloat(_level);
        _attackSpeed = attackSpeedData.CalculateFloat(_level);
        _character.MaxHealth = maxHealthData.CalculateFloat(_level) * maxHealthMultiplierRange.GetRandom();

        // if (!_character.IsEnemy)
        // {
        //     _followDistance *= UpgradeablesManager.Instance.Academy.RangerMultiplier;
        //     _damagePower *= UpgradeablesManager.Instance.Academy.PowerMultiplier;
        //     _attackSpeed *= UpgradeablesManager.Instance.Academy.PowerMultiplier;
        // }
        
        _character.AnimatorHandler.GetAnimator().SetFloat("AttackSpeed", AttackSpeed);
    }

    public void ManualUpdate()
    {
        
        if (GameManager.Instance.GameState != GameState.Gameplay)
        {
            _isAttacking = false;
            _character.AnimatorHandler.GetAnimator().SetBool("IsAttacking", _isAttacking);
            return;
        }
        
        if (!_character.FollowPoint || !_character.IsCanMove || !_character.IsAlive)
            return;

        _isAttacking = false;
        if (_currentTarget)
        {
            _passedTimeWithoutTarget = 0f;
            
            if (!_currentTarget.IsAlive || !IsInFollowRange)
            {
                _currentTarget = null;
                return;
            }
            
            if (_character.IsInRunnerMode)
                _character.MarkAsInAttackMode();
            
            CheckForMostClosestTarget();
            _isDetectedCloseTarget = false;

            if (!_currentTarget)
            {
                return;
            }

            Vector3 dir = _currentTarget.transform.position - _characterTransform.position;
            dir.y = 0;

            if (!_character.IsEnemy)
            {
                if (_currentTarget is Character &&
                    Mathf.Abs(transform.position.z - _currentTarget.transform.position.z) <= closeAttackDistance)
                {
                    _isDetectedCloseTarget = true;
                }
            }
            
            // float dot = Vector3.Dot(PlayerCharactersMovementController.MovementStyleDirection, dir);
            // if (_character.IsEnemy || PlayerCharactersMovementController.MovementStyle != MovementStyle.Joystick)
            // {
            //     var detectionCount = Physics.OverlapSphereNonAlloc(_characterTransform.position, closeAttackDistance, _detectedColliders, targetDetectLayer);
            //     for (int i = 0; i < detectionCount && i < _detectedColliders.Length; i++)
            //     {
            //         if (!_detectedColliders[i]) continue;
            //     
            //         var target = _detectedColliders[i].GetComponent<Character>();
            //         if (!target || !target.IsAlive) continue;
            //
            //         _currentTarget = target;
            //         _isDetectedCloseTarget = true;
            //         break;
            //     }
            //
            //     if (!_character.IsEnemy && PlayerCharactersMovementController.MovementStyle != MovementStyle.OnlyHorizontal
            //         && PlayerCharactersMovementController.MovementStyle != MovementStyle.Backward)
            //     {
            //         if (!_isDetectedCloseTarget && dot <= 0.25f)
            //         {
            //             _lastDiscardedTargets.Add(_currentTarget);
            //             _currentTarget = null;
            //             return;
            //         }
            //         _lastDiscardedTargets.Clear();
            //     }
            // }
            // else
            // {
            // }

            if (_character.IsEnemy || _isDetectedCloseTarget)
            {
                _characterTransform.rotation = Quaternion.Lerp(_characterTransform.rotation, Quaternion.LookRotation(dir), rotationSpeed / 2f * Time.deltaTime);
            }
            else
            {
                // if (PlayerCharactersMovementController.MovementStyleDirection != Vector3.zero && !_currentTarget)
                //     _characterTransform.rotation = Quaternion.Lerp(_characterTransform.rotation,
                //         Quaternion.LookRotation(PlayerCharactersMovementController.MovementStyleDirection), rotationSpeed * Time.deltaTime);
                // else
                //     _characterTransform.rotation = Quaternion.Lerp(_characterTransform.rotation, Quaternion.LookRotation(dir), rotationSpeed * Time.deltaTime);
            }

            if (!isRanger)
            {
                if (!IsInAttackRange)
                {
                    var mSpeed = moveSpeed;
                    _character.FollowPoint.position = Vector3.MoveTowards(_character.FollowPoint.position,
                        TargetFollowPos, mSpeed * Time.deltaTime);
                }
                else
                {
                    if (directApplyFirstAttack && !_isAttacking && _currentTarget)
                    {
                        DoAttack();
                    }
                    _isAttacking = true;
                
                    _character.FollowPoint.position = _characterTransform.position;
                }
            }
            else
            {
                if (!_character.IsEnemy || (_currentTarget.transform.position.z - 10 <= transform.position.z))
                {
                    if (!IsInAttackRange)
                    {
                        if (_character.IsHaveAgent && _character.IsEnemy)
                        {
                            var mSpeed = moveSpeed;
                            _character.FollowPoint.position = Vector3.MoveTowards(_character.FollowPoint.position,
                                TargetFollowPos, mSpeed * Time.deltaTime);
                        }
                    }
                    else
                    {
                        if (directApplyFirstAttack && !_isAttacking && _currentTarget)
                        {
                            DoAttack();
                        }
                    
                        _isAttacking = true;
                    }
                }
            }
        }
        else
        {
            _passedTimeWithoutTarget += Time.unscaledDeltaTime;
            if (_passedTimeWithoutTarget > 0.75f)
            {
                if (!_character.IsInRunnerMode)
                    _character.MarkAsInRunnerMode();
            }

            CheckForMostClosestTarget();
        }
        
        _character.SetAgentPriority(_isAttacking ? 50 : 1);
        _character.AnimatorHandler.GetAnimator().SetBool("IsAttacking", _isAttacking);
    }

    private bool _isAbleToGetTarget = true;
    public bool IsAbleToGetTarget
    {
        get => _isAbleToGetTarget;
        set
        {
            _isAbleToGetTarget = value;
            if (!_isAbleToGetTarget)
                _currentTarget = null;
        }
    }

    void CheckForMostClosestTarget()
    {
        if (!IsAbleToGetTarget)
            return;
        
        if (_character.IsEnemy)
            _currentTarget = CharacterTargettingManager.Instance.GetTargetForEnemy();
    }

    public void DoAttack()
    {
        if (canApplyDamage && _currentTarget && _currentTarget.IsAlive && _character.IsAlive)
        {
            _weapon.SetTarget(_currentTarget);

            if (_character.IsEnemy)
                _weapon.ApplyAttack(DamagePower);
            else
                _weapon.ApplyAttack(DamagePower);
            
            if (killForcefullyAfterFirstAttack)
            {
                _character.KillForcefully();
            }
        }
    }

    public void PlayUpgradeParticle()
    {
        if (upgradeParticle)
            upgradeParticle.Play();
    }

    public void PlayFeverParticle()
    {
        if (feverParticle)
            feverParticle.Play();
    }
    public void StopFeverParticle()
    {
        if (feverParticle)
            feverParticle.Stop();
    }

    private void OnCollisionEnter(Collision col)
    {
    }
}

[Serializable]
public class ViewsByEra
{
    public List<GameObject> Views = new List<GameObject>();
}