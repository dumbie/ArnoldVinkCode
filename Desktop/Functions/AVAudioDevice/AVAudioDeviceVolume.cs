using IMMDevice;
using System;
using System.Diagnostics;

namespace ArnoldVinkCode
{
    public partial class AVAudioDevice
    {
        //Set the current audio device volume (0-100)
        public static bool SetAudioVolume(int targetVolume)
        {
            try
            {
                IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
                IMMDevice.IMMDevice deviceItem = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);

                //Get the audio device volume endpoint
                deviceItem.Activate(typeof(IAudioEndpointVolume).GUID, 0, IntPtr.Zero, out object deviceActivated);
                IAudioEndpointVolume audioEndPointVolume = (IAudioEndpointVolume)deviceActivated;

                //Check the target volume
                if (targetVolume > 100)
                {
                    targetVolume = 100;
                }

                if (targetVolume < 0)
                {
                    targetVolume = 0;
                }

                //Set the audio device volume
                float volumeLevelFloat = (float)(targetVolume / 100F);
                audioEndPointVolume.SetMasterVolumeLevelScalar(volumeLevelFloat, Guid.Empty);

                Debug.WriteLine("Set volume: " + targetVolume + "% / " + volumeLevelFloat);
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed to set the audio device volume.");
                return false;
            }
        }

        //Get the current audio device volume (0-100)
        public static int GetAudioVolume()
        {
            try
            {
                IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
                IMMDevice.IMMDevice deviceItem = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);

                //Get the audio device volume endpoint
                deviceItem.Activate(typeof(IAudioEndpointVolume).GUID, 0, IntPtr.Zero, out object deviceActivated);
                IAudioEndpointVolume audioEndPointVolume = (IAudioEndpointVolume)deviceActivated;

                //Get the audio device volume
                audioEndPointVolume.GetMasterVolumeLevelScalar(out float volumeLevelFloat);
                int volumeLevelInt = Convert.ToInt32(volumeLevelFloat * 100);

                //Debug.WriteLine("Current volume: " + volumeLevelInt + "%");
                return volumeLevelInt;
            }
            catch
            {
                Debug.WriteLine("Failed to get the audio device volume.");
                return -1;
            }
        }

        //Switch the audio device mute status
        public static void SwitchAudioMute()
        {
            try
            {
                IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
                IMMDevice.IMMDevice deviceItem = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);

                //Get the audio device volume endpoint
                deviceItem.Activate(typeof(IAudioEndpointVolume).GUID, 0, IntPtr.Zero, out object deviceActivated);
                IAudioEndpointVolume audioEndPointVolume = (IAudioEndpointVolume)deviceActivated;

                //Get the current mute status
                audioEndPointVolume.GetMute(out bool muteStatus);

                //Set the switched mute status
                audioEndPointVolume.SetMute(!muteStatus, Guid.Empty);
                Debug.WriteLine("Switched the mute status: " + !muteStatus);
            }
            catch
            {
                Debug.WriteLine("Failed to switch the mute status.");
            }
        }
    }
}