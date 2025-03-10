using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Character : Damageable, IPoolObject
{
    protected AnimatorHandler _animatorHandler;
    public AnimatorHandler AnimatorHandler => _animatorHandler;

    protected Rigidbody _rb;
    
    protected NavMeshAgent _agent;
    public bool IsHaveAgent => _agent;

    public float Radius => (_agent ? _agent.radius : (_characterController ? _characterController.radius : 1f));

    protected bool _isCanMove = true;
    public virtual bool IsCanMove
    {
        get => _isCanMove;
        set
        {
            _isCanMove = value;
            if (_agent && _agent.isOnNavMesh)
            {
                _agent.isStopped = !_isCanMove;
                if (!_isCanMove)
                    _agent.ResetPath();
            }
            if (_characterController)
            {
                _characterController.enabled = _isCanMove;
            }
        }
    }

    private CharacterController _characterController;

    private Transform _followPoint;
    private Vector3 _defaultFollowPointLocalPos;
    public Transform FollowPoint => _followPoint;
    public bool IsTooAwayFromFollowPos => _followPoint && Mathf.Abs(transform.position.z - _followPoint.position.z) > 1f;
    
    [SerializeField] private Material diedMaterial;

    [SerializeField] private float throwBackPowerWhenDied = 0f;

    public bool IsInRunnerMode { get; set; }

    private bool _isInGame;
    
    [SerializeField] private float movementLerpSpeed = 10f;
    [SerializeField] private float rotationLerpSpeed = 10f;

    private AttackerCharacter _attackerCharacter;
    public AttackerCharacter AttackerCharacter
    {
        get
        {
            if (!_attackerCharacter)
                _attackerCharacter = GetComponent<AttackerCharacter>();
            return _attackerCharacter;
        }
    }
    
    public bool IsEnemy { get; set; }

    protected bool _isInitialized;
    public bool IsInitialized => _isInitialized;

    [Header("Patrol")]
    [SerializeField] private bool isCanPatrol = true;
    [SerializeField] private Vector2 patrolRange = new Vector3(-7.5f, 7.5f);
    protected Vector3 _patrolStartPosition;
    [SerializeField] private Vector2 patrolIdleWaitDurationRange = new Vector2(1f, 3f);
    protected float _patrolIdleWaitTimeLeft;
    
    protected override void ManualAwake()
    {
        _patrolStartPosition = transform.position;
        GlobalCharactersUpdater.Instance.AddCharacter(this);
        
        base.ManualAwake();
        
        _animatorHandler = GetComponent<AnimatorHandler>();
        _rb = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
        _characterController = GetComponent<CharacterController>();
        
        AttackerCharacter.Initialize(this);

        _isInitialized = true;
    }

    private void OnDestroy()
    {
        if (CurrencyManager.Instance && !IsEnemy)
            CurrencyManager.Instance.OnAInGameCurrencyChanged -= OnGainedCurrencyInLevelChanged;
    
        if (GlobalCharactersUpdater.Instance)
            GlobalCharactersUpdater.Instance.RemoveCharacter(this);
            
        if (ObjectPoolingManager.Instance)
            ObjectPoolingManager.Instance.APoolObjectDestroyed(this);
    }

    public void ManualLateUpdate()
    {
        if (_isInGame && _followPoint)
        {
            Vector3 localMove = (_followPoint.position - transform.position).normalized;
            Move(localMove * (movementLerpSpeed * Time.deltaTime));

            if (IsInRunnerMode)
            {
                if (isCanPatrol)
                {
                    if (Vector3.Distance(_followPoint.position, transform.position) <= .25f)
                    {
                        if (_patrolIdleWaitTimeLeft > 0f)
                        {
                            _patrolIdleWaitTimeLeft -= Time.deltaTime;
                        }
                        else
                        {
                            _followPoint.position = _patrolStartPosition + 
                                                    new Vector3(patrolRange.GetRandom(), 0, patrolRange.GetRandom());
                            _patrolIdleWaitTimeLeft = patrolIdleWaitDurationRange.GetRandom();
                        }
                    }
                }
             
                if (IsEnemy && IsAlive)
                {
                    localMove.y = 0;
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(localMove), rotationLerpSpeed * Time.deltaTime);
                }   
            }
        }
    }
    
    public virtual void InitializeInGame()
    {
        Vector3 random = Random.insideUnitSphere * 0.1f;
        random.y = 0;
        var t = transform;
        var localPosition = t.localPosition;
        localPosition += random;
        t.localPosition = localPosition;
        
        _isInGame = true;
    }
    
    public void InitializeForInGameMove()
    {
        var t = transform;
        t.DOComplete();
        _followPoint = new GameObject().transform;
        _followPoint.SetParent(t.parent);
        _followPoint.SetLocalPositionAndRotation(t.localPosition, t.localRotation);
        _defaultFollowPointLocalPos = _followPoint.localPosition;
        _followPoint.forward = t.forward;

        if (_agent)
        {
            t.SetParent(null);

            _agent.enabled = true;
            if (_agent.isOnNavMesh)
                _agent.isStopped = false;
        }
        IsCanMove = true;

        if (!IsEnemy)
        {
            CurrencyManager.Instance.OnAInGameCurrencyChanged += OnGainedCurrencyInLevelChanged;
        }
    }

    private void OnGainedCurrencyInLevelChanged(CurrencyData currencyData, int value)
    {
        if (IsAlive && GameManager.Instance.GameState == GameState.Gameplay)
        {
            PlaySoftUpgradeParticle();
        }
    }

    public override void GetHit(float damage)
    {
        if (!_isReadyToGetDamage)
            return;
        base.GetHit(damage);

        if (!IsEnemy)
        {
            // UIManager.Instance.overlay.ShowDamageEffect();
            // GameManager.Instance.audioManager.PlayPlayerCharacterGotDamage();
        }

        _animatorHandler.SetTriggerVariable("HitReaction");
    }
    
    public override void Died()
    {
        base.Died();
        
        _animatorHandler.SetBooleanVariable("IsAttacking", false);
        _animatorHandler.SetBooleanVariable("IsDied", true);

        IsCanMove = false;
        if (_agent) _agent.enabled = false;

        if (diedMaterial)
        {
            var renderers = GetComponentsInChildren<Renderer>(true);
            foreach (Renderer r in renderers)
            {
                if (r.GetComponent<ParticleSystem>()) continue;
                
                var mats = r.materials;
                for (var i = 0; i < mats.Length; i++)
                {
                    mats[i] = diedMaterial;
                }
                r.materials = mats;
            }
        }

        if (throwBackPowerWhenDied > 0f)
        {
            var t = transform;
            t.DOKill();
            t.DOJump(t.position - t.forward * throwBackPowerWhenDied, 0.1f, 1, 0.25f);
        }

        if (IsEnemy)
        {
            var enemy = GetComponent<Enemy>();
            if (enemy)
                enemy.Died();
        }
        else
        {
        }
    }

    public void Move(Vector3 move)
    {
        if (_rb)
        {
            _rb.Move(transform.position + move, transform.rotation);
        }
        
        if (_agent && _agent.enabled && _agent.isOnNavMesh)
        {
            _agent.Move(move.x * Vector3.right);
            if (NavMesh.SamplePosition(transform.position + transform.forward * 0.1f, out _, 0.1f, NavMesh.AllAreas))
                _agent.Move(move.z * Vector3.forward);
        }
        if (_characterController && _characterController.enabled)
            _characterController.Move(move);
    }

    public void MarkAsInRunnerMode()
    {
        if (_agent) _agent.updateRotation = true;
        transform.DOKill();
        transform.DORotateQuaternion(_followPoint.rotation, 0.25f);
        // if (_playerCharactersMovementController.IsStopsWhileInAttackMode)
        // {
        //     if (!IsEnemy && GameManager.Instance.IsRunnerGameplayStarted)
        //     {
        //         _followPoint.DOKill();
        //         Vector3 pos = GameManager.Instance.levelManager.currentLevel.CameraFollowerPoint.position;
        //         pos += _defaultFollowPointLocalPos;
        //         pos.z += 5;
        //         _followPoint.DOMove(pos, 0.5f);
        //     }
        // }
        IsInRunnerMode = true;
    }
    
    public void MarkAsInAttackMode()
    {
        if (_agent) _agent.updateRotation = false;
        IsInRunnerMode = false;
    }

    public void SetAgentPriority(int priorityNo)
    {
        if (_agent) _agent.avoidancePriority = priorityNo;
    }

    public void SetAnimator(Animator targetAnimator)
    {
        _animatorHandler.SetAnimator(targetAnimator);
    }

    public void DisableAgent() => _agent.enabled = false;
    public void EnableAgent() => _agent.enabled = true;

    private float _lastTimePlaySoftUpgradeParticle;
    public void PlaySoftUpgradeParticle()
    {
        if (_lastTimePlaySoftUpgradeParticle + 0.25f < Time.time)
        {
            _lastTimePlaySoftUpgradeParticle = Time.time;
        }
    }
    public void PlayUpgradeParticle()
    {
        if (_attackerCharacter)
            _attackerCharacter.PlayUpgradeParticle();
    }

    public void StopAgent()
    {
        if (_agent && _agent.enabled && _agent.isOnNavMesh)
            _agent.isStopped = true;
    }

    public void SetAgentRadius(float agentRadius)
    {
        if (_agent)
            _agent.radius = agentRadius;
    }
    
    [Header("Object Pooling")]
    public int OwnerPoolIndex = -1;

    public int GetOwnerPoolIndex()
    {
        return OwnerPoolIndex;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public virtual void OnReturnedToPool()
    {
    }
}
