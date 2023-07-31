using System.IO;
using UnityEditor;
using UnityEngine;

public class HairCreator : MonoBehaviour
{
    [MenuItem("Assets/Create Hairs")]
    private static void CreateHairs()
    {
        // Specify the folder where your sprites are located
        string spriteFolder = "Assets/Packages/Type3_Girl/Resources/Hair";

        // Specify the folder where your back hair sprites are located
        string backgroundSpriteFolder = "Assets/Packages/Type3_Girl/Resources/Hair Back";

        // Specify the folder where you want to save your Hair assets
        string saveFolder = "Assets/Resources/Hairs";

        // Get all png files in the sprite folder and its subfolders
        string[] spritePaths = Directory.GetFiles(spriteFolder, "*.png", SearchOption.AllDirectories);

        foreach (string spritePath in spritePaths)
        {
            // Load the sprite
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);

            if (sprite != null)
            {
                // Create a new Hair and assign the sprite to it
                Hair hair = ScriptableObject.CreateInstance<Hair>();
                hair.image = sprite;

                // Find the matching back hair sprite
                string backgroundSpritePath = Path.Combine(backgroundSpriteFolder, sprite.name + ".png");
                Sprite backgroundSprite = AssetDatabase.LoadAssetAtPath<Sprite>(backgroundSpritePath);

                // Assign the back hair sprite to the imageBackground property
                if (backgroundSprite != null)
                {
                    hair.imageBackground = backgroundSprite;
                }

                // Prepare the save path
                string savePath = Path.Combine(saveFolder, sprite.name + ".asset");
                savePath = AssetDatabase.GenerateUniqueAssetPath(savePath);

                // Save the Hair as a new asset
                AssetDatabase.CreateAsset(hair, savePath);
            }
        }

        // Refresh the asset database to make the new assets show up in the editor
        AssetDatabase.Refresh();
    }
}
