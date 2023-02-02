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

### Current Target Entity
```
{
// ORIGINAL CODE - INJECTION POINT: trose.exe+1AF7FC

trose.exe+1AF7C6: 83 F8 09                    - cmp eax,09
trose.exe+1AF7C9: 0F 85 AA 01 00 00           - jne trose.exe+1AF979
trose.exe+1AF7CF: 8B 0D 4B A5 F3 00           - mov ecx,[trose.exe+10E9D20]
trose.exe+1AF7D5: 65 48 8B 04 25 58 00 00 00  - mov rax,gs:[00000058]
trose.exe+1AF7DE: 41 B8 24 01 00 00           - mov r8d,00000124
trose.exe+1AF7E4: 48 8B 14 C8                 - mov rdx,[rax+rcx*8]
trose.exe+1AF7E8: 41 8B 04 10                 - mov eax,[r8+rdx]
trose.exe+1AF7EC: 39 05 22 8D F1 00           - cmp [trose.exe+10C8514],eax
trose.exe+1AF7F2: 0F 8F A0 01 00 00           - jg trose.exe+1AF998
trose.exe+1AF7F8: 0F BF 5F 1C                 - movsx ebx,word ptr [rdi+1C]
// ---------- INJECTING HERE ----------
trose.exe+1AF7FC: 48 8D 0D FD 8C F1 00        - lea rcx,[trose.exe+10C8500]
// ---------- DONE INJECTING  ----------
trose.exe+1AF803: E8 FE 67 E9 FF              - call trose.exe+46006
trose.exe+1AF808: 3B C3                       - cmp eax,ebx
trose.exe+1AF80A: 0F 94 C3                    - sete bl
trose.exe+1AF80D: 48 8B 07                    - mov rax,[rdi]
trose.exe+1AF810: 48 8B CF                    - mov rcx,rdi
trose.exe+1AF813: FF 50 40                    - call qword ptr [rax+40]
trose.exe+1AF816: 83 E8 06                    - sub eax,06
trose.exe+1AF819: 0F 84 24 01 00 00           - je trose.exe+1AF943
trose.exe+1AF81F: 83 E8 01                    - sub eax,01
trose.exe+1AF822: 74 72                       - je trose.exe+1AF896
}
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

### Game Object ID Conversion Map
```
{
// ORIGINAL CODE - INJECTION POINT: trose.exe+237E30

trose.exe+237E0F: CC                    - int 3 
trose.exe+237E10: 48 89 5C 24 10        - mov [rsp+10],rbx
trose.exe+237E15: 48 89 6C 24 18        - mov [rsp+18],rbp
trose.exe+237E1A: 56                    - push rsi
trose.exe+237E1B: 57                    - push rdi
trose.exe+237E1C: 41 56                 - push r14
trose.exe+237E1E: 48 83 EC 50           - sub rsp,50
trose.exe+237E22: 4C 8B F1              - mov r14,rcx
trose.exe+237E25: 0F B7 41 08           - movzx eax,word ptr [rcx+08]
trose.exe+237E29: 48 8B 0D 60 C2 E8 00  - mov rcx,[trose.exe+10C4090] // Entity map address
// ---------- INJECTING HERE ----------
trose.exe+237E30: 0F BF 54 41 0C        - movsx edx,word ptr [rcx+rax*2+0C]
// ---------- DONE INJECTING  ----------
trose.exe+237E35: 45 33 C0              - xor r8d,r8d
trose.exe+237E38: E8 41 0D DE FF        - call trose.exe+18B7E
trose.exe+237E3D: 48 8B D8              - mov rbx,rax
trose.exe+237E40: 49 8D 4E 50           - lea rcx,[r14+50]
trose.exe+237E44: E8 1E 77 E0 FF        - call trose.exe+3F567
trose.exe+237E49: 48 85 DB              - test rbx,rbx
trose.exe+237E4C: 0F 84 AB 01 00 00     - je trose.exe+237FFD
trose.exe+237E52: 48 8B 13              - mov rdx,[rbx]
trose.exe+237E55: 48 8B CB              - mov rcx,rbx
trose.exe+237E58: FF 92 E0 03 00 00     - call qword ptr [rdx+000003E0]
}
```

```
{
// ORIGINAL CODE - INJECTION POINT: trose.exe+21B605

trose.exe+21B5D9: 48 83 C0 F8              - add rax,-08
trose.exe+21B5DD: 48 83 F8 1F              - cmp rax,1F
trose.exe+21B5E1: 0F 87 5C 01 00 00        - ja trose.exe+21B743
trose.exe+21B5E7: 48 8B CF                 - mov rcx,rdi
trose.exe+21B5EA: E8 36 59 E2 FF           - call trose.exe+40F25
trose.exe+21B5EF: 45 84 E4                 - test r12l,r12l
trose.exe+21B5F2: 75 57                    - jne trose.exe+21B64B
trose.exe+21B5F4: E8 4F 06 E0 FF           - call trose.exe+1BC48
trose.exe+21B5F9: 48 0F BF 56 1C           - movsx rdx,word ptr [rsi+1C]
trose.exe+21B5FE: 48 8B 0D 8B 8A EA 00     - mov rcx,[trose.exe+10C4090] // Entity map address
// ---------- INJECTING HERE ----------
trose.exe+21B605: 0F B7 94 51 0A 00 02 00  - movzx edx,word ptr [rcx+rdx*2+0002000A]
// ---------- DONE INJECTING  ----------
trose.exe+21B60D: 48 8B C8                 - mov rcx,rax
trose.exe+21B610: E8 94 5C E2 FF           - call trose.exe+412A9
trose.exe+21B615: 84 C0                    - test al,al
trose.exe+21B617: 75 32                    - jne trose.exe+21B64B
trose.exe+21B619: 4C 8B 74 24 38           - mov r14,[rsp+38]
trose.exe+21B61E: 49 8B 06                 - mov rax,[r14]
trose.exe+21B621: 4C 8B 50 30              - mov r10,[rax+30]
trose.exe+21B625: F3 45 0F 2C C2           - cvttss2si r8d,xmm10
trose.exe+21B62A: F3 41 0F 2C D1           - cvttss2si edx,xmm9
trose.exe+21B62F: 41 8B 85 0C 02 00 00     - mov eax,[r13+0000020C]
}
```