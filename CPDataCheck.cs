using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPServer
{
    public class CPDataCheck
    {
        public UInt64 CHARGING_PILE_ADDRESS = 0x1122334455667788;
        // BCC:异或校验法(Block Check Character)
        // startNum 以开始点，从帧长度开始
        // endNum arr.length - 2 
        public byte GetBCC_Check(byte[] temp, int startNum, int endNum)
        {
            byte A = 0;
            for (int i = startNum; i < endNum; i++) {
                A ^= temp[i];
            }
            return A;
        }
        // 检测数据帧长度是否正确,正确返回ture,出错返回false
        public bool DataLengthCheck(byte[] array,int lenNum)
        {
            if (array.Length < lenNum) return false;
            if ((array[lenNum] == (array.Length - lenNum - 1))) return true;
            return false;
        }
        public bool AddressCheck(byte[] array)
        {
            // array[2] ~ array[9] 为充电桩地址
            UInt32 addressH = (UInt32)((array[2] << 24) | (array[3] << 16) 
                                    | (array[4] << 8) | (array[5]));
            UInt32 addressL = (UInt32)((array[6] << 24) | (array[7] << 16) 
                                    | (array[8] << 8) | (array[9]));
            UInt64 LocalAddress = CHARGING_PILE_ADDRESS;
            if ( (addressH == (LocalAddress >> 32))
              && (addressL == ((UInt32)LocalAddress))) return true;
            return false;
        }
        public bool AddressCheck(byte[] array,UInt64 address) {
            // array[2] ~ array[9] 为充电桩地址
            UInt32 addressH = (UInt32)((array[2] << 24) | (array[3] << 16)
                                    | (array[4] << 8) | (array[5]));
            UInt32 addressL = (UInt32)((array[6] << 24) | (array[7] << 16)
                                    | (array[8] << 8) | (array[9]));
            UInt64 LocalAddress = address;
            if ((addressH == (LocalAddress >> 32))
              && (addressL == ((UInt32)LocalAddress))) return true;
            return false;
        }

        public bool dataFirstCheck(byte[] dataArray,int dataLen) {
            if (dataArray.Length != dataLen) return false;
            if (0xff == dataArray[0] && 0x5a == dataArray[1]) return true;
            return false;
        }

        public bool dataPackageCheck(byte[] dataArray, int dataLen) {
            //bool addressCheck = this.AddressCheck(dataArray);
            //if (!addressCheck) return false;
            bool lenCheck = this.DataLengthCheck(dataArray, 10);
            if (!lenCheck) return false;

            byte bccCheckData = this.GetBCC_Check(dataArray, 10, dataArray.Length - 2);
            if ((bccCheckData == dataArray[dataLen - 2])
               && (0xED == dataArray[dataLen - 1])) {   // 参数校验正确
                   return true;
            }
            return false;
        }
    }
}
