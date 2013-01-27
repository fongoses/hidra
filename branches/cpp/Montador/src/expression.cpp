#include <string>
#include <list>

#include "expression.hpp"
#include "defs.hpp"

using namespace std;

Expression::Expression()
{

}

Expression::Expression(string expression)
{
	this->exp = expression;
}


/**
* se a frase satisfizer a expressao, faz o match entre as variaveis da frase com as da expressao
* se nenhuma expressao for passado, usa a do construtor
* retorna uma lista de pares onde o primeiro elemento eh a variavel e o segundo, seu tipo
* se a frase nao satisfizer, throws eUnmatchedExpression
*/
list<pair<string,char> > Expression::findAll(string phrase,string exp)
{

	typedef enum {STATE_INI,STATE_VAR,STATE_READ} e_state;

	list<pair<string,char> > vars;

	if(exp=="")
		exp=this->exp;

	unsigned int p=0,e=0;
	unsigned int b=0;
	e_state state = STATE_INI;

	while(p<phrase.size() && e<phrase.size())
	{
		char pc = phrase[p];
		char ec = exp[e];

		switch(state)
		{
			//elimina whitespaces no inicio
			case STATE_INI:

				if(ISWHITESPACE(pc))
					p++;
				else if(ISWHITESPACE(ec))
					e++;
				else
					state = STATE_READ;
				break;
			//le caracteres da expressao
			case STATE_READ:

				if(ISWHITESPACE(ec))
					e++;
				else if(isReserved(ec))
				{
					state = STATE_VAR;
					b=p;
				}
				else if(ec!=pc)
					throw(eUnmatchedExpression);
				else
				{
					e++;
					p++;
				}

				break;
			case STATE_VAR:
				//se o caractere nao eh o de uma variavel, a variavel acabou
				if(!isVarChar(pc))
				{
					state = STATE_READ;
					vars.push_back(pair<string,char>(phrase.substr(b,p-b),ec));
					e++;
				}
				else
					p++;
				break;
		}
	}

	return vars;
}

string Expression::expression()
{
	return this->exp;
}

/**
* verifica se o caractere passado eh um dos reservados para variaveis
*/
bool isVarChar(char c)
{
	if('A' <= c && c <= 'Z')
		return true;
	if('a' <= c && c <= 'z')
		return true;
	if('0' <= c && c<='9')
		return true;
	if(c=='_')
		return true;
	return false;
}

/**
* verifica se o caractere eh reservado
*/
bool isReserved(char c)
{
	switch(c)
	{
		case 'r':
		case 'e':
		case 'l':
		case 'w':
		case 'n':
			return true;
		default:
			return false;
	}
}
