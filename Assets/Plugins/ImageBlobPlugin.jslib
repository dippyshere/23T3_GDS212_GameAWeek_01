mergeInto(LibraryManager.library, {
    // Function to create a blob from a byte array (Uint8Array) and return a blob URL
    CreateBlobURL: function (array, size, mimeTypeStr) {
        var mimeType = UTF8ToString(mimeTypeStr);
        var bytes = new Uint8Array(size);

        // Copy data from the Unity heap to a local Uint8Array
        for (var i = 0; i < size; i++) {
            bytes[i] = HEAPU8[array + i];
        }

        var blob = new Blob([bytes], { type: mimeType });
        var blobUrl = URL.createObjectURL(blob);
        return stringToNewUTF8(blobUrl); // Return the URL string to Unity
    },

    // Function to revoke the blob URL when it's no longer needed (for memory management)
    RevokeBlobURL: function (urlStr) {
        var url = UTF8ToString(urlStr);
        URL.revokeObjectURL(url);
    }
});
