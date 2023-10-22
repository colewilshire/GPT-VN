using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : Singleton<CharacterManager>
{
    private CharacterPortrait activePortrait;
    public Dictionary<string, CharacterPortrait> Characters {get; private set;} = new Dictionary<string, CharacterPortrait>();

    public void CacheCharacterPortrait(CharacterPortrait characterPortrait)
    {
        Characters[characterPortrait.name] = characterPortrait;
    }

    public void ShowPortrait(string characterName, string expressionName)
    {
        if (activePortrait)
        {
            activePortrait.HidePortrait();
        }

        if (Characters.TryGetValue(characterName, out CharacterPortrait characterPortrait))
        {
            activePortrait = characterPortrait;

            Enum.TryParse(expressionName, out Mood expression);
            characterPortrait.ShowPortrait(expression);
        }
    }

    public void ClearCharacters()
    {
        foreach (KeyValuePair<string, CharacterPortrait> characterEntry in Characters)
        {
            Destroy(characterEntry.Value.gameObject);
        }

        Characters = new Dictionary<string, CharacterPortrait>();
    }
}
