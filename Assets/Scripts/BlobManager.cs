using System.Runtime.InteropServices;
using UnityEngine;

public static class BlobManager
{
#if UNITY_WEBGL && !UNITY_EDITOR
    // Import the JavaScript functions
    [DllImport("__Internal")]
    private static extern string CreateBlobURL(byte[] array, int size, string mimeType);

    [DllImport("__Internal")]
    public static extern void RevokeBlobURL(string url);

    // Example function to get a Texture2D from Unity and convert it to a blob URL
    public static string GetImageAsBlobUrl(Texture2D texture, string mimeType = "image/png")
    {
        byte[] textureBytes;

        switch (mimeType)
        {
            case "image/png":
                textureBytes = texture.EncodeToPNG();
                break;
            case "image/jpeg":
                textureBytes = texture.EncodeToJPG();
                break;
            default:
                Debug.LogError("Unsupported MIME type");
                return null;
        }

        string blobUrl = CreateBlobURL(textureBytes, textureBytes.Length, mimeType);
        return blobUrl;
    }

    public static string GetImageAsBlobUrl(byte[] textureBytes, string mimeType = "image/png")
    {
        string blobUrl = CreateBlobURL(textureBytes, textureBytes.Length, mimeType);
        return blobUrl;
    }
#endif
}
