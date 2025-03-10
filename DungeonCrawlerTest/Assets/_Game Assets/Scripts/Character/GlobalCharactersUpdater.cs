using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalCharactersUpdater : MonoBehaviour
{
    private static GlobalCharactersUpdater _instance;
    public static GlobalCharactersUpdater Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<GlobalCharactersUpdater>();
            }
            return _instance;
        }
        set => _instance = value;
    }

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private List<Character> _characters = new List<Character>();

    private void Update()
    {
        foreach (Character character in _characters)
        {
            if (!character) continue;
            character.AttackerCharacter.ManualUpdate();
            character.ManualLateUpdate();
        }
    }

    public void AddCharacter(Character character) => _characters.Add(character);
    public void RemoveCharacter(Character character) => _characters.Remove(character);
}
