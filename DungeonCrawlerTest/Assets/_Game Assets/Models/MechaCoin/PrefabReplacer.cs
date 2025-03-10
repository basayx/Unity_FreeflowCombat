using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabReplacer : MonoBehaviour
{
    // [SerializeField] private List<Transform> points = new List<Transform>();
    // [SerializeField] private GameObject prefab;
    //
    // [ContextMenu(nameof(Replace))]
    // public void Replace()
    // {
    //     foreach (Transform point in points)
    //     {
    //         var o = (UnityEditor.PrefabUtility.InstantiatePrefab(prefab) as GameObject)?.transform;
    //         if (o != null)
    //         {
    //             o.SetPositionAndRotation(point.position, point.rotation);
    //             o.SetParent(point.parent);
    //             o.localScale = point.localScale;
    //         }
    //         DestroyImmediate(point.gameObject);
    //     }
    // }
}
