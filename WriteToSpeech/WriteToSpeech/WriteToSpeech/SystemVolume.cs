using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace WriteToSpeech
{
    public class SystemVolume
    {
        /*
 * 弹出系统音量控制器
 * */
        public static void PopupController()
        {
            ProcessStartInfo Info = new ProcessStartInfo();
            Info.FileName = "Sndvol32";
            Process.Start(Info);
        }

        /*
         * 获得音量范围和获取/设置当前音量
         * */
        public static int MaxValue
        {
            get { return int.Parse(iMaxValue.ToString()); }
        }
        public static int MinValue
        {
            get { return int.Parse(iMinValue.ToString()); }
        }
        public static int CurrentValue
        {
            get
            {
                GetVolume();
                return iCurrentValue;
            }
            set
            {
                SetValue(MaxValue, MinValue, value);
            }
        }


        #region Private Static Data Members
        private const UInt32 iMaxValue = 0xFFFF;
        private const UInt32 iMinValue = 0x0000;
        private static int iCurrentValue = 0;
        #endregion


        [DllImport("winmm.dll")]
        private static extern long waveOutSetVolume(UInt32 deviceID, UInt32 Volume);
        [DllImport("winmm.dll")]
        private static extern long waveOutGetVolume(UInt32 deviceID, out UInt32 Volume);

        #region Private Static Method
        /*
         * 得到当前音量
         **/
        private static void GetVolume()
        {
            UInt32 d, v;
            d = 0;
            long i = waveOutGetVolume(d, out v);
            UInt32 vleft = v & 0xFFFF;
            UInt32 vright = (v & 0xFFFF0000) >> 16;
            UInt32 all = vleft | vright;
            UInt32 value = (all * UInt32.Parse((MaxValue - MinValue).ToString()) / ((UInt32)iMaxValue));
            iCurrentValue = int.Parse(value.ToString());
        }

        /*
         * 修改音量值
         * */
        private static void SetValue(int aMaxValue, int aMinValue, int aValue)
        {
            //先把trackbar的value值映射到0x0000～0xFFFF范围  
            UInt32 Value = (UInt32)((double)0xffff * (double)aValue / (double)(aMaxValue - aMinValue));
            //限制value的取值范围  
            if (Value < 0) Value = 0;
            if (Value > 0xffff) Value = 0xffff;
            UInt32 left = (UInt32)Value;//左声道音量  
            UInt32 right = (UInt32)Value;//右  
            waveOutSetVolume(0, left << 16 | right); //"<<"左移，“|”逻辑或运算  
        }
        #endregion

    }
}
