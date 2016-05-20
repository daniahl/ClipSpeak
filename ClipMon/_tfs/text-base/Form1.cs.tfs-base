/*  ClipMon
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClipMon
{
    public partial class Form1 : Form
    {
        IntPtr nextClipboardViewer;

        public Form1()
        {
            nextClipboardViewer = (IntPtr)WinAPI.SetClipboardViewer((int)this.Handle);
            InitializeComponent();
        }

        private void OnClose(object sender, FormClosingEventArgs e)
        {
            WinAPI.ChangeClipboardChain(this.Handle, nextClipboardViewer);
        }


        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            const int WM_DRAWCLIPBOARD = 0x308;
            const int WM_CHANGECBCHAIN = 0x030D;

            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    try
                    {
                        richTextBox1.Text += DateTime.Now.ToString()+": WM_DRAWCLIPBOARD :: "+Clipboard.GetText()+"\n";
                    }
                    catch (Exception) { }
                    WinAPI.SendMessage(nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                    
                    break;
                case WM_CHANGECBCHAIN:
                    try
                    {
                        richTextBox1.Text += DateTime.Now.ToString()+": WM_CHANGECBCHAIN\n";
                    }
                    catch (Exception) { }
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
