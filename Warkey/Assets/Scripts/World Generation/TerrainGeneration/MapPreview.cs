using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPreview : MonoBehaviour
{

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureData;
    public GroundSettings groundSettings;
    private EnviromentObjectData enviromentObjectData;
    public enum DrawMode { NoiseMap, DrawMesh, FallOff };
    public DrawMode drawMode;
    public Material terrainMaterial;
    [Range(0, MeshSettings.lodCount - 1)]
    public int editorLOD;
    public bool autoUpdate;

    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    private void Start() {
        foreach (Transform child in meshFilter.transform) {
            GameObject.DestroyImmediate(child.gameObject);
        }
    }

    public void DrawTexture(Texture2D texture){
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height)/10f;

        textureRenderer.gameObject.SetActive(true);
        meshFilter.gameObject.SetActive(false);
    }

    public void DrawMesh(MeshData meshData) {
        meshFilter.sharedMesh = meshData.CreateMesh();

        textureRenderer.gameObject.SetActive(false);
        meshFilter.gameObject.SetActive(true);
    }


    public void DrawMapInEditor() {
        textureData.ApplyToMaterial(terrainMaterial);
        textureData.UpdateMeshHeights(terrainMaterial, heightMapSettings.MinHeight, heightMapSettings.MaxHeight);
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.VerticesPerLineCount, meshSettings.VerticesPerLineCount, heightMapSettings, Vector2.zero);
        
        if (drawMode == DrawMode.NoiseMap)
            DrawTexture(TextureGenerator.CreateTexture(heightMap));
        else if (drawMode == DrawMode.DrawMesh)
            DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, editorLOD));
        else if (drawMode == DrawMode.FallOff)
            DrawTexture(TextureGenerator.CreateTexture(new HeightMap(FallOffGenerator.GenerateFalloffMap(meshSettings.VerticesPerLineCount),0,1)));
        textureData.ApplyToMaterial(terrainMaterial);


        
        List<Vector2> grid = EnviromentObjectGenerator.GenerateEnviroment(groundSettings.enviromentObjects[0], heightMap.values01, groundSettings.poissonDiscSettings);
        if(enviromentObjectData != null) {
            enviromentObjectData.DestroyObjects();
        }
        enviromentObjectData = new EnviromentObjectData(grid, groundSettings.enviromentObjects[0],gameObject.transform);
        enviromentObjectData.CreateObjects(heightMap.values, meshFilter.gameObject.transform);

    }


    private void OnValuesUpdated() {
        if (!Application.isPlaying) {
            DrawMapInEditor();
        }
    }

    private void OnTextureValuesUpdated() {
        textureData.ApplyToMaterial(terrainMaterial);
    }



    private void OnValidate() {
        if (meshSettings != null) {
            meshSettings.OnValuesUpdated -= OnValuesUpdated;
            meshSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (heightMapSettings != null) {
            heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
            heightMapSettings.OnValuesUpdated += OnValuesUpdated;
        }
        if (textureData != null) {
            textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }
        if (groundSettings != null) {
            groundSettings.OnValuesUpdated -= OnValuesUpdated;
            groundSettings.OnValuesUpdated += OnValuesUpdated;
        }
    }
}
