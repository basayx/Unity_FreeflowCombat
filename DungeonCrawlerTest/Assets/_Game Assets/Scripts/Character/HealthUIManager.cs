using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthUIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private bool displayTextAsFloat;
    private bool _isShowing;
    public bool IsShowing => _isShowing;

    private Transform _cameraTransform;

    [SerializeField] private bool hideAtStart = true;
    [SerializeField] private bool alwaysLookCamera = true;

    [SerializeField] private TextMeshProUGUI stepCountText;
    [SerializeField] private int maxHealthForASingleStep = 100;
    [SerializeField] private bool showStepCount;
    
    private void Start()
    {
        var canvas = canvasGroup.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        _cameraTransform = canvas.worldCamera.transform;

        if (hideAtStart)
        {
            canvasGroup.DOKill();
            canvasGroup.alpha = 0;
            _isShowing = false;
        }
        else
        {
            _isShowing = canvasGroup.alpha > 0;
        }
        
        stepCountText.gameObject.SetActive(showStepCount);
    }

    private void Update()
    {
        if (alwaysLookCamera && _cameraTransform)
            transform.forward = _cameraTransform.forward;
    }

    public void Show()
    {
        if (_isShowing)
            return;
        _isShowing = true;
        
        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, 0.5f);
    }
    
    public void Hide()
    {
        if (!_isShowing)
            return;
        _isShowing = false;
        
        canvasGroup.DOKill();
        canvasGroup.DOFade(0f, 0.5f).SetDelay(0.5f);
    }
    
    public void Refresh(float maxHealth, float currentHealth)
    {
        if (displayTextAsFloat)
            healthText.text = currentHealth.ToString(".0", CultureInfo.InvariantCulture);
        else
            healthText.text = Mathf.CeilToInt(currentHealth).ToString(CultureInfo.InvariantCulture);

        if (currentHealth < maxHealth - 0.01f)
            Show();

        if (showStepCount && maxHealthForASingleStep < maxHealth)
        {
            int stepCount = Mathf.FloorToInt((currentHealth - 0.01f) / maxHealthForASingleStep);
            stepCountText.text = "x" + (stepCount + 1);

            var healthValueInCurrentStep = stepCount >= 1 ? 
                (currentHealth - (stepCount * maxHealthForASingleStep)) :
                (currentHealth);
            healthBarSlider.value = healthValueInCurrentStep / maxHealthForASingleStep;
        }
        else
        {
            healthBarSlider.value = currentHealth / maxHealth;
        }
    }
}
