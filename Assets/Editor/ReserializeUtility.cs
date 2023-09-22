using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class ReserializeUtility
{
    [MenuItem("Assets/Force Reserialize Assets in Resources")]
    public static void ReserializeAssetsInResources()
    {
        // Get all asset paths in the Resources folder
        string[] allAssets = AssetDatabase.GetAllAssetPaths();
        List<string> resourcesAssets = allAssets.Where(path => path.StartsWith("Assets/Resources/")).ToList();

        // Force reserialize only those assets
        AssetDatabase.ForceReserializeAssets(resourcesAssets);
    }
}
