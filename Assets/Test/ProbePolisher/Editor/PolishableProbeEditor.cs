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

// Custom editor for polishable light probes.
[CustomEditor(typeof(LightProbes))]
class PolishableProbeEditor : Editor
{
    // Inspector GUI function.
    override public void OnInspectorGUI()
    {
        if (ProbePolisher.CheckPolishable(target as LightProbes))
        {
            EditorGUILayout.HelpBox("This is a polishable LightProbes asset.", MessageType.None);
            ShowPolisherGUI(target as LightProbes);
        }
        else
        {
            EditorGUILayout.HelpBox("This is not a polishable LightProbes asset.", MessageType.None);
            base.OnInspectorGUI();
        }
    }

    // Inspector GUI function for polishable probes.
    void ShowPolisherGUI(LightProbes probes)
    {
        ProbePolisher.ResetPolisherIfNeeded(probes);
        
        var coeffs = probes.coefficients;

        // Retrieve the optional information from the second probe.
        var baseIntensity = coeffs[28];
        var skyIntensity = coeffs[29];
        var skyColor1 = new Color(coeffs[30], coeffs[31], coeffs[32]);
        var skyColor2 = new Color(coeffs[33], coeffs[34], coeffs[35]);
        var yaw = coeffs[36];

        EditorGUI.BeginChangeCheck();

        // Show the controls.
        baseIntensity = EditorGUILayout.Slider("Base Intensity", baseIntensity, 0.0f, 10.0f);
        skyIntensity = EditorGUILayout.Slider("Sky Intensity", skyIntensity, 0.0f, 10.0f);
        skyColor1 = EditorGUILayout.ColorField("Sky Color Top", skyColor1);
        skyColor2 = EditorGUILayout.ColorField("Sky Color Bottom", skyColor2);
        yaw = EditorGUILayout.Slider("Rotation (Y-Axis)", yaw, -180.0f, 180.0f);

        if (EditorGUI.EndChangeCheck())
        {
            // Update the optional information.
            coeffs[28] = baseIntensity;
            coeffs[29] = skyIntensity;
            coeffs[30] = skyColor1.r;
            coeffs[31] = skyColor1.g;
            coeffs[32] = skyColor1.b;
            coeffs[33] = skyColor2.r;
            coeffs[34] = skyColor2.g;
            coeffs[35] = skyColor2.b;
            coeffs[36] = yaw;

            // Retrieve the SH coefficients from the first probe.
            var sh = ProbePolisher.NewVectorArrayFromCoeffs(coeffs, 0);

            // Apply the intensity value.
            for (var i = 0; i < 9; i++) sh[i] *= baseIntensity;

            // Apply y-axis rotation.
            sh = ProbePolisher.RotateSHCoeffs(sh, yaw);

            // Add the skylight.
            var sky1 = ProbePolisher.ColorToVector3(skyColor1) * skyIntensity;
            var sky2 = ProbePolisher.ColorToVector3(skyColor2) * skyIntensity;
            sh[0] += (sky1 + sky2) * 0.5f;
            sh[1] += (sky2 - sky1) * 0.5f;

            // Copy the coefficients to the all probes.
            ProbePolisher.UpdateCoeffsWithVectorArray(coeffs, sh);

            // Update the asset.
            probes.coefficients = coeffs;
        }

        EditorGUILayout.Space ();

        // Skybox generator button.
        if (GUILayout.Button ("Make/Update Skybox"))
        {
            // Look for the asset of the material.
            var probePath = AssetDatabase.GetAssetPath(target);
            var assetPath = probePath.Substring (0, probePath.Length - 6) + " skybox.mat";
            var material = AssetDatabase.LoadAssetAtPath (assetPath, typeof(Material)) as Material;

            // If there is no asset, make a new one.
            if (material == null)
            {
                material = new Material (Shader.Find ("ProbePolisher/SH Skybox"));
                AssetDatabase.CreateAsset (material, assetPath);
            }

            // Update the material.
            ProbePolisher.UpdateSkyboxMaterial(material, ProbePolisher.NewVectorArrayFromCoeffs(coeffs, 27 * 2));
        }
    }
}

// Cutom tool for baking polishable light probes.
static class ProbeBakingJig
{
    [MenuItem("GameObject/Create Other/Baking Jig")]
    static void CreateBakingJig ()
    {
        var go = GameObject.CreatePrimitive (PrimitiveType.Sphere);
        go.name = "Baking Jig";
        var group = go.AddComponent<LightProbeGroup> ();
        Object.DestroyImmediate (go.GetComponent<SphereCollider> ());
        var positions = new Vector3[]{
            new Vector3 (0, 20000, 0),
            new Vector3 (0, 40000, 0),
            new Vector3 (-10000, -10000, -10000),
            new Vector3 (+10000, -10000, -10000),
            new Vector3 (-10000, +10000, -10000),
            new Vector3 (+10000, +10000, -10000),
            new Vector3 (-10000, -10000, +10000),
            new Vector3 (+10000, -10000, +10000),
            new Vector3 (-10000, +10000, +10000),
            new Vector3 (+10000, +10000, +10000)
        };
        group.probePositions = positions;
        go.isStatic = true;
    }
}
