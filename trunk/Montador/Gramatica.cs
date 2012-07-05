﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Montador
{
    public class Gramatica
    {

		/**
		 * independente da maquina, cada linha do codigo fonte pode ser:
		 * 
		 * inst <operandos...>
		 * diretiva <operandos...>
		 * 
		 * precedida ou nao por uma definicao de label
		 * um endereco pode ser um numero ou uma palavra, seguido de um modo de enderecamento
		 * uma label eh uma palavra seguida de ':'
		 * 
		 */

        public string[] inst;
        public enum Tipos { DEFLABEL, INSTRUCAO, DIRETIVA, REGISTRADOR, ENDERECO, INVALIDO };
		public enum SubTipos { NONE,STRING, LABEL, NUMERO };

		public Linguagem linguagem = new Linguagem();

		/**
		 * retorna o valor inteiro de um digito hexadecimal
		 * ou -1 se nao foi hexadecimal
		 */
		public int getHexValue(Char d)
		{
			int v = (int)Char.GetNumericValue(d);
			if (v > 0)
				return v;

			switch (d)
			{
				case 'A':
				case 'a':
					return 10;
				case 'B':
				case 'b':
					return 11;
				case 'C':
				case 'c':
					return 12;
				case 'D':
				case 'd':
					return 13;
				case 'E':
				case 'e':
					return 14;
				case 'F':
				case 'f':
					return 15;
				default:
					return -1;
			}
				
		}
		/**
		 * converte um numero para um array de bytes onde a primeria posição é o byte menos significativo
		 * o array tera exatamente o tamanho especificado
		 */
		public byte[] num2byteArray(int num, int tamanho, ref Codigo.Estado estado)
		{

			//byte[] array = new byte[(int)(Math.Log(num, 2) / 8)+1];
			byte[] array = new byte[tamanho];
			byte mask = 255;

			if (tamanho >= ((int)(Math.Log(num, 2) / 8) + 1))
				estado = Codigo.Estado.OK;
			else
				estado = Codigo.Estado.TRUNCADO;

			for (int i = 0; i < tamanho; i++)
			{
				//pega o byte menos significativo do numero
				array[i] = (byte)(mask & num);
				num >>= 8;
			}

			return array;

		}

		/**
		 * converte uma string de uma string para uma string
		 * ex:
		 * "\"abc\"" -> "abc"
		 *
		public string rawString2string(string raw)
		{
			char final = raw[0];
			char[] str = new char[raw.Length];
			bool go = true;
			int i = 1;
			int p = 0;
			while (go)
			{

			}
		}*/

		/**
		 * converte uma string para um array de bytes
		 * tamanho eh o numero de bytes que cada caractere ocupara
		 * usa little-endian
		 */
		public byte[] string2byteArray(string s, int tamanho, ref Codigo.Estado estado)
		{
			byte[] vetor = new byte[s.Length*tamanho];
			int i;

			for (i = 0; i < s.Length; i+=tamanho)
			{
				vetor[i] = (byte)s[i];
				for (int k = 1; k < tamanho; k++)
					vetor[i + k] = 0;
			}

			return vetor;
		}

		/*
		 * recebe um string e converte para um inteiro
		 * numberos decimais terminam por 'd',
		 * hexadecimais terminam por 'h',
		 * números binarios podem terminar por 'b', mas nao precisam
		 */
		public int paraInteiro(string numero)
		{
			if (numero == null)
				return 0;
			return this.paraInteiro(numero, 0, numero.Length - 1);
		}
		public int paraInteiro(string numero,int begin, int end)
		{
			if (numero == null)
				return 0;
			if (begin < 0)
				begin = 0;
			if (end < begin)
				return 0;
			//se o numero eh binario, decimal ou hexadecimal
			char tipo = numero[end];
			int num = 0;

			switch (tipo)
			{
				//decimal
				case 'd':
				case 'D':
					for (int i = end-1, p = 1; i >= begin; i--, p *= 10)
					{
						num += (int)(p * Char.GetNumericValue(numero[i]));
					}
					return num;
				case 'h':
				case 'H':
					for (int i = end-1, p = 1; i >= begin; i--, p *= 16)
					{
						num += (int)(p * getHexValue(numero[i]));
					}
					return num;
				case 'b':
					for (int i = end-1, p = 1; i >= begin; i--, p *= 2)
					{
						if(numero[i] == '1')
							num += p;
					}
					return num;
				case '0':
				case '1':
				
					for (int i = end, p = 1; i >= begin; i--, p *= 2)
					{
						if(numero[i] == '1')
							num += p;
					}
					return num;
			}

			return 0;
		}

        /**
         * retorna a primeria subpalavra da string que seja um numero em hexadecimal
         * como nao eh possivel a existencia de 2 numeros na mesma palavra, apenas o primeiro eh necessario
         * se nao existir tal subpalavra, retorna ""
         * formato:
         * 0.(0-9+A-F)*.h
         */
        public string substringHexa(string palavra)
        {
            char[] numero = new char[palavra.Length];
            int i = 0,p = 0;
            bool achou = false;

            //enquanto nao encontrar a substring
            while (!achou && i < palavra.Length)
            {
                //busca um '0'
                for (; i < palavra.Length && palavra[i] != '0'; i++)
                    ;
                //copia os caracteres para o numero
                for (p = 0; i < palavra.Length && ehDigitoHexa(palavra[i]); numero[p] = palavra[i],p++,i++)
                    ;
                //se o proximo caracter for um 'H', o numero foi encontrado
                if (i < palavra.Length)
                    if (palavra[i] == 'H')
                        achou = true;
            }
            
            string n = new string(numero,0,p);

			if (achou)
				return n;
			else
			{
				return "";
			}
        }

        /**
         * converte uma string em hexa para um numero inteiro
         */
        public int hexa2int(string hexa)
        {
            int numero = 0;
            int i;
            double power = 0;
            for (i = hexa.Length - 1; i >= 0; i--,power++)
            {
                numero += (int)(valorHexa(hexa[i]) * Math.Pow(16, power));
            }

            return numero;
        }

        /**
         * retorna o valor inteiro do caractere hexa d
         * d deve estar em [0,f]
         */
        int valorHexa(char d)
        {
            if (d.CompareTo('A') >= 0)
                return 10 + d.CompareTo('A');
            else
                return d.CompareTo('0');
        }

		/*
		 * identifica o subtipo da palavra
		 */
		public int identificaSubTipo(string nome)
		{
			if (ehNumero(nome))
				return (int)SubTipos.NUMERO;
			else if (ehString(nome))
				return (int)SubTipos.STRING;
			else
				return (int)SubTipos.LABEL;
		}

        /*
         * identifica o tipo de uma palavra (olhar enum Tipos)
		 * se for um endereco, escreve em nome o nome desse endereco ou registrador caso nao seja um numero
         */
        public void identificaTipo(ref Linha linha, Gramatica gramatica)
        {
			int i=0;
			string palavra;
			string nome = "";

			for(i=0;i<linha.preprocessado.Length;i++)
			{
				palavra = linha.preprocessado[i];
				int tipo;
				if (ehNumero(palavra))
				{
					linha.tipos[i] =  (int)Tipos.ENDERECO;
					linha.subTipos[i] = (int)SubTipos.NUMERO;
					linha.nomes[i] = palavra;
				}
				else
				{
					byte[] end = new byte[1];
					end[0] = 0;
					//se nao for um numero, verifica se eh alguma palavra conhecida
					tipo = linguagem.identificaTipo(palavra, ref nome, ref end);

					linha.enderecamento.Add(end);

					if (tipo != (int)Tipos.INVALIDO)
					{
						linha.tipos[i] = tipo;
						//se for um endereco, determina o subtipo
						if (tipo == (int)Tipos.ENDERECO)
						{
							linha.subTipos[i] = identificaSubTipo(nome);
							if (linha.subTipos[i] == (int)SubTipos.STRING)
							{
								Stringer str = new Stringer();
								char[] parsedArr = new char[palavra.Length];
								int size = 0;

								str.parse(palavra, 1, parsedArr, ref size);
								linha.nomes[i] = new string(parsedArr);

								Console.WriteLine("Raw:" + palavra);
								Console.WriteLine("Parsed:" + linha.nomes[i]);
							}
							//se for uma string, verifica se existe um modo de enderecamento ao lado
							if (linha.subTipos[i] == (int)SubTipos.STRING && i +1 <linha.preprocessado.Length)
							{
								//somente se a linha contiver uma instrucao sera testado algum modo de enderecamento
								int k = 0;
								if (linha.tipos[k] == (int)Tipos.DEFLABEL)
									k++;
								if (linha.tipos[k] == (int)Tipos.INSTRUCAO)
								{
									string ope = String.Concat(linha.preprocessado[i], linha.preprocessado[i + 1]);
									if(linguagem.identificaTipo(ope,ref nome, ref end) == (int)Tipos.ENDERECO)
									{
										linha.enderecamento[i] = end;
										linha.nomes[i] = nome;
									}
								}
							}
						}
						linha.nomes[i] = nome;
					}
					else
					{
						if (ehLabel(palavra,palavra.Length-1))
						{
							if (palavra[palavra.Length - 1] == ':')
							{
								linha.tipos[i] = (int)Tipos.DEFLABEL;
								linha.nomes[i] = new string(palavra.ToCharArray(),0,palavra.Length-1);
							}
							else
							{
								linha.tipos[i] = (int)Tipos.ENDERECO;
								linha.subTipos[i] = (int)SubTipos.LABEL;
							}
						}
						else if (ehString(palavra))
						{
							linha.tipos[i] = (int)Tipos.ENDERECO;
							linha.subTipos[i] = (int)SubTipos.STRING;

							Stringer str = new Stringer();
							char[] parsedArr = new char[palavra.Length];
							int size = 0;
							
							str.parse(palavra, 1, parsedArr, ref size);
							linha.nomes[i] = new string(parsedArr);

							Console.WriteLine("Raw:" + palavra);
							Console.WriteLine("Parsed:" + linha.nomes[i]);
						}
						else
							linha.tipos[i] = (int)Tipos.INVALIDO;
					}
				}
			}
        }

		/*
		 * verifica se os tipos de uma linha são validos
		 * linhas sao do tipo
		 * 0?.(1).(3+4)* ou
		 * 0?.(2).(4)*
		 * retorna true se forem
		 */
		public bool verificaTipos(Linha linha,Escritor saida,Definicoes defs)
		{
			Instrucao inst;
			int i = 0;
			int j = 1;
			int size = linha.tipos.Length;
			//a linha pode comecar por definicao de label
			if (linha.tipos[0] == (int)Tipos.DEFLABEL)
			{
				defs.adicionaDef(linha.preprocessado[0].Substring(0,linha.preprocessado[0].Length-1),linha.linhaFonte,saida);
				i = 1;
				size--;
			}
			//a linha eh apenas a definicao de uma label
			if (i >= linha.tipos.Length)
				return true;

			if (linha.tipos[i] == (int)Tipos.INSTRUCAO)
			{
				inst = this.linguagem.instrucoes.Find(o => o.mnemonico == linha.preprocessado[i]);
				if(size < inst.formato.Length)
					saida.errorOut(Escritor.ERRO, linha.linhaFonte, "Número incorreto de operandos. Esperava-se " + (inst.formato.Length - 1) + ", encontrou-se " + (size - 1));
				//verifica se ha algo diferente de registradores e enderecos
				for (i++; i < linha.tipos.Length; i++)
				{
					if (j >= inst.formato.Length)
					{
						if(linha.tipos[i-1] != (int)Tipos.ENDERECO)
							saida.errorOut(Escritor.ERRO, linha.linhaFonte, "Número incorreto de operandos. Esperava-se " + (inst.formato.Length-1) + ", encontrou-se " + (size-1));
						break;
					}
					if (linha.tipos[i] != inst.formato[j])
					{
						switch (linha.tipos[i])
						{
							case (int)Tipos.INVALIDO:
								saida.errorOut(Escritor.ERRO, linha.linhaFonte, "Palavra inválida: " + linha.preprocessado[i]);
								break;
							case (int)Tipos.DEFLABEL:
								saida.errorOut(Escritor.ERRO, linha.linhaFonte, "Labels só podem ser definidas no início da linha.");
								break;
							case (int)Tipos.INSTRUCAO:
							case (int)Tipos.DIRETIVA:
								saida.errorOut(Escritor.ERRO, linha.linhaFonte, "Não pode ter mais de uma instrução ou diretiva por linha.");
								break;
						}
					}
					else if (linha.tipos[i] == (int)Tipos.ENDERECO)
					{
						if (ehLabel(linha.preprocessado[i]))
						{
							defs.adicionaRef(linha.preprocessado[i], linha.linhaFonte);
						}
					}
					j++;
				}
			}
			else if (linha.tipos[i] == (int)Tipos.DIRETIVA)
			{
				//se for a diretiva org, apenas 1 operando deve existir
				if(linha.preprocessado[i] == "ORG")
				{
					if(size != 2)
						saida.errorOut(Escritor.ERRO, linha.linhaFonte, "Número incorreto de operandos. Esperava-se 1, encontrou-se " + (size - 1));
				}
				i++;
				//o restante da linha so pode conter enderecos
				while (i < linha.tipos.Length)
				{
					switch (linha.tipos[i])
					{
						case (int)Tipos.INSTRUCAO:
						case (int)Tipos.DIRETIVA:
							saida.errorOut(Escritor.ERRO, linha.linhaFonte, "Não pode ter mais de uma instrução ou diretiva por linha.");
							break;
						case (int)Tipos.REGISTRADOR:
							saida.errorOut(Escritor.ERRO, linha.linhaFonte, linha.preprocessado[i] + " não é um operando válido.");
							break;
						case (int)Tipos.INVALIDO:
							saida.errorOut(Escritor.ERRO, linha.linhaFonte, "Palavra inválida: " + linha.preprocessado[i]);
							break;
					}
					i++;
				}
			}
			else
			{
				saida.errorOut(Escritor.ERRO, linha.linhaFonte, "Instrução ou diretiva inválida: " + linha.preprocessado[i]);
			}
			return true;
		}

		/*
		* verifica se a string corresponde a um numero em hexa ou em decimal, podendo ser precedida ou nao por IMEDIATO ou '-'
		*/
		public bool ehNumero(string palavra,int begin,int end)
		{
			int i = begin;
			bool hexa = false;
			bool numero = true;

			if (palavra.Length == 0)
				return false;

			if (palavra[i] == '0' && palavra[end] == 'H')
				hexa = true;

			for (; i <= end && numero; i++)
			{
				numero = false;
				if ((hexa && ehDigitoHexa(palavra[i])))
					numero = true;
				else if (Char.IsDigit(palavra[i]))
					numero = true;
			}

			if (numero)
				return true;
			//se o ultimo caracter encontrado foi um 'H' e este eh o ultimo caracter da string
			//entao eh um numero em hexadeciamal
			if (palavra[i - 1] == 'H' && hexa && i == end+1 && hexa)
				return true;
			return false;

		}

		public bool ehNumero(string palavra)
		{
			return ehNumero(palavra, 0, palavra.Length - 1);
		}

		/*
		 * verifica se uma determinada palavra pode ser uma label
		 */
		public bool ehLabel(string palavra,int length)
		{
			return ehLabel(palavra, 0, length-1);
		}

		/*
		 * verifica se uma determinada palavra pode ser uma label
		 */
		public bool ehLabel(string palavra, int begin, int end)
		{
			if (end < begin)
				return false;
			char[] invalid = { ',', '\'', '\"', ':' };

			if (Char.IsDigit(palavra[0]))
			{
				return false;
			}
			for (int i = begin; i <= end; i++)
			{
				//se for um dos caracteres invalidos
				if (Array.Exists(invalid, c => c == palavra[i]))
					return false;
			}
			return true;
		}

		/*
		 * verifica se uma determinada palavra pode ser uma label
		 */
		public bool ehLabel(string palavra)
		{
			return ehLabel(palavra,0, palavra.Length-1);
		}

		/*
		 * determina se a palavra eh uma definicao de string, ou seja
		 * comeca por ' ou " e termina pelo mesmo caractere
		 */
		public bool ehString(string palavra, int begin,int end)
		{
			int i;
			char final;
			bool escape = false;

			if (palavra.Length == 0)
				return false;

			if (palavra[begin] != '\'' && palavra[begin] != '\"')
				return false;
			
			final = palavra[begin];
			//verifica se termina pelo mesmo simbolo que comeca
			//e esse simbolo nao aparece em nenhum outro lugar da string
			for ( i = begin+1; i <end; i++)
			{
				if (!escape)
				{
					if (palavra[i] == '\\')
						escape = true;
					//se o simbolo de final de string esta no meio dela, nao eh uma string
					if (palavra[i] == final)
						return false;
				}
				else
				{
					escape = false;
				}
			}

			//se a palavra termina pelo caractere de final e este nao foi escapado, eh uma string
			return (palavra[end] == final && !escape);
		}
		public bool ehString(string palavra)
		{
			return ehString(palavra, 0, palavra.Length - 1);
		}

		// 0-F
		public bool ehDigitoHexa(char c)
		{
			//0-9
			if (c.CompareTo('0') >= 0 && c.CompareTo('9') <= 0)
			{
				return true;
			}
			//A-F
			if (c.CompareTo('A') >= 0 && c.CompareTo('F') <= 0)
			{
				return true;
			}

			return false;
		}

		/**
		 * converte uma string com um numero para um array de bytes
		 * a string deve estar em binario ou em hexadecimal (seguida de um 'H')
		 * a primeria posicao do array retornado contem o byte mais significativo
		 */
		public byte[] leCodigo(string numero)
		{
			byte[] codigo;
			int i = numero.Length - 1;
			int b;
			int bytes;
			byte valor = 0;
			//caso hexadecimal
			if (numero[i] == 'H' || numero[i] == 'h')
			{
				bytes = i / 2 + i%2;
				codigo = new byte[bytes];
				b = bytes - 1;
				for (i--; i >= 1; i-=2, b--)
				{
					valor = (byte)this.paraInteiro(numero, i - 1, i);
					codigo[b] = valor;
				}
			}
			//eh binario
			else
			{
				if (numero[i] == 'b' || numero[i] == 'B')
					i--;
				bytes = (i+1) / 8;
				if((i+1)%8 != 0)
					bytes++;
				codigo = new byte[bytes];
				b = bytes - 1;
				for (; i >= 7; i-=7, b--)
				{
					valor = (byte)this.paraInteiro(numero, i - 7, i);
					codigo[b] = valor;
				}
			}

			return codigo;
		}
    }
}
