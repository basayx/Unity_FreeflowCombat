using System;
using UnityEngine;
using UnityEngine.AI;

public class AgentAnimator : MonoBehaviour
{
    private Transform _agentTransform;
    protected Animator _animator;
    
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _agentTransform = transform.parent;
        _lastPosition = _agentTransform.position;
    }

    private void Update()
    {
        UpdateBlendSpeed();
    }

    private Vector3 _lastPosition;
    private float _verticalBlendTreeSpeed = 0;
    private float _horizontalBlendTreeSpeed = 0;
    private void UpdateBlendSpeed()
    {
        var position = _agentTransform.position;
        
        float dot = -Vector3.Dot(_agentTransform.forward, Vector3.forward);
        
        _verticalBlendTreeSpeed = Mathf.Lerp(_verticalBlendTreeSpeed, ((_lastPosition - position).z * 10f) * dot, 7.5f * Time.deltaTime);
        _animator.SetFloat(AnimatorParameterKey.VerticalSpeed, _verticalBlendTreeSpeed);
        _horizontalBlendTreeSpeed = Mathf.Lerp(_horizontalBlendTreeSpeed, ((_lastPosition - position).x * 10f) * dot, 7.5f * Time.deltaTime);
        _animator.SetFloat(AnimatorParameterKey.HorizontalSpeed, _horizontalBlendTreeSpeed);
        
        _lastPosition = position;
    }
    
    protected struct AnimatorParameterKey
    {
        public static readonly int VerticalSpeed = Animator.StringToHash("VerticalSpeed");
        public static readonly int HorizontalSpeed = Animator.StringToHash("HorizontalSpeed");
    }
}
