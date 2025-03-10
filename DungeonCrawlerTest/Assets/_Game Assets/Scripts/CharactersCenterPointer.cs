using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharactersCenterPointer : MonoBehaviour
{
    private static CharactersCenterPointer _instance;
    public static CharactersCenterPointer Instance
    {
        get
        {
            if (!_instance) _instance = FindObjectOfType<CharactersCenterPointer>();
            return _instance;
        }
    }

    public Vector3 CenterPos => transform.position;
}
