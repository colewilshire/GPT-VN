using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : Singleton<CharacterManager>
{
    Dictionary<string, CharacterPortraitController> characters = new Dictionary<string, CharacterPortraitController>();
    CharacterPortraitController activePortrait;

    public void CacheCharacterPortrait(CharacterPortraitController characterPortrait)
    {
        characters[characterPortrait.name] = characterPortrait;
    }

    public void ShowPortait(string characterName)
    {
        if (activePortrait)
        {
            activePortrait.HidePortrait();
        }

        if (characters.TryGetValue(characterName, out CharacterPortraitController characterPortrait))
        {
            activePortrait = characterPortrait;
            characterPortrait.ShowPortrait();
        }
    }
}
