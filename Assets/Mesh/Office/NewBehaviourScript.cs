using UnityEditor;
using UnityEngine;

public class MissingAssetFinder : MonoBehaviour
{
    [MenuItem("Tools/Find Missing Assets")]
    public static void FindMissingAssets()
    {
        string missingGuid = "213211111113"; // Your missing GUID
        string assetPath = AssetDatabase.GUIDToAssetPath(missingGuid);

        if (string.IsNullOrEmpty(assetPath))
        {
            Debug.Log("Missing asset not found with GUID: " + missingGuid);
        }
        else
        {
            Debug.Log("Asset found at path: " + assetPath);
        }
    }
}
