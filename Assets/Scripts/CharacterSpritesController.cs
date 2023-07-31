using System.Collections.Generic;
using UnityEngine;

public class CharacterSpritesController : Singleton<CharacterSpritesController>
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

    public void GenerateCharacterPortrait(List<AccessoryTag> accessoryTags, List<HairTag> hairTags, List<OutfitTag> outfitTags, List <FaceTag> faceTags)
    {
        CharacterAppearance characterAppearance = CharacterAppearance.CreateInstance<CharacterAppearance>();
        CharacterPortraitController characterPortrait = Instantiate(characterPortaitPrefab);

        characterAppearance.accessory = TaggedImageUtility.GetImageWithTags(accessoryTags, accessoryDictionary).image;
        characterAppearance.hairFront = TaggedImageUtility.GetImageWithTags(hairTags, hairDictionary).image;
        characterAppearance.outfit = TaggedImageUtility.GetImageWithTags(outfitTags, outfitDictionary).image;
        characterAppearance.face = TaggedImageUtility.GetImageWithTags(faceTags, faceDictionary).image;

        characterPortrait.SetAppearance(characterAppearance);
        characterPortrait.transform.parent = Instance.transform;
    }
}
