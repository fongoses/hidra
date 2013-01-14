#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include <string>

#include "sha1.hpp"

#include "assembler.hpp"

#define SIZE 512

using namespace std;

/**
	*	cria um montador, especificando quais sao as propriedades da maquina/arquiterua para qual os codigos serao montados
	*/
	//Assemlber::Assembler(Instructions *inst, Registers *reg, Machine *machine, Adressing *adr);

	//dumy
	Assembler::Assembler()
	{

	}

	/**
	*	le as caracteristicas da arquitetura que estao no arquivo dado
	*/
	Assembler::Assembler(const char *filename)
	{
		FILE *fl = fopen(filename,"rb");
		if(fl == NULL)
		{
			throw eFileNotFound;
		}
		else
		{

			int size;
			fseek(fl,0,SEEK_END);
			size = ftell(fl);
			char *data = (char *)malloc(size+1);
			fread(data,1,size,fl);
			fclose(fl);
			data[size] = '\0';

			string *text = new string(data)

			list<string> *lines = stringSplitChar(text,"\n\r");
			list<string>::iterator it;

			for(it=lines->begin() ; it!=lines->end() ; it++)
			{
				string line = *it;

				e_category category = CAT_NONE;
				for(int i=0 ; i<line.size() ; i++)
				{
					char c = line[i];
					//ignora caracteres em branco
					if(c==' ' || c=='\t')
						continue;

					//comentario
					if(c=='#')
						break;

					if(category == CAT_INST)
					{
						this->inst->load(line);
						break;
					}
					else if(category == CAT_ADDR)
					{
						this->addr->load(line);
						break;
					}
					else if(category == CAT_REGI)
					{
						this->regs->load(line);
						break;
					}
					else if(category == CAT_INST)
					{
						this->mach->load(line);
						break;
					}

					//se for um '[', eh a definicao de uma categoria
					if(c=='[')
					{
						//busca o ']'
						i++;
						int start = i;
						while(i<line.size && line[i]!=']')
							i++;
						if(line[i]!=']')
							throw eWrongFormat;
						string catName = stringTrim(string(line,start,i-start)," \t");

						if(stringCaselessCompare(catName,"instructions")==0)
							category = CAT_INST;
						else if(stringCaselessCompare(catName,"addressings")==0)
							category = CAT_ADDR;
						else if(stringCaselessCompare(catName,"machine")==0)
							category = CAT_MACH;
						else if(stringCaselessCompare(catName,"registers")==0)
							category = CAT_REGI;
						else
							category = CAT_NONE;
					}
				}
			}

			delete(text);
			delete(lines);
			free(data);

		}
	}

	/**
	*	monta o codigo assembly passado
	* escreve em size o tamanho da memoria
	* retorna a memoria gerada
	*/
	char *Assembler::assembleCode(string code,int *size)
	{

	}

	/**
	*	cria o arquivo binario para a memoria
	* o arquivo tera o seguinte formato:
	* primeiro byte: versao (0, no caso)
	* nome da maquina, terminado por um '\0'
	* dump da memoria (size bytes)
	* SHA1 do resto do arquivo (20 bytes)
	*/
	void Assembler::createBinaryV0(string filename,string machineName,char *memory, int size)
	{
		FILE *fl = fopen(filename.c_str(),"wb");
		//escreve a versão
		char version = 0;
		fwrite(&version,1,1,fl);
		//nome da maquina
		fwrite(machineName.c_str(),1,machineName.size(),fl);
		//termina com '\0'
		char zero = '\0';
		fwrite(&zero,1,1,fl);
		//calcula o SHA1 do arquivo
		//comeca concatenando o arquivo
		char *cat = (char *)malloc(1+machineName.size()+1+size);
		unsigned int pos=0;
		cat[pos++] = 0;
		memcpy(cat+pos,machineName.c_str(),machineName.size());
		pos+=machineName.size();
		cat[pos++]='\0';
		//copia a memoria
		memcpy(cat+pos,memory,size);
		pos+=size;

		//calcula o SHA1
		SHA1 *shaCalc = new SHA1();
		shaCalc->Input(cat,pos);
		unsigned int *sha = (unsigned int *)malloc(20);	//SHA1 = 160 bits = 20 bytes
		shaCalc->Result(sha);

		//faz um dump da memoria
		fwrite(memory,1,size,fl);

		//escreve o SHA1
		fwrite(sha,1,20,fl);

		fclose(fl);
		delete shaCalc;
		free(cat);
		free(sha);


	}

	/**
	*	monta uma linha, escrevendo seu codigo binario a partir de memory[byte]
	* line eh a linha a ser montado
	* se houver alguma label que ainda nao foi definida, reserva espaco e adiciona a pendencia na pilha
	* se for encontrada a definicao de uma label, acrescenta-a as Labels conhecidas
	* retorna a posicao da memoria em que a proxima linha deve comecar
	*/
	int Assembler::assembleLine(string *line, char *memory,int byte)
	{

	}
