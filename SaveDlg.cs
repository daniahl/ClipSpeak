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
    public partial class SaveDlg : Form
    {
        SpeechHandler sh;        

        public SaveDlg(SpeechHandler s)
        {
            sh = s;            
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {            
            Close();            
        }

        private void browseBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.Description = "Select a folder to put the MP3 file.";
            folder.ShowDialog();
            textBox1.Text = folder.SelectedPath;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Please choose a valid folder", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (nameBox.Text == "")
            {
                MessageBox.Show("Please enter a name for the output file", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            button1.Enabled = false;
            Hide();
            sh.SpeakToFile(textBox2.Text, textBox1.Text + "\\" + nameBox.Text + ".wav");            
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.CreateNoWindow = false;            
            p.StartInfo.FileName = System.IO.Directory.GetCurrentDirectory() + @"\LAME\lame.exe";
            p.StartInfo.Arguments = "-b 64 \"" + textBox1.Text + @"\" + nameBox.Text + ".wav\" \"" + textBox1.Text + @"\" + nameBox.Text + ".mp3\"";
            p.Start();
            p.WaitForExit();
            System.IO.File.Delete(textBox1.Text + "\\" + nameBox.Text + ".wav");
            MessageBox.Show(this, "MP3 file saved successfully!", "Save to MP3", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();            
        }

        private void Keypress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(1))
            {
                textBox2.SelectAll();
                e.Handled = true;
            }            
        }

        private void SaveDlg_Load(object sender, EventArgs e)
        {
            ActiveControl = browseBtn;
            CancelButton = button2;
        }
                             
    }
}
