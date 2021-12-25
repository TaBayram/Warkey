using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(HeightMapSettings), menuName = "World Generation/" + nameof(HeightMapSettings))]
public class HeightMapSettings : UpdateableData
{
    public NoiseSettings noiseSettings;

    public bool useFallOff;
    public float heightMultiplier;
    public AnimationCurve heightCurve;

    public float MinHeight {
        get {
            return heightMultiplier * heightCurve.Evaluate(0);
        }
    }

    public float MaxHeight {
        get {
            return heightMultiplier * heightCurve.Evaluate(1);
        }
    }

#if UNITY_EDITOR
    protected override void OnValidate() {
        noiseSettings.ValidateValues();
        base.OnValidate();
    }
#endif
}
