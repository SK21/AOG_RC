/*
 * Copyright (c) 2019 Gregory Tomasch.  All rights reserved.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to
 * deal with the Software without restriction, including without limitation the
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
 * sell copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 *  1. Redistributions of source code must retain the above copyright notice,
 *     this list of conditions and the following disclaimers.
 *  2. Redistributions in binary form must reproduce the above copyright
 *     notice, this list of conditions and the following disclaimers in the
 *     documentation and/or other materials provided with the distribution.
 *  3. The names of Gregory Tomasch and his successors
 *     may not be used to endorse or promote products derived from this Software
 *     without specific prior written permission.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL THE
 * CONTRIBUTORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
 * WITH THE SOFTWARE.
 */

#ifndef EM7180_h
#define EM7180_h

#include <Wire.h>
#include "I2Cdev.h"
#include "Alarms.h"
#include "def.h"

// EM7180 Register definitions
#define EM7180_ADDRESS                0x28
#define EM7180_QX                     0x00
#define EM7180_QY                     0x04
#define EM7180_QZ                     0x08
#define EM7180_QW                     0x0C
#define EM7180_QTIME                  0x10
#define EM7180_MX                     0x12
#define EM7180_MY                     0x14
#define EM7180_MZ                     0x16
#define EM7180_MTIME                  0x18
#define EM7180_AX                     0x1A
#define EM7180_AY                     0x1C
#define EM7180_AZ                     0x1E
#define EM7180_ATIME                  0x20
#define EM7180_GX                     0x22
#define EM7180_GY                     0x24
#define EM7180_GZ                     0x26
#define EM7180_GTIME                  0x28
#define EM7180_Baro                   0x2A
#define EM7180_BaroTIME               0x2C
#define EM7180_Temp                   0x2E
#define EM7180_TempTIME               0x30
#define EM7180_QRateDivisor           0x32
#define EM7180_EnableEvents           0x33
#define EM7180_HostControl            0x34
#define EM7180_EventStatus            0x35
#define EM7180_SensorStatus           0x36
#define EM7180_SentralStatus          0x37
#define EM7180_AlgorithmStatus        0x38
#define EM7180_FeatureFlags           0x39
#define EM7180_ParamAcknowledge       0x3A
#define EM7180_SavedParamByte0        0x3B
#define EM7180_SavedParamByte1        0x3C
#define EM7180_SavedParamByte2        0x3D
#define EM7180_SavedParamByte3        0x3E
#define EM7180_ActualMagRate          0x45
#define EM7180_ActualAccelRate        0x46
#define EM7180_ActualGyroRate         0x47
#define EM7180_ErrorRegister          0x50
#define EM7180_AlgorithmControl       0x54
#define EM7180_MagRate                0x55
#define EM7180_AccelRate              0x56
#define EM7180_GyroRate               0x57
#define EM7180_BaroRate               0x58
#define EM7180_TempRate               0x59
#define EM7180_LoadParamByte0         0x60
#define EM7180_LoadParamByte1         0x61
#define EM7180_LoadParamByte2         0x62
#define EM7180_LoadParamByte3         0x63
#define EM7180_ParamRequest           0x64
#define EM7180_ROMVersion1            0x70
#define EM7180_ROMVersion2            0x71
#define EM7180_RAMVersion1            0x72
#define EM7180_RAMVersion2            0x73
#define EM7180_ProductID              0x90
#define EM7180_RevisionID             0x91
#define EM7180_RunStatus              0x92
#define EM7180_UploadAddress          0x94
#define EM7180_UploadData             0x96  
#define EM7180_CRCHost                0x97
#define EM7180_ResetRequest           0x9B   
#define EM7180_PassThruStatus         0x9E   
#define EM7180_PassThruControl        0xA0
#define EM7180_ACC_LPF_BW             0x5B                                                                                 //Register GP36
#define EM7180_GYRO_LPF_BW            0x5C                                                                                 //Register GP37
#define EM7180_MAG_LPF_BW             0x5D                                                                                 //Register GP38
#define EM7180_BARO_LPF_BW            0x5E                                                                                 //Register GP39
#define EM7180_GP8                    0x3F
#define EM7180_GP9                    0x40
#define EM7180_GP10                   0x41
#define EM7180_GP11                   0x42
#define EM7180_GP12                   0x43
#define EM7180_GP13                   0x44
#define EM7180_GP20                   0x4B
#define EM7180_GP21                   0x4C
#define EM7180_GP22                   0x4D
#define EM7180_GP23                   0x4E
#define EM7180_GP24                   0x4F
#define EM7180_GP36                   0x5B
#define EM7180_GP37                   0x5C
#define EM7180_GP38                   0x5D
#define EM7180_GP39                   0x5E
#define EM7180_GP40                   0x5F
#define EM7180_GP50                   0x69
#define EM7180_GP51                   0x6A
#define EM7180_GP52                   0x6B
#define EM7180_GP53                   0x6C
#define EM7180_GP54                   0x6D
#define EM7180_GP55                   0x6E
#define EM7180_GP56                   0x6F

extern float                          qt[2][4];
extern int16_t                        QT_Timestamp[2];
extern int16_t                        gyroADC[2][3], acc[2][3], accADC[2][3], acc_calADC[2][3];
extern int16_t                        magADC[2][3];
extern int16_t                        accLIN[2][3];
extern uint16_t                       calibratingA[2];
extern uint8_t                        Accel_Cal_valid[2];
extern uint8_t                        Sentral_WS_valid[2];
extern uint32_t                       currentTime;
extern struct Sentral_WS_params       WS_params;
extern global_conf_t                  global_conf;
extern int64_t                        a_acc[2][3];
extern int64_t                        b_acc[2][3];
extern float                          gaps;
extern float                          mag_var;
extern float                          wobble;
extern float                          fiterror;
extern uint8_t                        eventStatus[2];

class EM7180
{
  public:
                                      EM7180(I2Cdev*, uint8_t);
     void                             ACC_getADC();
     void                             LIN_ACC_getADC();
     void                             ACC_Common();
     void                             Mag_getADC();
     void                             Save_Sentral_WS_params();
     void                             Gyro_getADC();
     void                             getQUAT();
     int16_t                          Baro_getPress();
     int16_t                          Baro_getTemp();
     void                             initSensors();
     I2Cdev*                          I2C;                                                                                 // Class constructor variable
     uint8_t                          SensorNum;                                                                           // Class constructor variable
  private:
     void                             i2c_getSixRawADC(uint8_t add, uint8_t reg);
     float                            uint32_reg_to_float (uint8_t *buf);
     void                             float_to_bytes (float param_val, uint8_t *buf);
     void                             EM7180_set_gyro_FS (uint16_t gyro_fs);
     void                             EM7180_set_mag_acc_FS (uint16_t mag_fs, uint16_t acc_fs);
     void                             EM7180_set_integer_param (uint8_t param, uint32_t param_val);
     void                             EM7180_set_float_param (uint8_t param, float param_val);
     void                             EM7180_acc_cal_upload();
     void                             EM7180_set_WS_params();
     void                             EM7180_get_WS_params();
     void                             WS_PassThroughMode();
     void                             WS_Resume();
     void                             readSenParams();
     void                             writeSenParams();
     void                             readAccelCal();
     void                             writeAccCal();
};

#endif // EM7180_h
