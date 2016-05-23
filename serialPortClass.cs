using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using System.IO.Ports;
using System.Timers;

namespace CPServer {
    class serialPortClass {

        public SerialPort comm = new SerialPort(); // 创建一个串口类

        private StringBuilder builder = new StringBuilder();//避免在事件处理方法中反复的创建，定义到外面。
        private long received_count = 0;//接收计数
        private long send_count = 0;//发送计数


        

        public serialPortClass(string str) {
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            for (int i = 0; i < ports.Length; i++ ) {
                if (ports[i] == str) {
                    comm.PortName = str;
                }
            }
            if (comm.PortName != str) {
                Console.WriteLine("端口不存在");
                return;
            }
            comm.BaudRate = int.Parse("9600");
            comm.DataBits = int.Parse("8");
            string szStopBits = "1";
            switch (szStopBits) {
                case "1":
                    comm.StopBits = StopBits.One;
                    break;
                case "1.5":
                    comm.StopBits = StopBits.OnePointFive;
                    break;
                case "2":
                    comm.StopBits = StopBits.Two;
                    break;
                default:
                    comm.StopBits = StopBits.One;
                    break;
            }
            comm.Parity = (Parity)Enum.Parse(typeof(Parity), "None");
            //此句要好好理解。Enum  提供一个指向枚举器（该枚举器可枚举复合名字对象的组件）的指针。

            //初始化SerialPort对象
            comm.NewLine = "\r\n";
            comm.RtsEnable = true;//根据实际情况吧。

            //添加事件注册
//             //comm.DataReceived += comm_DataReceived;
//             #region 定时器事件
//             Timer aTimer = new Timer();       //System.Timers，不是form的  
//             aTimer.Elapsed += new ElapsedEventHandler(TimedEvent);
//             aTimer.Interval = 200;    //配置文件中配置的秒数  
//             aTimer.Enabled = true;
//             #endregion 

            try {
                comm.Open();
                Console.WriteLine("打开串口成功！");
            } catch (Exception ex) {
                comm = new SerialPort();
                Console.WriteLine(ex.Message);
            }
        }
        
        ///////////  发送数据 ////////////////////////////
        public void sendData(byte[] buffer,int count) {
            comm.Write(buffer, 0, count);
        }
        
        /// <summary>
        /// 实现通过串口发送把整个数组数据
        /// </summary>
        /// <param name="buffer"></param>
        public void sendData(byte[] buffer) {
            this.sendData(buffer,buffer.Length);
        }

        
    }
}
