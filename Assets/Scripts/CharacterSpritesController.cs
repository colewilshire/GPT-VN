using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSpritesController : Singleton<CharacterSpritesController>
{
    [SerializeField] CharacterPortraitController characterPortaitPrefab;

    private List<Accessory> accessories = new List<Accessory>();
    private List<Hair> hairs = new List<Hair>();
    private List<Outfit> outfits = new List<Outfit>();
    private List<Face> faces = new List<Face>();

    private Dictionary<AccessoryTag, List<Sprite>> accessoryDictionary = new Dictionary<AccessoryTag, List<Sprite>>();
    private Dictionary<HairTag, List<Sprite>> hairDictionary = new Dictionary<HairTag, List<Sprite>>();
    private Dictionary<OutfitTag, List<Sprite>> outfitDictionary = new Dictionary<OutfitTag, List<Sprite>>();
    private Dictionary<FaceTag, List<Sprite>> faceDictionary = new Dictionary<FaceTag, List<Sprite>>();

    // private Dictionary<string, Sprite> accessoryCache = new Dictionary<string, Sprite>();
    // private Dictionary<string, Sprite> hairCache = new Dictionary<string, Sprite>();
    // private Dictionary<string, Sprite> outfitCache = new Dictionary<string, Sprite>();
    // private Dictionary<string, Sprite> faceCache = new Dictionary<string, Sprite>();
    
    private Image accessory;
    private Image hair;
    private Image outfit;
    private Image face;

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
        accessory = GetComponent<Image>();
        accessories.AddRange(Resources.LoadAll<Accessory>(""));

        TaggedImageUtility.OrganizeImagesByTag(accessories, accessoryDictionary);
    }

    private void OrganizeHairsByTag()
    {
        hair = GetComponent<Image>();
        hairs.AddRange(Resources.LoadAll<Hair>(""));

        TaggedImageUtility.OrganizeImagesByTag(hairs, hairDictionary);
    }

    private void OrganizeOutfitsByTag()
    {
        outfit = GetComponent<Image>();
        outfits.AddRange(Resources.LoadAll<Outfit>(""));

        TaggedImageUtility.OrganizeImagesByTag(outfits, outfitDictionary);
    }

    private void OrganizeFacesByTag()
    {
        face = GetComponent<Image>();
        faces.AddRange(Resources.LoadAll<Face>(""));

        TaggedImageUtility.OrganizeImagesByTag(faces, faceDictionary);
    }

    public void GenerateCharacterPortrait(List<AccessoryTag> accessoryTags, List<HairTag> hairTags, List<OutfitTag> outfitTags, List <FaceTag> faceTags)
    {
        CharacterAppearance characterAppearance = CharacterAppearance.CreateInstance<CharacterAppearance>();
        CharacterPortraitController characterPortrait = Instantiate(characterPortaitPrefab);

        characterAppearance.accessory = TaggedImageUtility.GetImageWithTags(accessoryTags, accessoryDictionary);
        characterAppearance.hairFront = TaggedImageUtility.GetImageWithTags(hairTags, hairDictionary);
        characterAppearance.outfit = TaggedImageUtility.GetImageWithTags(outfitTags, outfitDictionary);
        characterAppearance.face = TaggedImageUtility.GetImageWithTags(faceTags, faceDictionary);

        characterPortrait.SetAppearance(characterAppearance);
        characterPortrait.transform.parent = Instance.transform;
    }

    // public Sprite GetAccessoryWithTags(List<AccessoryTag> desiredTags)
    // {
    //     return TaggedImageUtility.GetImageWithTags(desiredTags, accessoryDictionary);
    // }

    // public Sprite GetHairWithTags(List<HairTag> desiredTags)
    // {
    //     return TaggedImageUtility.GetImageWithTags(desiredTags, hairDictionary);
    // }

    // public Sprite GetOutfitWithTags(List<OutfitTag> desiredTags)
    // {
    //     return TaggedImageUtility.GetImageWithTags(desiredTags, outfitDictionary);
    // }

    // public Sprite GetFaceWithTags(List<FaceTag> desiredTags)
    // {
    //     return TaggedImageUtility.GetImageWithTags(desiredTags, faceDictionary);
    // }
}
