float KalResult = 0;
float KalPc = 0.0;
float KalG = 0.0;
float KalP = 1.0;

// KalVariance, larger is more filtering
// KalProcess, smaller is more filtering

float KF(float DataPoint, float KalVariance, float KalProcess)
{
  KalPc = KalP + KalProcess;
  KalG = KalPc / (KalPc + KalVariance);
  KalP = (1 - KalG) * KalPc;
  KalResult = KalG * (DataPoint - KalResult) + KalResult;
  return KalResult;
}
