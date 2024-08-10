using System.Collections;
using System.Collections.Generic;
using DarlingEngine.Engine;
using DarlingEngine.Main;
using UnityEngine;
using UnityEngine.UI;

public class WavesProgress : MonoBehaviour
{
    public WavesUI wavesBack;
    public WavesUI wavesFront;
    public Text ProgressInfo;
    public float ProgressValue = 0;
    public int lerpSpeed = 5;

    const float MAX_WAVE_HEIGHT = 60;
    const float MIN_WAVE_HEIGHT = 30;

    RectTransform rectBack;
    RectTransform rectFront;
    float CanvasHeight = 0f;
    float currentProgress = 0f;


    void Start()
    {
        wavesBack = transform.GetChild(0).GetComponent<WavesUI>();
        wavesFront = transform.GetChild(1).GetComponent<WavesUI>();
        ProgressInfo = transform.GetChild(2).GetComponent<Text>();

        CanvasHeight = GameRoot.Instance.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y - 100f;
    }

    void Update()
    {
        currentProgress = Mathf.Lerp(currentProgress, ProgressValue, Time.deltaTime * lerpSpeed);
        wavesFront.wavesHeight = Mathf.Lerp(MIN_WAVE_HEIGHT, MAX_WAVE_HEIGHT, currentProgress / 100f);
        wavesBack.wavesHeight = Mathf.Lerp(MIN_WAVE_HEIGHT, MAX_WAVE_HEIGHT, currentProgress / 100f);

        if (rectBack == null) rectBack = wavesBack.GetComponent<RectTransform>();
        if (rectFront == null) rectFront = wavesFront.GetComponent<RectTransform>();

        float y = Mathf.Lerp(-400, CanvasHeight, currentProgress / 100f);

        Vector3 pos = rectFront.position;
        pos.y = y;
        rectFront.anchoredPosition = pos;
        rectBack.anchoredPosition = pos;
    }
}
