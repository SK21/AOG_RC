/* 
	Editor: https://www.visualmicro.com/
			This file is for intellisense purpose only.
			Visual micro (and the arduino ide) ignore this code during compilation. This code is automatically maintained by visualmicro, manual changes to this file will be overwritten
			The contents of the _vm sub folder can be deleted prior to publishing a project
			All non-arduino files created by visual micro and all visual studio project or solution files can be freely deleted and are not required to compile a sketch (do not delete your own code!).
			Note: debugger breakpoints are stored in '.sln' or '.asln' files, knowledge of last uploaded breakpoints is stored in the upload.vmps.xml file. Both files are required to continue a previous debug session without needing to compile and upload again
	
	Hardware: SparkFun 9DoF Razor IMU M0, Platform=samd, Package=SparkFun
*/

#if defined(_VMICRO_INTELLISENSE)

#ifndef _VSARDUINO_H_
#define _VSARDUINO_H_
#define F_CPU 48000000L
#define ARDUINO 10805
#define ARDUINO_SAMD_ZERO
#define ARDUINO_ARCH_SAMD
#define __SAMD21G18A__
#define USB_VID 0x1B4F
#define USB_PID 0x9D0F
#define USBCON
#define __cplusplus 201103L
//#define __GNUC__ 2
#define _Pragma(x)
#define __ARMCC_VERSION 6010050

#define __PTRDIFF_TYPE__ int
#define __ARM__
#define __arm__
#define always_inline
#define __inline__
#define __asm__(x)
#define __attribute__(x)
#define __extension__
#define __ATTR_PURE__
#define __ATTR_CONST__
#define __inline__
#define __volatile__
typedef int __SIZE_TYPE__;
typedef int __builtin_va_list;
typedef int __builtin_arm_nop;
typedef int __builtin_arm_wfi;
typedef int __builtin_arm_wfe;
typedef int __builtin_arm_sev;
typedef int __builtin_arm_isb;
typedef int __builtin_arm_dsb;
typedef int __builtin_arm_dmb;
typedef int __builtin_bswap32;
typedef int __builtin_bswap16;
#define _Pragma(x)
#define __ASM
#define __INLINE

#include "samd.h"
//#include "samd21/include/samd21.h"



#include "arduino.h"
#include <pins_arduino.h> 
#include <variant.h> 
#include <variant.cpp> 

#ifndef __math_68881
extern double atan(double);
extern double cos(double);
extern double sin(double);
extern double tan(double);
extern double tanh(double);
extern double frexp(double, int *);
extern double modf(double, double *);
extern double ceil(double);
extern double fabs(double);
extern double floor(double);
#endif 

#ifndef __math_68881
extern double acos(double);
extern double asin(double);
extern double atan2(double, double);
extern double cosh(double);
extern double sinh(double);
extern double exp(double);
extern double ldexp(double, int);
extern double log(double);
extern double log10(double);
extern double pow(double, double);
extern double sqrt(double);
extern double fmod(double, double);
#endif 

extern int __isinff(float x);
extern int __isinfd(double x);
extern int __isnanf(float x);
extern int __isnand(double x);
extern int __fpclassifyf(float x);
extern int __fpclassifyd(double x);
extern int __signbitf(float x);
extern int __signbitd(double x);
extern int finitel(long double);
extern double infinity(void);
extern double nan(const char *);
extern int finite(double);
extern double copysign(double, double);
extern double logb(double);
extern int ilogb(double);

extern double asinh(double);
extern double cbrt(double);
extern double nextafter(double, double);
extern double rint(double);
extern double scalbn(double, int);

extern double exp2(double);
extern double scalbln(double, long int);
extern double tgamma(double);
extern double nearbyint(double);
extern long int lrint(double);
extern long long int llrint(double);
extern long int lround(double);
extern long long int llround(double);
extern double trunc(double);
extern double remquo(double, double, int *);
extern double fdim(double, double);
extern double fmax(double, double);
extern double fmin(double, double);
extern double fma(double, double, double);

#ifndef __math_68881
extern double log1p(double);
extern double expm1(double);
#endif 

extern double acosh(double);
extern double atanh(double);
extern double remainder(double, double);
extern double gamma(double);
extern double lgamma(double);
extern double erf(double);
extern double erfc(double);
extern double log2(double);

#ifndef __math_68881
extern double hypot(double, double);
#endif

extern float atanf(float);
extern float cosf(float);
extern float sinf(float);
extern float tanf(float);
extern float tanhf(float);
extern float frexpf(float, int *);
extern float modff(float, float *);
extern float ceilf(float);
extern float fabsf(float);
extern float floorf(float);

#ifndef _REENT_ONLY
extern float acosf(float);
extern float asinf(float);
extern float atan2f(float, float);
extern float coshf(float);
extern float sinhf(float);
extern float expf(float);
extern float ldexpf(float, int);
extern float logf(float);
extern float log10f(float);
extern float powf(float, float);
extern float sqrtf(float);
extern float fmodf(float, float);
#endif 

