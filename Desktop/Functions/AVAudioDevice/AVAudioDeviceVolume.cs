using IMMDevice;
using System;
using System.Diagnostics;

namespace ArnoldVinkCode
{
    public partial class AVAudioDevice
    {
        //Set the current audio device volume (0-100)
        public static bool AudioVolumeSet(int targetVolume)
        {
            try
            {
                IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
                IMMDevice.IMMDevice deviceItem = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);

                //Get the audio device volume endpoint
                deviceItem.Activate(typeof(IAudioEndpointVolume).GUID, 0, IntPtr.Zero, out object deviceActivated);
                IAudioEndpointVolume audioEndPointVolume = (IAudioEndpointVolume)deviceActivated;

                //Check the target volume
                if (targetVolume > 100) { targetVolume = 100; }
                if (targetVolume < 0) { targetVolume = 0; }

                //Set the audio device volume
                float volumeLevelFloat = targetVolume / 100F;
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

        //Up the current audio device volume (0-100)
        public static bool AudioVolumeUp(int targetStep)
        {
            try
            {
                IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
                IMMDevice.IMMDevice deviceItem = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);

                //Get the audio device volume endpoint
                deviceItem.Activate(typeof(IAudioEndpointVolume).GUID, 0, IntPtr.Zero, out object deviceActivated);
                IAudioEndpointVolume audioEndPointVolume = (IAudioEndpointVolume)deviceActivated;

                //Get the audio device volume
                audioEndPointVolume.GetMasterVolumeLevelScalar(out float volumeLevelCurrentFloat);
                float volumeLevelFloat = volumeLevelCurrentFloat + (targetStep / 100F);

                //Check the target volume
                if (volumeLevelFloat > 1.00) { volumeLevelFloat = 1.00F; }
                if (volumeLevelFloat < 0.00) { volumeLevelFloat = 0.00F; }

                //Change the audio device volume
                audioEndPointVolume.SetMasterVolumeLevelScalar(volumeLevelFloat, Guid.Empty);

                Debug.WriteLine("Up volume: " + targetStep + "% / " + volumeLevelFloat);
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed to up the audio device volume.");
                return false;
            }
        }

        //Down the current audio device volume (0-100)
        public static bool AudioVolumeDown(int targetStep)
        {
            try
            {
                IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
                IMMDevice.IMMDevice deviceItem = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);

                //Get the audio device volume endpoint
                deviceItem.Activate(typeof(IAudioEndpointVolume).GUID, 0, IntPtr.Zero, out object deviceActivated);
                IAudioEndpointVolume audioEndPointVolume = (IAudioEndpointVolume)deviceActivated;

                //Get the audio device volume
                audioEndPointVolume.GetMasterVolumeLevelScalar(out float volumeLevelCurrentFloat);
                float volumeLevelFloat = volumeLevelCurrentFloat - (targetStep / 100F);

                //Check the target volume
                if (volumeLevelFloat > 1.00) { volumeLevelFloat = 1.00F; }
                if (volumeLevelFloat < 0.00) { volumeLevelFloat = 0.00F; }

                //Change the audio device volume
                audioEndPointVolume.SetMasterVolumeLevelScalar(volumeLevelFloat, Guid.Empty);

                Debug.WriteLine("Down volume: " + targetStep + "% / " + volumeLevelFloat);
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed to down the audio device volume.");
                return false;
            }
        }

        //Get the current audio device volume (0-100)
        public static int AudioVolumeGet()
        {
            try
            {
                IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
                IMMDevice.IMMDevice deviceItem = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);

                //Get the audio device volume endpoint
                deviceItem.Activate(typeof(IAudioEndpointVolume).GUID, 0, IntPtr.Zero, out object deviceActivated);
                IAudioEndpointVolume audioEndPointVolume = (IAudioEndpointVolume)deviceActivated;

                //Get the audio device volume
                audioEndPointVolume.GetMasterVolumeLevelScalar(out float volumeLevelCurrentFloat);
                int volumeLevelInt = Convert.ToInt32(volumeLevelCurrentFloat * 100);

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
        public static void AudioMuteSwitch()
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

        //Get the audio device mute status
        public static bool AudioMuteGetStatus()
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
                return muteStatus;
            }
            catch
            {
                Debug.WriteLine("Failed to get the mute status.");
                return false;
            }
        }
    }
}