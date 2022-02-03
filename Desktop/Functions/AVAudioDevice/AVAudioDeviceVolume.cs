using IMMDevice;
using System;
using System.Diagnostics;

namespace ArnoldVinkCode
{
    public partial class AVAudioDevice
    {
        //Set default audio device volume (0-100)
        public static bool AudioVolumeSet(int targetVolume, bool inputDevice)
        {
            try
            {
                IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
                IMMDevice.IMMDevice deviceItem = null;
                if (!inputDevice)
                {
                    deviceItem = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
                }
                else
                {
                    deviceItem = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eCapture, ERole.eMultimedia);
                }

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
                Debug.WriteLine("Failed to set default audio device volume.");
                return false;
            }
        }

        //Up default audio device volume (0-100)
        public static int AudioVolumeUp(int targetStep, bool inputDevice)
        {
            try
            {
                IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
                IMMDevice.IMMDevice deviceItem = null;
                if (!inputDevice)
                {
                    deviceItem = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
                }
                else
                {
                    deviceItem = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eCapture, ERole.eMultimedia);
                }

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
                return Convert.ToInt32(volumeLevelFloat * 100);
            }
            catch
            {
                Debug.WriteLine("Failed to up default audio device volume.");
                return -1;
            }
        }

        //Down default audio device volume (0-100)
        public static int AudioVolumeDown(int targetStep, bool inputDevice)
        {
            try
            {
                IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
                IMMDevice.IMMDevice deviceItem = null;
                if (!inputDevice)
                {
                    deviceItem = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
                }
                else
                {
                    deviceItem = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eCapture, ERole.eMultimedia);
                }

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
                return Convert.ToInt32(volumeLevelFloat * 100);
            }
            catch
            {
                Debug.WriteLine("Failed to down default audio device volume.");
                return -1;
            }
        }

        //Get default audio device volume (0-100)
        public static int AudioVolumeGet(bool inputDevice)
        {
            try
            {
                IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
                IMMDevice.IMMDevice deviceItem = null;
                if (!inputDevice)
                {
                    deviceItem = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
                }
                else
                {
                    deviceItem = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eCapture, ERole.eMultimedia);
                }

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
                Debug.WriteLine("Failed to get default audio device volume.");
                return -1;
            }
        }

        //Switch default audio device mute status
        public static bool AudioMuteSwitch(bool inputDevice)
        {
            try
            {
                IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
                IMMDevice.IMMDevice deviceItem = null;
                if (!inputDevice)
                {
                    deviceItem = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
                }
                else
                {
                    deviceItem = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eCapture, ERole.eMultimedia);
                }

                //Get the audio device volume endpoint
                deviceItem.Activate(typeof(IAudioEndpointVolume).GUID, 0, IntPtr.Zero, out object deviceActivated);
                IAudioEndpointVolume audioEndPointVolume = (IAudioEndpointVolume)deviceActivated;

                //Get the current mute status
                audioEndPointVolume.GetMute(out bool muteStatus);

                //Set the switched mute status
                audioEndPointVolume.SetMute(!muteStatus, Guid.Empty);
                Debug.WriteLine("Switched the mute status: " + !muteStatus);
                return !muteStatus;
            }
            catch
            {
                Debug.WriteLine("Failed to switch mute status.");
                return false;
            }
        }

        //Mute default audio device
        public static void AudioMute(bool inputDevice)
        {
            try
            {
                IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
                IMMDevice.IMMDevice deviceItem = null;
                if (!inputDevice)
                {
                    deviceItem = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
                }
                else
                {
                    deviceItem = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eCapture, ERole.eMultimedia);
                }

                //Get the audio device volume endpoint
                deviceItem.Activate(typeof(IAudioEndpointVolume).GUID, 0, IntPtr.Zero, out object deviceActivated);
                IAudioEndpointVolume audioEndPointVolume = (IAudioEndpointVolume)deviceActivated;

                //Set the switched mute status
                audioEndPointVolume.SetMute(true, Guid.Empty);
                Debug.WriteLine("Muted audio device.");
            }
            catch
            {
                Debug.WriteLine("Failed to mute audio device.");
            }
        }

        //Mute default audio device
        public static void AudioUnmute(bool inputDevice)
        {
            try
            {
                IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
                IMMDevice.IMMDevice deviceItem = null;
                if (!inputDevice)
                {
                    deviceItem = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
                }
                else
                {
                    deviceItem = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eCapture, ERole.eMultimedia);
                }

                //Get the audio device volume endpoint
                deviceItem.Activate(typeof(IAudioEndpointVolume).GUID, 0, IntPtr.Zero, out object deviceActivated);
                IAudioEndpointVolume audioEndPointVolume = (IAudioEndpointVolume)deviceActivated;

                //Set the switched mute status
                audioEndPointVolume.SetMute(false, Guid.Empty);
                Debug.WriteLine("Unmuted audio device.");
            }
            catch
            {
                Debug.WriteLine("Failed to unmute audio device.");
            }
        }

        //Get default audio device mute status
        public static bool AudioMuteGetStatus(bool inputDevice)
        {
            try
            {
                IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
                IMMDevice.IMMDevice deviceItem = null;
                if (!inputDevice)
                {
                    deviceItem = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
                }
                else
                {
                    deviceItem = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eCapture, ERole.eMultimedia);
                }

                //Get the audio device volume endpoint
                deviceItem.Activate(typeof(IAudioEndpointVolume).GUID, 0, IntPtr.Zero, out object deviceActivated);
                IAudioEndpointVolume audioEndPointVolume = (IAudioEndpointVolume)deviceActivated;

                //Get the current mute status
                audioEndPointVolume.GetMute(out bool muteStatus);
                return muteStatus;
            }
            catch
            {
                //Debug.WriteLine("Failed to get mute status.");
                return false;
            }
        }
    }
}