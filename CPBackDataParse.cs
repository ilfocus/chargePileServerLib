using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Socket监听类
using System.Net.Sockets;
using System.Net;
using System.Threading;

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
        
        //public List<ChargePileDevice> cpDevice = new List<ChargePileDevice>(); // creat charge pile object collection
        
        public CPGetHeartFrame cpGetHeartData = new CPGetHeartFrame();
        public CPGetSetTime cpGetTimeData = new CPGetSetTime();
        public CPGetSetRate cpGetRateData = new CPGetSetRate();
        public CPGetState cpGetStateData = new CPGetState();
        public CPGetStartup cpGetStartupData = new CPGetStartup();
        public CPGetCurInfo cpGetCurInfoData = new CPGetCurInfo();
        public BackDataType classType;
        public UInt64 cpAddress;

        
        private static byte[] result = new byte[1024];
        private static int myProt = 8885;   //端口
        static Socket serverSocket;
        static Socket clientSocket;/**/
        public CPBackDataParse() {
        }
        /*
        private bool updataDeviceList(ChargePileDevice newDevice) {
            for (int i = 0; i < cpDevice.Count; i++) {
                if (newDevice.chargePileMachineAddress == cpDevice[i].chargePileMachineAddress) {
                    cpDevice[i] = newDevice;
                    return true;
                }
            }
            return false;
        }*/
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
    public class ChargePileDevice {
        public bool isActive = false;
        //public IPAddress chargePileIPAddress = IPAddress.Parse("127.0.0.1");
        //public int chargePilePort = 0;
        //public UInt64 chargePileMachineAddress = 0;
        public CPBackDataParse chargePileData = new CPBackDataParse();
        //public Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public Socket clientSocket;

        private static byte[] result = new byte[1024];
        private static int myProt = 8885;   //端口
        static Socket serverSocket;

        public ChargePileDevice() {
            Console.WriteLine("创建类ChargePileDevice成功");
            /*
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ip, myProt));  //绑定IP地址：端口
            serverSocket.Listen(100);    //设定最多100个排队连接请求
            // 通过Clientsoket发送数据
            //clientSocket = serverSocket.Accept();
            Thread myThread = new Thread(ListenClientConnect);
            myThread.Start();*/
        }
        private void ListenClientConnect() {
            while (true) {
                Console.WriteLine("启动监听数据线程成功！！！");
                clientSocket = serverSocket.Accept();
                IPAddress ip = ((System.Net.IPEndPoint)clientSocket.RemoteEndPoint).Address;
                int port = ((System.Net.IPEndPoint)clientSocket.RemoteEndPoint).Port;
                Console.WriteLine("我是服务器，检测到的客户端ip是：" + ip);
                Console.WriteLine("端口号是：" + port);
                Thread receiveThread = new Thread(ReceiveMessage);
                receiveThread.Start(clientSocket);
            }
        }
        private void ReceiveMessage(object clientSocket) {
            Socket myClientSocket = (Socket)clientSocket;
            while (true) {
                try {
                    // 通过clientSocket接收数据
                    //Console.WriteLine("进入接收函数等待数据");
                    int receiveNumber = myClientSocket.Receive(result);
                    byte[] tempArray = new byte[receiveNumber];
                    for (int i = 0; i < receiveNumber; i++) {
                        tempArray[i] = result[i];
                    }
                    chargePileData = chargePileData.packageParser(tempArray, receiveNumber);

                    if (chargePileData != null) {
                        //Console.WriteLine("解析数据成功！！！");
                        isActive = true;
                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    break;
                }
            }
        }
    }

    public class chargePileDataPacketList {
        public List<chargePileDataPacket> cpDataPacket = new List<chargePileDataPacket>();
        public chargePileDataPacket cpData = new chargePileDataPacket();


        private static byte[] result = new byte[1024];
        private static int myProt = 8885;   //端口
        static Socket serverSocket;
        public chargePileDataPacketList() {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ip, myProt));  //绑定IP地址：端口
            serverSocket.Listen(100);    //设定最多100个排队连接请求
            // 通过Clientsoket发送数据
            //clientSocket = serverSocket.Accept();
            Thread myThread = new Thread(ListenClientConnect);
            myThread.Start();/**/
            Thread receiveThread = new Thread(ReceiveMessage);
            receiveThread.Start();
            Thread myThread1 = new Thread(addList);
            myThread1.Start(cpData);
            Thread getInfoThread = new Thread(getCPInfo);
            getInfoThread.Start(cpDataPacket);
        }
        private void ListenClientConnect() {
            while (true) {
                Console.WriteLine("启动监听数据线程成功！！！");
                cpData.clientSocket = serverSocket.Accept();
                IPAddress ip = ((System.Net.IPEndPoint)cpData.clientSocket.RemoteEndPoint).Address;
                int port = ((System.Net.IPEndPoint)cpData.clientSocket.RemoteEndPoint).Port;
                Console.WriteLine("我是服务器，检测到的客户端ip是：" + ip);
                Console.WriteLine("端口号是：" + port);
                Thread.Sleep(100);
            }

        }
        private bool clientSocketFlg = false;
        private void ReceiveMessage() {
            while (true) {
                try { // 通过clientSocket接收数据
                    Console.WriteLine("-------------进入接收函数等待数据-------------");
                    Thread.Sleep(1000);
                    if (cpData.clientSocket == null) {
                        continue;
                    }
                    clientSocketFlg = true;
                    int receiveNumber = cpData.clientSocket.Receive(result);
                    Console.WriteLine(receiveNumber);
                    byte[] tempArray = new byte[receiveNumber];
                    for (int i = 0; i < receiveNumber; i++) {
                        tempArray[i] = result[i];
                    }
                    cpData.chargePileData = cpData.chargePileData.packageParser(tempArray, receiveNumber);

                    if (cpData.chargePileData != null) {
                        Console.WriteLine("解析数据成功---机器地址为：" + cpData.chargePileData.cpAddress);
                        if (cpData.chargePileMachineAddress != cpData.chargePileData.cpAddress) {
                            cpData.chargePileMachineAddress = cpData.chargePileData.cpAddress;
                            cpData.isActive = true;
                        }
                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("ReceiveMessage");
                    if (clientSocketFlg) {
                        cpData.clientSocket = null;
                        clientSocketFlg = false;
                    }
                }
                Thread.Sleep(100);
            }

        }
        private void getCPInfo(object cpDataPacket) {
            List<chargePileDataPacket> myData = (List<chargePileDataPacket>)cpDataPacket;
            while (true) {
                try {
                    /*
                    for (int i = 0; i < myData.Count; i++) {
                        Console.WriteLine("enter send program!!" + myData.Count);
                        this.sendDataToChargePile(0x23, myData[i].chargePileMachineAddress);
                        Console.WriteLine("sendDataToChargePile!0x23---" + myData[i].chargePileMachineAddress);
                        Thread.Sleep(1000);
                        this.sendDataToChargePile(0x25, myData[i].chargePileMachineAddress);
                        Console.WriteLine("sendDataToChargePile!0x25---" + myData[i].chargePileMachineAddress);
                        Thread.Sleep(1000);
                    }*/
                    foreach (chargePileDataPacket temp in myData) {
                        Console.WriteLine("enter send program!!" + myData.Count + temp.chargePileMachineAddress);
                        if (this == null) {
                            Console.WriteLine("当前指针不存在！！！");
                            break;
                        }
                        this.sendDataToChargePile(0x23, temp.chargePileMachineAddress);
                        Thread.Sleep(10);
                        this.sendDataToChargePile(0x25, temp.chargePileMachineAddress);
                        Thread.Sleep(1000);
                    }

                } catch (System.Exception ex) {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("------wrong-----");
                }
                Thread.Sleep(500);
            }
        }
        private bool updataDeviceList(chargePileDataPacket newDevice) {
            for (int i = 0; i < cpDataPacket.Count; i++) {
                if (newDevice.chargePileData.cpAddress == cpDataPacket[i].chargePileData.cpAddress) {
                    cpDataPacket[i] = newDevice;
                    return true;
                }
            }
            return false;
        }
        public void sendDataToChargePile(byte cmdCode,UInt64 address) {
            CPSendDataPackage sendDataPack = new CPSendDataPackage();
            byte[] sendData = sendDataPack.sendDataPackage(cmdCode, address);

            foreach (chargePileDataPacket cp in cpDataPacket) {
                if (cp.chargePileData.cpAddress == address) {
                    cp.clientSocket.Send(sendData, sendData.Length, 0);
                }
            }
            /*
            for (int i = 0; i < this.cpDataPacket.Count; i++) {
                //Console.WriteLine("连接成功的机器地址为" + this.cpDataPacket[i].chargePileData.cpAddress);
                if (this.cpDataPacket[i].chargePileData.cpAddress == address) {
                    this.cpDataPacket[i].clientSocket.Send(sendData, sendData.Length, 0);
                    //IPAddress ip = ((System.Net.IPEndPoint)this.cpDataPacket[i].clientSocket.RemoteEndPoint).Address;
                    //int port = ((System.Net.IPEndPoint)this.cpDataPacket[i].clientSocket.RemoteEndPoint).Port;
                    //Console.WriteLine("我是服务器，检测到的客户端ip是：" + ip);
                    //Console.WriteLine("端口号是：" + port);
                    //Console.WriteLine("服务器发送数据成功---机器地址为" + address);
                }
            }*/
        }
        public void sendDataToChargePile(byte cmdCode, UInt64 address, CPSetSendDataRate sendRataData) {
            CPSendDataPackage sendDataPack = new CPSendDataPackage();
            byte[] sendData = sendDataPack.sendRateDataPackage(cmdCode,sendRataData, address);
            for (int i = 0; i < this.cpDataPacket.Count; i++) {
                //Console.WriteLine("连接成功的机器地址为" + this.cpDataPacket[i].chargePileData.cpAddress);
                if (this.cpDataPacket[i].chargePileData.cpAddress == address) {
                    this.cpDataPacket[i].clientSocket.Send(sendData, sendData.Length, 0);

                    IPAddress ip = ((System.Net.IPEndPoint)this.cpDataPacket[i].clientSocket.RemoteEndPoint).Address;
                    int port = ((System.Net.IPEndPoint)this.cpDataPacket[i].clientSocket.RemoteEndPoint).Port;
                    //Console.WriteLine("我是服务器，检测到的客户端ip是：" + ip);
                    //Console.WriteLine("端口号是：" + port);
                    //Console.WriteLine("服务器发送数据成功---机器地址为" + address);
                }
            }
        }
        public void sendDataToChargePile(byte cmdCode, UInt64 address, CPSetSendDataTime sendTimeData) {
            CPSendDataPackage sendDataPack = new CPSendDataPackage();
            byte[] sendData = sendDataPack.sendTimeDataPackage(cmdCode, sendTimeData, address);
            for (int i = 0; i < this.cpDataPacket.Count; i++) {
                //Console.WriteLine("连接成功的机器地址为" + this.cpDataPacket[i].chargePileData.cpAddress);
                if (this.cpDataPacket[i].chargePileData.cpAddress == address) {
                    this.cpDataPacket[i].clientSocket.Send(sendData, sendData.Length, 0);

                    IPAddress ip = ((System.Net.IPEndPoint)this.cpDataPacket[i].clientSocket.RemoteEndPoint).Address;
                    int port = ((System.Net.IPEndPoint)this.cpDataPacket[i].clientSocket.RemoteEndPoint).Port;
                    //Console.WriteLine("我是服务器，检测到的客户端ip是：" + ip);
                    //Console.WriteLine("端口号是：" + port);
                    //Console.WriteLine("服务器发送数据成功---机器地址为" + address);
                }
            }
        }
        public void sendDataToChargePile(byte cmdCode, UInt64 address, byte para) {
            CPSendDataPackage sendDataPack = new CPSendDataPackage();
            byte[] sendData = sendDataPack.sendCPStartupPackage(cmdCode, para, address);
            for (int i = 0; i < this.cpDataPacket.Count; i++) {
                //Console.WriteLine("连接成功的机器地址为" + this.cpDataPacket[i].chargePileData.cpAddress);
                if (this.cpDataPacket[i].chargePileData.cpAddress == address) {
                    this.cpDataPacket[i].clientSocket.Send(sendData, sendData.Length, 0);

                    IPAddress ip = ((System.Net.IPEndPoint)this.cpDataPacket[i].clientSocket.RemoteEndPoint).Address;
                    int port = ((System.Net.IPEndPoint)this.cpDataPacket[i].clientSocket.RemoteEndPoint).Port;
                    //Console.WriteLine("我是服务器，检测到的客户端ip是：" + ip);
                    //Console.WriteLine("端口号是：" + port);
                    //Console.WriteLine("服务器发送数据成功---机器地址为" + address);
                }
            }
        }
        private void addList(object cpData) {
            chargePileDataPacket myData = (chargePileDataPacket)cpData;
            while (true) {
                if (((chargePileDataPacket)cpData).isActive) {
                    //chargePileDataPacket myData = new chargePileDataPacket();
                    //myData = cpData;
                    // 接收数据成功
                    Console.WriteLine("dealData---添加数据到List中---myData" + myData.chargePileData.cpAddress);
                    // 处理成功cpDataPacket
                    if (false == updataDeviceList(myData)) {
                        Console.WriteLine("add myData to list！");
                        cpDataPacket.Add(myData);
                    }
                    ((chargePileDataPacket)cpData).isActive = false;
                }
                Thread.Sleep(500);
                //Console.WriteLine("addList程序正在运行！");
            }
        }
    }

    public class chargePileDataPacket { 
        public bool isActive = false;
        //public IPAddress chargePileIPAddress = IPAddress.Parse("127.0.0.1");
        //public int chargePilePort = 0;
        public UInt64 chargePileMachineAddress = 0;
        public CPBackDataParse chargePileData = new CPBackDataParse();
        //public Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public Socket clientSocket =  null;
        public chargePileDataPacket() {
            //Console.WriteLine("创建类chargePileDataPacket成功");
        }

        
    }
    public class CPGetHeartFrame {
        public bool cpHeartFrameExecuteResult = false;
        public CPGetHeartFrame() {
            Console.WriteLine("CPGetHeartFrame");
        }
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