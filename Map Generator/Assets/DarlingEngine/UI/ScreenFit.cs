using UnityEngine;


namespace DarlingEngine.UI
{
    public class ScreenFit : MonoBehaviour
    {
        public bool ReverseFit = false;
        //底部适配
        public bool BottomFit = true;

        void Awake()
        {
            SetScreenFit(GetComponent<RectTransform>());
        }

        void Start()
        {
            if (ReverseFit)
            {
                SetScreenFit(GetComponent<RectTransform>());
            }
        }

        public void SetScreenFit(RectTransform rect)
        {
            Vector3 offsetMax;
            Vector3 offsetMin;
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                    Rect[] cutou = Screen.cutouts;
                    if (cutou.Length > 0)
                    {
                        offsetMax = new Vector3(rect.offsetMax.x, -(cutou[0].height));
                        offsetMin = new Vector2(rect.offsetMin.x, (float)(cutou[0].height / 2.5));
                        //Debug.LogFormat("offsetMax:{0},offsetMin:{1},cutou:{2}", offsetMax, offsetMin, cutou[0]);
                        if (ReverseFit)
                        {
                            rect.offsetMax = new Vector2(offsetMax.x, -offsetMax.y);
                            if (BottomFit)
                                rect.offsetMin = new Vector2(offsetMin.x, -offsetMin.y);
                        }
                        else
                        {
                            rect.offsetMax = offsetMax;
                            if (BottomFit)
                                rect.offsetMin = offsetMin;
                        }
                    }
                    break;
                case RuntimePlatform.IPhonePlayer:
                    var phoneType = SystemInfo.deviceModel;
                    float spacingNum = 0;
                    if (phoneType == "iPhone12,1" || phoneType == "iPhone11,8")
                    {
                        spacingNum = 30;
                    }
                    offsetMax = new Vector3(rect.offsetMax.x, -(Screen.safeArea.y + spacingNum));
                    offsetMin = new Vector2(rect.offsetMin.x, (float)(Screen.safeArea.y / 2.5));
                    if (ReverseFit)
                    {
                        rect.offsetMax = new Vector2(offsetMax.x, -offsetMax.y);
                        if (BottomFit)
                            rect.offsetMin = new Vector2(offsetMin.x, -offsetMin.y);
                    }
                    else
                    {
                        rect.offsetMax = offsetMax;
                        if (BottomFit)
                            rect.offsetMin = offsetMin;
                    }
                    break;
            }
        }
    }
}

