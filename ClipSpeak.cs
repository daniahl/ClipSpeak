/*  ClipSpeak
    Copyright(c) 2008-2009, Daniel Innala Ahlmark
    <http://www.codeplex.com/clipspeak>

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using System;
using System.Windows.Forms;
using System.Reflection;
using Utilities;
using System.IO;

#region Assembly attributes
[assembly: AssemblyTitle("ClipSpeak")]
[assembly: AssemblyDescription("Listen to copied texts, or save them to MP3 files.")]
[assembly: AssemblyProduct("ClipSpeak")]
[assembly: AssemblyCopyright("Copyright(c) 2009, Daniel Innala Ahlmark")]
[assembly: AssemblyVersion("1.5.*")]

#endregion

namespace ClipSpeak
{
    /// <summary>
    /// This is the main class of ClipSpeak. The Main method instantiates this class which is then ready to handle events.
    /// </summary>
    public class ClipSpeak : Form
    {
        globalKeyboardHook keyhook = new globalKeyboardHook();
        SpeechHandler sh = new SpeechHandler();

        NotifyIcon trayicon = new NotifyIcon();
        VoiceDlg vdlg;
        SaveDlg savedlg;

        IntPtr nextClipboardViewer;        

        bool firstTime = true;       //used to prevent speaking at startup
        bool enabled = true;

        static Assembly a = Assembly.GetExecutingAssembly();
        static string[] icons = a.GetManifestResourceNames();

        [STAThread]
        public static void Main()
        {
            bool singleInstance;
            System.Threading.Mutex m = new System.Threading.Mutex(true, "ClipSpeak", out singleInstance);

            if (!singleInstance)
            {
                Application.Exit();
                return;
            }

            ClipSpeak cs = new ClipSpeak();            
            Application.Run();
            Application.Exit();
            return;
        }

        public ClipSpeak()
        {            
            keyhook.HookedKeys.Add(Keys.RControlKey);
            //keyhook.HookedKeys.Add(Keys.RShiftKey);            
            keyhook.KeyDown += new KeyEventHandler(Keyhook_KeyDown);            
                        
            ContextMenu traymenu = new ContextMenu();
            MenuItem selvoice = new MenuItem("Select Voice...", new EventHandler(Menu_selvoice));
            MenuItem savetomp3 = new MenuItem("Save to MP3...", new EventHandler(Menu_savetomp3));
            MenuItem help = new MenuItem("Help...", new EventHandler(Menu_help));
            MenuItem homepage = new MenuItem("Homepage...", new EventHandler(Menu_homepage));
            MenuItem exit = new MenuItem("Exit", new EventHandler(Menu_exit));
            traymenu.MenuItems.Add(selvoice);
            traymenu.MenuItems.Add(savetomp3);
            traymenu.MenuItems.Add(new MenuItem("-"));
            traymenu.MenuItems.Add(help);
            traymenu.MenuItems.Add(homepage);
            traymenu.MenuItems.Add(exit);
            
            trayicon.ContextMenu = traymenu;
            
            //Set up tray icon
            trayicon.Text = "ClipSpeak 1.5";            
            trayicon.Icon = new System.Drawing.Icon(a.GetManifestResourceStream(icons[2]));

            trayicon.Visible = true;            
            
            //Add to clipboard chain
            nextClipboardViewer = (IntPtr)WinAPI.SetClipboardViewer((int)this.Handle);
                        
        }

        void Menu_selvoice(object sender, EventArgs e)
        {
            vdlg = new VoiceDlg(sh);
            vdlg.Show();
        }

        void Menu_savetomp3(object sender, EventArgs e)
        {
            savedlg = new SaveDlg(sh);
            savedlg.Show();
        }

        void Menu_homepage(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.codeplex.com/clipspeak");
        }

        void Menu_help(object sender, EventArgs e)
        {
            if (File.Exists("readme.txt"))
            {
                System.Diagnostics.Process.Start("readme.txt");
            } else {
                System.Windows.Forms.MessageBox.Show("Cannot find readme.txt. The file should be in the same directory as the program.", "File not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        void Menu_exit(object sender, EventArgs e)
        {
            trayicon.Visible = false;
            WinAPI.ChangeClipboardChain(this.Handle, nextClipboardViewer);
            keyhook.unhook();
            Application.Exit();            
        }

        void Keyhook_KeyDown(object sender, KeyEventArgs e)
        {                        
            if (e.KeyCode == Keys.RControlKey || e.KeyCode == Keys.RShiftKey)
            {
                if (e.KeyCode == Keys.RControlKey && (System.Windows.Forms.Control.ModifierKeys & Keys.Shift) == Keys.Shift) CSToggle();
                sh.Stop();
            }
            
            //Do not set e.Handled = true
        }

        private void CSToggle()
        {            
            if (enabled)
            {
                WinAPI.ChangeClipboardChain(this.Handle, nextClipboardViewer);
                enabled = false;
                trayicon.Icon = new System.Drawing.Icon(a.GetManifestResourceStream(icons[3]));
                return;
            }
            nextClipboardViewer = (IntPtr)WinAPI.SetClipboardViewer((int)this.Handle);
            
            enabled = true;
            trayicon.Icon = new System.Drawing.Icon(a.GetManifestResourceStream(icons[2]));
            return;
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            const int WM_DRAWCLIPBOARD = 0x308;
            const int WM_CHANGECBCHAIN = 0x030D;

            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    if (!firstTime)
                    {
                        try
                        {
                            if (sh.IsDone()) sh.Speak(Clipboard.GetText());
                        }
                        catch (System.Runtime.InteropServices.COMException e)
                        {
                            MessageBox.Show("COM exception: " + e.StackTrace + ", " + e.Message + "\nInner exception: " + e.InnerException.StackTrace + " " + e.InnerException.Message);
                        }
                    }
                    WinAPI.SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    firstTime = false;
                    break;
                case WM_CHANGECBCHAIN:
                    if (m.WParam == nextClipboardViewer) nextClipboardViewer = m.LParam;
                    else WinAPI.SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}
