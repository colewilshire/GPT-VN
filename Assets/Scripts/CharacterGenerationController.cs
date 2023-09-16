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
            characterAppearance.accessory = accessory;
        }

        if (hair)
        {
            characterAppearance.hair = hair;
        }

        if (outfit)
        {
            characterAppearance.outfit = outfit;
        }

        if (face)
        {
            characterAppearance.face = face;
        }

        characterPortrait.name = characterName;
        characterPortrait.SetAppearance(characterAppearance);

        CharacterManager.Instance.CacheCharacterPortrait(characterPortrait);
    }
}
