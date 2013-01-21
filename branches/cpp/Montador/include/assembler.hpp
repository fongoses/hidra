#ifndef ASSEMBLER_HPP
#define ASSEMBLER_HPP

#include <string>
#include <stack>
#include <list>

#include "instructions.hpp"
#include "addressings.hpp"
#include "registers.hpp"
#include "machine.hpp"
#include "labels.hpp"

using namespace std;

typedef enum {CAT_NONE,CAT_INST,CAT_ADDR,CAT_REGI,CAT_MACH} e_category;

typedef struct s_pendency
{
	unsigned int byte;
	string *label;
	bool relative;	//se a label eh relativa ao PC ou nao
	unsigned int size;	//numero de bytes que devem ser usados para o valor
}t_pendency;

class Assembler
{

	public:
	/**
	*	cria um montador, especificando quais sao as propriedades da maquina/arquiterua para qual os codigos serao montados
	*/
	Assembler(Instructions *inst, Registers *reg, Machine *machine, Addressings *adr);

	/**
	*	le as caracteristicas da arquitetura que estao no arquivo dado
	*/
	Assembler(const char *filename);

	//dummy
	Assembler();

	/**
	*	monta o codigo assembly passado
	* escreve em size o tamanho da memoria
	* retorna a memoria gerada
	*/
	unsigned char *assembleCode(string code,int *size);

	/**
	*	cria o arquivo binario para a memoria
	* o arquivo tera o seguinte formato:
	* primeiro byte: versao (0, no caso)
	* nome da maquina, terminado por um '\0'
	* md5 do resto do arquivo (16 bytes)
	* dump da memoria (size bytes)
	*/
	void createBinaryV0(string filename,string machineName,Memory *memory);

	private:

	stack<t_pendency> *pendecies; //bytes em que ha labels pendentes
	Labels *labels; //labels definidas
	Instructions *inst;
	Registers *regs;
	Addressings *addr;
	Machine *mach;

	/**
	*	monta uma linha, escrevendo seu codigo binario a partir de memory[byte]
	* line eh a linha a ser montada
	* se houver alguma label que ainda nao foi definida, reserva espaco e adiciona a pendencia na pilha
	* se for encontrada a definicao de uma label, acrescenta-a as Labels conhecidas
	* retorna a posicao da memoria em que a proxima linha deve comecar
	*/
	unsigned int assembleLine(string line, Memory *memory,unsigned int byte);

};

#endif // ASSEMBLER_HPP

