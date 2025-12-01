using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace ArnoldVinkCode
{
    public partial class AVCertificate
    {
        public static bool InstallCertificate(string certificatePath)
        {
            try
            {
                //Load certificate from file
                using (X509Certificate2 certificateFile = X509CertificateLoader.LoadCertificateFromFile(certificatePath))
                {
                    //Install certificate to trusted root
                    using (X509Store certificateStore = new X509Store(StoreName.Root, StoreLocation.LocalMachine))
                    {
                        certificateStore.Open(OpenFlags.ReadWrite);
                        certificateStore.Add(certificateFile);
                    }

                    //Install certificate to trusted publisher
                    using (X509Store certificateStore = new X509Store(StoreName.TrustedPublisher, StoreLocation.LocalMachine))
                    {
                        certificateStore.Open(OpenFlags.ReadWrite);
                        certificateStore.Add(certificateFile);
                    }
                }

                //Return result
                Debug.WriteLine("Installed certificate to store.");
                return true;
            }
            catch (Exception ex)
            {
                //Return result
                Debug.WriteLine("Failed to install certificate: " + ex.Message);
                return false;
            }
        }

        public static bool InstallCertificate(byte[] certificateBytes)
        {
            try
            {
                //Load certificate from bytes
                using (X509Certificate2 certificateFile = X509CertificateLoader.LoadCertificate(certificateBytes))
                {
                    //Install certificate to trusted root
                    using (X509Store certificateStore = new X509Store(StoreName.Root, StoreLocation.LocalMachine))
                    {
                        certificateStore.Open(OpenFlags.ReadWrite);
                        certificateStore.Add(certificateFile);
                    }

                    //Install certificate to trusted publisher
                    using (X509Store certificateStore = new X509Store(StoreName.TrustedPublisher, StoreLocation.LocalMachine))
                    {
                        certificateStore.Open(OpenFlags.ReadWrite);
                        certificateStore.Add(certificateFile);
                    }
                }

                //Return result
                Debug.WriteLine("Installed certificate to store.");
                return true;
            }
            catch (Exception ex)
            {
                //Return result
                Debug.WriteLine("Failed to install certificate: " + ex.Message);
                return false;
            }
        }

        public static bool UninstallCertificate(string issuedTo)
        {
            try
            {
                //Uninstall certificate from trusted root
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
                                Debug.WriteLine("Removed certificate from store trusted root: " + issuedTo);
                            }
                        }
                        catch { }
                    }
                }

                //Uninstall certificate from trusted publisher
                using (X509Store certificateStore = new X509Store(StoreName.TrustedPublisher, StoreLocation.LocalMachine))
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
                                Debug.WriteLine("Removed certificate from store trusted publisher: " + issuedTo);
                            }
                        }
                        catch { }
                    }
                }

                //Return result
                return true;
            }
            catch (Exception ex)
            {
                //Return result
                Debug.WriteLine("Failed to uninstall certificate: " + ex.Message);
                return false;
            }
        }
    }
}