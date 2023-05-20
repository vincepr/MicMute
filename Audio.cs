using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace MicroMuteTerminal
{
    /*
     * We use WM_APPCOMMAND message to release signals to The OS.
     * https://learn.microsoft.com/en-us/windows/win32/inputdev/wm-appcommand?redirectedfrom=MSDN
     *  Parameters: 
     *      wParam: Handle to the window where the user clicked the button
     *      lParam: 
     *          uDevice = GET_DEVICE_LPARAM(lParam);
     *          dwKeys = GET_KEYSTATE_LPARAM(lParam);
     *          GET_APPCOMMAND_LPARAM(lParam)           - can be one of the following parameters:
     *  
     *  MAND_MIC_ON_OFF_TOGGLE              44      Toggle the microphone.
     *  APPCOMMAND_MICROPHONE_VOLUME_MUTE   24      Mute the microphone.    
     *  APPCOMMAND_MICROPHONE_VOLUME_DOWN   25      Decrease microphone volume. 
     *  APPCOMMAND_MICROPHONE_VOLUME_UP     26      Increase microphone volume.
     *  
     *  APPCOMMAND_VOLUME_DOWN              9       Lower the volume.
     *  APPCOMMAND_VOLUME_MUTE              8       Mute the volume.    (actually toggles volume)
     *  APPCOMMAND_VOLUME_UP                10      Raise the volume.
     *  
     *          uDevice: indicates the input device that generated the input:
     *          we use: FAPPCOMMAND_OEM     0x1000  An unidentified hardware source generated the event. It could be a mouse or a keyboard event.
     */

    public static class Audio
    {
        private const uint WM_APPCOMMAND = 0x319;       // We Signal that were an WM_APPCOMMAND
        private const uint MASK = 0x1000;               // We Signal that we are unidentified hardware source. (not a user input like Mouse- or Keypress)

        // Useful Cmd-Signals for our usecase
        private const uint CMD_VOLUME_TOGGLE = 8;
        private const uint CMD_VOLUME_DOWN = 9;
        private const uint CMD_VOLUME_UP = 10;

        //private const uint CMD_MIC_TOGGLE = 44;       // Seems NOT to work (on my home pc)
        private const uint CMD_MIC_VOLUME_MUTE = 24;    // actual Microphone On/OFF-toggle!
        private const uint CMD_MIC_VOLUME_DOWN = 25;
        private const uint CMD_MIC_VOLUME_UP = 26;

        // Import necessary ddls:
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        // Sends THE WM_APPCOMMAND to the foreground window:
        private static void Send(uint cmd_signal) => SendMessage(GetForegroundWindow(), WM_APPCOMMAND, IntPtr.Zero, (IntPtr)(cmd_signal << 16 | MASK));
       
        // We expose all Volume Control Functions public
        public static void Volume_Toggle() => Send(CMD_VOLUME_TOGGLE);
        public static void Volume_Down() => Send(CMD_VOLUME_DOWN);
        public static void Volume_Up() => Send(CMD_VOLUME_UP);

        // We expose all Microphone Control Functions public
        // public static void Mic_Toggle() => Send(CMD_MIC_TOGGLE);      // Seems NOT to work (on my home pc)
        public static void Mic_Mute() => Send(CMD_MIC_VOLUME_MUTE);
        public static void Mic_Volume_Down() => Send(CMD_MIC_VOLUME_DOWN);
        public static void Mic_Volume_Up() => Send(CMD_MIC_VOLUME_UP);
    }
}
