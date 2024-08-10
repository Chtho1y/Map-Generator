using UnityEngine;
using UnityEngine.UI;

namespace DarlingEngine.Main
{
    public class GameLoaderProgress : MonoBehaviour
    {
        public WavesProgress progress;

        // [SerializeField]
        // private Slider loadingSlider;
        // [SerializeField]
        // private Text loadingTip;

        public void UpdateLoadingProgress(long currentBytes, long totalBytes)
        {
            //loadingSlider.value = (float)currentBytes / totalBytes;

            float loadedMB = (float)currentBytes / 1024 / 1024;
            float allMB = (float)currentBytes / 1024 / 1024;
            float percent = (float)currentBytes / totalBytes * 100;

            progress.ProgressValue = percent;
            progress.ProgressInfo.text = string.Format("Now Loading... {0}%, {1}/{2}M", percent.ToString("f0"),
                loadedMB.ToString("f2"), allMB.ToString("f2"));
        }

        public void Begin()
        {
            transform.SetAsLastSibling();
            gameObject.SetActive(true);
            //loadingSlider.value = 0;
        }

        public void End()
        {
            transform.SetAsFirstSibling();
            gameObject.SetActive(false);
            //loadingSlider.value = 1;
        }
    }
}