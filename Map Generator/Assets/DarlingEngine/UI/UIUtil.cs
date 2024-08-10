using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DarlingEngine.Engine;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;


namespace DarlingEngine.UI
{
    public static class UIUtil
    {
        /// <summary>
        /// UI转换锚点
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="anchorCenter"></param>
        /// <returns></returns>
        public static Vector2 ResetAnchor(RectTransform rt, Vector2 anchorCenter)
        {
            //计算原锚点中心和组件大小
            Vector2 oldAnchorCenter = (rt.anchorMin + rt.anchorMax) / 2;
            Vector2 oldAnchoredPos = rt.anchoredPosition;
            //更改锚点
            rt.anchorMin = anchorCenter;
            rt.anchorMax = anchorCenter;
            rt.pivot = anchorCenter;
            //计算偏移量
            RectTransform rectTransFather = rt.parent as RectTransform; //获取父对象
            Vector2 deltaAnchor = anchorCenter - oldAnchorCenter;
            Vector2 Offset = new Vector2(deltaAnchor.x * rectTransFather.rect.width,
                deltaAnchor.y * rectTransFather.rect.height);
            return rt.anchoredPosition = oldAnchoredPos - Offset; //调整距离锚点的位置
        }

        /// <summary>
        /// 获取键盘弹出Canvas的偏移量
        /// </summary>
        /// <param name="panelRectTrans"></param>
        /// <returns></returns>
        public static float KeyboardOffset2Canvas(RectTransform panelRectTrans)
        {
            float keyBoard2DisplayRatio = GetKeyboardHeightRatio(true);
            float keyBoardRectHeight = panelRectTrans.rect.height * keyBoard2DisplayRatio;
            return keyBoardRectHeight;
            //KeyBoardRectHeight - panelRectTrans.rect.height / 2
            //E.G. InputField.anchoredPosition = new Vector2(0f, KeyboardOffset2Canvas());
        }

        /// <summary>
        /// 获取键盘高度（像素）对屏幕像素的比值
        /// </summary>
        public static float GetKeyboardHeightRatio(bool includeInput)
        {
            return Mathf.Clamp01((float)GetKeyboardHeight(includeInput) / Display.main.systemHeight);
        }

        /// <summary>
        /// 获取键盘高度 (以像素为单位)
        /// </summary>
        public static int GetKeyboardHeight(bool includeInput)
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXEditor:
                    return 0;//Display.main.systemHeight / 2;
                case RuntimePlatform.Android:
                    using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                    {
                        var unityPlayer = unityClass.GetStatic<AndroidJavaObject>("currentActivity")
                            .Get<AndroidJavaObject>("mUnityPlayer");
                        var view = unityPlayer.Call<AndroidJavaObject>("getView");
                        var dialog = unityPlayer.Get<AndroidJavaObject>("mSoftInputDialog");

                        if (view == null || dialog == null) return 0;
                        var decorHeight = 0;

                        if (includeInput)
                        {
                            var decorView = dialog.Call<AndroidJavaObject>("getWindow")
                                .Call<AndroidJavaObject>("getDecorView");
                            if (decorView != null) decorHeight = decorView.Call<int>("getHeight");
                        }

                        using (var rect = new AndroidJavaObject("android.graphics.Rect"))
                        {
                            view.Call("getWindowVisibleDisplayFrame", rect);
                            return Display.main.systemHeight - rect.Call<int>("height") + decorHeight;
                        }
                    }
                case RuntimePlatform.IPhonePlayer:
                    var height = Mathf.RoundToInt(TouchScreenKeyboard.area.height);
                    return height >= Display.main.systemHeight ? 0 : height;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// 设置一个父物体下所有UI的Alpha值 （可使用DoFade代替）
        /// </summary>
        /// <param name="go"></param>
        /// <param name="alpha"></param>
        /// <param name="time"></param>
        /// <param name="includeSelf"></param>
        /// <param name="ignoreTimeScale"></param>
        public static void SetAlpha(GameObject go, float alpha, float seconds, bool includeSelf = true, bool ignoreTimeScale = false)
        {
            if (go == null) return;
            Graphic[] graphics = go.GetComponentsInChildren<Graphic>();
            if (includeSelf)
            {
                go.TryGetComponent<Graphic>(out Graphic graphicComponents);
                if (graphicComponents != null) graphicComponents.CrossFadeAlpha(alpha, seconds, ignoreTimeScale);
            }
            foreach (var graphic in graphics)
            {
                graphic?.CrossFadeAlpha(alpha, seconds, ignoreTimeScale);
            }
        }

        public static void ShowUI(GameObject go, float seconds, bool needSetActive = false)
        {
            if (needSetActive) go.SetActive(true);
            var graphics = go.GetComponentsInChildren<Graphic>();
            foreach (var graphic in graphics)
            {
                TweenUtil.DoFade(graphic, 1, seconds);
            }
        }

