using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace ArnoldVinkCode
{
    public partial class AVCertificate
    {
        public static bool InstallCertificate(byte[] certificateBytes)
        {
            try
            {
                using (X509Certificate2 certificateFile = new X509Certificate2(certificateBytes))
                {
                    using (X509Store certificateStore = new X509Store(StoreName.Root, StoreLocation.LocalMachine))
                    {
                        certificateStore.Open(OpenFlags.ReadWrite);
                        certificateStore.Add(certificateFile);
                    }
                }

                Debug.WriteLine("Installed certificate to store root.");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to install certificate: " + ex.Message);
                return false;
            }
        }

        public static bool UninstallCertificate(string issuedTo)
        {
            try
            {
                using (X509Store certificateStore = new X509Store(StoreName.Root, StoreLocation.LocalMachine))
                {
                    certificateStore.Open(OpenFlags.ReadWrite);
                    foreach (X509Certificate2 cert in certificateStore.Certificates.Cast<X509Certificate2>())
                    {
                        try
                        {
                            string certIssuedTo = cert.GetNameInfo(X509NameType.SimpleName, false);
                            if (certIssuedTo == issuedTo)
                            {
                                certificateStore.Remove(cert);
                                Debug.WriteLine("Removed certificate from the store root: " + issuedTo);
                            }
                        }
                        catch { }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to uninstall certificate: " + ex.Message);
                return false;
            }
        }
    }
}