 using System;
using System.Collections.Generic;
using System.Text;

namespace CommsLib
{
    /// <summary>
    /// Public enumerable containing information about packet headers
    /// </summary>
    public enum CommandID : int
    {
        NullEcho = 0x00,
        GetLEDandSwitchStatus = 0x01,
        SetLEDs = 0x02,
        InternalCounter = 0x03,
        MotorPosition = 0x04,
        SetLeftMotorSpeed = 0x05,
        SetRightMotorSpeed = 0x06,
        SetMotorsSpeed = 0x07,
        LineFollowingData = 0x08,
        SetLineThresholds = 0x09,
        GetLightAuxValue = 0x11,
        SetServoPosition = 0x16,
        GetTermData = 0x1B,
        GetCrashedRoverData = 0x1C,
        GetMagnetValue= 0x2D,
        GetAccelValue=0x28,
        DriveSteps = 0x80,
        LightPositionData = 0x81,
        IRDistanceData = 0x82,
        IRDataBuffer = 0x83,
        AutomaticIRBuffer = 0x84,
        AutomaticAccelBuffer = 0x85,
        AccDATABuffer = 0x86,
        AccDATABufferPacket = 0x87,
        MagDATABuffer = 0x88,
        MagDATABufferPacketX = 0x89,
        MagDATABufferPacketY = 0x8A,
        MagDATABufferPacketZ = 0x8B,
        DRIVE_MAX_SPEED = 0xC8,
        MotorSpeedClosed = 0x38, //closed loop motor speed 
        PowerSwitch = 0x15, //Turn servo power on or off
    };
}