extern float exp2f(float);
extern float scalblnf(float, int);
extern float tgammaf(float);
extern float nearbyintf(float);
extern long int lrintf(float);
extern long long int llrintf(float);
extern float roundf(float);
extern long int lroundf(float);
extern long long int llroundf(float);
extern float truncf(float);
extern float remquof(float, float, int *);
extern float fdimf(float, float);
extern float fmaxf(float, float);
extern float fminf(float, float);
extern float fmaf(float, float, float);

extern float infinityf(void);
extern float nanf(const char *);
extern int finitef(float);
extern float copysignf(float, float);
extern float logbf(float);
extern int ilogbf(float);

extern float asinhf(float);
extern float cbrtf(float);
extern float nextafterf(float, float);
extern float rintf(float);
extern float scalbnf(float, int);
extern float log1pf(float);
extern float expm1f(float);

#ifndef _REENT_ONLY
extern float acoshf(float);
extern float atanhf(float);
extern float remainderf(float, float);
extern float gammaf(float);
extern float lgammaf(float);
extern float erff(float);
extern float erfcf(float);
extern float log2f(float);
extern float hypotf(float, float);
#endif 

extern double drem(double, double);
extern void sincos(double, double *, double *);
extern double gamma_r(double, int *);
extern double lgamma_r(double, int *);

extern double y0(double);
extern double y1(double);
extern double yn(int, double);
extern double j0(double);
extern double j1(double);
extern double jn(int, double);

extern float dremf(float, float);
extern void sincosf(float, float *, float *);
extern float gammaf_r(float, int *);
extern float lgammaf_r(float, int *);

extern float y0f(float);
extern float y1f(float);
extern float ynf(int, float);
extern float j0f(float);
extern float j1f(float);
extern float jnf(int, float);







_PTR 	 memchr(const _PTR, int size_t) {
	return 0;
}

int 	 memcmp(const _PTR, const _PTR size_t) {
	return 0;
}

_PTR 	 memcpy(_PTR __restrict, const _PTR  size_t) {
	return 0;
}

_PTR	 memmove(_PTR, const _PTR size_t) {
	return 0;
}

_PTR memset(void *, int size_t)
{
	return 0;
}

char 	strcat(char *, const char *) {
	return 0;
}

char 	strchr(const char *, int) {
	return 0;
}

int	 strcmp(const char *, const char *) {
	return 0;
}

int	 strcoll(const char *, const char *) {
	return 0;
}

char strcpy(const char *, const char *) {
	return 0;
}
char strcpy(char *, const char *) {
	return 0;
}
char strcpy(char *, char *) {
	return 0;
}

size_t	 strcspn(const char *, const char *) {
	return 0;
}

char 	*strerror(int) {
	return 0;
}

size_t	 strlen(const char *) {
	return 0;
}

char 	*strncat(char *, const char * size_t) {
	return 0;
}

int	 strncmp(const char *, const char *, size_t) {
	return 0;
}

char 	*strncpy(char *__restrict, const char *, size_t) {
	return 0;
}

char 	*strpbrk(const char *, const char *) {
	return 0;
}

char 	*strrchr(const char *, int) {
	return 0;
}

size_t	 strspn(const char *, const char *) {
	return 0;
}

char 	*strstr(const char *, const char *) {
	return 0;
}


#ifndef _REENT_ONLY
char 	*strtok(char *, const char *) {
	return 0;
}

#endif

size_t	 strxfrm(char *, const char *, size_t) {
	return 0;
}


#ifndef __STRICT_ANSI__
char 	*strtok_r(char *, const char *, char **) {
	return 0;
}


int	 bcmp(const void *, const void *, size_t) {
	return 0;
}

void	 bcopy(const void *, void *, size_t) {

}

void	 bzero(void *, size_t) {

}


char 	*index(const char *, int) {
	return 0;
}

_PTR	 memccpy(_PTR, const _PTR, int, size_t) {
	return 0;
}

_PTR	 mempcpy(_PTR, const _PTR, size_t) {
	return 0;
}

_PTR	 memmem(const _PTR, size_t, const _PTR, size_t) {
	return 0;
}

_PTR 	 memrchr(const _PTR, int, size_t) {
	return 0;
}

_PTR 	 rawmemchr(const _PTR, int) {
	return 0;
}

char 	*rindex(const char *, int) {
	return 0;
}

char 	*stpcpy(char *, const char *) {
	return 0;
}

char 	*stpncpy(char *, const char *, size_t) {
	return 0;
}

int	 strcasecmp(const char *, const char *) {
	return 0;
}

char	*strcasestr(const char *, const char *) {
	return 0;
}

char 	*strchrnul(const char *, int) {
	return 0;
}

#endif

#include "IMU_Razor.ino"
#endif
#endif
