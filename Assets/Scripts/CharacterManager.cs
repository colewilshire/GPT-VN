using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : Singleton<CharacterManager>
{
    private CharacterPortraitController activePortrait;
    public Dictionary<string, CharacterPortraitController> Characters {get; private set;} = new Dictionary<string, CharacterPortraitController>();

    public void CacheCharacterPortrait(CharacterPortraitController characterPortrait)
    {
        Characters[characterPortrait.name] = characterPortrait;
    }

    public void ShowPortait(string characterName, string expressionName)
    {
        if (activePortrait)
        {
            activePortrait.HidePortrait();
        }

        if (Characters.TryGetValue(characterName, out CharacterPortraitController characterPortrait))
        {
            activePortrait = characterPortrait;

            Enum.TryParse(expressionName, out Mood expression);
            characterPortrait.ShowPortrait(expression);
        }
    }
}
