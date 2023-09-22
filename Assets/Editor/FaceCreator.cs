using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FaceCreator : MonoBehaviour
{
    [MenuItem("Assets/Create Faces")]
    private static void CreateFaces()
    {
        string spriteFolder = "Assets/Packages/Type3_Girl/Resources/Face";
        string saveFolder = "Assets/Resources/Faces";
        string[] spritePaths = Directory.GetFiles(spriteFolder, "*.png", SearchOption.AllDirectories);
        List<Face> faceList = new List<Face>();
        
        for (int i = 0; i < spritePaths.Length; i = i + 6)
        {
            faceList.Add(Face.CreateInstance<Face>());
        }

        foreach (string spritePath in spritePaths)
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
           
            if (sprite != null)
            {
                int spriteNumber = int.Parse(sprite.name);
                int faceNumber = spriteNumber / 6;
                
                if (spriteNumber % 6 == 0)
                {
                    faceList[faceNumber].MainImage = sprite;
                }
                else if (spriteNumber % 6 == 1)
                {
                    faceList[faceNumber].Sad = sprite;
                }
                else if (spriteNumber % 6 == 2)
                {
                    faceList[faceNumber].Happy = sprite;
                }
                else if (spriteNumber % 6 == 3)
                {
                    faceList[faceNumber].Angry = sprite;
                }
                else if (spriteNumber % 6 == 4)
                {
                    faceList[faceNumber].Shocked = sprite;
                }
                else if (spriteNumber % 6 == 5)
                {
                    faceList[faceNumber].Awkward = sprite;
                }
            }
        }

        for (int i = 0; i < faceList.Count; ++i)
        {
            string savePath = Path.Combine(saveFolder, $"Face{i}" + ".asset");
            savePath = AssetDatabase.GenerateUniqueAssetPath(savePath);

            AssetDatabase.CreateAsset(faceList[i], savePath);
        }

        AssetDatabase.Refresh();
    }
}
