/*----------------------------------------
   RECORD1.C -- Waveform Audio Recorder
                (c) Charles Petzold, 1998
  ----------------------------------------*/

#include <windows.h>
#include "resource.h"
#include "projectheader.h"

#define INP_BUFFER_SIZE 16384
WAVEFORMATEX waveform;
HWND wndHwnd;
HMODULE hmodDLL;
PBYTE        pSaveBuffer;
DWORD dwDataLength;
BOOL CALLBACK DlgProc (HWND, UINT, WPARAM, LPARAM) ;
TCHAR szAppName [] = TEXT ("Record1") ;

BOOL APIENTRY DllMain(HMODULE hModule, DWORD reason, LPVOID reserved) 
{
	if (reason == DLL_PROCESS_ATTACH) {
		hmodDLL = hModule;
		DisableThreadLibraryCalls(hModule);
	}
	return TRUE;
}

void ReverseMemory (BYTE * pBuffer, int iLength)
{
     BYTE b ;
     int  i ;
     
     for (i = 0 ; i < iLength / 2 ; i++)
     {
          b = pBuffer [i] ;
          pBuffer [i] = pBuffer [iLength - i - 1] ;
          pBuffer [iLength - i - 1] = b ;
     }
}
//--------------------------- MY FUNCTIONS START HERE -----------///
//this is my function that starts the dialog box in C#.
BOOL createDialog()
{
	if (CreateDialog(hmodDLL, MAKEINTRESOURCE(DLG_RECORD), NULL, DlgProc)) {
		ShowWindow(wndHwnd, SW_HIDE);
		return TRUE;
	}
	else {
		return FALSE;
	}
}
//this function returns data from the dialog box to C#.
//it returns a pointer to the byte data and the length.
rData endDialog() {
	rData reco = { dwDataLength, pSaveBuffer};
	return reco;
}
//this passes to the win32 DLL the sample rate and bit
//depth to record at, and then calls the record message.
//in c#
int startRec(int sampleRate, short bitDepth) {

	waveform.nSamplesPerSec = sampleRate;
	waveform.wBitsPerSample = bitDepth;

	WPARAM msg = MAKEWPARAM(IDC_RECORD_BEG, 0);
	PostMessage(wndHwnd, WM_COMMAND, msg, 0);
	return 1;
}
//this calls the record stop message in c#.
int stopRec() {
	WPARAM msg = MAKEWPARAM(IDC_RECORD_END, 0);
	PostMessage(wndHwnd, WM_COMMAND, msg, 0);
	return 1;
}
//this sends the play message from c#. it passes back a pointer
//to the data marshalled and allocated in c# as well as the sample 
//rate and bit depth to play at.
//the only other difference is that those attirbutes of the 
//waveform are set here instead of inside play.
int sendPlay(PBYTE data, int size, int sampleRate, short bitDepth) {
	pSaveBuffer = realloc(pSaveBuffer, size*sizeof(byte));
	dwDataLength = size;
	for (int i = 0; i < size; i++) {
		pSaveBuffer[i] = data[i];
	}
	waveform.nSamplesPerSec = sampleRate;
	waveform.wBitsPerSample = bitDepth;
	WPARAM msg = MAKEWPARAM(IDC_PLAY_BEG, 0);
	PostMessage(wndHwnd, WM_COMMAND, msg, 0);
	return 1;
}
int sendPlayPause() {
	WPARAM msg = MAKEWPARAM(IDC_PLAY_PAUSE, 0);
	PostMessage(wndHwnd, WM_COMMAND, msg, 0);
	return 1;
}

int sendPlayStop() {
	WPARAM msg = MAKEWPARAM(IDC_PLAY_END, 0);
	PostMessage(wndHwnd, WM_COMMAND, msg, 0);
	return 1;
}

