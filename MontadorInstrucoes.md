# Função #

O arquivo deve determinar qual o mnemônico, formato, modos de endereçamento e registradores possíveis para cada instrução.

# Formato #

```
[instructions]
<tamanho> <mnemônico> <operandos> <endereçamento0,...> <<registrador0,...> | - | *> <formato binário> 
```

onde:
  * `<`tamanho> : [número](Numeros.md) de bits da instrução
  * <mnemônico> : qualquer sequência dos caracteres a-z e A-Z (case insensitive)
  * `<`operandos`>` : [Expressão](Expressao.md) que indica como os operandos da instrução devem aparecer. Uma expressão sem variáveis será ignorada e indicará que nenhum operando deve ser utilizado
  * <endereçamento0,...> : quais modos de [endereçamento](MontadorEnderecamentos.md) podem ser usados com essa instrução. `*` indica qualquer um.
  * <registrador0,...> : quais [registradores](MontadorRegistrador.md) podem ser usados por essa instrução. "-" indica nenhum,"`*`" indica todos
  * <formato binário> : como que a instrução é montada. São usados os símbolos:
    * a`[`n`]``[m]` : n-ésimo endereço (se o n nâo for informado, são colocados na ordem em que aparecem). m indica quantos bits terá o endereço, sendo desnecessário quando há somente um endereço no formato. Se houver mais de um endereço na expressão, todos usarão o mesmo número de bits.
    * m`[`n`]` : n-ésimo modo de endereçamento (se o n nâo for informado, são colocados na ordem em que aparecem)
    * r`[n]` : n-ésimo registrador (se o n nâo for informado, são colocados na ordem em que aparecem)
    * 0 : bit 0
    * 1 : bit 1
    * qualquer outro caractere será ignorado
    * n deve ser um número decimal e, se aparecer, deve estar entre `[``]`
    * m deve ser um número decimal e, se aparecer, deve estar entre `(``)`

Exemplo (Neander):
16 ADD a d - 0011000ma`[0](8)`