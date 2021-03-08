using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

internal sealed class AssetImporter
    : AssetPostprocessor
{
    private const int PixelsPerUnit = 16;
    private const TextureImporterCompression textureCompression = TextureImporterCompression.Uncompressed;

    private const FilterMode filterMode = FilterMode.Point;
    private const int anisolevel = 1;
    private const int compressionQuality = 100;

    // This event is raised when a texture asset is imported
    private void OnPreprocessTexture()
    {
        var importer = assetImporter as TextureImporter;
        //This is a 2d project, probably a Sprite
        importer.textureType = TextureImporterType.Sprite;
        importer.isReadable = false;
        importer.filterMode = filterMode;
        importer.anisoLevel = anisolevel;
        importer.spritePixelsPerUnit = PixelsPerUnit;
        importer.compressionQuality = compressionQuality;
        importer.textureCompression = textureCompression;
        importer.alphaIsTransparency = importer.DoesSourceTextureHaveAlpha();
        importer.spritePivot = new Vector2(0, 0);
    }

}

#endif