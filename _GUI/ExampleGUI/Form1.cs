using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CommsLib;

namespace Comms
{
    public partial class Form1 : Form
    {
        //List all variables here as global
        int accX; // Accelerometer Values
        int accY; 
        int accZ;

        short magX; //Magnetometer Values
        short magY;
        short magZ;
        double compass;

        int distanceValue; //Distance to move forward/backwards (in cm)

        bool penUp; //Whether pen is up or down for drawing

        public Form1()
        {
            InitializeComponent();
            myClient = new TCPClient();
            myClient.OnMessageReceived += new ClientBase.ClientMessageReceivedEvent(myClient_OnMessageReceived);

            myRequestTimer = new Timer();
            myRequestTimer.Interval = 500; //every half a second
            myRequestTimer.Tick += new EventHandler(myRequestTimer_Tick);
        }

        TCPClient myClient;
        private void btnCon_Click(object sender, EventArgs e)
        {
            if ("Connect" == btnCon.Text)
            {
                try
                {
                    myClient.ConnectToServer(txtIP.Text,9760);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message+"\r\n"+ex.StackTrace);
                    return;
                }

                //if we get here, then we are connected
                btnCon.Text="Disconnect";
                
                myRequestTimer.Start();
            }
            else
            {
                myRequestTimer.Stop();
                myClient.Disconnect();
                if (!myClient.isConnected) btnCon.Text="Connect";
            }
        }

        short leftPos, rightPos;
        void myClient_OnMessageReceived(Client_Message_EventArgs e)
        {
            //we shall process the message here, e.RawMessage contains all bytes
            // [0] is 255, [1] is length, [2] 255, [3] is cmd, [4-] are payload
            if (e.RawMessage[3] == (byte)CommandID.GetLEDandSwitchStatus)
            {
                //we have received an LED status, so let's change labels accordingly
                bool bGreenOn = (((e.RawMessage[4] & 0x10) ==0 ) ? true : false);
                bool bRedOn =   (((e.RawMessage[4] & 0x20) ==0 ) ? true : false);

                lblGreenStatus.BackColor = ((bGreenOn) ? Color.LimeGreen : SystemColors.ButtonFace);
                lblRedStatus.BackColor = ((bRedOn) ? Color.Red : SystemColors.ButtonFace);
            }

            if (e.RawMessage[3] == (byte)CommandID.MotorPosition)
            {
                leftPos = (short)((uint)e.RawMessage[6] | ((uint)e.RawMessage[5] << 8));
                rightPos = (short)((uint)e.RawMessage[9] | ((uint)e.RawMessage[8] << 8));
            }

            if (e.RawMessage[3] == (byte)CommandID.GetAccelValue)
            {
                accX = (int)(e.RawMessage[5] | (e.RawMessage[4] >> 6));
                accY = (int)(e.RawMessage[7] | (e.RawMessage[6] >> 6));
                accZ = (int)(e.RawMessage[9] | (e.RawMessage[8] >> 6));
            }


            if (e.RawMessage[3] == (byte)CommandID.GetMagnetValue)
            {
                magX = (short)(((int)e.RawMessage[4] << 8) | (int)e.RawMessage[5]);
                magY = (short)(((int)e.RawMessage[6] << 8) | (int)e.RawMessage[7]);
                magZ = (short)(((int)e.RawMessage[8] << 8) | (int)e.RawMessage[9]);
                double magXN = magX - ((-931 + -1507) / 2);
                double magYN = magY - ((2947 + 2320) / 2);
                compass = Math.Atan2(magYN, magXN) * (180 / 3.14);
                if(compass<0)
                {
                    compass = compass + 360;
                }
               
            }
        }
    

        Timer myRequestTimer;
        void myRequestTimer_Tick(object sender, EventArgs e)
        {
            if (!myClient.isConnected) return; //if no connection, don't do anything

            //we will request the status of the LEDs on a regular basis
            myClient.SendData(CommandID.GetLEDandSwitchStatus); //this type needs no payload
            myClient.SendData(CommandID.MotorPosition);

            lblPosLeft.Text = leftPos.ToString();
            lblPosRight.Text = rightPos.ToString();
        }

        Boolean bGreenRequestOn = true;
        Boolean bRedRequestOn = true;

        private void btnToggleGreen_Click(object sender, EventArgs e)
        {
            if (!myClient.isConnected) return;

            bGreenRequestOn = !bGreenRequestOn; //change from last time

            int value = 0;
            if (bGreenRequestOn) value |= 0x01;
            if (bRedRequestOn) value |= 0x02;

            myClient.SendData(CommandID.SetLEDs, new byte[] { (byte)value });
        }

        private void btnToggleRed_Click(object sender, EventArgs e)
        {
            if (!myClient.isConnected) return;

            bRedRequestOn = !bRedRequestOn; //change from last time

            int value = 0;
            if (bGreenRequestOn) value |= 0x01;
            if (bRedRequestOn) value |= 0x02;

            myClient.SendData(CommandID.SetLEDs, new byte[] { (byte)value });
        }


