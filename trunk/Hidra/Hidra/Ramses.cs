﻿using System.Windows.Forms;

namespace Hidra
{
    public partial class Ramses : MainWindow
    {
        Simulators.RamsesPericles Rams = new Simulators.RamsesPericles();
        public int carry;
        public byte rA, rB, rX,rN;

        public Ramses()
        {
            InitializeComponent();
            this.instructions.initRamsesInstructions();
            this.carry = 0;
            this.rA = 0;
            this.rB = 0;
            this.rX = 0;
            this.rN = 0;
        }

        override public void atualizaTela()
        {
            txt_ac.Text = ac.ToString();
            txt_pc.Text = pc.ToString();
            txt_ra.Text = rA.ToString();
            txt_rb.Text = rB.ToString();
            txt_rx.Text = rX.ToString();
            txt_acessos.Text = numeroAcessos.ToString();
            txt_instrucoes.Text = numeroInstrucoes.ToString();
            lbl_negative.Text = negative.ToString();
            lbl_zero.Text = zero.ToString();
            lbl_carryout.Text = carry.ToString();
        }

        override public void decodificaInstrucao()
        {
            endereco = byte.Parse(gridData.Rows[pc].Cells[1].Value.ToString());

            numeroInstrucoes++;
            numeroAcessos += 3;

            atualizaPC();
            switch (inst)
            {
                case 0:   //NOP;
                    Rams.Nop();
                    numeroAcessos -= 2;
                    voltaPC();
                    break;
                case 16:  //STA; //A,n
                    Rams.Store(this.rA, endereco, ref this.memoria);                    
                    break;                 
                    case 17: //A,nI
                        Rams.StoreIndirect(this.rA, endereco, ref this.memoria);                   
                        break;
                    case 18: //A,#n
                        Rams.StoreImmediat(this.rA, this.pc, ref this.memoria);
                        break;
                    case 19: //A,nX
                        Rams.StoreIndexed(this.rA, this.rX, this.pc, ref this.memoria);
                        break;
                    case 20: //B,n
                        Rams.Store(this.rB, endereco, ref this.memoria);   
                        break;
                    case 21: //B,nI
                        Rams.StoreIndirect(this.rB, endereco, ref this.memoria); 
                        break;
                    case 22: //B,#n
                        Rams.StoreImmediat(this.rB, this.pc, ref this.memoria);
                        break;
                    case 23: //B,nX
                        Rams.StoreIndexed(this.rB, this.rX, this.pc, ref this.memoria);
                        break;
                    case 24: //X,n
                        Rams.Store(this.rX, endereco, ref this.memoria);   
                        break;
                    case 25: //X,nI
                        Rams.StoreIndirect(this.rX, endereco, ref this.memoria); 
                        break;
                    case 26: //X,#n
                        Rams.StoreImmediat(this.rX, this.pc, ref this.memoria);
                        break;
                    case 27: //X,nX
                        Rams.StoreIndexed(this.rX, this.rX, this.pc, ref this.memoria);
                        break;
                    case 28: //?,n
                        Rams.Store(this.rN, endereco, ref this.memoria);   
                        break;
                    case 29: //?,nI
                        Rams.StoreIndirect(this.rN, endereco, ref this.memoria); 
                        break;
                    case 30: //?,#n
                        Rams.StoreImmediat(this.rN, this.pc, ref this.memoria);
                        break;
                    case 31: //?,nX
                        Rams.StoreIndexed(this.rN, this.rX, this.pc, ref this.memoria);
                        break;

                case 32:  //LDA;                   
                    this.ac = Rams.Load(endereco, memoria);
                    atualizaPC();
                    break;

                    case 33: //A,nI
                        break;
                    case 34: //A,#n
                        break;
                    case 35: //A,nX
                        break;
                    case 36: //B,n
                        break;
                    case 37: //B,nI
                        break;
                    case 38: //B,#n
                        break;
                    case 39: //B,nX
                        break;
                    case 40: //X,n
                        break;
                    case 41: //X,nI
                        break;
                    case 42: //X,#n
                        break;
                    case 43: //X,nX
                        break;
                    case 44: //?,n
                        break;
                    case 45: //?,nI
                        break;
                    case 46: //?,#n
                        break;
                    case 47: //?,nX
                        break;

                case 48:  //ADD;
                //    this.ac = (byte)Rams.Add(this.ac, endereco, this.memoria, out this.carry);
                    atualizaPC();
                    break;

                    case 49: //A,nI
                        break;
                    case 50: //A,#n
                        break;
                    case 51: //A,nX
                        break;
                    case 52: //B,n
                        break;
                    case 53: //B,nI
                        break;
                    case 54: //B,#n
                        break;
                    case 55: //B,nX
                        break;
                    case 56: //X,n
                        break;
                    case 57: //X,nI
                        break;
                    case 58: //X,#n
                        break;
                    case 59: //X,nX
                        break;
                    case 60: //?,n
                        break;
                    case 61: //?,nI
                        break;
                    case 62: //?,#n
                        break;
                    case 63: //?,nX
                        break;

                case 64:  //OR;
                    this.ac = (byte)Rams.Or(this.ac, endereco, this.memoria);
                    atualizaPC();
                    break;
                    
                    case 65: //A,nI
                        break;
                    case 66: //A,#n
                        break;
                    case 67: //A,nX
                        break;
                    case 68: //B,n
                        break;
                    case 69: //B,nI
                        break;
                    case 70: //B,#n
                        break;
                    case 71: //B,nX
                        break;
                    case 72: //X,n
                        break;
                    case 73: //X,nI
                        break;
                    case 74: //X,#n
                        break;
                    case 75: //X,nX
                        break;
                    case 76: //?,n
                        break;
                    case 77: //?,nI
                        break;
                    case 78: //?,#n
                        break;
                    case 79: //?,nX
                        break;

                case 80:  //AND;
                    this.ac = (byte)Rams.And(this.ac, endereco, this.memoria);
                    atualizaPC();
                    break;

                    case 81: //A,nI
                        break;
                    case 82: //A,#n
                        break;
                    case 83: //A,nX
                        break;
                    case 84: //B,n
                        break;
                    case 85: //B,nI
                        break;
                    case 86: //B,#n
                        break;
                    case 87: //B,nX
                        break;
                    case 88: //X,n
                        break;
                    case 89: //X,nI
                        break;
                    case 90: //X,#n
                        break;
                    case 91: //X,nX
                        break;
                    case 92: //?,n
                        break;
                    case 93: //?,nI
                        break;
                    case 94: //?,#n
                        break;
                    case 95: //?,nX
                        break;

                case 96:  //NOT A
                    this.ac = (byte)Rams.Not(this.ac);
                    numeroAcessos -= 2;
                    break;

                    case 100: //B
                        break;
                    case 104: //X
                        break;
                    case 108: //?
                        break;

                case 112: //SUB
                  //  this.ac = (byte)Rams.Subtract(this.ac, endereco, this.memoria, out borrow);
                    atualizaPC();
                    break;

                    case 113: //A,nI
                        break;
                    case 114: //A,#n
                        break;
                    case 115: //A,nX
                        break;
                    case 116: //B,n
                        break;
                    case 117: //B,nI
                        break;
                    case 118: //B,#n
                        break;
                    case 119: //B,nX
                        break;
                    case 120: //X,n
                        break;
                    case 121: //X,nI
                        break;
                    case 122: //X,#n
                        break;
                    case 123: //X,nX
                        break;
                    case 124: //?,n
                        break;
                    case 125: //?,nI
                        break;
                    case 126: //?,#n
                        break;
                    case 127: //?,nX
                        break;

                case 128: //JMP,n
                    this.pc = Rams.Jump((byte)endereco);
                    numeroAcessos -= 1;
                    break;

                    case 129: //A,nI
                        break;
                    case 130: //A,#n
                        break;
                    case 131: //A,nX
                        break;

                case 144: //JN,n
                    this.pc = Rams.JumpOnNegative(this.pc, this.negative, endereco);
                    numeroAcessos -= 1;
                    break;

                    case 145: //A,nI
                        break;
                    case 146: //A,#n
                        break;
                    case 147: //A,nX
                        break;

                case 160: //JZ,n
                    this.pc = (byte)Rams.JumpOnZero(this.pc, this.zero, endereco);
                    numeroAcessos -= 1;
                    break;

                    case 161: //A,nI
                        break;
                    case 162: //A,#n
                        break;
                    case 163: //A,nX
                        break;

                case 176: //JC,n
                    this.pc = Rams.JumpOnCarry(this.pc, this.carry, endereco);
                    numeroAcessos -= 1;
                    break;

                    case 177: //A,nI
                        break;
                    case 178: //A,#n
                        break;
                    case 179: //A,nX
                        break;

                case 192: //JSR,n
                    break;
                case 193: //nI
                    break;
                case 194: //#n
                    break;
                case 195: //nX
                    break;

                case 208: //NEG A
                    break;

                    case 212: //B
                        break;
                    case 216: //X
                        break;
                    case 220: //?
                        break;

                case 224: //SHR
                    this.ac = (byte)Rams.ShiftRight(this.ac, out carry);
                    numeroAcessos -= 2;
                    break;

                    case 228: //B
                        break;
                    case 232: //X
                        break;
                    case 236: //?
                        break;

                case 240: //HLT;
                    this.hlt = Rams.Halt();
                    numeroAcessos -= 2;
                    break;

                //qualquer numero diferente dos de cima
                default:
                    numeroAcessos -= 2;
                    break;
            }

            memToGrid();
            atualizaVariaveis();
            this.atualizaTela();
        }

        private void Ramses_Load(object sender, System.EventArgs e)
        {
            memToGrid();
            this.atualizaTela();
        }
    }
}
