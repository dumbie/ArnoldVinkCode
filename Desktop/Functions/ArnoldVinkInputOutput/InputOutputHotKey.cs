using System;
using System.Diagnostics;
using System.Windows.Interop;
using static ArnoldVinkCode.AVInputOutputClass;
using static ArnoldVinkCode.AVInteropDll;

namespace ArnoldVinkCode
{
    public partial class AVInputOutputHotKey
    {
        //Variables
        private int vHotKeyIdentifierCount = 0;

        //Events
        public delegate void HotKeyPressed(KeysModifier keysModifier, KeysVirtual keysVirtual);
        public event HotKeyPressed EventHotKeyPressed;

        //Register receiving messages
        public AVInputOutputHotKey()
        {
            try
            {
                ComponentDispatcher.ThreadFilterMessage += ReceivedFilterMessage;
            }
            catch { }
        }

        //Check received message
        void ReceivedFilterMessage(ref MSG windowMessage, ref bool messageHandled)
        {
            try
            {
                //Check window message identifier
                if (windowMessage.message == (int)WindowMessages.WM_HOTKEY)
                {
                    //Get pressed keys
                    KeysModifier keysModifier = (KeysModifier)((int)windowMessage.lParam & 0xFFFF);
                    KeysVirtual keysVirtual = (KeysVirtual)(((int)windowMessage.lParam >> 16) & 0xFFFF);

                    //Trigger event
                    EventHotKeyPressed(keysModifier, keysVirtual);
                }
            }
            catch { }
        }

        //Register hotkey
        public int RegisterHotKey(KeysModifier keysModifier, KeysVirtual keysVirtual)
        {
            try
            {
                int hotKeyIdentifier = vHotKeyIdentifierCount;
                bool hotkeyRegistered = AVInteropDll.RegisterHotKey(IntPtr.Zero, hotKeyIdentifier, (uint)keysModifier, (uint)keysVirtual);
                if (hotkeyRegistered)
                {
                    Debug.WriteLine("Registered hotkey: " + keysModifier + "/" + keysVirtual + "/ID" + hotKeyIdentifier);
                    vHotKeyIdentifierCount++;
                    return hotKeyIdentifier;
                }
            }
            catch { }
            Debug.WriteLine("Failed registering hotkey: " + keysModifier + "/" + keysVirtual);
            return -1;
        }

        //Unregister hotkey
        public bool UnregisterHotKey(int identifier)
        {
            try
            {
                bool hotkeyUnregistered = AVInteropDll.UnregisterHotKey(IntPtr.Zero, identifier);
                Debug.WriteLine("Unregistered hotkey: ID" + identifier);
                return hotkeyUnregistered;
            }
            catch
            {
                Debug.WriteLine("Failed unregistering hotkey: ID" + identifier);
                return false;
            }
        }

        //Unregister all hotkeys
        public bool UnregisterHotKeyAll()
        {
            try
            {
                for (int hkId = 0; hkId < vHotKeyIdentifierCount; hkId++)
                {
                    AVInteropDll.UnregisterHotKey(IntPtr.Zero, hkId);
                }
                Debug.WriteLine("Unregistered all hotkeys.");
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed unregistering all hotkeys.");
                return false;
            }
        }
    }
}