#ifndef STRINGER_HPP
#define STRINGER_HPP

#include <list>
#include <string>

using namespace std;
/**
	*	quebra a string nos divisores passados
	* nao acrescenta elementos vazios a lista
	*/
list<string> *stringSplitChar(string text, string dividers);

/**
	*	remove todos os dividers que estiverem nos cantos de s, retornando a nova string
	*/
string stringTrim(string s,string dividers);

/**
	* verifica se c esta em s
	*/
bool stringIn(char c, string s);

/**
*	compara duas strings ignorando maiusculas e minusculas
*	retorna 0 se forem iguais, >0 se a >b, <0 se a<b
*/
int stringCaselessCompare(string a, string b);

#endif // STRINGER_HPP

