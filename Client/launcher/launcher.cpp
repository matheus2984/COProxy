// launcher.cpp : Defines the entry point for the application.
//

#include "stdafx.h"
#include "launcher.h"
#include <string>
#pragma comment(lib, "user32.lib")

char dllPath[MAX_PATH]="csv3hook.dll";
char coPath[MAX_PATH]="Conquer.exe blacknull";

//
// This is not my function, I forgot where I got it from.
// I used it in another one of my projects.
// Personally, I think it looks disgusting, but it works, so w/e
//
bool InjectDLL(HANDLE hProcess, char* strDLLName)
{
            // Length of string containing the DLL file name +1 byte padding
            DWORD LenWrite = strlen(strDLLName) + 1;
            // Allocate memory within the virtual address space of the target process
            PVOID AllocMem = VirtualAllocEx(hProcess, NULL, LenWrite, 0x1000, 0x40); //allocation pour WriteProcessMemory

            // Write DLL file name to allocated memory in target process
            WriteProcessMemory(hProcess, AllocMem, strDLLName, LenWrite, NULL);
            // Function pointer "Injector"
            PVOID Injector = GetProcAddress(GetModuleHandleA("kernel32.dll"), "LoadLibraryA");

            if (Injector == NULL)
                return false;

            // Create thread in target process, and store handle in hThread
            HANDLE hThread = CreateRemoteThread(hProcess, NULL, 0, (LPTHREAD_START_ROUTINE)Injector, AllocMem, 0, NULL);
            // Make sure thread handle is valid
            if (hThread == NULL)
                return false;

            // Time-out is 10 seconds...
            int Result = WaitForSingleObject(hThread, 10 * 1000);
            // Check whether thread timed out...
            if (Result == 0x00000080L || Result == 0x00000102L || Result == 0xFFFFFFFF)
            {
                /* Thread timed out... */
                // Make sure thread handle is valid before closing... prevents crashes.
                if (hThread != NULL)
                {
                    //Close thread in target process
                    CloseHandle(hThread);
                }
                return false;
            }
            // Sleep thread for 1 second
            Sleep(1000);
            // Make sure thread handle is valid before closing... prevents crashes.
            if (hThread != NULL)
            {
                //Close thread in target process
                CloseHandle(hThread);
            }
            // return succeeded
            return true;
}

int APIENTRY _tWinMain(HINSTANCE hInstance,
                     HINSTANCE hPrevInstance,
                     LPTSTR    lpCmdLine,
                     int       nCmdShow)
{
	UNREFERENCED_PARAMETER(hPrevInstance);
	UNREFERENCED_PARAMETER(lpCmdLine);

 	STARTUPINFOA si;
    PROCESS_INFORMATION pi;
    memset( &si, 0, sizeof(si));
    si.cb = sizeof(si);
    memset( &pi, 0, sizeof(pi));
	std::string path(coPath);
	std::size_t ls=path.find_last_of("\\");
	if(ls!=std::string::npos) path.assign(path.substr(0,ls));
	char pathc[MAX_PATH];
	memcpy(pathc, path.c_str(), path.size()+1);
	if (!CreateProcessA(NULL, coPath, NULL, NULL, FALSE, 0, NULL, ls==std::string::npos?0:pathc, &si, &pi)) 
	{
		MessageBoxA(NULL, "Failed To Start Conquer.exe", "ERROR", MB_OK); // print error
		return 0; // abort
	}

	void *dllString;
	void *stub;
	unsigned long stubLen, oldIP, oldprot, loadLibAddy;
	::InjectDLL(pi.hProcess, "csv3hook.dll");
}