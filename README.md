 # ElfBot
 ElfBot is a client to enable automation of actions in ROSE Online.
 
## AOBs
### Active Character
```
trose.exe+1AECE3 - 48 8B 0D D624F100 - mov rcx,[trose.exe+10C11C0]
trose.exe+1B0C80 - 48 8B 0D 3905F100 - mov rcx,[trose.exe+10C11C0]
trose.exe+1B0D68 - 48 8B 0D 5104F100 - mov rcx,[trose.exe+10C11C0]
trose.exe+132D73 - 48 8B 0D 46E4F800 - mov rcx,[trose.exe+10C11C0]
trose.exe+1B0633 - 48 8B 0D 860BF100 - mov rcx,[trose.exe+10C11C0]
```

### Entity List
```
trose.exe+21B20F - 48 8B 0D 7A8EEA00 - mov rcx,[trose.exe+10C4090]
trose.exe+21B2B0 - 4C 8B 0D D98DEA00 - mov r9,[trose.exe+10C4090]
trose.exe+E57D8  - 48 8B 05 B1E8FD00 - mov rax,[trose.exe+10C4090]
trose.exe+21B5FE - 48 8B 0D 8B8AEA00 - mov rcx,[trose.exe+10C4090]
trose.exe+E5930  - 48 8B 1D 59E7FD00 - mov rbx,[trose.exe+10C4090]
```

### Current Target Entity
```
trose.exe+1AEBEC - 48 8D 0D FD48F100 - lea rcx,[trose.exe+10C34F0]
trose.exe+DB859  - 48 8D 05 907CFE00 - lea rax,[trose.exe+10C34F0]
```

### Party
```
trose.exe+166B19 - 48 8D 05 7005F600     - lea rax,[trose.exe+10C7090]
trose.exe+166B3B - 48 8D 1D 4E05F600     - lea rbx,[trose.exe+10C7090]
```

### NoClip
```
{
// ORIGINAL CODE - INJECTION POINT: trose.exe+B5430

trose.exe+B5426: CC                       - int 3 
trose.exe+B5427: CC                       - int 3 
trose.exe+B5428: CC                       - int 3 
trose.exe+B5429: CC                       - int 3 
trose.exe+B542A: CC                       - int 3 
trose.exe+B542B: CC                       - int 3 
trose.exe+B542C: CC                       - int 3 
trose.exe+B542D: CC                       - int 3 
trose.exe+B542E: CC                       - int 3 
trose.exe+B542F: CC                       - int 3 
// ---------- INJECTING HERE ----------
trose.exe+B5430: 40 57                    - push rdi
// ---------- DONE INJECTING  ----------
trose.exe+B5432: 48 83 EC 20              - sub rsp,20
trose.exe+B5436: 48 8B F9                 - mov rdi,rcx
trose.exe+B5439: 0F 57 C0                 - xorps xmm0,xmm0
trose.exe+B543C: 48 8B 49 48              - mov rcx,[rcx+48]
trose.exe+B5440: F3 0F 10 89 84 00 00 00  - movss xmm1,[rcx+00000084]
trose.exe+B5448: 0F 2E C8                 - ucomiss xmm1,xmm0
trose.exe+B544B: 7A 02                    - jp trose.exe+B544F
trose.exe+B544D: 74 56                    - je trose.exe+B54A5
trose.exe+B544F: 8B 47 58                 - mov eax,[rdi+58]
trose.exe+B5452: F2 0F 10 47 50           - movsd xmm0,[rdi+50]
}
```