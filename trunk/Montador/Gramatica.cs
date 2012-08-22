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
		public enum SubTipos { NONE, STRING, LABEL, NUMERO, ARRAY };

		public Linguagem linguagem = new Linguagem();

		/**
		 * retorna o valor inteiro de um digito hexadecimal
		 * ou -1 se nao foi hexadecimal
		 */
		public int getHexValue(Char d)
		{
			int v = (int)Char.GetNumericValue(d);
			if (v >= 0)
			{
				return v;
			}

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
		 * converte uma string para um array de bytes
		 * tamanho eh o numero de bytes que cada caractere ocupara
		 * usa little-endian
		 */
		public byte[] string2byteArray(string s, int tamanho, ref Codigo.Estado estado)
		{
			byte[] vetor = new byte[s.Length * tamanho];
			int i;

			for (i = 0; i < s.Length; i += tamanho)
			{
				vetor[i] = (byte)s[i];
				for (int k = 1; k < tamanho; k++)
					vetor[i + k] = 0;
			}

			return vetor;
		}

		/*
		 * recebe uma string e converte para um inteiro
		 * numberos decimais terminam por algum algarismo
		 * hexadecimais terminam por 'h',
		 * números binarios podem terminar por 'b'
		 */
		public int paraInteiro(string numero)
		{
			if (numero == null)
				return 0;
			return this.paraInteiro(numero, 0, numero.Length - 1);
		}
		public int paraInteiro(string numero, int begin, int end)
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
				case 'h':
				case 'H':
					for (int i = end - 1, p = 1; i >= begin; i--, p *= 16)
					{
						num += (int)(p * getHexValue(numero[i]));
					}
					break;
				case 'b':
				case 'B':
					for (int i = end - 1, p = 1; i >= begin; i--, p *= 2)
					{
						if (numero[i] == '1')
							num += p;
					}
					break;
				//decimal
				default:
					for (int i = end, p = 1; i >= begin; i--, p *= 10)
					{
						num += (int)(p * Char.GetNumericValue(numero[i]));
					}
					break;
			}
			return num;
		}

		/**
		 * recebe um array de bytes e converte para um numero inteiro
		 */
		public int arrayParaInteiro(byte[] num, Linguagem.Endianness endianness)
		{
			int val = 0;
			int dir;
			int i;
			int p = 0;

			//se for little-endian, o primeiro byte eh o menos significativo
			if (endianness == Linguagem.Endianness.Little)
			{
				dir = 1;
				i = 0;
			}
			else
			{
				dir = -1;
				i = num.Length - 1;
			}

			for (; i >= 0 && i < num.Length; i += dir, p += 8)
			{				
				val += num[i] << p;
			}

			return val;
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
			int i = 0, p = 0;
			bool achou = false;

			//enquanto nao encontrar a substring
			while (!achou && i < palavra.Length)
			{
				//busca um '0'
				for (; i < palavra.Length && palavra[i] != '0'; i++)
					;
				//copia os caracteres para o numero
				for (p = 0; i < palavra.Length && ehDigitoHexa(palavra[i]); numero[p] = palavra[i], p++, i++)
					;
				//se o proximo caracter for um 'H', o numero foi encontrado
				if (i < palavra.Length)
					if (palavra[i] == 'H')
						achou = true;
			}

			string n = new string(numero, 0, p);

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
			for (i = hexa.Length - 1; i >= 0; i--, power++)
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
		public Gramatica.SubTipos identificaSubTipo(string nome)
		{
			if (ehNumero(nome))
				return SubTipos.NUMERO;
			else if (ehString(nome))
				return SubTipos.STRING;
			else if (ehLabel(nome))
				return SubTipos.LABEL;
			else if (ehArray(nome))
				return SubTipos.ARRAY;
			else
				return SubTipos.NONE;

		}

		/*
		* identifica o tipo de cada palavra de uma linha (olhar enum Tipos)
		* atualiza o vetor de nomes da linha
		*/
		public void identificaTipo(ref Linha linha, Gramatica gramatica)
		{
			int i = 0;
			string palavra;
			string nome = "";
			for (i = 0; i < linha.preprocessado.Length; i++)
			{
				byte[] end;
				//enderecamento padrao
				end = (byte[])linguagem.enderecamentos[linguagem.enderecamentos.Count -1].codigo.Clone();

				palavra = linha.preprocessado[i];
				Gramatica.Tipos tipo;
				if (ehNumero(palavra))
				{
					linha.tipos[i] = Tipos.ENDERECO;
					linha.subTipos[i] = SubTipos.NUMERO;
					linha.nomes[i] = palavra;
				}
				else
				{
					nome = linha.preprocessado[i];
					Gramatica.SubTipos subt = Gramatica.SubTipos.NONE;
					//se nao for um numero, verifica se eh alguma palavra conhecida
					tipo = linguagem.identificaTipo(palavra, ref nome, ref end,ref subt);

					if (tipo != Tipos.INVALIDO)
					{
						linha.tipos[i] = tipo;
						//se for um endereco, determina o subtipo
						if (tipo == Tipos.ENDERECO)
						{
							if(subt == Gramatica.SubTipos.NONE)
								linha.subTipos[i] = identificaSubTipo(nome);
							else
								linha.subTipos[i] = subt;
							if (linha.subTipos[i] == SubTipos.STRING)
							{
								Stringer str = new Stringer();
								char[] parsedArr = new char[palavra.Length];
								int size = 0;

								str.parse(palavra, 1, parsedArr, ref size);
								linha.nomes[i] = new string(parsedArr,0,size);
								nome = linha.nomes[i];
							}
						}
						linha.nomes[i] = nome;
					}
					else
					{
						if (ehLabel(palavra, palavra.Length - 1))
						{
							if (palavra[palavra.Length - 1] == ':')
							{
								linha.tipos[i] = Tipos.DEFLABEL;
								linha.nomes[i] = new string(palavra.ToCharArray(), 0, palavra.Length - 1);
							}
							else
							{
								linha.tipos[i] = Tipos.ENDERECO;
								linha.subTipos[i] = SubTipos.LABEL;
							}
						}
						else if (ehString(palavra))
						{
							linha.tipos[i] = Tipos.ENDERECO;
							linha.subTipos[i] = SubTipos.STRING;

							Stringer str = new Stringer();
							char[] parsedArr = new char[palavra.Length];
							int size = 0;

							str.parse(palavra, 1, parsedArr, ref size);
							linha.nomes[i] = new string(parsedArr);
						}
						else
							linha.tipos[i] = Tipos.INVALIDO;
					}
				}
				linha.enderecamento.Add(end);
			}//end for
		}

		/*
		 * verifica se os tipos de uma linha são validos
		 * retorna true se forem
		 */
		public bool verificaTipos(Linha linha, Escritor saida, Definicoes defs)
		{
			Instrucao inst;
			int i = 0;
			int j = 1;
			int size = linha.tipos.Length;
			//a linha pode comecar por definicao de label
			if (linha.tipos[0] == Tipos.DEFLABEL)
			{
				defs.adicionaDef(linha.preprocessado[0].Substring(0, linha.preprocessado[0].Length - 1), linha.linhaFonte, saida);
				i = 1;
				size--;
			}
			//a linha eh apenas a definicao de uma label
			if (i >= linha.tipos.Length)
				return true;

			if (linha.tipos[i] == Tipos.INSTRUCAO)
			{
				inst = this.linguagem.instrucoes.Find(o => o.mnemonico == linha.preprocessado[i]);
				if (size < inst.formato.Length)
					saida.errorOut(Escritor.Message.IncorrectNumOperands, linha.linhaFonte, (inst.formato.Length - 1), (size - 1));
				//verifica se ha algo diferente de registradores e enderecos
				for (i++; i < linha.tipos.Length; i++)
				{
					if (j >= inst.formato.Length)
					{
						if (linha.tipos[i - 1] != Tipos.ENDERECO)
							saida.errorOut(Escritor.Message.IncorrectNumOperands, linha.linhaFonte, (inst.formato.Length - 1), (size - 1));
						break;
					}
					if (linha.tipos[i] != inst.formato[j])
					{
						switch (linha.tipos[i])
						{
							case Tipos.INVALIDO:
								saida.errorOut(Escritor.Message.InvalidWord, linha.linhaFonte, linha.preprocessado[i]);
								break;
							case Tipos.DEFLABEL:
								saida.errorOut(Escritor.Message.IncorrectLabelDef, linha.linhaFonte);
								break;
							case Tipos.INSTRUCAO:
							case Tipos.DIRETIVA:
								saida.errorOut(Escritor.Message.IncorrectNumInstructions, linha.linhaFonte);
								break;
						}
					}
					else if (linha.tipos[i] == Tipos.ENDERECO)
					{
						if(linha.subTipos[i] == Gramatica.SubTipos.LABEL)
						{
							defs.adicionaRef(linha.preprocessado[i], linha.linhaFonte);
						}
					}
					j++;
				}
			}
			else if (linha.tipos[i] == Tipos.DIRETIVA)
			{
				//se for a diretiva org, apenas 1 operando deve existir
				if (linha.preprocessado[i] == "ORG")
				{
					if (size != 2)
						saida.errorOut(Escritor.Message.IncorrectNumOperands, linha.linhaFonte, 1, (size - 1));
				}
				i++;
				//o restante da linha so pode conter enderecos
				while (i < linha.tipos.Length)
				{
					switch (linha.tipos[i])
					{
						case Tipos.INSTRUCAO:
						case Tipos.DIRETIVA:
							saida.errorOut(Escritor.Message.IncorrectNumInstructions, linha.linhaFonte);
							break;
						case Tipos.REGISTRADOR:
							saida.errorOut(Escritor.Message.InvalidOperand, linha.linhaFonte, linha.preprocessado[i]);
							break;
						case Tipos.INVALIDO:
							saida.errorOut(Escritor.Message.InvalidWord, linha.linhaFonte, linha.preprocessado[i]);
							break;
					}
					i++;
				}
			}
			else
			{
				saida.errorOut(Escritor.Message.InvalidInstructionOrDirective, linha.linhaFonte, linha.preprocessado[i]);
			}
			return true;
		}

		/*
		* verifica se a string corresponde a um numero em hexa ou em decimal, podendo ser precedida ou nao por IMEDIATO ou '-'
		*/
		public bool ehNumero(string palavra, int begin, int end)
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
			if (palavra[i - 1] == 'H' && hexa && i == end + 1 && hexa)
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
		public bool ehLabel(string palavra, int length)
		{
			return ehLabel(palavra, 0, length - 1);
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
			return ehLabel(palavra, 0, palavra.Length - 1);
		}

		/*
		 * verifica se a palavra eh a definicao de uma label
		 * se for, escreve seu nome sem ':' em nome
		 */
		public bool ehDefLabel(string palavra,int begin,int end,ref string nome)
		{
			if (begin >= end)
				return false;
			if (palavra[end] != ':')
				return false;
			if (ehLabel(palavra, begin, end - 1))
			{
				nome = new string(palavra.ToCharArray(),begin,end-begin);
				return true;
			}
			return false;
		}

		/*
		 * determina se a palavra eh uma definicao de string, ou seja
		 * comeca por ' ou " e termina pelo mesmo caractere
		 */
		public bool ehString(string palavra, int begin, int end)
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
			for (i = begin + 1; i < end; i++)
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

		//verifica se a palavra eh um array de palavras validas ou nao
		//um elemento sozinho eh considerado um array
		public bool ehArray(string palavra)
		{
			int i = 0;
			int b = 0;

			while (i < palavra.Length)
			{
				//ignora espacos a esquerda
				while (palavra[i] == ' ')
					i++;
				b = i;
				//se for uma string, busca o final
				if (palavra[i] == '\'' || palavra[i] == '\"')
				{
					i = finalString(palavra, i, palavra.Length);
					if (i == -1)
						return false;

				}
				//busca a virgula
				while (i < palavra.Length)
				{
					if (palavra[i] == ',')
						break;
					i++;
				}
				if (b == i)
					return false;
				i++;
				
			}
			return true;
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
				bytes = i / 2 + i % 2;
				codigo = new byte[bytes];
				b = bytes - 1;
				for (i--; i >= 1; i -= 2, b--)
				{
					valor = ((byte)getHexValue(numero[i]));
					valor += (byte)((byte)getHexValue(numero[i - 1]) << 4);
					codigo[b] = valor;
				}
			}
			//eh binario
			else
			{
				if (numero[i] == 'b' || numero[i] == 'B')
					i--;
				bytes = (i + 1) / 8;
				if ((i + 1) % 8 != 0)
					bytes++;
				codigo = new byte[bytes];
				b = bytes - 1;
				for (; i >= 7; i -= 7, b--)
				{
					valor = (byte)this.paraInteiro(numero, i - 7, i);
					codigo[b] = valor;
				}
				if (i >= 0)
				{
					valor = (byte)this.paraInteiro(numero, 0, i);
					codigo[0] = valor;
				}
			}
			return codigo;
		}

		/*
		 * recebe uma string que contenha uma string.
		 * retorna o indice do caractere do final da string interna
		 * ou -1, caso ela esteja aberta
		 * palavra[begin] deve ser '\'' ou '\"'
		 */
		public int finalString(string palavra,int begin, int end)
		{
			if (begin > end)
				return end;
			char final = palavra[begin];
			if (final != '\'' && final != '\"')
				return begin;

			begin++;
			bool escape = false;
			while (begin <= end)
			{
				if (escape == false)
				{
					if (palavra[begin] == final)
						return begin;
					else if (palavra[begin] == '\\')
						escape = true;
				}
				else
				{
					escape = false;
				}
				begin++;
			}

			return -1;
		}

		/*
		 * recebe um array onde cada elemento esta separado por virgulas
		 * retorna o proximo elemento a partir da posição dada
		 * atualiza a posicao
		 * exemplo:
		 * proximoElemento("ab,cd",2,5) = "cd"
		 * proximoElemento("ab,cd",1,5) = "b"
		 */
		public string proximoElemento(string array, ref int pos, int end)
		{
			string elemento;
			if (pos > end || pos >= array.Length)
				return "";
			

			if (array[pos] == ',')
				pos++;
			int b = pos;
			pos = finalString(array,pos,end);
			if (pos == -1)
				return "";
			
			while (pos <= end)
			{
				if (array[pos] == ',')
					break;
				pos++;
			}
			elemento = new string(array.ToCharArray(), b, pos - b);
			
			return elemento;
		}
	}
}
