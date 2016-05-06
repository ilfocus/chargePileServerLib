using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CPServer
{
    public class CPBackDataParse
    {
        public enum BackDataType {
            HeartFrameType,
            SetTimeType,
            SetRateType,
            StateType,
            StartupType,
            CurInfoType
        }
        
        public CPGetHeartFrame cpGetHeartData = new CPGetHeartFrame();
        public CPGetSetTime cpGetTimeData = new CPGetSetTime();
        public CPGetSetRate cpGetRateData = new CPGetSetRate();
        public CPGetState cpGetStateData = new CPGetState();
        public CPGetStartup cpGetStartupData = new CPGetStartup();
        public CPGetCurInfo cpGetCurInfoData = new CPGetCurInfo();

        public BackDataType classType;
        public UInt64 cpAddress;
        //private UInt32 fourByteCombinationUint32()

        public CPBackDataParse packageParser(byte[] arr, int length) {
            // parameter check
            if (arr == null) return null;
            if (arr.Length < length) return null;
            // dataPackage check
            if (arr[0] == 0xff && arr[1] == 0x5a) {
                UInt32 addressH = (UInt32)((arr[2] << 24) | (arr[3] << 16)
                                    | (arr[4] << 8) | (arr[5]));
                UInt32 addressL = (UInt32)((arr[6] << 24) | (arr[7] << 16)
                                    | (arr[8] << 8) | (arr[9]));
                this.cpAddress = ((UInt64)addressH) << 32 | addressL;
            } else {
                return null;
            }
            CPDataCheck dataCheck = new CPDataCheck();
            if (dataCheck.dataPackageCheck(arr,length)) {
                switch (arr[11]) {
                    case 0x20: {
                        this.classType = BackDataType.HeartFrameType;
                        if (arr[13] == 0x00) {
                            this.cpGetHeartData.cpHeartFrameExecuteResult = true;
                        } else {
                            this.cpGetHeartData.cpHeartFrameExecuteResult = false;
                        }
                        break;
                    }
                    case 0x21: {
                        this.classType = BackDataType.SetTimeType;
                        if (arr[13] == 0x00) {
                            this.cpGetTimeData.cpSetTimeExecuteResult = true;
                        } else {
                            this.cpGetTimeData.cpSetTimeExecuteResult = false;
                        }
                        break;
                    }
                    case 0x22: {
                        this.classType = BackDataType.SetRateType;
                        if (arr[13] == 0x00) {
                            this.cpGetRateData.cpSetRateExecuteResult = true;
                        } else {
                            this.cpGetRateData.cpSetRateExecuteResult = false;
                        }
                        break;
                    }
                    case 0x23: {
                        this.classType = BackDataType.StateType;
                        if (arr[13] == 0x00) {
                            this.cpGetStateData.cpGetStateExecuteResult = true;
                            // deal parameter message
                            this.cpGetStateData.cpVoltage = (UInt32)((arr[14] << 24) | (arr[15] << 16)
                                                                   | (arr[16] << 8) | (arr[17]));
                            this.cpGetStateData.cpCurrent = (UInt32)((arr[18] << 24) | (arr[19] << 16)
                                                                   | (arr[20] << 8) | (arr[21]));
                            this.cpGetStateData.cpTotalElect = (UInt32)((arr[22] << 24) | (arr[23] << 16)
                                                                   | (arr[24] << 8) | (arr[25]));
                            this.cpGetStateData.cpPointElect = (UInt32)((arr[26] << 24) | (arr[27] << 16)
                                                                   | (arr[28] << 8) | (arr[29]));
                            this.cpGetStateData.cpPeakElect = (UInt32)((arr[30] << 24) | (arr[31] << 16)
                                                                   | (arr[32] << 8) | (arr[33]));
                            this.cpGetStateData.cpFlatElect = (UInt32)((arr[34] << 24) | (arr[35] << 16)
                                                                   | (arr[36] << 8) | (arr[37]));
                            this.cpGetStateData.cpValleyElect = (UInt32)((arr[38] << 24) | (arr[39] << 16)
                                                                   | (arr[40] << 8) | (arr[41]));
                            
                            this.cpGetStateData.cpEmergencyBtn = arr[42];
                            this.cpGetStateData.cpSpace1 = arr[43];
                            this.cpGetStateData.cpMeterState = arr[44];
                            this.cpGetStateData.cpSpace2 = arr[45];
                            this.cpGetStateData.cpChargePlug = arr[46];
                            this.cpGetStateData.cpSpace3 = arr[47];
                            this.cpGetStateData.cpCurrentState = arr[48];
                            
                        } else {
                            this.cpGetStateData.cpGetStateExecuteResult = false;
                        }
                        break;
                    }
                    case 0x24: {
                        this.classType = BackDataType.StartupType;
                        if (arr[13] == 0x00) {
                            this.cpGetStartupData.cpControlExecuteResult = true;
                            if (arr[14] == 0x01) {
                                this.cpGetStartupData.cpWorkState = CPGetStartup.CP_WORK_STATE.cpStartWork;
                            } else if (arr[14] == 0x02) {
                                this.cpGetStartupData.cpWorkState = CPGetStartup.CP_WORK_STATE.cpPauseWork;
                            } else if (arr[14] == 0x03) {
                                this.cpGetStartupData.cpWorkState = CPGetStartup.CP_WORK_STATE.cpStopWork;
                            } else if (arr[14] == 0x22) {
                                this.cpGetStartupData.cpWorkState = CPGetStartup.CP_WORK_STATE.cpRecoverWork;
                            }
                         } else {
                             this.cpGetStartupData.cpControlExecuteResult = false;
                        }
                        break;
                    }
                    case 0x25: {
                        this.classType = BackDataType.CurInfoType;
                        if (arr[13] == 0x00) {
                            this.cpGetCurInfoData.cpGetCurInfoExecuteResult = true;
                            // deal parameter
                            this.cpGetCurInfoData.cpChargeTotalElect = (UInt32)((arr[14] << 24) | (arr[15] << 16)
                                                                   | (arr[16] << 8) | (arr[17]));

                            this.cpGetCurInfoData.cpChargeTotalPrice = (UInt32)((arr[18] << 24) | (arr[19] << 16)
                                                                   | (arr[20] << 8) | (arr[21]));
                            this.cpGetCurInfoData.cpChargePointElect = (UInt32)((arr[22] << 24) | (arr[23] << 16)
                                                                   | (arr[24] << 8) | (arr[25]));
                            this.cpGetCurInfoData.cpChargePeakElect = (UInt32)((arr[26] << 24) | (arr[27] << 16)
                                                                   | (arr[28] << 8) | (arr[29]));
                            this.cpGetCurInfoData.cpChargeFlatElect = (UInt32)((arr[30] << 24) | (arr[31] << 16)
                                                                   | (arr[32] << 8) | (arr[33]));
                            this.cpGetCurInfoData.cpChargeValleyElect = (UInt32)((arr[34] << 24) | (arr[35] << 16)
                                                                   | (arr[36] << 8) | (arr[37]));
                            this.cpGetCurInfoData.cpPointElectPrice = (UInt32)((arr[38] << 24) | (arr[39] << 16)
                                                                   | (arr[40] << 8) | (arr[41]));


                            this.cpGetCurInfoData.cpPeakElectPrice = (UInt32)((arr[42] << 24) | (arr[43] << 16)
                                                                   | (arr[44] << 8) | (arr[45]));
                            this.cpGetCurInfoData.cpFlatElectPrice = (UInt32)((arr[46] << 24) | (arr[47] << 16)
                                                                   | (arr[48] << 8) | (arr[49]));
                            this.cpGetCurInfoData.cpValleyElectPrice = (UInt32)((arr[50] << 24) | (arr[51] << 16)
                                                                   | (arr[52] << 8) | (arr[53]));
                            this.cpGetCurInfoData.cpPointCost = (UInt32)((arr[54] << 24) | (arr[55] << 16)
                                                                   | (arr[56] << 8) | (arr[57]));
                            this.cpGetCurInfoData.cpPeakCost = (UInt32)((arr[58] << 24) | (arr[59] << 16)
                                                                   | (arr[60] << 8) | (arr[61]));
                            this.cpGetCurInfoData.cpFlatCost = (UInt32)((arr[62] << 24) | (arr[63] << 16)
                                                                   | (arr[64] << 8) | (arr[65]));

                            this.cpGetCurInfoData.cpValleyCost = (UInt32)((arr[66] << 24) | (arr[67] << 16)
                                                                   | (arr[68] << 8) | (arr[69]));

                        } else {
                            this.cpGetCurInfoData.cpGetCurInfoExecuteResult = false; 
                        }
                        break;
                    }
                    default: {
                            break;
                        }
                } 
            }
            return this;
        }

        public string dealParaChange(UInt32 para) {

            UInt32 iPart = para / 100;
            UInt32 fPart = para % 100;
            string temp = iPart.ToString() + "." + fPart.ToString();
            return temp;
        }
    }
    public class CPGetHeartFrame
    {
        public bool cpHeartFrameExecuteResult = false;
    }
    public class CPGetSetTime
    {
        public bool cpSetTimeExecuteResult = false;
    }
    public class CPGetSetRate
    {
        public bool cpSetRateExecuteResult = false;
    }
    public class CPGetState
    {
        public bool cpGetStateExecuteResult = false;
        // accurate to 2 decimal places
        public UInt32 cpVoltage = 0;
        public UInt32 cpCurrent = 0;
        public UInt32 cpTotalElect = 0;
        public UInt32 cpPointElect = 0;
        public UInt32 cpPeakElect = 0;
        public UInt32 cpFlatElect = 0;
        public UInt32 cpValleyElect = 0;

        public byte cpEmergencyBtn = 0;  // 0x00:normal;0x01:push down
        public byte cpSpace1 = 0;        // space data
        public byte cpMeterState = 0;    // 0x00 normal;0x01:fault
        public byte cpSpace2 = 0;        // space data
        public byte cpChargePlug = 0;    // 0x00:normal;0x01:fault
        public byte cpSpace3 = 0;        // space data
        public byte cpCurrentState = 0;  // 0x00:normal;0x01:fault
    }
    public class CPGetStartup
    {
        public bool cpControlExecuteResult = false;
        public enum CP_WORK_STATE {
            cpStartWork,
            cpPauseWork,
            cpRecoverWork,
            cpStopWork
        }
        public CP_WORK_STATE cpWorkState;
    }
    public class CPGetCurInfo
    {
        public bool cpGetCurInfoExecuteResult = false;
        
        public UInt32 cpChargeTotalElect = 0;
        public UInt32 cpChargeTotalPrice = 0;
        public UInt32 cpChargePointElect = 0;
        public UInt32 cpChargePeakElect = 0;
        public UInt32 cpChargeFlatElect = 0;
        public UInt32 cpChargeValleyElect = 0;
        public UInt32 cpPointElectPrice = 0;
        public UInt32 cpPeakElectPrice = 0;
        public UInt32 cpFlatElectPrice = 0;
        public UInt32 cpValleyElectPrice = 0;
        public UInt32 cpPointCost = 0;
        public UInt32 cpPeakCost = 0;
        public UInt32 cpFlatCost = 0;
        public UInt32 cpValleyCost = 0;

    }
}