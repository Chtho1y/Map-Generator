using UnityEngine;

namespace DarlingEngine.Main
{
    public static class BundleServerConfig
    {
        public static readonly string ServerUrl = Application.platform switch
        {
            // don't forget to add '/'
            RuntimePlatform.WindowsEditor => @"https://unityassetbundle.oss-cn-shanghai.aliyuncs.com/Windows/AssetBundle/",
            RuntimePlatform.OSXEditor => @"https://unityassetbundle.oss-cn-shanghai.aliyuncs.com/Windows/AssetBundle/",
            RuntimePlatform.Android => @"https://unityassetbundle.oss-cn-shanghai.aliyuncs.com/Windows/AssetBundle/",
            RuntimePlatform.IPhonePlayer => @"https://unityassetbundle.oss-cn-shanghai.aliyuncs.com/Windows/AssetBundle/",
            _ => "Server not configured for this platform",
        };
    }
}
