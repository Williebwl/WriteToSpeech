using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace WriteToSpeech
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        #region[引用参数]
        /// <summary>
        /// 设置系统音量
        /// </summary>
        /// <param name="deviceID">设备id(多声卡使用)</param>
        /// <param name="Volume">音量值</param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public static extern long waveOutSetVolume(UInt32 deviceID, UInt32 Volume);

        /// <summary>
        /// 获取系统音量
        /// </summary>
        /// <param name="deviceID">设备id(多声卡使用)</param>
        /// <param name="Volume">音量值</param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public static extern long waveOutGetVolume(UInt32 deviceID, out UInt32 Volume);    
        #endregion

        #region[参数定义]
        DotNetSpeech.SpeechVoiceSpeakFlags SpFlags ;
        DotNetSpeech.SpVoice voice = null;
        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            SpFlags = DotNetSpeech.SpeechVoiceSpeakFlags.SVSFlagsAsync;
            voice = new DotNetSpeech.SpVoice();
            string str = "";
            //遍历系统语音类型
            for (int i = 0; i < voice.GetVoices(string.Empty, string.Empty).Count; i++)
            {
                str = voice.GetVoices(string.Empty, string.Empty).Item(i).Id;
                str=str.Substring(59,str.Length-59);
                comboBox1.Items.Add(str);
            }
            trackBar1.Value = voice.Rate;
            if (comboBox1.Items.Count != 0)
                comboBox1.SelectedIndex = 0;
            trackBar2.Value= PC_VolumeControl.VolumeControl.GetVolume();
        }


        //音量设置
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            PC_VolumeControl.VolumeControl.SetVolume(trackBar2.Value);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string str = "";
                ////遍历系统语音类型
                for (int i = 0; i < voice.GetVoices(string.Empty, string.Empty).Count; i++)
                {
                    str = voice.GetVoices(string.Empty, string.Empty).Item(i).Id;
                    str = str.Substring(59, str.Length - 59);
                    //判断是否存在中文语音类型
                    if (str==comboBox1.Text)
                    {
                        voice.Voice = voice.GetVoices(string.Empty, string.Empty).Item(i);
                        voice.Volume = trackBar3.Value;
                        voice.Speak(richTextBox1.Text, SpFlags);
                        break;
                    }

                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //设置语速/
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            voice.Rate = trackBar1.Value;
        }

        //生成文件
        private void button3_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.SaveFileDialog SFD = new System.Windows.Forms.SaveFileDialog();
            SFD.Filter = "All files (*.*)|*.*|wav files (*.wav)|*.wav";
            SFD.Title = "Save to a wav file";
            SFD.FilterIndex = 2;
            SFD.RestoreDirectory = true;
            if (SFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DotNetSpeech.SpeechStreamFileMode SSFM = DotNetSpeech.SpeechStreamFileMode.SSFMCreateForWrite;
                DotNetSpeech.SpFileStream SFS = new DotNetSpeech.SpFileStreamClass();
                SFS.Open(SFD.FileName, SSFM, false);
                string str = "";
                ////遍历系统语音类型
                for (int i = 0; i < voice.GetVoices(string.Empty, string.Empty).Count; i++)
                {
                    str = voice.GetVoices(string.Empty, string.Empty).Item(i).Id;
                    str = str.Substring(59, str.Length - 59);
                    //判断是否存在中文语音类型
                    if (str == comboBox1.Text)
                    {
                        voice.Voice = voice.GetVoices(string.Empty, string.Empty).Item(i);
                        break;
                    }

                }
                voice.Volume = trackBar3.Value;
                voice.AudioOutputStream = SFS;
                voice.Speak(this.richTextBox1.Text, SpFlags);
                voice.WaitUntilDone(System.Threading.Timeout.Infinite);
                SFS.Close();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://g.iciba.com/dictdown/tts.html");
        }
 
    }
}
