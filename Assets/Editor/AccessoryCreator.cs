using System.IO;
using UnityEditor;
using UnityEngine;

public class AccessoryCreator : MonoBehaviour
{
    [MenuItem("Assets/Create Accessories")]
    private static void CreateAccessories()
    {
        // Specify the folder where your sprites are located
        string spriteFolder = "Assets/Packages/Type3_Girl/Resources/Acc";

        // Specify the folder where you want to save your Accessory assets
        string saveFolder = "Assets/Resources/Accessories";

        // Get all png files in the sprite folder and its subfolders
        string[] spritePaths = Directory.GetFiles(spriteFolder, "*.png", SearchOption.AllDirectories);

        foreach (string spritePath in spritePaths)
        {
            // Load the sprite
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);

            if (sprite != null)
            {
                // Create a new Accessory and assign the sprite to it
                Accessory accessory = ScriptableObject.CreateInstance<Accessory>();
                accessory.image = sprite;

                // Prepare the save path
                string savePath = Path.Combine(saveFolder, sprite.name + ".asset");
                savePath = AssetDatabase.GenerateUniqueAssetPath(savePath);

                // Save the Accessory as a new asset
                AssetDatabase.CreateAsset(accessory, savePath);
            }
        }

        // Refresh the asset database to make the new assets show up in the editor
        AssetDatabase.Refresh();
    }
}
