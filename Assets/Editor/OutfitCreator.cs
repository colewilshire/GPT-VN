using System.IO;
using UnityEditor;
using UnityEngine;

public class OutfitCreator : MonoBehaviour
{
    [MenuItem("Assets/Create Outfits")]
    private static void CreateOutfits()
    {
        // Specify the folder where your sprites are located
        string spriteFolder = "Assets/Packages/Type3_Girl/Resources/Body";

        // Specify the folder where you want to save your Outfit assets
        string saveFolder = "Assets/Resources/Outfits";

        // Get all png files in the sprite folder and its subfolders
        string[] spritePaths = Directory.GetFiles(spriteFolder, "*.png", SearchOption.AllDirectories);

        foreach (string spritePath in spritePaths)
        {
            // Load the sprite
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);

            if (sprite != null)
            {
                // Create a new Outfit and assign the sprite to it
                Outfit outfit = ScriptableObject.CreateInstance<Outfit>();
                outfit.MainImage = sprite;

                // Prepare the save path
                string savePath = Path.Combine(saveFolder, sprite.name + ".asset");
                savePath = AssetDatabase.GenerateUniqueAssetPath(savePath);

                // Save the Outfit as a new asset
                AssetDatabase.CreateAsset(outfit, savePath);
            }
        }

        // Refresh the asset database to make the new assets show up in the editor
        AssetDatabase.Refresh();
    }
}
