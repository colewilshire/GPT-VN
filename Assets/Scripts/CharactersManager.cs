using System.Collections.Generic;
using UnityEngine;

public class CharactersManager : Singleton<CharactersManager>
{
    [SerializeField] private CharacterPortraitController characterPortraitPrefab;

    protected override void Awake()
    {
        base.Awake();
    }

    private void CreateCharacter(CharacterAppearance characterAppearance)
    {
        CharacterPortraitController characterPortrait = Instantiate(characterPortraitPrefab);
        characterPortrait.SetAppearance(characterAppearance);
    }

    private void GenerateAppearance(List<string> tags)
    {
        CharacterAppearance characterAppearance = CharacterAppearance.CreateInstance<CharacterAppearance>();
    }
}
