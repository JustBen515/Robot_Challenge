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

        double startingAngle;
        double newAngle; //Angle we wish to move it by total
        double requestedAngle; //Input from text box
        bool clockwise = true;

        int distanceValue; //Distance to move forward/backwards (in cm)

        bool penUp; //Whether pen is up or down for drawing

        int listPointer = -1;
        public List<procedurePair> procedureList = new List<procedurePair>();
        public int sideA = (int)((23 / 12) * 500) + 500;           //sides converted to a time
        public int sideB = (int)((33 / 12) * 500) + 500;
        public int sideC = (int)((43 / 12) * 500) + 500;
        public double angle1 = 180 - 90;                                 //angles of the turn required
        public double angle2 = 180 - 36.87;
        public double angle3 = 180 - 53.13;

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
            //label4.Text = accX.ToString(); //Update accelerometer values
            //label5.Text = accY.ToString();
            //label6.Text = accZ.ToString();

            label7.Text = magX.ToString(); //Update magnetometer values
            label8.Text = magY.ToString();
            //label9.Text = magZ.ToString();
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

        public void MoveLeft()
        {
            myClient.SendData(CommandID.MotorSpeedClosed, new byte[] { 252, 0, 3, 0, 1 });
            robotIsMoving = true;
        }

        public void MoveRight()
        {
            myClient.SendData(CommandID.MotorSpeedClosed, new byte[] { 3, 0, 252, 0, 1 });
            robotIsMoving = true;
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
            timer4.Enabled = false;
        }

        private void lblPosLeft_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

            penDOWN();
            MoveForward(sideA);
            penUP();
            checkAndTurn(angle1);
            penDOWN();
            MoveForward(sideB);
            penUP();
            checkAndTurn(angle2);
            penDOWN();
            MoveForward(sideC);
            penUP();
            checkAndTurn(angle3);

            

            var PenUP = new procedurePair();
            PenUP.index = 1;
            PenUP.procedure = "PenUP";
            PenUP.paramiters = 0;

            var PenDOWN = new procedurePair();

            PenDOWN.procedure = "PenDOWN";
            PenDOWN.paramiters = 0;
            
            var Move1 = new procedurePair();
            Move1.procedure = "Move1";
            Move1.paramiters = sideA;

            var Move2 = new procedurePair();
            Move2.procedure = "Move2";
            Move2.paramiters = sideB;

            var Move3 = new procedurePair();
            Move3.procedure = "Move3";
            Move3.paramiters = sideC;

            var Turn1 = new procedurePair();
            Turn1.procedure = "Turn1";
            Turn1.paramiters = angle1;

            var Turn2 = new procedurePair();
            Turn2.procedure = "Turn2";
            Turn2.paramiters = angle2;

            var Turn3 = new procedurePair();
            Turn3.procedure = "Turn3";
            Turn3.paramiters = angle3;

            procedureList.Add(PenDOWN);
            procedureList.Add(Move1);
            procedureList.Add(PenUP);
            procedureList.Add(Turn1);
            procedureList.Add(PenDOWN);
            procedureList.Add(Move2);
            procedureList.Add(PenUP);
            procedureList.Add(Turn2);
            procedureList.Add(PenDOWN);
            procedureList.Add(Move3);
            procedureList.Add(PenUP);
            procedureList.Add(Turn3);

            

        }

        private void button8_Click_1(object sender, EventArgs e) //Draw triangle button
        {
            if (listPointer > 12)
            {

                listPointer++;
                String procedureItem = procedureList[listPointer].procedure;
                switch (procedureItem)
                {
                    case "PenUP":
                        penUP();
                        break;
                    case "PenDOWN":
                        penDOWN();
                        break;
                    case "Move1":
                        MoveForward(Convert.ToInt32(procedureList[listPointer].paramiters));
                        break;
                    case "Move2":
                        MoveForward(Convert.ToInt32(procedureList[listPointer].paramiters));
                        break;
                    case "Move3":
                        MoveForward(Convert.ToInt32(procedureList[listPointer].paramiters));
                        break;
                    case "Turn1":
                        newAngle = angle1;
                        timer4.Enabled = true;
                        break;
                    case "Turn2":
                        newAngle = angle2;
                        timer4.Enabled = true;
                        break;
                    case "Turn3":
                        newAngle = angle3;
                        timer4.Enabled = false;
                        break;
                }
            }
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
            distanceValue = Convert.ToInt32(textBox1.Text);
            distanceValue = int.Parse(textBox1.Text);

            if (distanceValue > 0)                             //for forwards
            {
                int time = 0;
                int time2 = 0;
                float dist = 0;
                if (distanceValue > 8)                         //initial 7 cm is weird
                {
                    distanceValue = distanceValue - 8;  
                    time = 500;                                 //the initial 7 cm takes 500ms
                    dist = distanceValue;                       //make remaining distance a float for division
                    time2 = (int)((dist / 15) * 500);           //get the remaining time working, moves 12cm in 500ms
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
                if (distanceValue > 0)
                {
                    distanceValue = distanceValue - 0s;
                    time = 500;
                    dist = distanceValue;
                    time2 = (int)((dist / 17) * 500);
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

        }
        private void button7_Click(object sender, EventArgs e) //Move pen up/down with servo
        {
            if (!myClient.isConnected) return;
            myClient.SendData(CommandID.PowerSwitch, new byte[] { 3 }); //Switches pen servo on

            if (penUp) 
            {
                penUP();
                label13.Text = "Up";
            }
            else {
                penDOWN();
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

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // converting the text from the textbox to an int
            //requestedAngle = Convert.ToDouble(textBox2.Text);
            //requestedAngle = Double.Parse(textBox2.Text);
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            checkAndTurn(newAngle);
        }

        private void checkAndTurn(double inputAngle)
        {
            if (clockwise)
            {
                if (compass > (inputAngle + 5))
                {
                    MoveRight();
                }
                if (compass < (inputAngle - 5)) //Use newAngle in here
                {
                    MoveRight();
                }
                else
                {
                    label14.Text = "Finished rotation";
                    timer4.Enabled = false;
                    StopMoving();
                }
            }
            else if (!clockwise)
            {
                if (compass < (inputAngle - 5)) //Use newAngle in here
                {
                    MoveLeft();
                }
                if (compass > (inputAngle + 5))
                {
                    MoveLeft();
                }
                else
                {
                    label14.Text = "Finished rotation";
                    timer4.Enabled = false;
                    StopMoving();
                }
            }
        }

        private void button9_Click(object sender, EventArgs e) //angle Go button
        {
            startingAngle = compass;
            newAngle = Convert.ToDouble(textBox2.Text) + startingAngle; //takes in the value
            if (newAngle > 360)
            {
                newAngle -= 360;
            }
            if (newAngle < 0)
            {
                newAngle += 360;
            }
            //newAngle = newAngle * 0.95;
            timer4.Enabled = true;
        }

        private void penUP()
        {
            if (!myClient.isConnected) return;
            myClient.SendData(CommandID.PowerSwitch, new byte[] { 3 }); //Switches pen servo on
            myClient.SendData(CommandID.SetServoPosition, new byte[] { 80 });
        }
        private void penDOWN()
        {
            if (!myClient.isConnected) return;
            myClient.SendData(CommandID.PowerSwitch, new byte[] { 3 }); //Switches pen servo on
            myClient.SendData(CommandID.SetServoPosition, new byte[] { 225 });
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            clockwise = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            clockwise = false;
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
    public class procedurePair
    {
        public int index;
        public String procedure;
        public double paramiters;

    }
}
