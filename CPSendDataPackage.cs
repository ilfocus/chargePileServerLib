using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPServer
{
    public class CPSendDataPackage
    {
        public static byte ConvertBCD(byte b) {
            //高四位  
            byte b1 = (byte)(b / 10);
            //低四位  
            byte b2 = (byte)(b % 10);

            return (byte)((b1 << 4) | b2);
        }

        #region 充电桩数据打包
        public byte[] sendDataPackage(byte cmd) {

            const int QUERY_MSG_NUM = 14;
            byte[] bRequestCmd = new byte[QUERY_MSG_NUM];     // 设置数组，并进行初始化，保存发送数据数组

            CPDataCheck dataCheck = new CPDataCheck();
            UInt64 cpAddress = dataCheck.CHARGING_PILE_ADDRESS;

            for (int i = 0; i < QUERY_MSG_NUM; i++) {
                bRequestCmd[i] = 0x00;
            }   // 要发送的数据初始化                      
            bRequestCmd[0] = 0xff;             // 起始字符高位
            bRequestCmd[1] = 0x5a;             // 起始字符低位
            // 充电桩地址
            bRequestCmd[2] = (byte)(cpAddress >> 56);
            bRequestCmd[3] = (byte)(cpAddress >> 48);
            bRequestCmd[4] = (byte)(cpAddress >> 40);
            bRequestCmd[5] = (byte)(cpAddress >> 32);
            bRequestCmd[6] = (byte)(cpAddress >> 24); ;
            bRequestCmd[7] = (byte)(cpAddress >> 16); ;
            bRequestCmd[8] = (byte)(cpAddress >> 8); ;
            bRequestCmd[9] = (byte)(cpAddress);

            bRequestCmd[10] = 0x03;               // frame length
            bRequestCmd[11] = cmd;            // commade code--heart frame commade

            // frame tail
            bRequestCmd[12] = dataCheck.GetBCC_Check(bRequestCmd, 10, bRequestCmd.Length - 2); // bcc校验
            bRequestCmd[13] = 0xed;

            return bRequestCmd;
        }
        public byte[] sendDataPackage(byte cmd,UInt64 address) {

            const int QUERY_MSG_NUM = 14;
            byte[] bRequestCmd = new byte[QUERY_MSG_NUM];     // 设置数组，并进行初始化，保存发送数据数组

            CPDataCheck dataCheck = new CPDataCheck();
            UInt64 cpAddress = dataCheck.CHARGING_PILE_ADDRESS;

            for (int i = 0; i < QUERY_MSG_NUM; i++) {
                bRequestCmd[i] = 0x00;
            }   // 要发送的数据初始化                      
            bRequestCmd[0] = 0xff;             // 起始字符高位
            bRequestCmd[1] = 0x5a;             // 起始字符低位
            // 充电桩地址
            bRequestCmd[2] = (byte)(address >> 56);
            bRequestCmd[3] = (byte)(address >> 48);
            bRequestCmd[4] = (byte)(address >> 40);
            bRequestCmd[5] = (byte)(address >> 32);
            bRequestCmd[6] = (byte)(address >> 24); ;
            bRequestCmd[7] = (byte)(address >> 16); ;
            bRequestCmd[8] = (byte)(address >> 8); ;
            bRequestCmd[9] = (byte)(address);

            bRequestCmd[10] = 0x03;               // frame length
            bRequestCmd[11] = cmd;            // commade code--heart frame commade

            // frame tail
            bRequestCmd[12] = dataCheck.GetBCC_Check(bRequestCmd, 10, bRequestCmd.Length - 2); // bcc校验
            bRequestCmd[13] = 0xed;

            return bRequestCmd;
        }
        #endregion
        #region 充电桩启停数据打包
        public byte[] sendCPStartupPackage(byte cmd,byte para) {

            const int QUERY_MSG_NUM = 15;
            byte[] bRequestCmd = new byte[QUERY_MSG_NUM];     // 设置数组，并进行初始化，保存发送数据数组

            CPDataCheck dataCheck = new CPDataCheck();
            UInt64 cpAddress = dataCheck.CHARGING_PILE_ADDRESS;

            for (int i = 0; i < QUERY_MSG_NUM; i++) {
                bRequestCmd[i] = 0x00;
            }   // 要发送的数据初始化                      
            bRequestCmd[0] = 0xff;             // 起始字符高位
            bRequestCmd[1] = 0x5a;             // 起始字符低位
            // 充电桩地址
            bRequestCmd[2] = (byte)(cpAddress >> 56);
            bRequestCmd[3] = (byte)(cpAddress >> 48);
            bRequestCmd[4] = (byte)(cpAddress >> 40);
            bRequestCmd[5] = (byte)(cpAddress >> 32);
            bRequestCmd[6] = (byte)(cpAddress >> 24); ;
            bRequestCmd[7] = (byte)(cpAddress >> 16); ;
            bRequestCmd[8] = (byte)(cpAddress >> 8); ;
            bRequestCmd[9] = (byte)(cpAddress); ;

            bRequestCmd[10] = 0x04;               // frame length
            bRequestCmd[11] = cmd;            // commade code--heart frame commade

            // parameter
            bRequestCmd[12] = para;
            // frame tail
            bRequestCmd[13] = dataCheck.GetBCC_Check(bRequestCmd, 10, bRequestCmd.Length - 2); // bcc校验
            bRequestCmd[14] = 0xed;

            return bRequestCmd;
        }
        public byte[] sendCPStartupPackage(byte cmd, byte para,UInt64 address) {

            const int QUERY_MSG_NUM = 15;
            byte[] bRequestCmd = new byte[QUERY_MSG_NUM];     // 设置数组，并进行初始化，保存发送数据数组

            for (int i = 0; i < QUERY_MSG_NUM; i++) {
                bRequestCmd[i] = 0x00;
            }   // 要发送的数据初始化                      
            bRequestCmd[0] = 0xff;             // 起始字符高位
            bRequestCmd[1] = 0x5a;             // 起始字符低位
            // 充电桩地址
            bRequestCmd[2] = (byte)(address >> 56);
            bRequestCmd[3] = (byte)(address >> 48);
            bRequestCmd[4] = (byte)(address >> 40);
            bRequestCmd[5] = (byte)(address >> 32);
            bRequestCmd[6] = (byte)(address >> 24); ;
            bRequestCmd[7] = (byte)(address >> 16); ;
            bRequestCmd[8] = (byte)(address >> 8); ;
            bRequestCmd[9] = (byte)(address); ;

            bRequestCmd[10] = 0x04;               // frame length
            bRequestCmd[11] = cmd;            // commade code--heart frame commade

            // parameter
            bRequestCmd[12] = para;
            // frame tail
            CPDataCheck dataCheck = new CPDataCheck();
            bRequestCmd[13] = dataCheck.GetBCC_Check(bRequestCmd, 10, bRequestCmd.Length - 2); // bcc校验
            bRequestCmd[14] = 0xed;

            return bRequestCmd;
        }
        #endregion
        #region 设置费率数据打包
        public byte[] sendRateDataPackage(byte cmd, CPSetSendDataRate data) {
            const int QUERY_MSG_NUM = 30;
            byte[] bRequestCmd = new byte[QUERY_MSG_NUM];     // set array and init

            CPDataCheck dataCheck = new CPDataCheck();
            UInt64 cpAddress = dataCheck.CHARGING_PILE_ADDRESS;

            for (int i = 0; i < QUERY_MSG_NUM; i++) {
                bRequestCmd[i] = 0x00;
            }   // send data init                      
            bRequestCmd[0] = 0xff;             // start char high bit
            bRequestCmd[1] = 0x5a;             // start char low  bit
            // Charging pile address
            bRequestCmd[2] = (byte)(cpAddress >> 56);
            bRequestCmd[3] = (byte)(cpAddress >> 48);
            bRequestCmd[4] = (byte)(cpAddress >> 40);
            bRequestCmd[5] = (byte)(cpAddress >> 32);
            bRequestCmd[6] = (byte)(cpAddress >> 24); ;
            bRequestCmd[7] = (byte)(cpAddress >> 16); ;
            bRequestCmd[8] = (byte)(cpAddress >> 8); ;
            bRequestCmd[9] = (byte)(cpAddress); ;
            //
            bRequestCmd[10] = 0x13;               // frame length
            bRequestCmd[11] = cmd;            // commade code--heart frame commade

            // parameter
            // pointed electricity price
            
            bRequestCmd[12] = (byte)(data.cpPointPrice >> 24);
            bRequestCmd[13] = (byte)(data.cpPointPrice >> 16);
            bRequestCmd[14] = (byte)(data.cpPointPrice >> 8);
            bRequestCmd[15] = (byte)(data.cpPointPrice >> 0);
            // peak electricity price
            bRequestCmd[16] = (byte)(data.cpPeakPrice >> 24);
            bRequestCmd[17] = (byte)(data.cpPeakPrice >> 16); ;
            bRequestCmd[18] = (byte)(data.cpPeakPrice >> 8);
            bRequestCmd[19] = (byte)(data.cpPeakPrice >> 0);
            // flat electricity price
            bRequestCmd[20] = (byte)(data.cpFlatPrice >> 24);
            bRequestCmd[21] = (byte)(data.cpFlatPrice >> 16); ;
            bRequestCmd[22] = (byte)(data.cpFlatPrice >> 8);
            bRequestCmd[23] = (byte)(data.cpFlatPrice >> 0);
            // valley electricity price
            bRequestCmd[24] = (byte)(data.cpVallPrice >> 24);
            bRequestCmd[25] = (byte)(data.cpVallPrice >> 16); ;
            bRequestCmd[26] = (byte)(data.cpVallPrice >> 8);
            bRequestCmd[27] = (byte)(data.cpVallPrice >> 0);

            // frame tail
            bRequestCmd[28] = dataCheck.GetBCC_Check(bRequestCmd, 10, bRequestCmd.Length - 2); // bcc校验
            bRequestCmd[29] = 0xed;

            return bRequestCmd;
        }
        public byte[] sendRateDataPackage(byte cmd, CPSetSendDataRate data,UInt64 address) {
            const int QUERY_MSG_NUM = 30;
            byte[] bRequestCmd = new byte[QUERY_MSG_NUM];     // set array and init

            for (int i = 0; i < QUERY_MSG_NUM; i++) {
                bRequestCmd[i] = 0x00;
            }   // send data init                      
            bRequestCmd[0] = 0xff;             // start char high bit
            bRequestCmd[1] = 0x5a;             // start char low  bit
            // Charging pile address
            bRequestCmd[2] = (byte)(address >> 56);
            bRequestCmd[3] = (byte)(address >> 48);
            bRequestCmd[4] = (byte)(address >> 40);
            bRequestCmd[5] = (byte)(address >> 32);
            bRequestCmd[6] = (byte)(address >> 24); ;
            bRequestCmd[7] = (byte)(address >> 16); ;
            bRequestCmd[8] = (byte)(address >> 8); ;
            bRequestCmd[9] = (byte)(address); ;
            //
            bRequestCmd[10] = 0x13;               // frame length
            bRequestCmd[11] = cmd;            // commade code--heart frame commade

            // parameter
            // pointed electricity price

            bRequestCmd[12] = (byte)(data.cpPointPrice >> 24);
            bRequestCmd[13] = (byte)(data.cpPointPrice >> 16);
            bRequestCmd[14] = (byte)(data.cpPointPrice >> 8);
            bRequestCmd[15] = (byte)(data.cpPointPrice >> 0);
            // peak electricity price
            bRequestCmd[16] = (byte)(data.cpPeakPrice >> 24);
            bRequestCmd[17] = (byte)(data.cpPeakPrice >> 16); ;
            bRequestCmd[18] = (byte)(data.cpPeakPrice >> 8);
            bRequestCmd[19] = (byte)(data.cpPeakPrice >> 0);
            // flat electricity price
            bRequestCmd[20] = (byte)(data.cpFlatPrice >> 24);
            bRequestCmd[21] = (byte)(data.cpFlatPrice >> 16); ;
            bRequestCmd[22] = (byte)(data.cpFlatPrice >> 8);
            bRequestCmd[23] = (byte)(data.cpFlatPrice >> 0);
            // valley electricity price
            bRequestCmd[24] = (byte)(data.cpVallPrice >> 24);
            bRequestCmd[25] = (byte)(data.cpVallPrice >> 16); ;
            bRequestCmd[26] = (byte)(data.cpVallPrice >> 8);
            bRequestCmd[27] = (byte)(data.cpVallPrice >> 0);

            // frame tail
            CPDataCheck dataCheck = new CPDataCheck();
            bRequestCmd[28] = dataCheck.GetBCC_Check(bRequestCmd, 10, bRequestCmd.Length - 2); // bcc校验
            bRequestCmd[29] = 0xed;

            return bRequestCmd;
        }
        #endregion
        #region 设置时间数据打包
        public byte[] sendTimeDataPackage(byte cmd, CPSetSendDataTime time) {
            const int QUERY_MSG_NUM = 21;
            byte[] bRequestCmd = new byte[QUERY_MSG_NUM];     // set array and init

            CPDataCheck dataCheck = new CPDataCheck();
            UInt64 cpAddress = dataCheck.CHARGING_PILE_ADDRESS;

            for (int i = 0; i < QUERY_MSG_NUM; i++) {
                bRequestCmd[i] = 0x00;
            }   // send data init                      
            bRequestCmd[0] = 0xff;             // start char high bit
            bRequestCmd[1] = 0x5a;             // start char low  bit
            // Charging pile address
            bRequestCmd[2] = (byte)(cpAddress >> 56);
            bRequestCmd[3] = (byte)(cpAddress >> 48);
            bRequestCmd[4] = (byte)(cpAddress >> 40);
            bRequestCmd[5] = (byte)(cpAddress >> 32);
            bRequestCmd[6] = (byte)(cpAddress >> 24); ;
            bRequestCmd[7] = (byte)(cpAddress >> 16); ;
            bRequestCmd[8] = (byte)(cpAddress >> 8); ;
            bRequestCmd[9] = (byte)(cpAddress); ;
            //
            bRequestCmd[10] = 0x0a;               // frame length
            bRequestCmd[11] = cmd;            // commade code--heart frame commade


            // parameter BCD code
            bRequestCmd[12] = ConvertBCD( (byte)(Convert.ToInt16(time.year) / 100));
            bRequestCmd[13] = ConvertBCD( (byte)(Convert.ToInt16(time.year) % 100));  // year

            bRequestCmd[14] = ConvertBCD(Convert.ToByte(time.month));  // month

            bRequestCmd[15] = ConvertBCD(Convert.ToByte(time.day));  // day

            bRequestCmd[16] = ConvertBCD(Convert.ToByte(time.hour));  // hour

            bRequestCmd[17] = ConvertBCD(Convert.ToByte(time.minute));  // minute

            bRequestCmd[18] = ConvertBCD(Convert.ToByte(time.second));  // second 
            // frame tail
            bRequestCmd[19] = dataCheck.GetBCC_Check(bRequestCmd, 10, bRequestCmd.Length - 2); // bcc校验
            bRequestCmd[20] = 0xed;

            return bRequestCmd;
        }
        public byte[] sendTimeDataPackage(byte cmd, CPSetSendDataTime time,UInt64 address) {
            const int QUERY_MSG_NUM = 21;
            byte[] bRequestCmd = new byte[QUERY_MSG_NUM];     // set array and init

            for (int i = 0; i < QUERY_MSG_NUM; i++) {
                bRequestCmd[i] = 0x00;
            }   // send data init                      
            bRequestCmd[0] = 0xff;             // start char high bit
            bRequestCmd[1] = 0x5a;             // start char low  bit
            // Charging pile address
            bRequestCmd[2] = (byte)(address >> 56);
            bRequestCmd[3] = (byte)(address >> 48);
            bRequestCmd[4] = (byte)(address >> 40);
            bRequestCmd[5] = (byte)(address >> 32);
            bRequestCmd[6] = (byte)(address >> 24); ;
            bRequestCmd[7] = (byte)(address >> 16); ;
            bRequestCmd[8] = (byte)(address >> 8); ;
            bRequestCmd[9] = (byte)(address); ;
            //
            bRequestCmd[10] = 0x0a;               // frame length
            bRequestCmd[11] = cmd;            // commade code--heart frame commade


            // parameter BCD code
            bRequestCmd[12] = ConvertBCD((byte)(Convert.ToInt16(time.year) / 100));
            bRequestCmd[13] = ConvertBCD((byte)(Convert.ToInt16(time.year) % 100));  // year

            bRequestCmd[14] = ConvertBCD(Convert.ToByte(time.month));  // month

            bRequestCmd[15] = ConvertBCD(Convert.ToByte(time.day));  // day

            bRequestCmd[16] = ConvertBCD(Convert.ToByte(time.hour));  // hour

            bRequestCmd[17] = ConvertBCD(Convert.ToByte(time.minute));  // minute

            bRequestCmd[18] = ConvertBCD(Convert.ToByte(time.second));  // second 
            // frame tail
            CPDataCheck dataCheck = new CPDataCheck();
            bRequestCmd[19] = dataCheck.GetBCC_Check(bRequestCmd, 10, bRequestCmd.Length - 2); // bcc校验
            bRequestCmd[20] = 0xed;

            return bRequestCmd;
        }
        #endregion


        public void setCPAddress(string str) {
            CPDataCheck dataCheck = new CPDataCheck();
            dataCheck.CHARGING_PILE_ADDRESS = Convert.ToUInt64(str);
        }
        public void setCPAddress(UInt64 address) {
            CPDataCheck dataCheck = new CPDataCheck();
            dataCheck.CHARGING_PILE_ADDRESS = address;
        }
    }

    public class CPSetSendDataRate
    {
        private UInt32 _cpPointPrice = 0;
        private UInt32 _cpPeakPrice = 0;
        private UInt32 _cpFlatPrice = 0;
        private UInt32 _cpVallPrice = 0;

        public double cpPointPriceD = 0.0;
        public double cpPeakPriceD = 0.0;
        public double cpFlatPriceD = 0.0;
        public double cpVallPriceD = 0.0;

        public UInt32 cpPointPrice {
            get {
                _cpPointPrice = (UInt32)(cpPointPriceD * 100);
                return _cpPointPrice;
            }
        }
        public UInt32 cpPeakPrice {
            get {
                _cpPeakPrice = (UInt32)(cpPeakPriceD * 100);
                return _cpPeakPrice;
            }
        }
        public UInt32 cpFlatPrice {
            get {
                _cpFlatPrice = (UInt32)(cpFlatPriceD * 100);
                return _cpFlatPrice;
            }
        }
        public UInt32 cpVallPrice {
            get {
                _cpVallPrice = (UInt32)(cpVallPriceD * 100);
                return _cpVallPrice;
            }
        }
    }

    public class CPSetSendDataTime
    {

        public string year;
        public string month;
        public string day;
        public string hour;
        public string minute;
        public string second;
    }
}
