using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateableData : ScriptableObject
{
    public event System.Action OnValuesUpdated;
    public bool autoUpdate;

#if UNITY_EDITOR

    protected virtual void OnValidate() {
        if (autoUpdate) {
            UnityEditor.EditorApplication.update += NotifyUpdateValues;

        }
    }

    public void NotifyUpdateValues() {
        UnityEditor.EditorApplication.update -= NotifyUpdateValues;
        if (OnValuesUpdated != null) {
            OnValuesUpdated();
        }
    }
#endif
}