        //Clean up our mess if user clicks the X button, hits AltF4 etc
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            myRequestTimer.Stop(); //just in case ;-)
            if (myClient.isConnected) myClient.Disconnect();
        }

        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        bool robotIsMoving = false;
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    myClient.SendData(CommandID.SetMotorsSpeed, new byte[] { 60, 60 });
                    robotIsMoving = true;
                    e.Handled=true;
                    break;

                case Keys.Down:
                    myClient.SendData(CommandID.SetMotorsSpeed, new byte[] { 200, 200 });
                    robotIsMoving = true;
                    e.Handled=true;
                    break;

                case Keys.Left:
                    myClient.SendData(CommandID.SetMotorsSpeed, new byte[] { 190, 60 });
                    robotIsMoving = true;
                    e.Handled=true;
                    break;
                case Keys.Right:
                    myClient.SendData(CommandID.SetMotorsSpeed, new byte[] { 60, 190 });
                    robotIsMoving = true;
                    e.Handled=true;
                    break;

                default: e.Handled=false;
                    break;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (robotIsMoving)
            {
                myClient.SendData(CommandID.SetMotorsSpeed, new byte[] { 0,0 });
                robotIsMoving = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label4.Text = accX.ToString(); //Update accelerometer values
            label5.Text = accY.ToString();
            label6.Text = accZ.ToString();

            label7.Text = magX.ToString(); //Update magnetometer values
            label8.Text = magY.ToString();
            label9.Text = magZ.ToString();
            label10.Text = compass.ToString("N0");
        }

        private void timer2_Tick(object sender, EventArgs e) //Constantly update readings
        {
            if (!myClient.isConnected) return;
            myClient.SendData(CommandID.GetMagnetValue);
            myClient.SendData(CommandID.GetAccelValue);
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        //Move forward button
        private void button2_Click_1(object sender, EventArgs e)  
        {
            timer3.Enabled = true;
            MoveForward(1000); //Arbitrary value to get it to move, can be changed
        }

        public bool MoveForward(int inputTime) 
        {
            timer3.Interval = inputTime;
            myClient.SendData(CommandID.MotorSpeedClosed, new byte[] { 59, 0, 61, 0, 1 });
            //SpeedL, 0, SpeedR, 0, 1 to call closedloop
            timer3.Enabled = true;
            robotIsMoving = true;
            return robotIsMoving;
        }

        public bool MoveBackward(int inputTime)
        {
            timer3.Interval = inputTime;
            myClient.SendData(CommandID.MotorSpeedClosed, new byte[] { 200, 0, 200, 0, 1 });
            //SpeedL, 0, SpeedR, 0, 1 to call closedloop
            timer3.Enabled = true;
            robotIsMoving = true;
            return robotIsMoving;
        }

        public bool StopMoving()
        {

            myClient.SendData(CommandID.MotorSpeedClosed, new byte[] { 0, 0, 0, 0, 1 });
            robotIsMoving = false;
            return robotIsMoving;
        }

        //Move left
        private void button4_Click(object sender, EventArgs e)
        {
            myClient.SendData(CommandID.MotorSpeedClosed, new byte[] { 190, 0, 60, 0, 1 });
            robotIsMoving = true;
        }

        //Move right
        private void button5_Click(object sender, EventArgs e)
        {
            myClient.SendData(CommandID.MotorSpeedClosed, new byte[] { 60, 0, 190, 0, 1 });
            robotIsMoving = true;
        }

        //Stop
        private void button6_Click(object sender, EventArgs e)
        {
            StopMoving();
        }

        private void lblPosLeft_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label10_Click_1(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {}

        private void groupBox6_Enter(object sender, EventArgs e)
        {}

        private void button1_Click(object sender, EventArgs e) //Go button
        {
            if (distanceValue > 0)                             //for forwards
            {
                int time = 0;
                int time2 = 0;
                float dist = 0;
                if (distanceValue > 7)                         //initial 7 cm is weird
                {
                    distanceValue = distanceValue - 7;  
                    time = 500;                                 //the initial 7 cm takes 500ms
                    dist = distanceValue;                       //make remaining distance a float for division
                    time2 = (int)((dist / 12) * 500);           //get the remaining time working, moves 12cm in 500ms
                    time = time + time2;                        
                }
                MoveForward(time);                              //calls move fowrward function, for the time needed
            }
            if (distanceValue < 0)
            {
                distanceValue = Math.Abs(distanceValue);        //needs calibrating but the same principle as before
                int time = 0;
                int time2 = 0;
                float dist = 0;
                if (distanceValue > 7)
                {
                    distanceValue = distanceValue - 7;
                    time = 500;
                    dist = distanceValue;
                    time2 = (int)((dist / 12) * 500);
                    time = time + time2;
                }
                MoveBackward(time);
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            timer3.Enabled = false;
            StopMoving();
        }

        private void textBox1_TextChanged(object sender, EventArgs e) //used for entering distance to travel
        {
            // converting the text from the textbox to an int
            distanceValue = Convert.ToInt32(textBox1.Text);
            distanceValue = int.Parse(textBox1.Text);
        }

        private void button7_Click(object sender, EventArgs e) //Move pen up/down with servo
        {
            if (!myClient.isConnected) return;
            myClient.SendData(CommandID.PowerSwitch, new byte[] { 3 }); //Switches pen servo on

            if (penUp) 
            {
                myClient.SendData(CommandID.SetServoPosition, new byte[] { 80 });
                label13.Text = "Up";
            }
            else {
                myClient.SendData(CommandID.SetServoPosition, new byte[] { 255 });
                label13.Text = "Down";
            }
            penUp = !penUp;
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            int n;
            for(n=0; n<10000; n++)
    
            {
                System.IO.StreamWriter file1 = new System.IO.StreamWriter("file.txt", true);
                file1.WriteLine(magX + "," + magY + "," + magZ + "," + ",");
                file1.Close();
            }
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void txtIP_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnCon_Click(sender, null);
                btnToggleGreen.Focus();
            }
        }
    }
}
