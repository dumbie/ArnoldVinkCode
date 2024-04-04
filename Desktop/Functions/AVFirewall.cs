using NetFwTypeLib; //C:\Windows\System32\FirewallAPI.dll
using System;
using System.Diagnostics;
using System.IO;

namespace ArnoldVinkCode
{
    public partial class AVFirewall
    {
        //Firewall executable allow
        public static bool Firewall_ExecutableAllow(string applicationName, string executablePath, bool allowPublic)
        {
            try
            {
                //Remove current firewall rule
                Firewall_ExecutableRemove(executablePath);

                //Add application firewall rule
                INetFwRule firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
                firewallRule.Name = applicationName + " (" + executablePath + ")";
                firewallRule.ApplicationName = executablePath;
                firewallRule.Description = "Arnold Vink";
                firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
                firewallRule.Protocol = (int)NET_FW_IP_PROTOCOL.NET_FW_IP_PROTOCOL_ANY;
                firewallRule.InterfaceTypes = NET_FW_INTERFACE_TYPE.All.ToString();
                firewallRule.Enabled = true;
                if (allowPublic)
                {
                    firewallRule.Profiles = (int)NET_FW_PROFILE_TYPE2.NET_FW_PROFILE2_PRIVATE | (int)NET_FW_PROFILE_TYPE2.NET_FW_PROFILE2_PUBLIC;
                }
                else
                {
                    firewallRule.Profiles = (int)NET_FW_PROFILE_TYPE2.NET_FW_PROFILE2_PRIVATE;
                }

                INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                firewallPolicy.Rules.Add(firewallRule);

                Debug.WriteLine("Allowed executable in firewall: " + executablePath);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed allowing in firewall: " + executablePath + " / " + ex.Message);
                return false;
            }
        }

        //Firewall executable remove
        public static bool Firewall_ExecutableRemove(string executableName)
        {
            try
            {
                string executableNameFile = Path.GetFileName(executableName).ToLower();
                INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                foreach (INetFwRule firewallRule in firewallPolicy.Rules)
                {
                    try
                    {
                        if (firewallRule.ApplicationName == null) { continue; }
                        string executableNameRule = Path.GetFileName(firewallRule.ApplicationName).ToLower();
                        if (executableNameRule == executableNameFile)
                        {
                            firewallPolicy.Rules.Remove(firewallRule.Name);
                            Debug.WriteLine("Removed executable from firewall: " + executableName);
                        }
                    }
                    catch { }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed removing from firewall: " + executableName + " / " + ex.Message);
                return false;
            }
        }

        //Open Windows Firewall Settings
        public static void Firewall_OpenWindowsSettings()
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "wf.msc";
                process.StartInfo.UseShellExecute = true;
                process.Start();
            }
            catch { }
        }

        //Firewall variables
        private enum NET_FW_PROFILE_TYPE2 : int
        {
            NET_FW_PROFILE2_DOMAIN = 0x1,
            NET_FW_PROFILE2_PRIVATE = 0x2,
            NET_FW_PROFILE2_PUBLIC = 0x4,
            NET_FW_PROFILE2_ALL = 0x7fffffff
        }
        private enum NET_FW_IP_PROTOCOL : int
        {
            NET_FW_IP_PROTOCOL_TCP = 6,
            NET_FW_IP_PROTOCOL_UDP = 17,
            NET_FW_IP_PROTOCOL_ANY = 256
        }
        private enum NET_FW_INTERFACE_TYPE
        {
            RemoteAccess,
            Wireless,
            Lan,
            All
        }
    }
}