using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOverlayExtension
{
    public static class KeyboardHelper
    {
        [DllImport("user32.dll",
            CallingConvention     = CallingConvention.StdCall,
            CharSet               = CharSet.Unicode,
            EntryPoint            = "LoadKeyboardLayout",
            SetLastError          = true,
            ThrowOnUnmappableChar = false)]
        static extern uint LoadKeyboardLayout(StringBuilder pwszKLID,
                                              uint flags);

        [DllImport("user32.dll", ExactSpelling = true)]
        internal static extern IntPtr GetKeyboardLayout(uint threadId);

        [DllImport("user32.dll", ExactSpelling = true)]
        internal static extern bool GetKeyboardState(Keys[] keyStates);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hhwnd, uint msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int ToUnicodeEx(uint          wVirtKey,
                                              uint          wScanCode,
                                              Keys[]        lpKeyState,
                                              StringBuilder pwszBuff,
                                              int           cchBuff,
                                              uint          wFlags,
                                              IntPtr        dwhkl);


        [DllImport("user32.dll", ExactSpelling = true)]
        internal static extern uint GetWindowThreadProcessId(IntPtr hwindow, out uint processId);

        public static void SwitchLang()
        {
            for (var i = 0; i < InputLanguage.InstalledInputLanguages.Count; i++)
            {
                if (Equals(InputLanguage.CurrentInputLanguage, InputLanguage.InstalledInputLanguages[i]))
                {
                    i++;

                    if (i == InputLanguage.InstalledInputLanguages.Count)
                        i = 0;

                    string layoutName = InputLanguage.InstalledInputLanguages[i].Culture.LCID.ToString("x8");

                    var pwszKlid = new StringBuilder(layoutName);
                    var hkl      = LoadKeyboardLayout(pwszKlid, 1);

                    PostMessage(GetForegroundWindow(), 0x0050, IntPtr.Zero, (IntPtr) hkl);

                    break;
                }
            }
        }

        public static string CodeToString(int scanCode)
        {
            var thread = GetWindowThreadProcessId(Process.GetCurrentProcess().MainWindowHandle, out _);
            var hkl    = GetKeyboardLayout(thread);

            if (hkl == IntPtr.Zero)
            {
                //Console.WriteLine("Sorry, that keyboard does not seem to be valid.");
                return string.Empty;
            }

            var keyStates = new Keys[256];

            if (!GetKeyboardState(keyStates))
                return string.Empty;

            var sb = new StringBuilder(10);
            ToUnicodeEx((uint) scanCode, (uint) scanCode, keyStates, sb, sb.Capacity, 0, hkl);

            return sb.ToString();
        }
    }
}