BOOL CALLBACK DlgProc (HWND hwnd, UINT message, WPARAM wParam, LPARAM lParam)
{
     static BOOL         bRecording, bPlaying, bReverse, bPaused,
                         bEnding, bTerminating ;
     static DWORD        dwRepetitions = 1 ;
     static HWAVEIN      hWaveIn ;
     static HWAVEOUT     hWaveOut ;
     static PBYTE        pBuffer1, pBuffer2, pNewBuffer;
     static PWAVEHDR     pWaveHdr1, pWaveHdr2 ;
     static TCHAR        szOpenError[] = TEXT ("Error opening waveform audio!");
     static TCHAR        szMemError [] = TEXT ("Error allocating memory!") ;
     
     switch (message)
     {
     case WM_INITDIALOG:
               // Allocate memory for wave header
          
          pWaveHdr1 = malloc (sizeof (WAVEHDR)) ;
          pWaveHdr2 = malloc (sizeof (WAVEHDR)) ;
          
		  wndHwnd = hwnd;
		  
               // Allocate memory for save buffer
          
          pSaveBuffer = malloc(1) ;

          return TRUE ;
          
	 //case WM_SHOWWINDOW:
  		 //break;

     case WM_COMMAND:
          switch (LOWORD (wParam))
          {
          case IDC_RECORD_BEG:
                    // Allocate buffer memory

               pBuffer1 = malloc (INP_BUFFER_SIZE) ;
               pBuffer2 = malloc (INP_BUFFER_SIZE) ;
               
               if (!pBuffer1 || !pBuffer2)
               {
                    if (pBuffer1) free (pBuffer1) ;
                    if (pBuffer2) free (pBuffer2) ;

                    MessageBeep (MB_ICONEXCLAMATION) ;
                    MessageBox (hwnd, szMemError, szAppName,
                                      MB_ICONEXCLAMATION | MB_OK) ;
                    return TRUE ;
               }            
                    // Open waveform audio for input
			   waveform.wFormatTag = WAVE_FORMAT_PCM;
			   waveform.nChannels = 1;
			   waveform.nBlockAlign = (waveform.nChannels * waveform.wBitsPerSample / 8);
			   waveform.nAvgBytesPerSec = waveform.nBlockAlign * waveform.nSamplesPerSec;
			   waveform.cbSize = 0;
			   
               if (waveInOpen (&hWaveIn, WAVE_MAPPER, &waveform, 
                               (DWORD) hwnd, 0, CALLBACK_WINDOW))
               {
                    free (pBuffer1) ;
                    free (pBuffer2) ;
                    MessageBeep (MB_ICONEXCLAMATION) ;
                    MessageBox (hwnd, szOpenError, szAppName,
                                      MB_ICONEXCLAMATION | MB_OK) ;
               }
                    // Set up headers and prepare them
               
               pWaveHdr1->lpData          = pBuffer1 ;
               pWaveHdr1->dwBufferLength  = INP_BUFFER_SIZE ;
               pWaveHdr1->dwBytesRecorded = 0 ;
               pWaveHdr1->dwUser          = 0 ;
               pWaveHdr1->dwFlags         = 0 ;
               pWaveHdr1->dwLoops         = 1 ;
               pWaveHdr1->lpNext          = NULL ;
               pWaveHdr1->reserved        = 0 ;

               waveInPrepareHeader (hWaveIn, pWaveHdr1, sizeof (WAVEHDR)) ;
          
               pWaveHdr2->lpData          = pBuffer2 ;
               pWaveHdr2->dwBufferLength  = INP_BUFFER_SIZE ;
               pWaveHdr2->dwBytesRecorded = 0 ;
               pWaveHdr2->dwUser          = 0 ;
               pWaveHdr2->dwFlags         = 0 ;
               pWaveHdr2->dwLoops         = 1 ;
               pWaveHdr2->lpNext          = NULL ;
               pWaveHdr2->reserved        = 0 ;
          
               waveInPrepareHeader (hWaveIn, pWaveHdr2, sizeof (WAVEHDR)) ;
               return TRUE ;
               
          case IDC_RECORD_END:
                    // Reset input to return last buffer
               
               bEnding = TRUE ;
               waveInReset (hWaveIn) ;
               return TRUE ;
               
          case IDC_PLAY_BEG:
                    // Open waveform audio for output
               
               waveform.wFormatTag      = WAVE_FORMAT_PCM ;
               waveform.nChannels       = 1 ;
			   waveform.nBlockAlign     = (waveform.nChannels * waveform.wBitsPerSample / 8);
               waveform.nAvgBytesPerSec = waveform.nBlockAlign * waveform.nSamplesPerSec;
               waveform.cbSize          = 0 ;
               
               if (waveOutOpen (&hWaveOut, WAVE_MAPPER, &waveform, 
                                (DWORD) hwnd, 0, CALLBACK_WINDOW))
               {
                    MessageBeep (MB_ICONEXCLAMATION) ;
                    MessageBox (hwnd, szOpenError, szAppName,
                         MB_ICONEXCLAMATION | MB_OK) ;
               }
               return TRUE ;
               
          case IDC_PLAY_PAUSE:
                    // Pause or restart output
               
               if (!bPaused)
               {
                    waveOutPause (hWaveOut) ;
                    SetDlgItemText (hwnd, IDC_PLAY_PAUSE, TEXT ("Resume")) ;
                    bPaused = TRUE ;
               }
               else
               {
                    waveOutRestart (hWaveOut) ;
                    SetDlgItemText (hwnd, IDC_PLAY_PAUSE, TEXT ("Pause")) ;
                    bPaused = FALSE ;
               }
               return TRUE ;
               
          case IDC_PLAY_END:
                    // Reset output for close preparation
               
               bEnding = TRUE ;
               waveOutReset (hWaveOut) ;
               return TRUE ;
               
          case IDC_PLAY_REV:
                    // Reverse save buffer and play
               
               bReverse = TRUE ;
               ReverseMemory (pSaveBuffer, dwDataLength) ;
               
               SendMessage (hwnd, WM_COMMAND, IDC_PLAY_BEG, 0) ;
               return TRUE ;
               
          case IDC_PLAY_REP:
                    // Set infinite repetitions and play
               
               dwRepetitions = -1 ;
               SendMessage (hwnd, WM_COMMAND, IDC_PLAY_BEG, 0) ;
               return TRUE ;
               
          case IDC_PLAY_SPEED:
                    // Open waveform audio for fast output
               
               waveform.wFormatTag      = WAVE_FORMAT_PCM ;
               waveform.nChannels       = 1 ;
               waveform.nSamplesPerSec  = 22050 ;
               waveform.nAvgBytesPerSec = 44100 ;
               waveform.nBlockAlign     = 2 ;
               waveform.wBitsPerSample  = 16 ;
               waveform.cbSize          = 0 ;
               
               if (waveOutOpen (&hWaveOut, 0, &waveform, (DWORD) hwnd, 0,
                                           CALLBACK_WINDOW))
               {
                    MessageBeep (MB_ICONEXCLAMATION) ;
                    MessageBox (hwnd, szOpenError, szAppName,
                                      MB_ICONEXCLAMATION | MB_OK) ;
               }
               return TRUE ;
          }
          break ;
               
     case MM_WIM_OPEN:
               // Shrink down the save buffer
          
          pSaveBuffer = realloc(pSaveBuffer, 1) ;

               // Add the buffers
          
          waveInAddBuffer (hWaveIn, pWaveHdr1, sizeof (WAVEHDR)) ;
          waveInAddBuffer (hWaveIn, pWaveHdr2, sizeof (WAVEHDR)) ;
          
               // Begin sampling
          
          bRecording = TRUE ;
          bEnding = FALSE ;
          dwDataLength = 0 ;
          waveInStart (hWaveIn) ;
          return TRUE ;
          
     case MM_WIM_DATA:
         
               // Reallocate save buffer memory
          
          pNewBuffer = realloc(pSaveBuffer, dwDataLength +
                                   ((PWAVEHDR) lParam)->dwBytesRecorded) ;
          
          if (pNewBuffer == NULL)
          {
               waveInClose (hWaveIn) ;
               MessageBeep (MB_ICONEXCLAMATION) ;
               MessageBox (hwnd, szMemError, szAppName,
                                 MB_ICONEXCLAMATION | MB_OK) ;
               return TRUE ;
          }
          
          pSaveBuffer = pNewBuffer ;
          CopyMemory (pSaveBuffer + dwDataLength, ((PWAVEHDR) lParam)->lpData,
                         ((PWAVEHDR) lParam)->dwBytesRecorded) ;
          
          dwDataLength += ((PWAVEHDR) lParam)->dwBytesRecorded ;
          
          if (bEnding)
          {
               waveInClose (hWaveIn) ;
               return TRUE ;
          }
          
               // Send out a new buffer
          
          waveInAddBuffer (hWaveIn, (PWAVEHDR) lParam, sizeof (WAVEHDR)) ;
          return TRUE ;
          
     case MM_WIM_CLOSE:
               // Free the buffer memory

          waveInUnprepareHeader (hWaveIn, pWaveHdr1, sizeof (WAVEHDR)) ;
          waveInUnprepareHeader (hWaveIn, pWaveHdr2, sizeof (WAVEHDR)) ;

          free (pBuffer1) ;
          free (pBuffer2) ;

          bRecording = FALSE ;
          
          if (bTerminating)
               SendMessage (hwnd, WM_SYSCOMMAND, SC_CLOSE, 0L) ;
          
          return TRUE ;
          
     case MM_WOM_OPEN:
          
               // Set up header
          
          pWaveHdr1->lpData          = pSaveBuffer ;
          pWaveHdr1->dwBufferLength  = dwDataLength ;
          pWaveHdr1->dwBytesRecorded = 0 ;
          pWaveHdr1->dwUser          = 0 ;
          pWaveHdr1->dwFlags         = WHDR_BEGINLOOP | WHDR_ENDLOOP ;
          pWaveHdr1->dwLoops         = dwRepetitions ;
          pWaveHdr1->lpNext          = NULL ;
          pWaveHdr1->reserved        = 0 ;
          
               // Prepare and write
          
          waveOutPrepareHeader (hWaveOut, pWaveHdr1, sizeof (WAVEHDR)) ;
          waveOutWrite (hWaveOut, pWaveHdr1, sizeof (WAVEHDR)) ;
          
          bEnding = FALSE ;
          bPlaying = TRUE ;
          return TRUE ;
          
     case MM_WOM_DONE:
          waveOutUnprepareHeader (hWaveOut, pWaveHdr1, sizeof (WAVEHDR)) ;
          waveOutClose (hWaveOut) ;
          return TRUE ;
          
     case MM_WOM_CLOSE:
          
          SetDlgItemText (hwnd, IDC_PLAY_PAUSE, TEXT ("Pause")) ;
          bPaused = FALSE ;
          dwRepetitions = 1 ;
          bPlaying = FALSE ;
          
          if (bReverse)
          {
               ReverseMemory (pSaveBuffer, dwDataLength) ;
               bReverse = FALSE ;
          }
          
          if (bTerminating)
               SendMessage (hwnd, WM_SYSCOMMAND, SC_CLOSE, 0L) ;
          
          return TRUE ;
          
     case WM_SYSCOMMAND:
          switch (LOWORD (wParam))
          {
          case SC_CLOSE:
               if (bRecording)
               {
                    bTerminating = TRUE ;
                    bEnding = TRUE ;
                    waveInReset (hWaveIn) ;
                    return TRUE ;
               }
               
               if (bPlaying)
               {
                    bTerminating = TRUE ;
                    bEnding = TRUE ;
                    waveOutReset (hWaveOut) ;
                    return TRUE ;
               }
               
               free (pWaveHdr1) ;
               free (pWaveHdr2) ;
               free(pSaveBuffer) ;
			   free(pNewBuffer) ;
               EndDialog (hwnd, 0) ;
               return TRUE ;
          }
          break ;
     }
     return FALSE ;
}