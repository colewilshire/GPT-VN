using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterManager : Singleton<CharacterManager>
{
    [SerializeField] private CharacterPortrait characterPortaitPrefab;
    private Dictionary<string, CharacterPortrait> characterPortraits = new();
    private CharacterPortrait activePortrait;
    private readonly Dictionary<string, Accessory> accessories = new();
    private readonly Dictionary<string, Hair> hairs = new();
    private readonly Dictionary<string, Outfit> outfits = new();
    private readonly Dictionary<string, Face> faces = new();
    public Dictionary<string, CharacterDescription> CharacterDescriptions;

    protected override void Awake()
    {
        base.Awake();

        SortCharacterParts();
    }

    private void SortCharacterParts()
    {
        List<Accessory> unsortedAccessories = Resources.LoadAll<Accessory>("Accessories").ToList();
        List<Hair> unsortedHairs = Resources.LoadAll<Hair>("Hairs").ToList();
        List<Outfit> unsortedOutfits = Resources.LoadAll<Outfit>("Outfits").ToList();
        List<Face> unsortedFaces = Resources.LoadAll<Face>("Faces").ToList();

        foreach(Accessory accessory in unsortedAccessories)
        {
            accessories[accessory.Description] = accessory;
        }

        foreach(Hair hair in unsortedHairs)
        {
            hairs[hair.Description] = hair;
        }

        foreach(Outfit outfit in unsortedOutfits)
        {
            outfits[outfit.Description] = outfit;
        }

        foreach(Face face in unsortedFaces)
        {
            faces[face.Description] = face;
        }
    }

    private void ClearCharacterPortraits()
    {
        foreach(KeyValuePair<string, CharacterPortrait> keyValuePair in characterPortraits)
        {
            Destroy(characterPortraits[keyValuePair.Key].gameObject);
        }
    }

    private CharacterPortrait GenerateCharacterPortrait(string characterName, CharacterDescription characterDescription, CharacterAppearance appearanceOverride = null)
    {
        CharacterPortrait characterPortrait = Instantiate(characterPortaitPrefab, transform);
        CharacterAppearance characterAppearance = ScriptableObject.CreateInstance<CharacterAppearance>();

        characterPortrait.gameObject.name = characterName;
        characterPortrait.DisplayName = characterName;

        if (!appearanceOverride)
        {
            accessories.TryGetValue(characterDescription.Accessory, out characterAppearance.Accessory);
            hairs.TryGetValue(characterDescription.Hair, out characterAppearance.Hair);
            outfits.TryGetValue(characterDescription.Outfit, out characterAppearance.Outfit);
            faces.TryGetValue(characterDescription.Eyes, out characterAppearance.Face);
            characterPortrait.SetAppearance(characterAppearance);
        }
        else
        {
            characterPortrait.SetAppearance(appearanceOverride);
        }

        return characterPortrait;
    }

    public void GenerateCharacterPortraits(Dictionary<string, CharacterDescription> castList)
    {
        CharacterDescriptions = castList;

        ClearCharacterPortraits();
        characterPortraits = new();
    
        foreach(KeyValuePair<string, CharacterDescription> keyValuePair in castList)
        {
            string characterName = keyValuePair.Key;
            CharacterDescription characterDescription = keyValuePair.Value;

            characterPortraits[characterName] = GenerateCharacterPortrait(characterName, characterDescription);
        }
    }

    public CharacterPortrait ShowPortrait(string characterName, string moodName)
    {
        if (activePortrait)
        {
            activePortrait.HidePortrait();
        }

        if (characterName.ToLower() == "narrator")
        {
            return null;
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

    public string ListAccessories()
    {
        string listedAccessories = "none, ";

        foreach(KeyValuePair<string, Accessory> keyValuePair in accessories)
        {
            string accessoryDescription = $"'{keyValuePair.Key}'";
            listedAccessories += $"{accessoryDescription}, ";
        }

        listedAccessories = listedAccessories.TrimEnd(',', ' ');

        return listedAccessories;
    }

    public string ListHairs()
    {
        string listedHairs = "none, ";

        foreach(KeyValuePair<string, Hair> keyValuePair in hairs)
        {
            string hairDescription = $"'{keyValuePair.Key}'";
            listedHairs += $"{hairDescription}, ";
        }

        listedHairs = listedHairs.TrimEnd(',', ' ');

        return listedHairs;
    }

    public string ListOutfits()
    {
        string listedOutfits = "none, ";

        foreach(KeyValuePair<string, Outfit> keyValuePair in outfits)
        {
            string outfitDescription = $"'{keyValuePair.Key}'";
            listedOutfits += $"{outfitDescription}, ";
        }

        listedOutfits = listedOutfits.TrimEnd(',', ' ');

        return listedOutfits;
    }

    public string ListFaces()
    {
        string listedFaces = "none, ";

        foreach(KeyValuePair<string, Face> keyValuePair in faces)
        {
            string faceDescription = $"'{keyValuePair.Key}'";
            listedFaces += $"{faceDescription}, ";
        }

        listedFaces = listedFaces.TrimEnd(',', ' ');

        return listedFaces;
    }
}
