using System.Collections.Generic;
using UnityEngine;

public class CharacterGenerationController : Singleton<CharacterGenerationController>
{
    [SerializeField] CharacterPortraitController characterPortaitPrefab;

    private List<Accessory> accessories = new List<Accessory>();
    private List<Hair> hairs = new List<Hair>();
    private List<Outfit> outfits = new List<Outfit>();
    private List<Face> faces = new List<Face>();

    private Dictionary<AccessoryTag, List<Accessory>> accessoryDictionary = new Dictionary<AccessoryTag, List<Accessory>>();
    private Dictionary<HairTag, List<Hair>> hairDictionary = new Dictionary<HairTag, List<Hair>>();
    private Dictionary<OutfitTag, List<Outfit>> outfitDictionary = new Dictionary<OutfitTag, List<Outfit>>();
    private Dictionary<FaceTag, List<Face>> faceDictionary = new Dictionary<FaceTag, List<Face>>();

    private Dictionary<string, CharacterPortraitController> characters = new Dictionary<string, CharacterPortraitController>();

    protected override void Awake()
    {
        base.Awake();

        OrganizeAccessoriesByTag();
        OrganizeHairsByTag();
        OrganizeOutfitsByTag();
        OrganizeFacesByTag();
    }

    private void OrganizeAccessoriesByTag()
    {
        accessories.AddRange(Resources.LoadAll<Accessory>(""));
        TaggedImageUtility.OrganizeImagesByTag(accessories, accessoryDictionary);
    }

    private void OrganizeHairsByTag()
    {
        hairs.AddRange(Resources.LoadAll<Hair>(""));
        TaggedImageUtility.OrganizeImagesByTag(hairs, hairDictionary);
    }

    private void OrganizeOutfitsByTag()
    {
        outfits.AddRange(Resources.LoadAll<Outfit>(""));
        TaggedImageUtility.OrganizeImagesByTag(outfits, outfitDictionary);
    }

    private void OrganizeFacesByTag()
    {
        faces.AddRange(Resources.LoadAll<Face>(""));
        TaggedImageUtility.OrganizeImagesByTag(faces, faceDictionary);
    }

    public void GenerateCharacterPortrait(string characterName, List<AccessoryTag> accessoryTags, List<HairTag> hairTags, List<OutfitTag> outfitTags, List <FaceTag> faceTags)
    {
        if (characters.ContainsKey(characterName)) return;

        CharacterPortraitController characterPortrait = Instantiate(characterPortaitPrefab, Instance.transform);
        characters[characterName] = characterPortrait;

        CharacterAppearance characterAppearance = CharacterAppearance.CreateInstance<CharacterAppearance>();
        Accessory accessory = TaggedImageUtility.GetImageWithTags(accessoryTags, accessoryDictionary);
        Hair hair = TaggedImageUtility.GetImageWithTags(hairTags, hairDictionary);
        Outfit outfit = TaggedImageUtility.GetImageWithTags(outfitTags, outfitDictionary);
        Face face = TaggedImageUtility.GetImageWithTags(faceTags, faceDictionary);

        if (accessory)
        {
            characterAppearance.Accessory = accessory;
        }

        if (hair)
        {
            characterAppearance.Hair = hair;
        }

        if (outfit)
        {
            characterAppearance.Outfit = outfit;
        }

        if (face)
        {
            characterAppearance.Face = face;
        }

        characterPortrait.name = characterName;
        characterPortrait.SetAppearance(characterAppearance);

        CharacterManager.Instance.CacheCharacterPortrait(characterPortrait);
    }

    public void LoadCharactersFromSave(SaveData saveData)
    {
        for (int i = 0; i < saveData.CharacterNames.Count; ++i)
        {
            string characterName = saveData.CharacterNames[i];
            string serializedCharacterAppearance = saveData.CharacterAppearances[i];
            CharacterAppearance characterAppearance = CharacterAppearance.CreateInstance<CharacterAppearance>();
            List<string> deserializedCharacterAppearance = new List<string>(serializedCharacterAppearance.Split('|'));

            string accessoryName = deserializedCharacterAppearance[0];
            string hairName = deserializedCharacterAppearance[1];
            string outfitName = deserializedCharacterAppearance[2];
            string faceName = deserializedCharacterAppearance[3];

            characterAppearance.Accessory = Resources.Load<Accessory>($"Accessories/{accessoryName}");
            characterAppearance.Hair = Resources.Load<Hair>($"Hairs/{hairName}");
            characterAppearance.Outfit = Resources.Load<Outfit>($"Outfits/{outfitName}");
            characterAppearance.Face = Resources.Load<Face>($"Faces/{faceName}");

            CharacterPortraitController characterPortrait = Instantiate(characterPortaitPrefab, Instance.transform);
            characters[characterName] = characterPortrait;

            characterPortrait.name = characterName;
            characterPortrait.SetAppearance(characterAppearance);

            CharacterManager.Instance.CacheCharacterPortrait(characterPortrait);
        }
    }
}
