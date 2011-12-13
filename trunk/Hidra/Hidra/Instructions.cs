﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hidra
{
    public enum Instructions
    {
        HLT,NOP,STR,LDR,STA,LDA,OR,AND,NOT,ADD,SUB,NEG,CLR,INC,DEC,IF,JMP,JN,JP,JV,JNV,JZ,JNZ,JC,JNC,JD,
        JB,JNB,JSR,SHR,SHL,ROR,ROL,ASR,ASL,SZ,SNZ,SPL,SMI,SPZ,SMZ,SEQ,SNE,SGR,SLS,SGE,SLE,RTS,PSH,POP
    }
}
