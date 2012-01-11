﻿using System;
using System.Windows.Forms;

namespace Hidra
{
    public partial class Neander : MainWindow
    {
        Simulators.Neander Neand = new Simulators.Neander();
        
        public Neander()
        {
            InitializeComponent();
            this.instructions.initNeanderInstructions();
            atualizaVariaveis();
        }

        override public void atualizaTela()
        {
            txt_ac.Text = ac.ToString();
            txt_pc.Text = pc.ToString();
            txt_acessos.Text = numeroAcessos.ToString();
            txt_instrucoes.Text = numeroInstrucoes.ToString();
            if (negative == 1)
                picture_neg.Image = Properties.Resources.luz_acessa;
            else if (negative == 0)
                picture_neg.Image = Properties.Resources.luz_apagada;
            if (zero == 1)
                picture_zero.Image = Properties.Resources.luz_acessa;
            else if (zero == 0)
                picture_zero.Image = Properties.Resources.luz_apagada;
        }

        override public void decodificaInstrucao()
        {
            endereco = byte.Parse(gridData.Rows[pc].Cells[1].Value.ToString());

            numeroInstrucoes++;
            numeroAcessos += 3;

            switch (inst)
            {
                case 0:   //NOP;
                    Neand.Nop();
                    numeroAcessos -= 2;
                    break;
                case 16:  //STA;
                    Neand.Store(this.ac, endereco, ref this.memoria);
                    atualizaPC();
                    break;
                case 32:  //LDA;
                    Neand.Load(ref this.ac, endereco, memoria);
                    atualizaPC();
                    break;
                case 48:  //ADD;
                    Neand.Add(ref this.ac, endereco, this.memoria);
                    atualizaPC();
                    break;
                case 64:  //OR;
                    Neand.Or(ref this.ac, endereco, this.memoria);
                    atualizaPC();
                    break;
                case 80:  //AND;
                    Neand.And(ref this.ac, endereco, this.memoria);
                    atualizaPC();
                    break;
                case 96:  //NOT;
                    this.ac = (byte)Neand.Not(this.ac);
                    numeroAcessos -= 2;
                    break;
                case 128: //JMP;
                    this.pc = Neand.Jump((byte)endereco);
                    numeroAcessos -= 1;
                    break;
                case 144: //JN;
                    this.pc = (byte)Neand.JumpOnNegative(this.pc, this.negative, endereco);
                    numeroAcessos -= 1;
                    break;
                case 160: //JZ;
                    this.pc = (byte)Neand.JumpOnZero(this.pc, this.zero, endereco);
                    numeroAcessos -= 1;
                    break;
                case 240: //HLT;
                    this.hlt = Neand.Halt();
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

        private void Neander_Load(object sender, EventArgs e)
        {
            memToGrid();
            this.atualizaTela();
        }  
    }
}
