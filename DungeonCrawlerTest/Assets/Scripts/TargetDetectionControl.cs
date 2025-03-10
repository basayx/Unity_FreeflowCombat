using System;
using System.Collections;
using System.Collections.Generic;
using FoW;
using StarterAssets;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class TargetDetectionControl : MonoBehaviour
{

    public static TargetDetectionControl instance;

    [Header("Components")]
    public PlayerControl playerControl;
    public StarterAssetsInputs starterAssetsInputs;

    [Header("Scene")]
    public List<Transform> allTargetsInScene = new List<Transform>();
    
    [Space]
    [Header("Target Detection")]
    public LayerMask whatIsEnemy;
    public bool canChangeTarget = true;

    [Tooltip("Detection Range: \n Player range for detecting potential targets.")]
    [Range(0f, 15f)] public float detectionRange = 10f;

    [Tooltip("Dot Product Threshold \nHigher Values: More strict alignment required \nLower Values: Allows for broader targeting")]
    [Range(0f, 1f)] public float dotProductThreshold = 0.15f;

    [Space]
    [Header("Range Display")]
    public Transform rangeDisplayTransform;
    private Vector3 rangeDisplayFollowOffset;
    public DecalProjector rangeDisplayDecalProjector;
    private Vector3 rangeDisplayDecalSize;
    public float rangeDisplayDecalSizeMultiplier = 2f;
    public Color rangeDisplayTargetDetectedColor;
    private Color rangeDisplayTargetDefaultColor;

    [Space] 
    [Header("Fog of War")] 
    public FogOfWarUnit fogOfWarUnit;
    public float fogOfWarUnitRadiusMultiplier = 2f;
    
    [Space]
    [Header("Debug")]
    public bool debug;
    public Transform checkPos;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        rangeDisplayTransform.SetParent(null);
        rangeDisplayFollowOffset = transform.position - rangeDisplayTransform.position;
        rangeDisplayDecalSize = rangeDisplayDecalProjector.size;
        
        rangeDisplayTargetDefaultColor = rangeDisplayDecalProjector.material.color;
        rangeDisplayDecalProjector.material = Instantiate(rangeDisplayDecalProjector.material); 
        
        PopulateTargetInScene();
        StartCoroutine(RunEveryXms());
    }

    private void Update()
    {
        rangeDisplayTransform.position = transform.position + rangeDisplayFollowOffset;
        rangeDisplayDecalSize.x = detectionRange;
        rangeDisplayDecalSize.y = detectionRange;
        rangeDisplayDecalProjector.size = rangeDisplayDecalSize * rangeDisplayDecalSizeMultiplier;
        
        rangeDisplayDecalProjector.material.color = playerControl.CurrentTarget ? rangeDisplayTargetDetectedColor : rangeDisplayTargetDefaultColor;
        
        fogOfWarUnit.circleRadius = detectionRange * fogOfWarUnitRadiusMultiplier;
    }

    private void PopulateTargetInScene()
    {
        // Find all active GameObjects in the scene
        EnemyBase[] allGameObjects = FindObjectsOfType<EnemyBase>();

        // Convert the array to a list
        List<EnemyBase> gameObjectList = new List<EnemyBase>(allGameObjects);

        // Output the number of GameObjects found
        if (debug)
            Debug.Log("Number of targets found: " + gameObjectList.Count);

        // Optionally, iterate over the list and do something with each GameObject
        foreach (EnemyBase obj in gameObjectList)
        {
            allTargetsInScene.Add(obj.transform);
        }
    }

    private IEnumerator RunEveryXms()
    {
        while (true)
        {
            yield return new WaitForSeconds(.1f); // Wait for 'x' milliseconds
            GetEnemyInInputDirection();
        }
    }

    #region Get Enemy In Input Direction

    public void GetEnemyInInputDirection()
    {
        if (canChangeTarget)
        {
            Vector3 inputDirection = new Vector3(starterAssetsInputs.move.x, 0, starterAssetsInputs.move.y).normalized;
            
            if (inputDirection != Vector3.zero)
            {
                inputDirection = Camera.main.transform.TransformDirection(inputDirection);
                inputDirection.y = 0;
                inputDirection.Normalize();
            
            
            }

            Transform closestEnemy = GetClosestEnemyInDirection(inputDirection);

            playerControl.ChangeTarget(closestEnemy);
            if (closestEnemy != null)
            {
                // Do something with the closest enemy in the input direction
                Debug.Log("Closest enemy in direction: " + closestEnemy.name);
            }
        }
    }
    
    Transform GetClosestEnemyInDirection(Vector3 inputDirection)
    {
        Transform closestEnemy = null;
        // float maxDotProduct = dotProductThreshold; // Start with the threshold value
        float lastDist = Mathf.Infinity;

        var center = transform.position;
        foreach (Transform enemy in allTargetsInScene)
        {
            if (!enemy) continue;
            
            var pos = enemy.position;
            // Vector3 dir = (pos - center).normalized;
            // var dotProduct = Vector3.Dot(inputDirection, dir);

            var dist = Vector3.Distance(center, pos);
            if (!(dist <= lastDist && dist <= detectionRange)) continue;
            // if (!(dotProduct > maxDotProduct)) continue;

            if (enemy.TryGetComponent(out Damageable damageable) && !damageable.IsAlive)
                continue;   
            
            lastDist = dist;
            // maxDotProduct = dotProduct;
            closestEnemy = enemy;
        }

        return closestEnemy;
    }

    #endregion

    #region Unused Code/ Might Delete Later


    #endregion
}
