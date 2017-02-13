﻿using System;

namespace Comms
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCon = new System.Windows.Forms.Button();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblRedStatus = new System.Windows.Forms.Label();
            this.lblGreenStatus = new System.Windows.Forms.Label();
            this.btnToggleRed = new System.Windows.Forms.Button();
            this.btnToggleGreen = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lblPosRight = new System.Windows.Forms.Label();
            this.lblPosLeft = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.button7 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCon);
            this.groupBox1.Controls.Add(this.txtIP);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(217, 127);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Communications";
            // 
            // btnCon
            // 
            this.btnCon.Location = new System.Drawing.Point(73, 76);
            this.btnCon.Name = "btnCon";
            this.btnCon.Size = new System.Drawing.Size(75, 23);
            this.btnCon.TabIndex = 2;
            this.btnCon.Text = "Connect";
            this.btnCon.UseVisualStyleBackColor = true;
            this.btnCon.Click += new System.EventHandler(this.btnCon_Click);
            // 
            // txtIP
            // 
            this.txtIP.Location = new System.Drawing.Point(65, 30);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(98, 20);
            this.txtIP.TabIndex = 1;
            this.txtIP.Text = "10.215.2.36";
            this.txtIP.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtIP_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblRedStatus);
            this.groupBox2.Controls.Add(this.lblGreenStatus);
            this.groupBox2.Controls.Add(this.btnToggleRed);
            this.groupBox2.Controls.Add(this.btnToggleGreen);
            this.groupBox2.Location = new System.Drawing.Point(235, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(295, 127);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "LEDs and buttons";
            // 
            // lblRedStatus
            // 
            this.lblRedStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblRedStatus.Location = new System.Drawing.Point(166, 69);
            this.lblRedStatus.Name = "lblRedStatus";
            this.lblRedStatus.Size = new System.Drawing.Size(100, 18);
            this.lblRedStatus.TabIndex = 1;
            this.lblRedStatus.Text = "Red Status";
            this.lblRedStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblGreenStatus
            // 
            this.lblGreenStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblGreenStatus.Location = new System.Drawing.Point(166, 40);
            this.lblGreenStatus.Name = "lblGreenStatus";
            this.lblGreenStatus.Size = new System.Drawing.Size(100, 18);
            this.lblGreenStatus.TabIndex = 1;
            this.lblGreenStatus.Text = "Green Status";
            this.lblGreenStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnToggleRed
            // 
            this.btnToggleRed.Location = new System.Drawing.Point(21, 67);
            this.btnToggleRed.Name = "btnToggleRed";
            this.btnToggleRed.Size = new System.Drawing.Size(108, 23);
            this.btnToggleRed.TabIndex = 0;
            this.btnToggleRed.Text = "Toggle Red";
            this.btnToggleRed.UseVisualStyleBackColor = true;
            this.btnToggleRed.Click += new System.EventHandler(this.btnToggleRed_Click);
            // 
            // btnToggleGreen
            // 
            this.btnToggleGreen.Location = new System.Drawing.Point(21, 38);
            this.btnToggleGreen.Name = "btnToggleGreen";
            this.btnToggleGreen.Size = new System.Drawing.Size(108, 23);
            this.btnToggleGreen.TabIndex = 0;
            this.btnToggleGreen.Text = "Toggle Green";
            this.btnToggleGreen.UseVisualStyleBackColor = true;
            this.btnToggleGreen.Click += new System.EventHandler(this.btnToggleGreen_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.lblPosRight);
            this.groupBox3.Controls.Add(this.lblPosLeft);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(536, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(214, 127);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Tracks";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Starboard:";
            // 
            // lblPosRight
            // 
            this.lblPosRight.AutoSize = true;
            this.lblPosRight.Location = new System.Drawing.Point(84, 72);
            this.lblPosRight.Name = "lblPosRight";
            this.lblPosRight.Size = new System.Drawing.Size(13, 13);
            this.lblPosRight.TabIndex = 0;
            this.lblPosRight.Text = "0";
            // 
            // lblPosLeft
            // 
            this.lblPosLeft.AutoSize = true;
            this.lblPosLeft.Location = new System.Drawing.Point(84, 33);
            this.lblPosLeft.Name = "lblPosLeft";
            this.lblPosLeft.Size = new System.Drawing.Size(13, 13);
            this.lblPosLeft.TabIndex = 0;
            this.lblPosLeft.Text = "0";
            this.lblPosLeft.Click += new System.EventHandler(this.lblPosLeft_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(49, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Port:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(87, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "label4";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(87, 43);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "label5";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(87, 68);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "label6";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(29, 28);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "label7";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(129, 28);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 13);
            this.label8.TabIndex = 9;
            this.label8.Text = "label8";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(231, 28);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(35, 13);
            this.label9.TabIndex = 10;
            this.label9.Text = "label9";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(216, 19);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(145, 38);
            this.button2.TabIndex = 11;
            this.button2.Text = "Move Forward";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(367, 19);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(145, 38);
            this.button3.TabIndex = 12;
            this.button3.Text = "Move Backward";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(216, 63);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(145, 38);
            this.button4.TabIndex = 13;
            this.button4.Text = "Move Left";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(367, 63);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(145, 38);
            this.button5.TabIndex = 14;
            this.button5.Text = "Move Right";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(287, 108);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(145, 44);
            this.button6.TabIndex = 15;
            this.button6.Text = "STOP";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(128, 65);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 13);
            this.label10.TabIndex = 16;
            this.label10.Text = "label10";
            this.label10.Click += new System.EventHandler(this.label10_Click_1);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Location = new System.Drawing.Point(235, 149);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(295, 100);
            this.groupBox4.TabIndex = 18;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Magnetometer Values:";
            this.groupBox4.Enter += new System.EventHandler(this.groupBox4_Enter);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Location = new System.Drawing.Point(12, 149);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(217, 100);
            this.groupBox5.TabIndex = 19;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Accelerometer Values:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(19, 24);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(112, 13);
            this.label11.TabIndex = 20;
            this.label11.Text = "DistanceToTravel(cm)";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.button1);
            this.groupBox6.Controls.Add(this.button6);
            this.groupBox6.Controls.Add(this.textBox1);
            this.groupBox6.Controls.Add(this.button2);
            this.groupBox6.Controls.Add(this.button4);
            this.groupBox6.Controls.Add(this.button5);
            this.groupBox6.Controls.Add(this.button3);
            this.groupBox6.Controls.Add(this.label11);
            this.groupBox6.Location = new System.Drawing.Point(12, 255);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(518, 158);
            this.groupBox6.TabIndex = 21;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Precise Movement";
            this.groupBox6.Enter += new System.EventHandler(this.groupBox6_Enter);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(151, 52);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(34, 23);
            this.button1.TabIndex = 22;
            this.button1.Text = "Go";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(22, 54);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(126, 20);
            this.textBox1.TabIndex = 21;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // timer3
            // 
            this.timer3.Tick += new System.EventHandler(this.timer3_Tick);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(588, 168);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(94, 46);
            this.button7.TabIndex = 22;
            this.button7.Text = "Move Pen";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(762, 419);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "2nd Year Challenge - Team 6";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.Form1_PreviewKeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);

        }



        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnCon;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblRedStatus;
        private System.Windows.Forms.Label lblGreenStatus;
        private System.Windows.Forms.Button btnToggleRed;
        private System.Windows.Forms.Button btnToggleGreen;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblPosRight;
        private System.Windows.Forms.Label lblPosLeft;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Timer timer3;
        private System.Windows.Forms.Button button7;
    }
}

