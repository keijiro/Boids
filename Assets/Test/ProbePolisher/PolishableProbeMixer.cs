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
using System.Collections;

// Interpolates two light probes.
public class PolishableProbeMixer : MonoBehaviour
{
    // Input light probes.
    public LightProbes[] sourceProbes;

    // Mixing parameters.
    public float mix = 0.0f;
    public float intensity = 1.0f;
    float prevMix = -1.0f;
    float prevIntensity = -1.0f;

    // Skybox parameters.
    public bool updateSkybox = false;
    public float skyboxIntensity = 1.0f;
    float prevSkyboxIntensity;

    // Temporary objects.
    LightProbes probe;
    Material skybox;

    void Awake ()
    {
        if (enabled) Update();
    }

    void Update ()
    {
        // Do nothing if nothing was changed.
        if (mix == prevMix && intensity == prevIntensity && skyboxIntensity == prevSkyboxIntensity) return;

        if (probe == null)
        {
            // Clone the first probe and set it to the scene.
            probe = Instantiate(sourceProbes[0]) as LightProbes;
            LightmapSettings.lightProbes = probe;
        }

        if (updateSkybox && skybox == null)
        {
            // Make a empty skybox and set it to the scene.
            skybox = ProbePolisher.NewSkyboxMaterial(null);
            RenderSettings.skybox = skybox;
        }

        // Clamp the mixing position.
        mix = Mathf.Clamp(mix, 0.0f, 1.0f * (sourceProbes.Length - 1));

        // Choose two probes to mix.
        var mixIndex = Mathf.FloorToInt(mix);
        if (mixIndex == sourceProbes.Length - 1) mixIndex--;

        var source1 = ProbePolisher.NewVectorArrayFromCoeffs(sourceProbes[mixIndex + 0].coefficients, 27 * 2);
        var source2 = ProbePolisher.NewVectorArrayFromCoeffs(sourceProbes[mixIndex + 1].coefficients, 27 * 2);

        // Mix the two probes.
        var coeffs = new Vector3[9];
        var mixRate = mix - mixIndex;
        for (var i = 0; i < 9; i++)
            coeffs[i] = Vector3.Lerp(source1[i], source2[i], mixRate) * intensity;

        // Update the probe with the mixed coefficients.
        var temp = probe.coefficients;
        ProbePolisher.UpdateCoeffsWithVectorArray(temp, coeffs);
        probe.coefficients = temp;

        // Update the skybox if needed.
        if (updateSkybox && skybox != null)
        {
            ProbePolisher.UpdateSkyboxMaterial(skybox, coeffs);
            skybox.SetFloat("_Intensity", skyboxIntensity);
        }

        prevMix = mix;
        prevIntensity = intensity;
        prevSkyboxIntensity = skyboxIntensity;
    }
}
