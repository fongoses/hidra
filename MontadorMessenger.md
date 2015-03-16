# Objetivo #

As mensagens de erro e os avisos estarão em um arquivo separado, de forma a facilitar eventuais traduções do programa além de permitir diferentes níveis de rigidez para os erros.


# Arquivo #

O arquivo que descreve as mensagens estará no seguinte formato:

```
<código> <e | w> <mensagem>
```

onde:
  * código: número da mensagem;
  * e: erro
  * w: aviso
  * mensagem: a mensagem que será usada.

Comentários são feitos com o símbolo '#'.

# Variáveis #

Para que o usuário possua mais informações, as mensagens podem conter variáveis que descrevem a linha em que ocorreu o problema. São elas:

  * ADDRESSING\_MODE: modo de endereçamento usado;
  * DISTANCE: valor, em bytes, do último endereço relativo ao PC (sem truncamentos);
  * EXPECTED\_OPERANDS: número de operandos esperado pela diretiva ou instrução
  * FOUND\_OPERANDS: número de operandos encontrados
  * LABEL: nome da label;
  * LAST\_ORG\_LINE: número da linha do último ORG encontrado;
  * LINE: a linha atual do código;
  * MNEMONIC: mnemônico da instrução ou diretiva.
  * OPERAND\_SIZE: valor máximo que o operando pode assumir.

Para serem usadas, as variáveis devem ser precedidas por '$'.

# Exemplo #

#Erros

0 e Instrução ou diretiva desconhecida: $MNEMONIC

1 e Número incorreto de operandos: esperava-se $EXPECTED\_OPERANDS, encontrou-se $FOUND\_OPERANDS

2 e Endereço relativo ao PC muito grande: $LABEL está a uma distância de $DISTANCE bytes do PC, mas o máximo possível são $OPERAND\_SIZE bytes

#Avisos

3 w Região possivelmente sobrescrita a partir da linha $LAST\_ORG\_LINE

4 w Modo de endereçamento desconhecido: $ADDRESSING\_MODE