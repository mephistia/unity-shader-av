using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AudioDataSetter : MonoBehaviour
{
    private AudioAnalyser audioAnalyser;
    private const int freqBandsCount = 8;
    private float[] freqBands;
    private float[] spectrum;

    public Material[] materials = new Material[freqBandsCount];
    public float scale = 10f;

    public int samplesPerBand;

    private void Start()
    {
        audioAnalyser = AudioAnalyser.Instance;
        freqBands = new float[freqBandsCount];

        samplesPerBand = audioAnalyser.Spectrum.Length / freqBandsCount;

        foreach(Material mat in materials)
        {
            mat.SetFloat("TimeSpeed", 0.5f);
            mat.SetFloat("NoiseScale", 1f);
        }
    }

    private void Update()
    {
        spectrum = audioAnalyser.Spectrum;
        CreateFreqBands();
        UpdateShaderValues();        
    }

    void CreateFreqBands()
    {
        // (64 samples por banda)

        // para cada banda desejada
        for (int i = 0; i < freqBandsCount; i++)
        {
            int offset = i * samplesPerBand;
            float[] rangedItems = new float[samplesPerBand];

            // dividir o spectrum
            System.Array.Copy(spectrum, offset, rangedItems, 0, samplesPerBand - 1);

            // e atribuir a média
            float average = rangedItems.Average();

            if (i > 0)
            {
                average *= (i + 1f) * 100f;
            }

            freqBands[i] = Mathf.Abs(average * scale);
        }

    
    }

    void UpdateShaderValues()
    {
        for (int i = 0; i < freqBandsCount; i++)
        {
            float min = 0.35f;
            float max = 100f;

            float val;

            if (freqBands[i] < min)
            {
                val = min;
            }
            else if (freqBands[i] > max)
            {
                val = max;
            }
            else
            {
                val = freqBands[i];
            }

            float prev = materials[i].GetFloat("NoiseScale");
            float lerpValue = Mathf.Lerp(prev, val, Time.deltaTime);
            materials[i].SetFloat("NoiseScale", lerpValue);
        }
    }

}