        public static void HideUI(GameObject go, float seconds, bool needSetActive = false)
        {
            var graphics = go.GetComponentsInChildren<Graphic>();
            foreach (var graphic in graphics)
            {
                TweenUtil.DoFade(graphic, 0, seconds);
            }
            if (needSetActive)
            {
                GameManager.Instance.WaitTime2Do(seconds, () => { go.SetActive(false); });
            }
        }

        /// <summary>
        /// 物体的RectTransform是否在某个UI视窗内(如ScrollRect)
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="viewport"></param>
        /// <returns></returns>
        public static bool IsRectTransformInsideScreen(RectTransform rectTransform, RectTransform viewport)
        {
            Camera camera = GameManager.Instance.GetUICamera();

            // 计算 viewport 的屏幕空间矩形
            Vector3[] viewportCorners = new Vector3[4];
            viewport.GetWorldCorners(viewportCorners);
            Vector3 viewportMin = camera.WorldToScreenPoint(viewportCorners[0]);
            Vector3 viewportMax = camera.WorldToScreenPoint(viewportCorners[2]);
            Rect viewportRect = new Rect(viewportMin.x, viewportMin.y, viewportMax.x - viewportMin.x, viewportMax.y - viewportMin.y);

            // 检查 rectTransform 的每个角是否至少有一个在视窗内
            Vector3[] rectCorners = new Vector3[4];
            rectTransform.GetWorldCorners(rectCorners);
            foreach (var corner in rectCorners)
            {
                Vector3 screenPoint = camera.WorldToScreenPoint(corner);
                if (viewportRect.Contains(screenPoint))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 从URL加载一张图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="img"></param>
        /// <param name="fitSize"></param>
        public static void DownloadAndSaveTextureThenLoadImg(string url, Image img, string fileName, bool fitSize = false)
        {
            if (url == String.Empty) return;
            GameManager.Instance.StartCoroutine(UIUtilHelper.Instance.DownloadAndSaveTextureThenLoadImg(url, img, fileName, fitSize));
        }

        public static void DownloadAndSaveTexture(string url, string fileName)
        {
            GameManager.Instance.StartCoroutine(UIUtilHelper.Instance.DownloadAndSaveTexture(url, fileName));
        }

        public static Texture2D LoadTextureFromPersistentData(string fileName)
        {
            Texture2D texture = null;
            string path = Path.Combine(Application.persistentDataPath, fileName);

            if (File.Exists(path))
            {
                byte[] bytes = File.ReadAllBytes(path);
                texture = new Texture2D(2, 2);
                texture.LoadImage(bytes);
                return texture;
            }
            else
            {
                return null;
            }
        }

        public static bool IsTextureExistsInPersistentData(string fileName)
        {
            return File.Exists(Path.Combine(Application.persistentDataPath, fileName));
        }

        public static void ImgFitSize(Image img, Texture2D texture, bool fitSize = false)
        {
            UIUtilHelper.Instance.ImgFitSize(img, texture, fitSize);
        }

        public static readonly string DARLING003POLICYURL = "https://app.darling003.com/privacy-policy";
        /// <summary>
        /// 跳转外部URL
        /// </summary>
        /// <param name="URL"></param>
        public static void Jump2Browser(string URL)
        {
            var webRequest = new UnityWebRequest(URL)
            {
                certificateHandler = new UnityWebRequestCertify()
            };
            Application.OpenURL(webRequest.url);
        }

        private static Coroutine slideCor;
        /// <summary>
        /// 开始监听向右滑动手势
        /// </summary>
        /// <param name="handler"></param>
        public static void StartListenSlideInput(Action handler)
        {
            slideCor = GameManager.Instance.StartCoroutine(UIUtilHelper.Instance.ListenSlideInput(handler));
        }
        /// <summary>
        /// 结束监听向右滑动手势
        /// </summary>
        public static void StopListenSlideInput()
        {
            GameManager.Instance.StopCoroutine(slideCor);
        }

        public static Queue<string> TipsText = new Queue<string>();
        public static string GetTips()
        {
            if (TipsText.Count != 0)
            {
                return TipsText.Dequeue();
            }
            return null;
        }


        #region UIUtilHelper

        public class UIUtilHelper
        {
            public static readonly UIUtilHelper Instance = new();

            public IEnumerator DownloadAndSaveTexture(string url, string fileName)
            {
                string path = Path.Combine(Application.persistentDataPath, fileName);

                // 如果文件已经存在，就不下载
                if (File.Exists(path))
                {
#if UNITY_EDITOR
                    Debug.Log("File already exists at: " + path);
#endif
                    yield break;
                }

                using UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);
                webRequest.certificateHandler = new UnityWebRequestCertify();
                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(webRequest.error);
                }
                else
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);

                    // 将纹理保存为 PNG or JPG
                    byte[] bytes = texture.EncodeToPNG(); // .EncodeToJPG()
                    File.WriteAllBytes(path, bytes);
                }
            }

