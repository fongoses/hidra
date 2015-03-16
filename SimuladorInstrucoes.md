# Ideia #

Para que seja possível descrever processamento, serão usadas micro-instruções. Assim, cada instrução da máquina a ser simulada será descrita como uma sequência de micro-instruções. Serão usados registradores internos, os quais possuirão tamanho arbitrário e serão tantos quanto necessário.


# Conjunto de micro-instruções #

Existem as seguintes micro-instruções para serem usadas:

  * ADD A,B : **A = B + A**
  * SUB A,B : **A = B - A**
  * MUL A,B : **A = B `*` A**
  * DIV A,B : **A = B / A**
  * POW A,B : **A = B `**` A**
  * MOD A,B : **A = B % A**
  * MOV A,B : **A = B**
  * LDR A,B : **A = MEM(B)**
  * STR B,A : **MEM(B) = A**
  * AND A,B : **A = B & A**
  * OR  A,B : **A = B | A**
  * XOR A,B : **A = B ^ A**
  * NOT A : **A = ! A**
  * JMP L : **PC = L**
  * JON F,L : **se F == 1, PC =  L**
  * JOF F,L : **se F == 0, PC =  L**
  * SET F : **F = 1**
  * NIL F : **F = 0**
  * TOG F : **F = ! F**

onde:

  * A : registrador
  * B : registrador ou imediato
  * F : flag
  * L : label
  * ^ : ou-exclusivo bitwise
  * `**` : potência
  * & : e bitwise
  * | : ou bitwise
  * ! : not bitwise