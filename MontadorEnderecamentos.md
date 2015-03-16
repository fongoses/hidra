# Função #

Descreve os modos de endereçamento de cada máquina, como o formato pelo qual é identificado, sobre quais tipos é válido e se é relativo ao PC ou não.

# Formato #
```
[addressings]
<nome> [=/-] <código binário> <identificador>
```
onde:
  * `<`nome> : nome do modo de endereçamento (usado internamente). Pode ser qualquer sequência de caracteres a-z e A-Z
  * [=/-] : (Default: =) Determina se o valor do endereço deve ser calculado em relação ao PC ou não. "=" indica que deve ser simplesmente copiado, "-" que é relativo ao PC.
  * <código binário>: [Número](Numeros.md)
  * `<`identificador>: [Expressão](Expressao.md) que identifica o modo de endereçamento. Somente um registrador ou endereço pode aparecer por expressão.

Exemplo:
Modo indireto, Neander
```
i = 0 e,i
```
Identificará:
ADD 128,i