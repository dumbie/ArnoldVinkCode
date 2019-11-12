﻿using IMMDevice;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static ArnoldVinkCode.AVInteropCom;

namespace ArnoldVinkCode
{
    public class AVSwitchAudioDevice
    {
        public class AudioDeviceSummary
        {
            public string Id = string.Empty;
            public string Name = string.Empty;
        }

        //Set a new default audio device
        public static bool SetDefaultDevice(string deviceId)
        {
            try
            {
                PolicyConfigClient autoPolicyConfigClient = new PolicyConfigClient();
                autoPolicyConfigClient.SetDefaultEndpoint(deviceId, ERole.eMultimedia);
                Debug.WriteLine("Changed default audio device: " + deviceId);
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed to set new default audio device: " + deviceId);
                return false;
            }
        }

        //Get the current default audio device
        public static AudioDeviceSummary GetDefaultDevice()
        {
            try
            {
                IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
                IMMDevice.IMMDevice deviceItem = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);

                //Get the audio device id
                string deviceId = deviceItem.GetId();

                //Get the audio device name
                PropertyVariant propertyVariant = new PropertyVariant();
                IPropertyStore propertyStore = deviceItem.OpenPropertyStore(STGM.STGM_READ);
                propertyStore.GetValue(ref PKEY_Device_FriendlyName, out propertyVariant);
                string deviceName = Marshal.PtrToStringUni(propertyVariant.pwszVal);

                return new AudioDeviceSummary() { Id = deviceId, Name = deviceName };
            }
            catch
            {
                Debug.WriteLine("Failed to get the default audio device.");
                return null;
            }
        }

        //Get all the playback audio devices
        public static List<AudioDeviceSummary> ListAudioDevices()
        {
            try
            {
                List<AudioDeviceSummary> deviceListSummary = new List<AudioDeviceSummary>();
                IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
                IMMDeviceCollection deviceCollection = deviceEnumerator.EnumAudioEndpoints(EDataFlow.eRender, DeviceState.ACTIVE);

                uint deviceCount = deviceCollection.GetCount();
                for (uint deviceIndex = 0; deviceIndex < deviceCount; deviceIndex++)
                {
                    IMMDevice.IMMDevice deviceItem = deviceCollection.Item(deviceIndex);

                    //Get the audio device id
                    string deviceId = deviceItem.GetId();

                    //Get the audio device name
                    PropertyVariant propertyVariant = new PropertyVariant();
                    IPropertyStore propertyStore = deviceItem.OpenPropertyStore(STGM.STGM_READ);
                    propertyStore.GetValue(ref PKEY_Device_FriendlyName, out propertyVariant);
                    string deviceName = Marshal.PtrToStringUni(propertyVariant.pwszVal);

                    //Add device to summary list
                    deviceListSummary.Add(new AudioDeviceSummary() { Id = deviceId, Name = deviceName });
                }

                return deviceListSummary;
            }
            catch
            {
                Debug.WriteLine("Failed to get audio devices.");
                return null;
            }
        }
    }
}