using System.Collections.Generic;
using UnityEngine;

public class CharacterGenerationController : Singleton<CharacterGenerationController>
{
    [SerializeField] private CharacterPortrait characterPortaitPrefab;

    private List<Accessory> accessories = new();
    private List<Hair> hairs = new();
    private List<Outfit> outfits = new();
    private List<Face> faces = new();

    private Dictionary<AccessoryTag, List<Accessory>> accessoryDictionary = new();
    private Dictionary<HairTag, List<Hair>> hairDictionary = new();
    private Dictionary<OutfitTag, List<Outfit>> outfitDictionary = new();
    private Dictionary<FaceTag, List<Face>> faceDictionary = new();

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
        CharacterPortrait characterPortrait = Instantiate(characterPortaitPrefab, Instance.transform);
        CharacterAppearance characterAppearance = ScriptableObject.CreateInstance<CharacterAppearance>();
        Accessory accessory = TaggedImageUtility.GetImageWithTags(accessoryTags, accessoryDictionary);
        Hair hair = TaggedImageUtility.GetImageWithTags(hairTags, hairDictionary);
        Outfit outfit = TaggedImageUtility.GetImageWithTags(outfitTags, outfitDictionary);
        Face face = TaggedImageUtility.GetImageWithTags(faceTags, faceDictionary);

        if (characterName.ToLower() != "narrator")
        {
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

            characterPortrait.DisplayName = characterName.Split(' ')[0];
        }
        else
        {
            characterPortrait.DisplayName = "";
        }

        characterPortrait.name = characterName;
        characterPortrait.SetAppearance(characterAppearance);
    }
}
