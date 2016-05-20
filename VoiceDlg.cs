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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClipSpeak
{
    public partial class VoiceDlg : Form
    {
        private SpeechHandler sh;

        public VoiceDlg(SpeechHandler handler)
        {
            sh = handler;
            InitializeComponent();           

            voiceList.Items.AddRange(sh.GetVoiceIds());            

            volBar.Value = sh.GetVolume();
            speedBar.Value = sh.GetSpeed();
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            string selected = (string)voiceList.SelectedItem;
            for (int i = 0; i < voiceList.Items.Count; i++)
            {
                if (selected == sh.GetTokenString(i)) {
                    sh.SelectVoice(i);
                }                
            }
            sh.SetVolume(volBar.Value);
            sh.SetSpeed(speedBar.Value);
            Close();
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void VoiceDlg_Load(object sender, EventArgs e)
        {
            ActiveControl = button_ok;
            CancelButton = cancelBtn;            
        }        
    }
}
