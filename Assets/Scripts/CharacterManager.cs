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

    public CharacterPortrait ShowPortrait(string characterName, Mood mood)
    {
        if (activePortrait)
        {
            activePortrait.HidePortrait();
        }

        if (Characters.TryGetValue(characterName, out CharacterPortrait characterPortrait))
        {
            activePortrait = characterPortrait;

            //Mood expression = Thesaurus.Instance.GetMoodSynonym(mood);
            //Enum.TryParse(expressionName, out Mood expression);
            characterPortrait.ShowPortrait(mood);

            return characterPortrait;
        }

        return null;
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
