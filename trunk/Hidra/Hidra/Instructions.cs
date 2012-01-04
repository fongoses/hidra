﻿using System.Collections.Generic;

namespace Hidra
{
    public class Instructions
    {
        Dictionary<int, string> instructions;

        public Instructions()
        {
            this.instructions = new Dictionary<int,string>();
        }

        public void initNeanderInstructions()
        {
            this.instructions.Add(0, "NOP");
            this.instructions.Add(16, "STA");
            this.instructions.Add(32, "LDA");
            this.instructions.Add(48, "ADD");
            this.instructions.Add(64, "OR");
            this.instructions.Add(80, "AND");
            this.instructions.Add(96, "NOT");
            this.instructions.Add(128, "JMP");
            this.instructions.Add(144, "JN");
            this.instructions.Add(160, "JZ");
            this.instructions.Add(240, "HLT");
        }

        public void initAhmesInstructions()
        {
            this.initNeanderInstructions();

            this.instructions.Add(112, "SUB");
            this.instructions.Add(148, "JP");
            this.instructions.Add(152, "JV");
            this.instructions.Add(156, "JNV");
            this.instructions.Add(176, "JC");
            this.instructions.Add(180, "JNC");
            this.instructions.Add(184, "JB");
            this.instructions.Add(188, "JNB");
            this.instructions.Add(224, "SHR");
            this.instructions.Add(225, "SHL");
            this.instructions.Add(226, "ROR");
            this.instructions.Add(227, "ROL");
        }

        public string getInstructionCode(int value)
        {
            if (this.instructions.ContainsKey(value))
                return this.instructions[value];
            else
                return value.ToString();
        }
    }
}
