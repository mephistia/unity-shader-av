using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAnalyser : Singleton<AudioAnalyser>
{
    protected AudioAnalyser() { }

    private float[] spectrum = new float[512];

    public float[] Spectrum
    {
        get
        {
            return this.spectrum;
        }
    }

    private void Update()
    {
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Blackman);
    }
}