            public IEnumerator DownloadAndSaveTextureThenLoadImg(string url, Image img, string fileName, bool fitSize)
            {
                string path = Path.Combine(Application.persistentDataPath, fileName);

                // 如果文件已经存在，就不下载
                if (File.Exists(path))
                {
#if UNITY_EDITOR
                    Debug.Log("File already exists at: " + path);
#endif
                    // 直接从文件加载
                    Texture2D texture = LoadTextureFromPersistentData(fileName);

                    ImgFitSize(img, texture, fitSize);
                    UnityEngine.Object.Destroy(texture);
                    yield break;
                }

                using UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url);
                webRequest.certificateHandler = new UnityWebRequestCertify();
                yield return webRequest.SendWebRequest();

                if (webRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log(webRequest.error);
                }
                else
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);

                    byte[] bytes = texture.EncodeToPNG();
                    File.WriteAllBytes(path, bytes);

                    ImgFitSize(img, texture, fitSize);

                    UnityEngine.Object.Destroy(texture);
                }
            }


            public void ImgFitSize(Image img, Texture2D texture, bool fitSize)
            {
                var rect = img.rectTransform.rect;
                if (fitSize)
                {
                    // 获取texture的宽和高
                    int textureWidth = texture.width;
                    int textureHeight = texture.height;

                    // 获取image的宽和高
                    float imageWidth = rect.width;
                    float imageHeight = rect.height;

                    // 计算按照高度适应的缩放比例
                    float scale = imageHeight / textureHeight;

                    // 根据缩放比例计算缩放后的宽和高
                    int scaledWidth = Mathf.FloorToInt(textureWidth * scale);
                    int scaledHeight = Mathf.FloorToInt(textureHeight * scale);

                    // 创建缩放后的texture
                    var scaledTexture = new Texture2D(scaledWidth, scaledHeight);
                    for (int y = 0; y < scaledHeight; y++)
                    {
                        for (int x = 0; x < scaledWidth; x++)
                        {
                            // 计算缩放后的坐标
                            float textureX = (float)x / scaledWidth;
                            float textureY = (float)y / scaledHeight;

                            // 缩放图像并设置给缩放后的texture
                            Color pixelColor = texture.GetPixelBilinear(textureX, textureY);
                            scaledTexture.SetPixel(x, y, pixelColor);
                        }
                    }
                    scaledTexture.Apply();

                    // 计算裁切后的宽和高
                    int cropWidth = Mathf.FloorToInt(imageWidth);
                    int cropHeight = Mathf.FloorToInt(imageHeight);

                    // 计算裁切的起始点
                    int startX = (scaledWidth - cropWidth) / 2;
                    int startY = (scaledHeight - cropHeight) / 2;

                    // 创建裁切后的texture
                    var croppedTexture = new Texture2D(cropWidth, cropHeight);
                    for (int y = 0; y < cropHeight; y++)
                    {
                        for (int x = 0; x < cropWidth; x++)
                        {
                            // 裁切图像并设置给裁切后的texture
                            Color pixelColor = scaledTexture.GetPixel(startX + x, startY + y);
                            croppedTexture.SetPixel(x, y, pixelColor);
                        }
                    }
                    croppedTexture.Apply();


                    // 创建sprite并设置给image
                    img.sprite = Sprite.Create(croppedTexture, new Rect(0, 0, cropWidth, cropHeight), new Vector2(0.5f, 0.5f));

                    UnityEngine.Object.Destroy(scaledTexture);
                    Resources.UnloadUnusedAssets();
                }
                else
                {
                    var rt = RenderTexture.GetTemporary((int)rect.width, (int)rect.height);
                    Graphics.Blit(texture, rt);
                    RenderTexture previous = RenderTexture.active;
                    RenderTexture.active = rt;
                    var readableTexture = new Texture2D((int)rect.width, (int)rect.height);
                    readableTexture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
                    readableTexture.Apply();
                    RenderTexture.active = previous;
                    RenderTexture.ReleaseTemporary(rt);

                    img.sprite = Sprite.Create(readableTexture,
                        new Rect(0, 0, readableTexture.width, readableTexture.height), img.rectTransform.pivot);
                    Resources.UnloadUnusedAssets();
                }
            }

            private Vector2 startPosition;
            private float slideThresholdSquared = 360000f; //滑动阈值的平方
            public IEnumerator ListenSlideInput(Action handler)
            {
                while (true)
                {
                    yield return null;
                    if (Input.touchCount > 0)
                    {
                        Touch touch = Input.GetTouch(0);
                        if (touch.phase == TouchPhase.Began)
                        {
                            startPosition = touch.position;
                        }
                        else if (touch.phase == TouchPhase.Ended)
                        {
                            Vector2 deltaPosition = touch.position - startPosition;
                            if (deltaPosition.sqrMagnitude > slideThresholdSquared &&
                                Mathf.Abs(deltaPosition.x) > Mathf.Abs(deltaPosition.y) && deltaPosition.x > 0)
                            {
                                handler?.Invoke();
                                yield break;
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}