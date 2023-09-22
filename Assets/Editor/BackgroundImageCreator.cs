using System.IO;
using UnityEditor;
using UnityEngine;

public class BackgroundImageCreator : MonoBehaviour
{
    [MenuItem("Assets/Create Background Images")]
    private static void CreateBackgroundImages()
    {
        // Specify the folder where your sprites are located
        string spriteFolder = "Assets/Packages/Anime_Backgrounds/Textures";

        // Specify the folder where you want to save your BackgroundImage assets
        string saveFolder = "Assets/Resources/BackgroundImages";

        // Get all png files in the sprite folder and its subfolders
        string[] spritePaths = Directory.GetFiles(spriteFolder, "*.png", SearchOption.AllDirectories);

        foreach (string spritePath in spritePaths)
        {
            // Load the sprite
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);

            if (sprite != null)
            {
                // Create a new BackgroundImage and assign the sprite to it
                BackgroundImage backgroundImage = ScriptableObject.CreateInstance<BackgroundImage>();
                backgroundImage.MainImage = sprite;

                // Prepare the save path
                string savePath = Path.Combine(saveFolder, sprite.name + ".asset");
                savePath = AssetDatabase.GenerateUniqueAssetPath(savePath);

                // Save the BackgroundImage as a new asset
                AssetDatabase.CreateAsset(backgroundImage, savePath);
            }
        }

        // Refresh the asset database to make the new assets show up in the editor
        AssetDatabase.Refresh();
    }
}
