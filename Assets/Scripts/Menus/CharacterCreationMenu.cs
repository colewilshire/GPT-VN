using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCreationMenu : Menu
{
    protected override HashSet<GameState> ActiveStates { get; set; } = new HashSet<GameState>
    {
        GameState.CharacterCreation
    };

    [SerializeField] private CharacterPortrait characterPortrait;
    [SerializeField] private CharacterCreationButton accessoryButtons;
    [SerializeField] private CharacterCreationButton hairButtons;
    [SerializeField] private CharacterCreationButton outfitButtons;
    [SerializeField] private CharacterCreationButton faceButtons;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button randomizeButton;
    [SerializeField] private Button exitButton;
    // [SerializeField] private TMP_Text characterName;

    private readonly List<Accessory> accessories = new();
    private readonly List<Hair> hairs = new();
    private readonly List<Outfit> outfits = new();
    private readonly List<Face> faces = new();

    private int accessoryIndex = 0;
    private int hairIndex = 0;
    private int outfitIndex = 0;
    private int faceIndex = 0;

    private void Awake()
    {
        accessories.Add(ScriptableObject.CreateInstance<Accessory>());
        hairs.Add(ScriptableObject.CreateInstance<Hair>());

        accessories.AddRange(Resources.LoadAll<Accessory>(""));
        hairs.AddRange(Resources.LoadAll<Hair>(""));
        outfits.AddRange(Resources.LoadAll<Outfit>(""));
        faces.AddRange(Resources.LoadAll<Face>(""));

        accessoryButtons.ForwardButton.onClick.AddListener(IncrementAccessory);
        hairButtons.ForwardButton.onClick.AddListener(IncrementHair);
        outfitButtons.ForwardButton.onClick.AddListener(IncrementOutfit);
        faceButtons.ForwardButton.onClick.AddListener(IncrementFace);

        accessoryButtons.BackButton.onClick.AddListener(DecrementAccessory);
        hairButtons.BackButton.onClick.AddListener(DecrementHair);
        outfitButtons.BackButton.onClick.AddListener(DecrementOutfit);
        faceButtons.BackButton.onClick.AddListener(DecrementFace);

        resetButton.onClick.AddListener(SetDefaultAppearance);
        randomizeButton.onClick.AddListener(SetRandomAppearance);

        characterPortrait.Appearance = ScriptableObject.CreateInstance<CharacterAppearance>();
        characterPortrait.Appearance.Accessory = accessories[accessoryIndex];
        characterPortrait.Appearance.Hair = hairs[hairIndex];
        characterPortrait.Appearance.Outfit = outfits[outfitIndex];
        characterPortrait.Appearance.Face = faces[faceIndex];

        SetDefaultAppearance();
    }

    protected override void Start()
    {
        base.Start();

        exitButton.onClick.AddListener(ExitMenu);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        accessoryButtons.ForwardButton.onClick.RemoveListener(IncrementAccessory);
        hairButtons.ForwardButton.onClick.RemoveListener(IncrementHair);
        outfitButtons.ForwardButton.onClick.RemoveListener(IncrementOutfit);
        faceButtons.ForwardButton.onClick.RemoveListener(IncrementFace);

        accessoryButtons.BackButton.onClick.RemoveListener(DecrementAccessory);
        hairButtons.BackButton.onClick.RemoveListener(DecrementHair);
        outfitButtons.BackButton.onClick.RemoveListener(DecrementOutfit);
        faceButtons.BackButton.onClick.RemoveListener(DecrementFace);

        resetButton.onClick.RemoveListener(SetDefaultAppearance);
        randomizeButton.onClick.RemoveListener(SetRandomAppearance);
        exitButton.onClick.RemoveListener(ExitMenu);
    }

    protected override void ResetMenu()
    {
        base.ResetMenu();

        SetDefaultAppearance();
        // characterName.text = "";
    }

    protected override void ExitMenu()
    {
        SaveAppearance();
        // characterPortrait.DisplayName = characterName.text;

        base.ExitMenu();
    }

    private void SaveAppearance()
    {
        characterPortrait.Appearance = ScriptableObject.CreateInstance<CharacterAppearance>();
        characterPortrait.Appearance.Accessory = accessories[accessoryIndex];
        characterPortrait.Appearance.Hair = hairs[hairIndex];
        characterPortrait.Appearance.Outfit = outfits[outfitIndex];
        characterPortrait.Appearance.Face = faces[faceIndex];
    }

    private void SetAppearance(int newAcessoryIndex, int newHairIndex, int newOutfitIndex, int newFaceIndex)
    {
        SetAccessory(newAcessoryIndex);
        SetHair(newHairIndex);
        SetOutfit(newOutfitIndex);
        SetFace(newFaceIndex);
    }

    private void SetDefaultAppearance()
    {
        SetAppearance(7, 1, 0, 1);
        SaveAppearance();
    }

    private void SetRandomAppearance()
    {
        int randomAccessoryIndex = Random.Range(0, accessories.Count);
        int randomHairIndex = Random.Range(0, hairs.Count);
        int randomFaceIndex = Random.Range(0, faces.Count);
        int randomOutfitIndex = Random.Range(0, outfits.Count);

        SetAppearance(randomAccessoryIndex, randomHairIndex, randomOutfitIndex, randomFaceIndex);
    }

    private void SetAccessory(int index = 0)
    {
        accessoryIndex = index;
        accessoryButtons.NameDisplay.text = accessoryIndex.ToString();

        characterPortrait.SetAccessory(accessories[accessoryIndex]);
    }

    private void SetHair(int index = 0)
    {
        hairIndex = index;
        hairButtons.NameDisplay.text = hairIndex.ToString();

        characterPortrait.SetHair(hairs[hairIndex]);
    }

    private void SetOutfit(int index = 0)
    {
        outfitIndex = index;
        outfitButtons.NameDisplay.text = outfitIndex.ToString();

        characterPortrait.SetOutfit(outfits[outfitIndex]);
    }

    private void SetFace(int index = 0)
    {
        faceIndex = index;
        faceButtons.NameDisplay.text = faceIndex.ToString();

        characterPortrait.SetFace(faces[faceIndex]);
    }

    private void IncrementAccessory()
    {
        ++accessoryIndex;

        if (accessoryIndex >= accessories.Count)
        {
            accessoryIndex = 0;
        }

        SetAccessory(accessoryIndex);
    }

    private void IncrementHair()
    {
        ++hairIndex;

        if (hairIndex >= hairs.Count)
        {
            hairIndex = 0;
        }

        SetHair(hairIndex);
    }

    private void IncrementOutfit()
    {
        ++outfitIndex;

        if (outfitIndex >= outfits.Count)
        {
            outfitIndex = 0;
        }

        SetOutfit(outfitIndex);
    }

    private void IncrementFace()
    {
        ++faceIndex;

        if (faceIndex >= faces.Count)
        {
            faceIndex = 0;
        }

        SetFace(faceIndex);
    }

    private void DecrementAccessory()
    {
        --accessoryIndex;

        if (accessoryIndex < 0)
        {
            accessoryIndex = accessories.Count - 1;
        }

        SetAccessory(accessoryIndex);
    }

    private void DecrementHair()
    {
        --hairIndex;

        if (hairIndex < 0)
        {
            hairIndex = hairs.Count - 1;
        }

        SetHair(hairIndex);
    }

    private void DecrementOutfit()
    {
        --outfitIndex;

        if (outfitIndex < 0)
        {
            outfitIndex = outfits.Count - 1;
        }

        SetOutfit(outfitIndex);
    }

    private void DecrementFace()
    {
        --faceIndex;

        if (faceIndex < 0)
        {
            faceIndex = faces.Count - 1;
        }

        SetFace(faceIndex);
    }
}
