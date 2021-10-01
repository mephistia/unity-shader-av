using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class AudioDataSetter : MonoBehaviour
{
    private AudioAnalyser audioAnalyser;
    private const int freqBandsCount = 10;
    private float[] freqBands;
    private float[] spectrum;

    public Material[] materials = new Material[freqBandsCount];
    public float scale = 10f;
    public UIManager uiManager;

    //public int samplesPerBand;
    int totalSamples;

    private void Start()
    {
        audioAnalyser = AudioAnalyser.Instance;
        freqBands = new float[freqBandsCount];
        totalSamples = audioAnalyser.Spectrum.Length;
        spectrum = new float[totalSamples];

        uiManager.videoPlayer.prepareCompleted += OnWillPlayAudio;

        foreach (Material mat in materials)
        {
            mat.SetFloat("TimeSpeed", 1f);
            mat.SetFloat("NoiseScale", 3f);
        }

        InvokeRepeating("UpdateFreqValues", 0.0f, 1.0f / 23f);
    }

    void OnWillPlayAudio(VideoPlayer player)
    {
        foreach (Material mat in materials)
        {
            mat.SetFloat("TimeSpeed", .1f);
        }
    }

    void UpdateFreqValues()
    {
        // atualiza o array com os samples
        spectrum = audioAnalyser.Spectrum;

        int currentBand = 0;
        int freqLimit = 2;

        // pra cada um dos samples
        for (int i = 0; i < totalSamples; i++)
        {
            float currentSpectrumVal = spectrum[i];
            float currentBandVal = freqBands[currentBand];

            // atribui à banda o valor maior (val atual ou do sample)
            freqBands[currentBand] = currentSpectrumVal > currentBandVal ? currentSpectrumVal : currentBandVal;

            // freq ultrapassa o limite? (próxima banda)
            if (i > (freqLimit - 3f))
            {
                currentBand++;
                freqLimit *= 2;

                float noiseValue = freqBands[currentBand] * scale;

                float min = 1f;
                float max = 100f;

                if (currentBand >= Mathf.FloorToInt(freqBandsCount / 2f))
                {
                    noiseValue *= scale / 2f;
                }

                if (noiseValue < min)
                {
                    noiseValue = min;
                }

                else if (noiseValue > max)
                {
                    noiseValue = max;
                }


                materials[currentBand].SetFloat("NoiseScale", noiseValue);
                

                freqBands[currentBand] = 0f;
            }


        }

    }

    //private void Update()
    //{
    //    spectrum = audioAnalyser.Spectrum;
    //    CreateFreqBands();
    //    UpdateShaderValues();        
    //}

    //void CreateFreqBands()
    //{
    //    // (64 samples por banda)

    //    // para cada banda desejada
    //    for (int i = 0; i < freqBandsCount; i++)
    //    {
    //        int offset = i * samplesPerBand;
    //        float[] rangedItems = new float[samplesPerBand];

    //        // dividir o spectrum
    //        System.Array.Copy(spectrum, offset, rangedItems, 0, samplesPerBand - 1);

    //        // e atribuir a média
    //        float average = rangedItems.Average();

    //        if (i > 0)
    //        {
    //            average *= (i + 1f) * 100f;
    //        }

    //        freqBands[i] = Mathf.Abs(average * scale);
    //    }


    //}

    //void UpdateShaderValues()
    //{
    //    for (int i = 0; i < freqBandsCount; i++)
    //    {
    //        float min = 0.35f;
    //        float max = 100f;

    //        float val;

    //        if (freqBands[i] < min)
    //        {
    //            val = min;
    //        }
    //        else if (freqBands[i] > max)
    //        {
    //            val = max;
    //        }
    //        else
    //        {
    //            val = freqBands[i];
    //        }

    //        float prev = materials[i].GetFloat("NoiseScale");
    //        float lerpValue = Mathf.Lerp(prev, val, Time.deltaTime);
    //        materials[i].SetFloat("NoiseScale", val);
    //    }
    //}

}
