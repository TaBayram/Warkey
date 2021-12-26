using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPreview : MonoBehaviour
{

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureSettings textureData;
    public GroundSettings groundSettings;
    public PathSettings pathSettings;
    private List<EnviromentObjectData> enviromentObjectDatas = new List<EnviromentObjectData>();
    public enum DrawMode { NoiseMap, DrawMesh, FallOff };
    public DrawMode drawMode;
    public Material terrainMaterial;
    [Range(0, MeshSettings.lodCount - 1)]
    public int editorLOD;
    public bool autoUpdate;

    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public MeshFilter meshWaterFilter;
    public MeshRenderer meshWaterRenderer;

    public MeshFilter meshPathFilter;
    public MeshRenderer meshPathRenderer;

    private void Start() {
        gameObject.SetActive(false);
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
    public void DrawWaterMesh(MeshData meshData) {
        meshWaterFilter.sharedMesh = meshData.CreateMesh();
        meshWaterFilter.gameObject.transform.position = Vector3.up * meshSettings.height;

        textureRenderer.gameObject.SetActive(false);
        meshWaterFilter.gameObject.SetActive(true);
    }

    public void DrawPathMesh(MeshData meshData) {
        meshPathFilter.sharedMesh = meshData.CreateMesh();

        meshPathFilter.gameObject.SetActive(true);
    }

    public void DrawMapInEditor() {
        textureData.ApplyToMaterial(terrainMaterial);
        textureData.UpdateMeshHeights(terrainMaterial, heightMapSettings.MinHeight, heightMapSettings.MaxHeight);
        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.VerticesPerLineCount, meshSettings.VerticesPerLineCount, heightMapSettings, Vector2.zero,Vector2.zero,null);
        
        if (drawMode == DrawMode.NoiseMap)
            DrawTexture(TextureGenerator.CreateTexture(heightMap));
        else if (drawMode == DrawMode.DrawMesh) {
            DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, editorLOD));
            DrawWaterMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values01, meshSettings, editorLOD, meshSettings.minValue, meshSettings.maxValue));
            
        }
        else if (drawMode == DrawMode.FallOff)
            DrawTexture(TextureGenerator.CreateTexture(new HeightMap(FallOffGenerator.GenerateFalloffMap(meshSettings.VerticesPerLineCount, meshSettings.VerticesPerLineCount),0,1)));
        textureData.ApplyToMaterial(terrainMaterial);

        for(int i = meshFilter.transform.childCount-1; i >= 0; --i) {
            GameObject.DestroyImmediate(meshFilter.transform.GetChild(i).gameObject);
        }
        enviromentObjectDatas.Clear();
        
        for(int i = 0; i < groundSettings.enviromentObjects.Length; i++) {
            if (groundSettings.enviromentObjects[i].enabled) {
                List<ValidPoint> grid = EnviromentObjectGenerator.GenerateValidPoints(groundSettings.enviromentObjects[i], heightMap.values01, groundSettings.poissonDiscSettings,Vector2.zero);
                EnviromentObjectData  enviromentObjectData = new EnviromentObjectData(grid, groundSettings.enviromentObjects[i], meshFilter.transform, heightMap.values,groundSettings.poissonDiscSettings.seed);
                enviromentObjectData.CreateObjects(true);
                enviromentObjectDatas.Add(enviromentObjectData);
            }
        }


        PathData pathData = PathGenerator.GeneratePath(pathSettings, heightMap, start, direction);

        DrawPathMesh(MeshGenerator.GenerateTerrainMesh(pathData.heightMap, meshSettings, editorLOD,0,float.MaxValue));

    }

    public int seed;
    public Vector2 start;
    public Vector2 direction;

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
        if (pathSettings != null) {
            pathSettings.OnValuesUpdated -= OnValuesUpdated;
            pathSettings.OnValuesUpdated += OnValuesUpdated;
        }
    }
}
