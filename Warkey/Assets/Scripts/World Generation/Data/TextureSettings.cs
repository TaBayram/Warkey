using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu()]
public class TextureSettings : UpdateableData
{
    const int textureSize = 512;
    const TextureFormat textureFormat = TextureFormat.RGB565;

    public Gradient gradient;
    public List<Layer> layers = new List<Layer>();

    Texture2D texture2D;
    const int textureResolution = 50;
    float savedMinHeight;
    float savedMaxHeight;

    [Range(0, 1)]
    public float textureStrength;
    public float textureScale;

    public bool resetLayers;

    public void ApplyToMaterial(Material material) {
        if(texture2D == null) {
            texture2D = new Texture2D(textureResolution, 1);
        }
        material.SetVector("_minmax", new Vector4(savedMinHeight, savedMaxHeight));
        UpdateColor(material);
    }

    public void UpdateMeshHeights(Material material, float minHeight, float maxHeight) {
        savedMinHeight = minHeight;
        savedMaxHeight = maxHeight;

        material.SetVector("_minmax", new Vector4(savedMinHeight, savedMaxHeight));
    }

    public void UpdateColor(Material material) {
        Color[] colors = new Color[textureResolution];
        for(int i = 0; i < textureResolution; i++) {
            colors[i] = gradient.Evaluate(i / (textureResolution - 1f));
        }
        texture2D.SetPixels(colors);
        texture2D.Apply();
        material.SetTexture("_colortexture", texture2D);
        Texture2D[] texture2Ds = (LayerTexturesArray());
        if(texture2Ds.Length != 0) {
            material.SetTexture("_textures", GenerateTextureArray(texture2Ds));
            material.SetFloat("_texturesCount", (texture2Ds).Length);
        }
        material.SetFloat("_texturesScale", textureScale);
        material.SetFloat("_texturesStrength", textureStrength);

    }

    private Texture2D[] LayerTexturesArray() {
        return ((from layer in layers where layer.texture != null select layer.texture)).ToArray();
    }

    private Texture2DArray GenerateTextureArray(Texture2D[] textures) {
        Texture2DArray texture2DArray = new Texture2DArray(textureSize, textureSize, textures.Length, textureFormat, true);
        for (int i = 0; i < textures.Length; i++) {
            texture2DArray.SetPixels(textures[i].GetPixels(), i);
        }
        texture2DArray.Apply();
        return texture2DArray;
    }

    private void ValidateValues() {
        if (!resetLayers) return;
        List<Layer> newLayers = new List<Layer>();

        for(int i = 0; i < gradient.colorKeys.Length; i++) {
            GradientColorKey colorKey = gradient.colorKeys[i];
            bool exists = false;
            foreach(Layer layer in layers) {
                if(colorKey.color == layer.colorKey.color && colorKey.time == layer.colorKey.time) {
                    exists = true;
                    newLayers.Add(layer);
                    break;
                }
            }
            if (!exists) {
                newLayers.Add(new Layer(colorKey));
            }
        }

        newLayers.Sort((a,b)=>(int)(a.colorKey.time - b.colorKey.time));

        layers = newLayers;
    }


#if UNITY_EDITOR
    protected override void OnValidate() {
        ValidateValues();
        base.OnValidate();
    }
#endif

    [System.Serializable]
    public class Layer
    {
        [Range(0,1)]
        public float time;
        public readonly GradientColorKey colorKey;
        public Texture2D texture;
        /*[Range(0, 1)]
        public float textureStrength;
        public float textureScale;*/

        public Layer(GradientColorKey colorKey) {
            this.colorKey = colorKey;
            time = this.colorKey.time;
        }
    }
}
