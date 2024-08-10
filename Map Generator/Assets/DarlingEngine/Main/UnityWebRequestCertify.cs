using UnityEngine.Networking;


namespace DarlingEngine
{
    public class UnityWebRequestCertify : UnityEngine.Networking.CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            //return base.ValidateCertificate(certificateData);
            return true;
        }
    }
}
