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
using System.Text;
using System.IO;

namespace ClipSpeak
{
    /// <summary>
    /// This class wraps SAPI speech synthesis into a single component.    
    /// </summary>
    public class SpeechHandler
    {        
        private SpeechLib.SpVoiceClass voice;
        private SpeechLib.ISpeechObjectTokens tokens;
        delegate int SpeakDelegate(string text, SpeechLib.SpeechVoiceSpeakFlags flags);
        private SpeakDelegate sd;

        public SpeechHandler()
        {
            /*if (File.Exists(Environment.GetEnvironmentVariable("SystemRoot")+@"\Speech\Xvoice.dll")) {
                SAPI4Supported = true;
            } else {
                SAPI4Supported = false;
            }*/            
            voice = new SpeechLib.SpVoiceClass();
            tokens = voice.GetVoices("", "");            
            sd = new SpeakDelegate(voice.Speak);
        }

        /// <summary>
        /// Speaks the input text using the selected voice.
        /// </summary>
        /// <param name="s">The string to be spoken.</param>
        public void Speak(string s)
        {
            sd.Invoke(s, SpeechLib.SpeechVoiceSpeakFlags.SVSFlagsAsync);
        }
        /// <summary>
        /// Speak text into a file.
        /// </summary>
        /// <param name="s">The text to be saved.</param>
        /// <param name="path">The file to save to.</param>
        /// <returns>false if a file write fails, true otherwise.</returns>
        public bool SpeakToFile(string s, string path)
        {
            try
            {                
                SpeechLib.SpFileStream stream = new SpeechLib.SpFileStream();
                SpeechLib.SpAudioFormat format = new SpeechLib.SpAudioFormat();

                format.Type = SpeechLib.SpeechAudioFormatType.SAFT32kHz16BitStereo;
                
                stream.Format = format;
                stream.Open(path, SpeechLib.SpeechStreamFileMode.SSFMCreateForWrite, false);
                voice.AudioOutputStream = stream;

                voice.Speak(s, SpeechLib.SpeechVoiceSpeakFlags.SVSFDefault);
                stream.Close();
                voice.AudioOutputStream = null;
                return true;
            } catch (Exception) {
                return false;
            }
        }

        /// <summary>
        /// Stops ongoing speech. This is done by "speaking" the empty string, "".
        /// </summary>
        public void Stop()
        {
            sd.Invoke("", SpeechLib.SpeechVoiceSpeakFlags.SVSFPurgeBeforeSpeak);
        }

        /// <summary>
        /// Returns voice ID's.
        /// </summary>
        /// <returns>An array of strings representing each voice.</returns>
        public string[] GetVoiceIds()
        {
            string[] voiceIds;            
            voiceIds = new string[tokens.Count];
            for (int i = 0; i < tokens.Count; i++)
            {
                voiceIds[i] = tokens.Item(i).GetDescription(1033);
            }
            return voiceIds;
        }

        /// <summary>
        /// Gets the speed (rate) of the active voice.
        /// </summary>
        /// <returns>An integer value between -10 and 10.</returns>
        public int GetSpeed()
        {
            int s;
            voice.GetRate(out s);
            return s;
        }

        /// <summary>
        /// Sets the speed (rate) of the selected voice.
        /// </summary>
        /// <param name="s">An integer between -10 and 10.</param>
        public void SetSpeed(int s)
        {
            voice.SetRate(s);
        }

        /// <summary>
        /// Gets the volume of the active voice.
        /// </summary>
        /// <returns>An integer between 0 and 100.</returns>
        public int GetVolume()
        {
            ushort v;
            voice.GetVolume(out v);
            return v;
        }

        /// <summary>
        /// Sets the volume of the active voice.
        /// </summary>
        /// <param name="v">An integer between 0 and 100.</param>
        public void SetVolume(int v)
        {
            voice.SetVolume((ushort)v);
        }

        /// <summary>
        /// Sets the active voice.
        /// </summary>
        /// <param name="i">The integer ID of the voice to be selected.</param>
        public void SelectVoice(int i)
        {
            voice.SetVoice((SpeechLib.ISpObjectToken)tokens.Item(i));
        }

        /// <summary>
        /// Gets the name of a voice by ID.
        /// </summary>
        /// <param name="i">The ID of the voice.</param>
        /// <returns>The voice name as a string.</returns>
        public string GetTokenString(int i)
        {
            return tokens.Item(i).GetDescription(1033);
        }

        /// <summary>
        /// Checks if speech is done (i.e. it is silent).
        /// </summary>
        /// <returns>True if silent, false otherwise.</returns>
        public bool IsDone()
        {
            if (voice.Status.RunningState == SpeechLib.SpeechRunState.SRSEDone) return true;
            return false;
        }
    }
}
