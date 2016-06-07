#pragma once

#include <windows.h>
#pragma comment(lib, "winmm.lib")

#ifdef __cplusplus
#define EXPORT extern "C" __declspec (dllexport)
#else
#define EXPORT __declspec (dllexport)
#endif

#define INP_BUFFER_SIZE 16384

typedef struct {
	DWORD len;
	PBYTE p;
}rData;

EXPORT BOOL createDialog();

EXPORT rData endDialog();

EXPORT INT startRec(int, short);

EXPORT INT stopRec();

EXPORT INT sendPlay(PBYTE, int, int, short);

EXPORT INT sendPlayPause();

EXPORT INT sendPlayStop();