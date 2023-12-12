using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewCharacterManager : Singleton<NewCharacterManager>
{
    [SerializeField] private CharacterPortrait characterPortaitPrefab;
    private Dictionary<string, CharacterPortrait> characterPortraits = new();
    private CharacterPortrait activePortrait;
    private List<Accessory> accessories;
    private List<Hair> hairs;
    private List<Outfit> outfits;
    private List<Face> faces;

    protected override void Awake()
    {
        base.Awake();

        accessories = Resources.LoadAll<Accessory>("").ToList();
        hairs = Resources.LoadAll<Hair>("").ToList();
        outfits = Resources.LoadAll<Outfit>("").ToList();
        faces = Resources.LoadAll<Face>("").ToList();
    }

    private CharacterPortrait GenerateCharacterPortrait()
    {
        CharacterPortrait characterPortrait = Instantiate(characterPortaitPrefab, transform);
        CharacterAppearance characterAppearance = ScriptableObject.CreateInstance<CharacterAppearance>();

        characterAppearance.Accessory = accessories[UnityEngine.Random.Range(0, accessories.Count)];
        characterAppearance.Hair = hairs[UnityEngine.Random.Range(0, hairs.Count)];
        characterAppearance.Outfit = outfits[UnityEngine.Random.Range(0, outfits.Count)];
        characterAppearance.Face = faces[UnityEngine.Random.Range(0, faces.Count)];

        characterPortrait.SetAppearance(characterAppearance);

        return characterPortrait;
    }

    public void GenerateCharacterPortraits(Dictionary<string, Character> characterDictionary)
    {
        characterPortraits = new();
    
        foreach(KeyValuePair<string, Character> keyValuePair in characterDictionary)
        {
            characterPortraits[keyValuePair.Key] = GenerateCharacterPortrait();
        }
    }

    public CharacterPortrait ShowPortrait(string characterName, string moodName)
    {
        if (activePortrait)
        {
            activePortrait.HidePortrait();
        }

        if (characterPortraits.TryGetValue(characterName, out CharacterPortrait characterPortrait))
        {
            activePortrait = characterPortrait;

            Enum.TryParse(moodName, true, out Mood mood);
            characterPortrait.ShowPortrait(mood);

            return characterPortrait;
        }

        return null;
    }
}
