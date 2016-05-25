 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// Socket监听类
using System.Net.Sockets;
using System.Net;
using System.Threading;

// 串口通信类
using System.Timers;
using System.IO.Ports;
using System.Collections;

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
        //private static int myProt = 8885;   //端口
        //static Socket serverSocket;
        //static Socket clientSocket;/**/
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
                            this.cpGetStateData.cpComState = 0x01;
                        } else {
                            this.cpGetHeartData.cpHeartFrameExecuteResult = false;
                            this.cpGetStateData.cpComState = 0x00;
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
                            this.cpGetStateData.cpOutState = arr[48];

                            this.cpGetStateData.cpFaultH = arr[49];
                            this.cpGetStateData.cpFaultL = arr[50];
                            this.cpGetStateData.cpState = arr[51];
                            //this.cpGetStateData.cpComState = arr[52];

                            
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
        //private static int myProt = 8885;   //端口
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

        public enum ComMethod {
            SerialPort,
            TcpIp
        }
        private serialPortClass serial = null;
        private bool blDataFlag = false;                        // 数据接收完成标志
        Queue receiveByteQueue = Queue.Synchronized(new Queue());//线程安全的数据队列，用来中转串口来的数据

        private static byte[] result = new byte[1024];
        private static int myProt = 8885;   //端口
        static Socket serverSocket;

        public chargePileDataPacketList() {
            try {
                IPAddress ip = IPAddress.Parse("127.0.0.1");
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(new IPEndPoint(ip, myProt));  //绑定IP地址：端口
                serverSocket.Listen(100);    //设定最多100个排队连接请求

                Thread myThread = new Thread(ListenClientConnect);
                myThread.Start();/**/

            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
        public chargePileDataPacketList(ComMethod method) {
            if (method == ComMethod.SerialPort) {
                serial = new serialPortClass("COM4");
                //添加事件注册
                serial.comm.DataReceived += comm_DataReceived;

                #region 定时器事件---处理接收数据
                System.Timers.Timer aTimer = new System.Timers.Timer();       //System.Timers，不是form的  
                aTimer.Elapsed += new ElapsedEventHandler(TimedEvent);
                aTimer.Interval = 200;    //配置文件中配置的秒数  
                aTimer.Enabled = true;
                #endregion

                #region 定时器事件---发送心跳报文
                System.Timers.Timer heartFrameTimer = new System.Timers.Timer();       //System.Timers，不是form的  
                heartFrameTimer.Elapsed += new ElapsedEventHandler(HeartFrameEvent);
                heartFrameTimer.Interval = 10000;    // 10s  
                heartFrameTimer.Enabled = true;
                #endregion

                #region 定时器事件---发送获取状态命令报文
                System.Timers.Timer CPStateTimer = new System.Timers.Timer();       //System.Timers，不是form的  
                CPStateTimer.Elapsed += new ElapsedEventHandler(CPStateEvent);
                CPStateTimer.Interval = 3000;    // 3s  
                CPStateTimer.Enabled = true;
                #endregion

                #region 定时器事件---发送获取当前信息报文
                System.Timers.Timer CPCurrentInfoTimer = new System.Timers.Timer();       //System.Timers，不是form的  
                CPCurrentInfoTimer.Elapsed += new ElapsedEventHandler(CPCurrentInfoEvent);
                CPCurrentInfoTimer.Interval = 3500;    // 3s 
                CPCurrentInfoTimer.Enabled = true;
                #endregion 

            } else if (method == ComMethod.TcpIp) {
                try {
                    IPAddress ip = IPAddress.Parse("127.0.0.1");
                    serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    serverSocket.Bind(new IPEndPoint(ip, myProt));  //绑定IP地址：端口
                    serverSocket.Listen(100);    //设定最多100个排队连接请求

                    Thread myThread = new Thread(ListenClientConnect);
                    myThread.Start();/**/

                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        #region 串口接收和自动发送方法
        private void HeartFrameEvent(object source, ElapsedEventArgs e) {

            if (cpData.chargePileMachineAddress != 0) {
                this.sendDataToChargePile(ComMethod.SerialPort, 0x20, cpData.chargePileMachineAddress);
                //Console.WriteLine("-----[发送心跳包]-----成功");
            }
            
        }
        private void CPStateEvent(object source, ElapsedEventArgs e) 
        {
            if (cpData.chargePileMachineAddress != 0) {
                this.sendDataToChargePile(ComMethod.SerialPort, 0x23, cpData.chargePileMachineAddress);
                //Console.WriteLine("-----[发送充电桩状态包]-----成功");
            }
        }
        private void CPCurrentInfoEvent(object source, ElapsedEventArgs e) 
        {
            if (cpData.chargePileMachineAddress != 0) {
                this.sendDataToChargePile(ComMethod.SerialPort, 0x25, cpData.chargePileMachineAddress);
                //Console.WriteLine("-----[发送充电桩当前信息包]-----成功");
            }
        }
        private void TimedEvent(object source, ElapsedEventArgs e) 
        {
            if (true == blDataFlag) {   // 数据处理标志
                blDataFlag = false;

                int count = receiveByteQueue.Count;
                byte[] tempArray = (byte[])receiveByteQueue.Dequeue();//通过队列收到数据

                //CPBackDataParse dataParser = new CPBackDataParse();
                //dataParser = dataParser.packageParser(tempArray, tempArray.Length);

                Console.WriteLine("接收到了串口数据");

                UInt32 addressH = (UInt32)((tempArray[2] << 24) | (tempArray[3] << 16)
                                    | (tempArray[4] << 8) | (tempArray[5]));
                UInt32 addressL = (UInt32)((tempArray[6] << 24) | (tempArray[7] << 16)
                                    | (tempArray[8] << 8) | (tempArray[9]));
                UInt64 cpAddress = ((UInt64)addressH) << 32 | addressL;
                    //Console.WriteLine("接收到了数据---" + cpAddress + "端口号：" + port);

                chargePileDataPacket cpdeviceDataPacket = null;
                for (int i = 0; i < cpDataPacket.Count; i++) {
                    chargePileDataPacket data = cpDataPacket[i];
                    if (data.chargePileMachineAddress == cpAddress) {
                        cpdeviceDataPacket = data;
                    }
                }
                if (cpdeviceDataPacket == null) {
                    cpdeviceDataPacket = new chargePileDataPacket();
                }
                cpdeviceDataPacket.chargePileData = cpdeviceDataPacket.chargePileData.packageParser(tempArray, tempArray.Length);
                    
                if (cpdeviceDataPacket.chargePileData != null) {
                    cpdeviceDataPacket.isActive = true;
                    cpdeviceDataPacket.chargePileMachineAddress = cpdeviceDataPacket.chargePileData.cpAddress;
                        
                    if (false == updataDeviceList(cpdeviceDataPacket)) {
                        cpDataPacket.Add(cpdeviceDataPacket);

                        Console.WriteLine("enter add data!!!");
                    }

                    cpData = cpdeviceDataPacket;// 设置一个全局量用于串口发送
                } else {
                    Console.WriteLine("cpdeviceDataPacket 为 空");
                }
            }

        }
        private void comm_DataReceived(object sender, SerialDataReceivedEventArgs e) {
            byte[] byteArray = new byte[serial.comm.ReadBufferSize];        // 创建串口接收数据数组

            int len = serial.comm.Read(byteArray, 0, byteArray.Length);

            CPDataCheck dataCheck = new CPDataCheck();

            // 对接收到的数据进行预处理
            if (dataCheck.dataFirstCheck(byteArray, byteArray.Length)) { //快速初步校验

                byte[] ubReceDataBuff = new byte[len];
                for (int i = 0; i < len; i++) {
                    ubReceDataBuff[i] = byteArray[i];
                }   // 把一帧数据暂存在ubReceiveDataBuffer数组中
                receiveByteQueue.Enqueue(ubReceDataBuff);//把数据放入队列中，先进先出
                blDataFlag = true;
            }
        }
        #endregion
        #region TCPIP接收和自动发送方法
        private object lockobject = new object();
        private static bool threadReceiveFlg = true;
        private static bool threadSendFlg = false;
        private bool clientSocketFlg = false;
        private void ListenClientConnect(IAsyncResult ar) {
            try {
                Socket clientSocket = serverSocket.EndAccept(ar);
                //完成接收后，开始等待后续的请求接入
                serverSocket.BeginAccept(new AsyncCallback(ReceiveMessage), null);
                //一旦建立连接，那么开始接收客户端发送的信息
                clientSocket.BeginReceive(result, 0, result.Length, SocketFlags.None,
                    new AsyncCallback(ReceiveMessage), clientSocket);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message + " OnTCPAccept");
            }
        }
        private void ReceiveMessage(IAsyncResult ar) {
            Socket clientSocket = (Socket)ar.AsyncState;
            int port = ((System.Net.IPEndPoint)clientSocket.RemoteEndPoint).Port;
            Console.WriteLine("创建新的线程来接收数据，端口号为：" + port);
            //while (true) {
                try { // 通过clientSocket接收数据
                    Thread.Sleep(100);
                    if (clientSocket == null) {
                        return;
                    }
                    clientSocketFlg = true;
                    Console.WriteLine("--------进入接收函数等待数据----------" + port);
                    int receiveNumber = clientSocket.EndReceive(ar);
                    Console.WriteLine(receiveNumber);
                    if (receiveNumber <= 0) return;
                    byte[] tempArray = new byte[receiveNumber];
                    try {
                        for (int i = 0; i < receiveNumber; i++) {
                            tempArray[i] = result[i];
                        }
                    } catch (Exception ex) {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("数组赋值错误！");
                    }
                    chargePileDataPacket cpdeviceDataPacket = new chargePileDataPacket();
                    cpdeviceDataPacket.chargePileData = cpdeviceDataPacket.chargePileData.packageParser(tempArray, receiveNumber);

                    if (cpdeviceDataPacket.chargePileData != null) {
                        Console.WriteLine("解析数据成功---机器地址为：" + cpData.chargePileData.cpAddress);

                        //cpdeviceDataPacket.chargePileData = cpData.chargePileData;
                        cpdeviceDataPacket.isActive = true;
                        //cpdeviceDataPacket.chargePileIPAddress = ((System.Net.IPEndPoint)myClientSocket.RemoteEndPoint).Address;
                        //cpdeviceDataPacket.chargePilePort = ((System.Net.IPEndPoint)myClientSocket.RemoteEndPoint).Port;
                        cpdeviceDataPacket.chargePileMachineAddress = cpdeviceDataPacket.chargePileData.cpAddress;
                        cpdeviceDataPacket.clientSocket = clientSocket;
                        Console.WriteLine("cpdeviceDataPacket-address:" + cpdeviceDataPacket.chargePileData.cpAddress);
                        Console.WriteLine("voltage:" + cpdeviceDataPacket.chargePileData.cpGetStateData.cpVoltage);
                        Console.WriteLine("Current:" + cpdeviceDataPacket.chargePileData.cpGetStateData.cpCurrent);
                        //Console.WriteLine("list length" + cpDataPacket.Count);
                        if (false == updataDeviceList(cpdeviceDataPacket)) {
                            cpDataPacket.Add(cpdeviceDataPacket);

                            // 为每一个接收线程创建一个发送线程
                            Thread getInfoThread = new Thread(SendInfoToCP);
                            getInfoThread.Start(cpdeviceDataPacket);
                            //Thread getInfoThread = new Thread(getCPInfo);
                            //getInfoThread.Start(cpDataPacket);
                            Console.WriteLine("enter add data!!!");
                        }
                        for (int i = 0; i < cpDataPacket.Count; i++) {
                            Console.WriteLine("cpDataPacket-address:" + cpDataPacket[i].chargePileData.cpAddress);
                        }
                        //if (cpData.chargePileMachineAddress != cpData.chargePileData.cpAddress) {
                        //    cpData.chargePileMachineAddress = cpData.chargePileData.cpAddress;

                        //    if (false == updataDeviceList(cpData)) {
                        //        Console.WriteLine("add myData to list！");
                        //        cpDataPacket.Add(cpData);
                        //    }
                        //    cpData.isActive = true;
                        //}
                    }/**/
                    //}

                    //继续接收数据
                    clientSocket.BeginReceive(result, 0, result.Length, SocketFlags.None,
                                            new AsyncCallback(ReceiveMessage), clientSocket);
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("ReceiveMessage");
                    if (clientSocketFlg) {
                        clientSocket = null;
                        clientSocketFlg = false;
                    }
                    //Thread.CurrentThread.Abort();
                }
            //}

        }
        private void ListenClientConnect() {
            while (true) {
                Console.WriteLine("启动监听数据线程成功！！！");
                cpData.clientSocket = serverSocket.Accept();
                //cpData.clientSocket.D;
                //IPAddress ip = ((System.Net.IPEndPoint)cpData.clientSocket.RemoteEndPoint).Address;
                //int port = ((System.Net.IPEndPoint)cpData.clientSocket.RemoteEndPoint).Port;
                //Console.WriteLine("我是服务器，检测到的客户端ip是：" + ip);
                //Console.WriteLine("端口号是：" + port);
                Thread.Sleep(100);
                // 每次收到一个新的socket都会开启一个新线程
                Thread receiveThread = new Thread(ReceiveMessage);
                receiveThread.IsBackground = true;
                receiveThread.Start(cpData.clientSocket);
                int port = ((System.Net.IPEndPoint)cpData.clientSocket.RemoteEndPoint).Port;
                Console.WriteLine("启动新的接收线程，端口号：" + port);
                //cpData.clientSocket.
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
        

        private void ReceiveMessage(object socket) {
            Socket mySocket = (Socket)socket;
            int port = ((System.Net.IPEndPoint)mySocket.RemoteEndPoint).Port;
            Console.WriteLine("创建新的线程来接收数据，端口号为：" + port);
            try { // 通过clientSocket接收数据
                while (true) {
                    if (mySocket == null) {
                        Thread.Sleep(1000);
                        continue;
                    }
                    clientSocketFlg = true;
                    //Console.WriteLine("--------进入接收函数等待数据----------" + port);
                    //Thread.Sleep(2000);
                    //continue;
                    int receiveNumber = mySocket.Receive(result);

                    //Console.WriteLine("接收到了数据---"+ receiveNumber + "端口号：" + port);
                    if (receiveNumber == 0) {
                        Console.WriteLine("receiveNumber---{0}",receiveNumber);
                        //Thread.Sleep(1000);
                        mySocket.Shutdown(SocketShutdown.Both);
                        mySocket.Close();
                        mySocket = null;
                        break;
                    }
                    byte[] tempArray = new byte[receiveNumber];
                    try {
                        for (int i = 0; i < receiveNumber; i++) {
                            tempArray[i] = result[i];
                        }
                    } catch (Exception ex) {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("数组赋值错误！");
                    }


                    UInt32 addressH = (UInt32)((tempArray[2] << 24) | (tempArray[3] << 16)
                                    | (tempArray[4] << 8) | (tempArray[5]));
                    UInt32 addressL = (UInt32)((tempArray[6] << 24) | (tempArray[7] << 16)
                                        | (tempArray[8] << 8) | (tempArray[9]));
                    UInt64 cpAddress = ((UInt64)addressH) << 32 | addressL;
                    //Console.WriteLine("接收到了数据---" + cpAddress + "端口号：" + port);

                    chargePileDataPacket cpdeviceDataPacket = null;
                    for (int i = 0; i < cpDataPacket.Count; i++) {
                        chargePileDataPacket data = cpDataPacket[i];
                        if (data.chargePileMachineAddress == cpAddress) {
                            cpdeviceDataPacket = data;
                        }
                    }
                    if (cpdeviceDataPacket == null) {
                        cpdeviceDataPacket = new chargePileDataPacket();
                    }
                    cpdeviceDataPacket.chargePileData = cpdeviceDataPacket.chargePileData.packageParser(tempArray, receiveNumber);
                    if (cpdeviceDataPacket.chargePileData != null) {
                        cpdeviceDataPacket.isActive = true;
                        cpdeviceDataPacket.chargePileMachineAddress = cpdeviceDataPacket.chargePileData.cpAddress;
                        cpdeviceDataPacket.clientSocket = mySocket;
                        if (false == updataDeviceList(cpdeviceDataPacket)) {
                            cpDataPacket.Add(cpdeviceDataPacket);
                            // 为每一个接收线程创建一个发送线程
                            Thread getInfoThread = new Thread(SendInfoToCP);
                            getInfoThread.IsBackground = true;
                            getInfoThread.Start(cpdeviceDataPacket);

                            Thread HeartFrameThread = new Thread(SendHeartFrameToCP);
                            HeartFrameThread.IsBackground = true;
                            HeartFrameThread.Start(cpdeviceDataPacket);

                            Console.WriteLine("enter add data!!!");
                        }
                    } else {
                        Console.WriteLine("cpdeviceDataPacket 为 空");
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                Console.WriteLine("ReceiveMessage");
                if (mySocket != null) {
                    mySocket.Shutdown(SocketShutdown.Both);
                    mySocket.Close();
                    mySocket = null;
                }
                
            }
            
            
        }
        private void SendHeartFrameToCP(object cpDataPacket) {
            chargePileDataPacket myData = (chargePileDataPacket)cpDataPacket;
            Socket mySocket = ((chargePileDataPacket)cpDataPacket).clientSocket;
            int port = ((System.Net.IPEndPoint)mySocket.RemoteEndPoint).Port;
            Console.WriteLine("创建新的线程来发送数据，端口号为：" + port);
            try {
                while (true) {
                    //Console.WriteLine("--------进入发送-heartFrame----------" + port);
                    Thread.Sleep(10000);
                    CPSendDataPackage sendDataPack = new CPSendDataPackage();
                    byte[] sendData = sendDataPack.sendDataPackage(0x20, myData.chargePileMachineAddress);
                    Socket soc = myData.clientSocket;

                    soc.Send(sendData, sendData.Length, 0);

                }
            } catch (Exception ex) {
                Console.WriteLine("heartFrame---发送数据错误！" + ex.Message);
                myData = null;
            }
        }
        private void SendInfoToCP(object cpDataPacket) {
            chargePileDataPacket myData = (chargePileDataPacket)cpDataPacket;
            Socket mySocket = ((chargePileDataPacket)cpDataPacket).clientSocket;
            int port = ((System.Net.IPEndPoint)mySocket.RemoteEndPoint).Port;
            Console.WriteLine("创建新的线程来发送数据，端口号为：" + port);
            while (true) {
                //Console.WriteLine("--------进入发送函数发送数据----------" + port);
                Thread.Sleep(3000);
                CPSendDataPackage sendDataPack = new CPSendDataPackage();
                byte[] sendData = sendDataPack.sendDataPackage(0x23, myData.chargePileMachineAddress);
                
                
                try {
                    Socket soc = myData.clientSocket;
                    soc.Send(sendData, sendData.Length, 0);
                    Thread.Sleep(100);
                    byte[] sendData1 = sendDataPack.sendDataPackage(0x25, myData.chargePileMachineAddress);
                    soc.Send(sendData1, sendData1.Length, 0);
                } catch (Exception ex) {
                    Console.WriteLine("state info --- 发送数据错误！" + ex.Message);
                    myData = null;
                    break;
                }
                
            }
        }
        private void getCPInfo(object cpDataPacket) {
            List<chargePileDataPacket> myData = (List<chargePileDataPacket>)cpDataPacket;
            while (true) {
                //Console.WriteLine("--------进入发送函数发送数据----------" + port);
                if (myData == null) {
                    continue;
                }
                if (this == null) {
                    Console.WriteLine("this is empty");
                    break;
                }
                //foreach (chargePileDataPacket temp in myData) {
                for (int i = 0; i < myData.Count; i++ ) {
                    chargePileDataPacket temp = myData[i];
                    Console.WriteLine("enter send program!!" + myData.Count + "," + temp.chargePileMachineAddress);
                    if (this == null) {
                        Console.WriteLine("this is empty");
                        break;
                    }
                    this.sendDataToChargePile(0x23, temp.chargePileMachineAddress);
                    Thread.Sleep(1);
                    this.sendDataToChargePile(0x25, temp.chargePileMachineAddress);
                    Thread.Sleep(1000);
                    try {
                        
                    } catch (Exception ex) {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("发送数据错误！");
                    }
                }
                Thread.Sleep(1000);
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
        #endregion
        #region 数据发送方法--串口及TCPIP
        public void sendDataToChargePile(byte cmdCode, chargePileDataPacket data) {
            CPSendDataPackage sendDataPack = new CPSendDataPackage();
            byte[] sendData = sendDataPack.sendDataPackage(cmdCode, data.chargePileMachineAddress);
            if (data == null) {
                Console.WriteLine("data ==  null");
                return;
            }
            if (sendData == null) {
                Console.WriteLine("sendData ==  null");
                return;
            }
            Socket soc = data.clientSocket;
            soc.Send(sendData, sendData.Length, 0);
            try {
                //Socket soc = data.clientSocket;
                //soc.Send(sendData, sendData.Length, 0);
                //for (int i = 0; i < cpDataPacket.Count; i++) {
                //    chargePileDataPacket cp = cpDataPacket[i];
                //    if (cp.chargePileData.cpAddress == data.chargePileMachineAddress) {
                //        Socket soc = cp.clientSocket;
                //        soc.Send(sendData, sendData.Length, 0);
                //    }
                //}
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Socket 发送数据时发生异常！");
            }
        }
        public void sendDataToChargePile(byte cmdCode, UInt64 address) {
            CPSendDataPackage sendDataPack = new CPSendDataPackage();
            byte[] sendData = sendDataPack.sendDataPackage(cmdCode, address);
            for (int i = 0; i < cpDataPacket.Count; i++) {
                chargePileDataPacket cp = cpDataPacket[i];
                if (cp.chargePileData.cpAddress == address) {
                    Socket soc = cp.clientSocket;
                    soc.Send(sendData, sendData.Length, 0);
                }
            }
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

        public void sendDataToChargePile(ComMethod method,byte cmdCode, chargePileDataPacket data) {
            CPSendDataPackage sendDataPack = new CPSendDataPackage();
            byte[] sendData = sendDataPack.sendDataPackage(cmdCode, data.chargePileMachineAddress);
            if (data == null) {
                Console.WriteLine("data ==  null");
                return;
            }
            if (sendData == null) {
                Console.WriteLine("sendData ==  null");
                return;
            }
            if (method == ComMethod.SerialPort) {
                try {
                    serial.sendData(sendData);
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            } else if (method == ComMethod.TcpIp) {
                try {
                    Socket soc = data.clientSocket;
                    soc.Send(sendData, sendData.Length, 0);
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Socket 发送数据时发生异常！");
                }
            }
            
        }
        public void sendDataToChargePile(ComMethod method,byte cmdCode, UInt64 address) {
            CPSendDataPackage sendDataPack = new CPSendDataPackage();
            byte[] sendData = sendDataPack.sendDataPackage(cmdCode, address);

            if (method == ComMethod.SerialPort) {
                try {
                    serial.sendData(sendData);
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            } else if (method == ComMethod.TcpIp) {
                try {
                    for (int i = 0; i < cpDataPacket.Count; i++) {
                        chargePileDataPacket cp = cpDataPacket[i];
                        if (cp.chargePileData.cpAddress == address) {
                            Socket soc = cp.clientSocket;
                            soc.Send(sendData, sendData.Length, 0);
                        }
                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Socket 发送数据时发生异常！");
                }
            }

            
        }
        public void sendDataToChargePile(ComMethod method,byte cmdCode, UInt64 address, CPSetSendDataRate sendRataData) {
            CPSendDataPackage sendDataPack = new CPSendDataPackage();
            byte[] sendData = sendDataPack.sendRateDataPackage(cmdCode, sendRataData, address);
            
            if (method == ComMethod.SerialPort) {
                try {
                    serial.sendData(sendData);
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            } else if (method == ComMethod.TcpIp) {
                try {
                    for (int i = 0; i < this.cpDataPacket.Count; i++) {
                        if (this.cpDataPacket[i].chargePileData.cpAddress == address) {
                            this.cpDataPacket[i].clientSocket.Send(sendData, sendData.Length, 0);
                        }
                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Socket 发送数据时发生异常！");
                }
            }
        }
        public void sendDataToChargePile(ComMethod method,byte cmdCode, UInt64 address, CPSetSendDataTime sendTimeData) {
            CPSendDataPackage sendDataPack = new CPSendDataPackage();
            byte[] sendData = sendDataPack.sendTimeDataPackage(cmdCode, sendTimeData, address);

            if (method == ComMethod.SerialPort) {
                try {
                    serial.sendData(sendData);
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            } else if (method == ComMethod.TcpIp) {
                try {
                    for (int i = 0; i < this.cpDataPacket.Count; i++) {
                        if (this.cpDataPacket[i].chargePileData.cpAddress == address) {
                            this.cpDataPacket[i].clientSocket.Send(sendData, sendData.Length, 0);
                        }
                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Socket 发送数据时发生异常！");
                }
            }
        }
        public void sendDataToChargePile(ComMethod method,byte cmdCode, UInt64 address, byte para) {
            CPSendDataPackage sendDataPack = new CPSendDataPackage();
            byte[] sendData = sendDataPack.sendCPStartupPackage(cmdCode, para, address);

            if (method == ComMethod.SerialPort) {
                try {
                    serial.sendData(sendData);
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
            } else if (method == ComMethod.TcpIp) {
                try {
                    for (int i = 0; i < this.cpDataPacket.Count; i++) {
                        if (this.cpDataPacket[i].chargePileData.cpAddress == address) {
                            this.cpDataPacket[i].clientSocket.Send(sendData, sendData.Length, 0);
                        }
                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("Socket 发送数据时发生异常！");
                }
            }

        }
        #endregion
    }
    //  
    public class chargePileDataPacket { 
        public bool isActive = false;
        //public IPAddress chargePileIPAddress = IPAddress.Parse("127.0.0.1");
        //public int chargePilePort = 0;
        public UInt64 chargePileMachineAddress = 0;
        public CPBackDataParse chargePileData = new CPBackDataParse(); // 数据都在这里面

        #region 数据包私有方法
        // 数据包
        /// <summary>
        /// 采样信息
        /// </summary>
        private CPGetState.CPCurrentState _CurrentState;
        private byte _CommState;
        private float _CurrentSOC = 0.38f; // 初始值38%，一分钟增加1%，< 90%
        private int _ChargeTime = 30;
        private int _RemainTime = 40;// 步长为一分钟，< 10分钟，大于0分钟
        private float _CurrentVOL = 560.5f;// 步长0.1V，< 750V
        private float _CurrentCur = 87.5f; // 步长为0.1
        /// <summary>
        /// RT Info:_CurrentVOL,_CurrentCur
        /// </summary>
        private float _OutPower;// 输出功率，P= _CurrentVOL * _CurrentCur / 1000
        private float _OutQuantity; // 输出电量，Q=P*T(T为小时)
        private int _ACCTime = 30; // T初始30分钟，步长1分钟

        /// <summary>
        /// 故障信息
        /// </summary>
        private bool[] _CurrentAlarmInfo;// 里面存10种故障信息

        /// <summary>
        /// 费率信息
        /// </summary>

        private float _TotalQuantity; //总电量<100度
        private float _TotalFee;      //总费用< 100元

        private float _JianQ;      //尖
        private float _JianPrice;  //1.2-1.3 元
        private float _JianFee;

        private float _fengQ;    //峰
        private float _fengPrice;//1.0-1.1
        private float _fengFee;

        private float _PingQ;    //平
        private float _PingPrice;//0.7-0.8
        private float _PingFee;

        private float _GUQ;   //谷
        private float _GUPrice;//0.3-0.4
        private float _GUFee;

        /// <summary>
        /// Battery array Info（option)
        /// </summary>
        /// 
        private float _BatterySoc = 0.6f; //电池组SOC60%
        private bool _BMSState = true;  //BMS状态正常
        private float _PortVol = 538.80f;   //端电压  538.80  V
        private int _CellNum = 0;   //单体数量0  个
        private int _TempNum = 0;  //温度点数量0  个
        private float _MaxVol = 3.921f;    //最高允许充电单体电压3.921  V
        private float _MaxCTemp = 60.0f;   //最高允许充电温度 60 度

        /// <summary>
        /// Battery safty Info
        /// </summary>
        private float _CellMaxVol = 3.421f;  //单体最高电压3.412 V
        private int _CellPos = 2;    //单体最高电压位置2 
        private float _CellMinVol = 0.0f; //单体最低电压 0.000 V
        private int _CellMinVolPos = 0; //单体最低电压位置0
        private float _MaxTemp = 38.0f;  //最高温度 38 度
        private float _MinTemp = 31.0f;  //最低温度31 度

        /// <summary>
        /// BMS alarm info
        /// </summary>
        private bool _VolDataAlarm = false;  //电压数据异常无故障
        private bool _SampleVolFault = false; //电压采样故障
        private bool _UvorOvAlarm = false;    //单体欠压过压报警
        private bool _SystemParaAlarm = false; //系统参数报警
        private bool _FanFailFault = false;    //风扇故障
        private bool _SampleTempFault = false;  //温度采样错误
        #endregion

        #region 数据包接口
        // 数据包
        /// <summary>
        /// 采样信息
        /// </summary>
        public CPGetState.CPCurrentState CurrentState {
            get {
                _CurrentState = chargePileData.cpGetStateData.cpCurrentSta;
                return _CurrentState;
            }
        }
        public byte CommState {
            get {
                _CommState = chargePileData.cpGetStateData.cpComState;
                return _CommState;
            }
        }
        public float CurrentSOC { // 初始值38%，一分钟增加1%，< 90%
            get {
                _CurrentSOC = ((float)chargePileData.cpGetStateData.cpSpace1) / 100;
                if (_CurrentSOC > 0.9f) {
                    _CurrentSOC = 0.9f;
                }
                return _CurrentSOC;
            }
        }
        public int ChargeTime {
            get {
                _ChargeTime = (int)chargePileData.cpGetStateData.cpSpace2;
                return _ChargeTime;
            }
        }
        public int RemainTime {// 步长为一分钟，< 10分钟，大于0分钟
            get {
                _RemainTime = (int)chargePileData.cpGetStateData.cpSpace3;
                return _RemainTime;
            }
        }
        public float CurrentVOL {// 步长0.1V，< 750V
            get {
                _CurrentVOL = ((float)chargePileData.cpGetStateData.cpVoltage) / 100;
                return _CurrentVOL;
            }
        }
        public float CurrentCur { // 步长为0.1
            get {
                _CurrentCur = ((float)chargePileData.cpGetStateData.cpCurrent) / 100;
                return _CurrentCur;
            }
        }
        /// <summary>
        /// RT Info:_CurrentVOL,_CurrentCur
        /// </summary>
        public float OutPower {// 输出功率，P= _CurrentVOL * _CurrentCur / 1000
            get {
                float voltage = ((float)chargePileData.cpGetStateData.cpVoltage) / 100;
                float current = ((float)chargePileData.cpGetStateData.cpCurrent) / 100;
                _OutPower = current * voltage / 1000;
                return _OutPower;
            }
        }
        public float OutQuantity {// 输出电量，Q=P*T(T为小时)
            get {
                float time = ((float)chargePileData.cpGetStateData.cpSpace2) / 60;
                float voltage = ((float)chargePileData.cpGetStateData.cpVoltage) / 100;
                float current = ((float)chargePileData.cpGetStateData.cpCurrent) / 100;
                _OutQuantity = voltage * current / 1000 * time;
                return _OutQuantity;
            }
        }
        public int ACCTime { // T初始30分钟，步长1分钟
            get {
                _ACCTime = (int)chargePileData.cpGetStateData.cpSpace2;
                return _ACCTime;
            }
        }

        /// <summary>
        /// 故障信息
        /// </summary>
        public bool[] CurrentAlarmInfo {// 里面存10种故障信息
            get {
                CPGetState state = chargePileData.cpGetStateData;
                if (_CurrentAlarmInfo == null) {
                    _CurrentAlarmInfo = new bool[10];
                }
                _CurrentAlarmInfo[0] = state.cpInOverVol;
                _CurrentAlarmInfo[1] = state.cpOutOverVol;
                _CurrentAlarmInfo[2] = state.cpInUnderVol;
                _CurrentAlarmInfo[3] = state.cpOutUnderVol;
                _CurrentAlarmInfo[4] = state.cpInOverCur;
                _CurrentAlarmInfo[5] = state.cpOutOverCur;
                _CurrentAlarmInfo[6] = state.cpInUnderCur;
                _CurrentAlarmInfo[7] = state.cpOutUnderCur;
                _CurrentAlarmInfo[8] = state.cpTempHigh;
                _CurrentAlarmInfo[9] = state.cpOutShort;

                return _CurrentAlarmInfo;
            }
        }

        /// <summary>
        /// 费率信息
        /// </summary>

        public float TotalQuantity { //总电量<100度
            get {
                _TotalQuantity = ((float)chargePileData.cpGetCurInfoData.cpChargeTotalElect) / 100;
                if (_TotalQuantity > 100.0f) {
                    _TotalQuantity = -1.0f;
                }
                return _TotalQuantity;
            }
        }
        public float TotalFee {      //总费用< 100元
            get {
                _TotalFee = ((float)chargePileData.cpGetCurInfoData.cpChargeTotalPrice) / 100;
                if (_TotalFee > 100.0f) {
                    _TotalFee = -1.0f;
                }
                return _TotalFee;
            }    
        }
        public float JianQ {    //尖
            get {
                _JianQ = ((float)chargePileData.cpGetCurInfoData.cpChargePointElect) / 100;
                return _JianQ;
            }
        }
        public float JianPrice {  //1.2-1.3 元
            get {
                _JianPrice = ((float)chargePileData.cpGetCurInfoData.cpPointElectPrice) / 100;
                return _JianPrice;
            }
        }
        public float JianFee {
            get {
                _JianFee = ((float)chargePileData.cpGetCurInfoData.cpPointCost) / 100;
                return _JianFee;
            }
        }
        public float fengQ {    //峰
            get {
                _fengQ = ((float)chargePileData.cpGetCurInfoData.cpChargePeakElect) / 100;
                return _fengQ;
            }
        }
        public float fengPrice { //1.0-1.1
            get {
                _fengPrice = ((float)chargePileData.cpGetCurInfoData.cpPeakElectPrice) / 100;
                return _fengPrice;
            }
        }
        public float fengFee {
            get {
                _fengFee = ((float)chargePileData.cpGetCurInfoData.cpPeakCost) / 100;
                return _fengFee;
            }
        }

        public float PingQ {   //平
            get {
                _fengFee = ((float)chargePileData.cpGetCurInfoData.cpChargeFlatElect) / 100;
                return _PingQ;
            }
        }
        public float PingPrice {//0.7-0.8
            get {
                _PingPrice = ((float)chargePileData.cpGetCurInfoData.cpFlatElectPrice) / 100;
                return _PingPrice;
            }
        }
        public float PingFee {
            get {
                _PingFee = ((float)chargePileData.cpGetCurInfoData.cpFlatCost) / 100;
                return _PingFee;
            }
        }

        public float GUQ {   //谷
            get {
                _GUQ = ((float)chargePileData.cpGetCurInfoData.cpChargeValleyElect) / 100; 
                return _GUQ;
            }
        }
        public float GUPrice {//0.3-0.4
            get {
                _GUPrice = ((float)chargePileData.cpGetCurInfoData.cpValleyElectPrice) / 100; 
                return _GUPrice;
            }
        }
        public float GUFee {
            get {
                _GUFee = ((float)chargePileData.cpGetCurInfoData.cpValleyCost) / 100; 
                return _GUFee;
            }
        }

        /// <summary>
        /// Battery array Info（option)
        /// </summary>
        /// 
        public float BatterySoc { //电池组SOC60%
            get {
                _BatterySoc = ((float)chargePileData.cpGetStateData.cpSpace1) / 100;
                if (_BatterySoc > 0.9f) {
                    _BatterySoc = 0.9f;
                }
                return _BatterySoc;
            }
        }
        public bool BMSState {  //BMS状态正常
            get {
                return _BMSState;
            }
        }
        public float PortVol {   //端电压  538.80  V
            get {
                return _PortVol;
            }
        }
        public int CellNum {   //单体数量0  个
            get {
                return _CellNum;
            }
        }
        public int TempNum {  //温度点数量0  个
            get {
                return _TempNum;
            }
        }
        public float MaxVol {    //最高允许充电单体电压3.921  V
            get {
                return _MaxVol;
            }
        }
        public float MaxCTemp {   //最高允许充电温度 60 度
            get {
                return _MaxCTemp;
            }
        }

        /// <summary>
        /// Battery safty Info
        /// </summary>
        public float CellMaxVol {  //单体最高电压3.412 V
            get {
                return _CellMaxVol;
            }
        }
        public int CellPos {   //单体最高电压位置2 
            get {
                return _CellPos;
            }
        }
        public float CellMinVol { //单体最低电压 0.000 V
            get {
                return _CellMinVol;
            }
        }
        public int CellMinVolPos { //单体最低电压位置0
            get {
                return _CellMinVolPos;
            }
        }
        public float MaxTemp {  //最高温度 38 度
            get {
                return _MaxTemp;
            }
        }
        public float MinTemp {  //最低温度31 度
            get {
                return _MinTemp;
            }
        }

        /// <summary>
        /// BMS alarm info
        /// </summary>
        public bool VolDataAlarm {  //电压数据异常无故障
            get {
                return _VolDataAlarm;
            }
        }
        public bool SampleVolFault { //电压采样故障
            get {
                return _SampleVolFault;
            }
        }
        public bool UvorOvAlarm {   //单体欠压过压报警
            get {
                return _UvorOvAlarm;
            }
        }
        public bool SystemParaAlarm { //系统参数报警
            get {
                return _SystemParaAlarm;
            }
        }
        public bool FanFailFault {    //风扇故障
            get {
                return _FanFailFault;
            }
        }
        public bool SampleTempFault {  //温度采样错误
            get {
                return _SampleTempFault;
            }
        }
        #endregion
        //public Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public Socket clientSocket = null;
        public chargePileDataPacket() {
            //Console.WriteLine("创建类chargePileDataPacket成功");
        }
    }
    public class CPGetHeartFrame {
        public bool cpHeartFrameExecuteResult = false;
        public CPGetHeartFrame() {
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
        public enum CPCurrentState {
            FaultState,
            FreeState,
            ChargeState,
            ParkingState,
            OrderState,
            MainTainState
        }
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
        public byte cpSpace1 = 0;        // space data---soc数
        public byte cpMeterState = 0;    // 0x00 normal;0x01:fault
        public byte cpSpace2 = 0;        // space data---charge time
        public byte cpChargePlug = 0;    // 0x00:normal;0x01:fault
        public byte cpSpace3 = 0;        // space data --- left time
        public byte cpOutState = 0;  // 0x00:normal;0x01:fault

        private byte _cpState = 0;
        private CPCurrentState _cpCurrentSta = CPCurrentState.FreeState;

        private byte _cpComState = 0; // commucation state

        private byte _cpFaultH = 0;
        private byte _cpFaultL = 0;
        private bool _cpInOverVol = false;
        private bool _cpOutOverVol = false;
        private bool _cpInUnderVol = false;
        private bool _cpOutUnderVol = false;

        private bool _cpInOverCur = false;
        private bool _cpOutOverCur = false;
        private bool _cpInUnderCur = false;
        private bool _cpOutUnderCur = false;

        private bool _cpTempHigh = false;
        private bool _cpOutShort = false;
        private UInt16 _fault = 0;

        #region 充电桩状态
        public byte cpState {
            set {
                _cpState = value;
                switch (_cpState) {
                    case 0: { _cpCurrentSta = CPCurrentState.FaultState; break; }
                    case 1: { _cpCurrentSta = CPCurrentState.FreeState; break; }
                    case 2: { _cpCurrentSta = CPCurrentState.ChargeState; break; }
                    case 3: { _cpCurrentSta = CPCurrentState.ParkingState; break; }
                    case 4: { _cpCurrentSta = CPCurrentState.OrderState; break; }
                    case 5: { _cpCurrentSta = CPCurrentState.MainTainState; break; }
                    default: { break; };
                }
            }
        }
        public CPCurrentState cpCurrentSta {
            get {
                return _cpCurrentSta;
            }
        }
        #endregion

        #region 通信状态
        public byte cpComState {
            get {
                return _cpComState;
            }
            set {
                _cpComState = value;
            }
        }
        #endregion

        #region 故障值
        private UInt16 fault {
            get {
                _fault = (UInt16)(((UInt16)_cpFaultH) << 8 | _cpFaultL);
                //Console.WriteLine("get fault:" + _fault);
                return _fault;
            }
            set {
                Console.WriteLine("set fault" + _fault);
                _fault = value;
            }
        }
        public bool cpInOverVol {
            get {
                _cpInOverVol = checkBoxFault(fault,9);
                return _cpInOverVol;
            }
        }
        public bool cpOutOverVol
        {
            get {
                _cpOutOverVol = checkBoxFault(fault,8);
                return _cpOutOverVol;
            }
        }
        public bool cpInUnderVol {
            get {
                _cpInUnderVol = checkBoxFault(fault, 7);
                return _cpInUnderVol;
            }
        }
        public bool cpOutUnderVol {
            get {
                _cpOutUnderVol = checkBoxFault(fault,6);
                return _cpOutUnderVol;
            }
        }
        public bool cpInOverCur {
            get {
                _cpInOverCur = checkBoxFault(fault, 5);
                return _cpInOverCur;
            }
        }
        public bool cpOutOverCur {
            get {
                _cpOutOverCur = checkBoxFault(fault, 4);
                return _cpOutOverCur;
            }
        }
        public bool cpInUnderCur {
            get {
                _cpInUnderCur = checkBoxFault(fault, 3);
                return _cpInUnderCur;
            }
        }
        public bool cpOutUnderCur {
            get {
                _cpOutUnderCur = checkBoxFault(fault, 2);
                return _cpOutUnderCur;
            }
        }
        public bool cpTempHigh {
            get {
                _cpTempHigh = checkBoxFault(fault, 1);
                return _cpTempHigh;
            }
        }
        public bool cpOutShort {
            get {
                _cpOutShort = checkBoxFault(fault, 0);
                return _cpOutShort;
            }
        }
        public byte cpFaultH {
            get {
                return _cpFaultH;
            }
            set {
                _cpFaultH = value;
            }
        }
        public byte cpFaultL {
            get {
                return _cpFaultL;
            }
            set {
                _cpFaultL = value;
            }
        }
        #endregion

        private bool checkBoxFault(UInt16 data, int bit) {
            return !((data & (1 << bit)) == 0);
        }

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