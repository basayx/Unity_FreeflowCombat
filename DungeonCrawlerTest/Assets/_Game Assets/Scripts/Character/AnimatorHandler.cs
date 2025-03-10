using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimatorVariableTypes
{
    _float,
    _integer,
    _boolean,
    _trigger
}

[Serializable]
public struct AnimatorHandlerVariableStruct
{
    public string Key;
    public AnimatorVariableTypes VariableType;
    public float Value;
}
    
public class AnimatorVariableHandling
{
    private AnimatorHandler animatorHandler;
    private bool isActive = false;
    public string Key;
    public int handlingNo = 0;

    public void Initialize(AnimatorHandler _animatorHandler, int _handlingNo, AnimatorVariableTypes _type, string _key)
    {
        animatorHandler = _animatorHandler;
        handlingNo = _handlingNo;
        Key = _key;
        
        isActive = true;
        
        if (_type == AnimatorVariableTypes._trigger)
        {
            animatorHandler.GetAnimator().SetTrigger(_key);
        }
    }
    public void Initialize(AnimatorHandler _animatorHandler, int _handlingNo, AnimatorVariableTypes _type, string _key, float _value, float smoothSpeed = 0f)
    {
        Initialize(_animatorHandler, _handlingNo, _type, _key);
        if (smoothSpeed != 0f)
            _animatorHandler.StartCoroutine(SmoothHandlingCoroutine(_type, _key, _value, smoothSpeed));
        else
            animatorHandler.GetAnimator().SetFloat(_key, _value);
    }
    public void Initialize(AnimatorHandler _animatorHandler, int _handlingNo, AnimatorVariableTypes _type, string _key, int _value, float smoothSpeed = 0f)
    {
        Initialize(_animatorHandler, _handlingNo, _type, _key);
        if (smoothSpeed != 0f)
            _animatorHandler.StartCoroutine(SmoothHandlingCoroutine(_type, _key, _value, smoothSpeed));
        else
            animatorHandler.GetAnimator().SetInteger(_key, _value);
    }
    public void Initialize(AnimatorHandler _animatorHandler, int _handlingNo, AnimatorVariableTypes _type, string _key, bool _value)
    {
        Initialize(_animatorHandler, _handlingNo, _type, _key);
        animatorHandler.GetAnimator().SetBool(_key, _value);
    }

    IEnumerator SmoothHandlingCoroutine(AnimatorVariableTypes _type, string _key, float _value, float smoothSpeed)
    {
        float smoothValue = 0f;
        if (_type == AnimatorVariableTypes._float)
            smoothValue = animatorHandler.GetAnimator().GetFloat(_key);
        else if (_type == AnimatorVariableTypes._integer)
            smoothValue = animatorHandler.GetAnimator().GetInteger(_key);
        while (isActive && Mathf.Abs(smoothValue - _value) > 0.01f)
        {
            smoothValue = Mathf.Lerp(smoothValue,_value, smoothSpeed * Time.deltaTime);
            if (_type == AnimatorVariableTypes._float)
                animatorHandler.GetAnimator().SetFloat(_key, smoothValue);
            else if (_type == AnimatorVariableTypes._integer)
                animatorHandler.GetAnimator().SetInteger(_key, Mathf.FloorToInt(smoothValue));
            yield return new WaitForEndOfFrame();
        }

        if (isActive)
        {
            if (_type == AnimatorVariableTypes._float)
                animatorHandler.GetAnimator().SetFloat(_key, _value);
            else if (_type == AnimatorVariableTypes._integer)
                animatorHandler.GetAnimator().SetInteger(_key, Mathf.CeilToInt(_value));
            Finished();
        }
    }

    public void Finished()
    {
        isActive = false;
        animatorHandler.AHandlingFinished(this);
    }
}

public class AnimatorHandler : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    private int lastHandlingNo = 0;
    private List<AnimatorVariableHandling> currentHandlingAnimatorVariables = new List<AnimatorVariableHandling>();

    public Animator GetAnimator()
    {
        if (!animator)
            animator = GetComponentInChildren<Animator>();
        return animator;
    }
    
    public void FinishAllHandlingsWhichHaveTargetKey(string key)
    {
        currentHandlingAnimatorVariables.FindAll(x => x.Key == key).ForEach(x => x.Finished());
    }
    
    public void SetFloatVariable(string key, float value, float smoothSpeed = 0f)
    {
        FinishAllHandlingsWhichHaveTargetKey(key);
        AnimatorVariableHandling variableHandling = new AnimatorVariableHandling();
        variableHandling.Initialize(this, lastHandlingNo, AnimatorVariableTypes._float, key, value, smoothSpeed);
        currentHandlingAnimatorVariables.Add(variableHandling);
    }
    
    public void SetIntegerVariable(string key, int value, float smoothSpeed = 0f)
    {
        FinishAllHandlingsWhichHaveTargetKey(key);
        AnimatorVariableHandling variableHandling = new AnimatorVariableHandling();
        variableHandling.Initialize(this, lastHandlingNo, AnimatorVariableTypes._integer, key, value, smoothSpeed);
        currentHandlingAnimatorVariables.Add(variableHandling);
    }
    
    public void SetBooleanVariable(string key, bool value)
    {
        FinishAllHandlingsWhichHaveTargetKey(key);
        AnimatorVariableHandling variableHandling = new AnimatorVariableHandling();
        variableHandling.Initialize(this, lastHandlingNo, AnimatorVariableTypes._boolean, key, value);
        currentHandlingAnimatorVariables.Add(variableHandling);
    }
    
    public void SetTriggerVariable(string key)
    {
        FinishAllHandlingsWhichHaveTargetKey(key);
        AnimatorVariableHandling variableHandling = new AnimatorVariableHandling();
        variableHandling.Initialize(this, lastHandlingNo, AnimatorVariableTypes._trigger, key);
        currentHandlingAnimatorVariables.Add(variableHandling);
    }

    public void AHandlingFinished(AnimatorVariableHandling variableHandling)
    {
        currentHandlingAnimatorVariables.Remove(variableHandling);
    }

    public void SetAnimator(Animator targetAnimator)
    {
        animator = targetAnimator;
    }
}
