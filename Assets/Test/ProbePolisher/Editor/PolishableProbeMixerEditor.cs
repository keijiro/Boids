//
// ProbePolisher - Light Probe Editor Plugin for Unity
//
// Copyright (C) 2014 Keijiro Takahashi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using UnityEngine;
using UnityEditor;
using System.Collections;

// Custom editor for the probe mixer.
[CustomEditor(typeof(PolishableProbeMixer))]
class PolishableProbeMixerEditor : Editor
{
    SerializedProperty propSourceProbes;
    SerializedProperty propMix;
    SerializedProperty propIntensity;
    SerializedProperty propUpdateSkybox;
    SerializedProperty propSkyboxIntensity;

    void OnEnable()
    {
        propSourceProbes = serializedObject.FindProperty("sourceProbes");
        propMix = serializedObject.FindProperty("mix");
        propIntensity = serializedObject.FindProperty("intensity");
        propUpdateSkybox = serializedObject.FindProperty("updateSkybox");
        propSkyboxIntensity = serializedObject.FindProperty("skyboxIntensity");
    }

    override public void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(propSourceProbes, true);

        EditorGUILayout.Space();

        EditorGUILayout.Slider(propMix, 0.0f, propSourceProbes.arraySize - 1, "Mix");
        EditorGUILayout.Slider(propIntensity, 0.0f, 10.0f);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(propUpdateSkybox);
        if (propUpdateSkybox.boolValue)
            EditorGUILayout.Slider(propSkyboxIntensity, 0.0f, 10.0f);

        serializedObject.ApplyModifiedProperties();
    }
}
