﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Montador
{
    class Linguagem
    {
        public List<Instrucao> instrucoes;
		public List<Registrador> registradores;
		public List<Enderecamento> enderecamentos;
		public string[] diretivas = { "DAB", "DAW", "DB", "DW", "ORG" };
		
		public enum Tipos { DEFLABEL, INSTRUCAO, DIRETIVA, REGISTRADOR, ENDERECO, ENDERECAMENTO, INVALIDO };

		/*
         * carrega os dados relativos a maquina fornecida
         * TODO: tratar casos em que os arquivos nao existem
         * 
         * arquivos usados (data/maquina/):
         * inst.txt, formato:
         * codigo mnemonico [r...] [end...]
         * ex: 10010000 add r end
         * 
         * end.txt, modos de enderecameto
         * d: direto
         * i: indireto
         * #: imediato
         * x: indexado
         * p: relativo ao PC
         * 
         * reg.txt, registradores, uma palavra por linha, precedida pelo seu codigo
         */
		public void carrega(string maquina)
		{
			Registrador reg = new Registrador();
			Instrucao inst = new Instrucao();
			Gramatica gram = new Gramatica();
			this.instrucoes = new List<Instrucao>();
			this.registradores = new List<Registrador>();
			this.enderecamentos = new List<Enderecamento>();

			string linha;
			int[] formato;
			string[] words;
			string arquivo = "data/" + maquina;
			char[] space = { ' ' };

			//adiciona as instrucoes
			using (StreamReader file = new StreamReader(arquivo + "/inst.txt"))
			{
				while ((linha = file.ReadLine()) != null)
				{
					words = linha.Split(space);

					//determina o formato da instrucao
					formato = new int[words.Length];
					for (int i = 1; i < words.Length; i++)
					{
						if (words[i] == "r")
							formato[i] = (int)Tipos.REGISTRADOR;
						else if (words[i] == "end")
							formato[i] = (int)Tipos.ENDERECO;
						else
							formato[i] = (int)Tipos.INSTRUCAO;
					}
					this.instrucoes.Add(new Instrucao(words[1].ToUpper(), formato, gram.paraInteiro(words[0])));
				}
			}

			//adiciona os registradores
			using (StreamReader file = new StreamReader(arquivo + "/reg.txt"))
			{
				while ((linha = file.ReadLine()) != null)
				{
					words = linha.Split(space);
					this.registradores.Add(new Registrador(words[1].ToUpper(), gram.paraInteiro(words[0])));
				}
			}

			//adiciona os modos de enderecamentos
			using (StreamReader file = new StreamReader(arquivo + "/end.txt"))
			{
				while ((linha = file.ReadLine()) != null)
				{
					words = linha.Split(space);
					this.enderecamentos.Add(new Enderecamento(words[1].ToUpper(), gram.paraInteiro(words[0])));
				}
			}

			this.registradores.Sort(reg.regCompare);
			this.instrucoes.Sort(inst.instCompare);
		}

		/*
		 * verifica se a palavra eh conhecida
		 * retorna um valor de enum Tipos
		 */
		public int identificaTipo(string palavra)
		{
			Enderecamento end = new Enderecamento();

			if (this.registradores.FindIndex(o => o.nome == palavra) >= 0)
				return (int)Tipos.REGISTRADOR;
			if (this.instrucoes.FindIndex(o => o.mnemonico == palavra) >= 0)
				return (int)Tipos.INSTRUCAO;

			if (end.identifica(palavra,this.enderecamentos) >= 0)
				return (int)Tipos.ENDERECAMENTO;

			return (int)Tipos.INVALIDO;
		}
    }
}
