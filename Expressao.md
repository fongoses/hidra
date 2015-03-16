# Definição #

Uma expressão identifica um padrão de forma similar a uma expressão regular. São usados os símbolos:
  * r - registrador
  * n - endereço numérico
  * l - label
  * a - n ou l
  * o - qualquer um dos acima (o de operando)
  * \ - caractere de escape
  * qualquer outro caractere indica algum que deve aparecer na frase

As variáveis identificadas podem ter somente os caracteres em a-zA-Z0-9 e `_` . Elas devem estar separadas por um caractere em branco se a expressão exigir algum dos caracteres que as compõem (ver exemplo).

Ao ler uma expressão, espaços entre os caracteres são ignorados. Por isso, uma expressão não pode conter espaços (se contiver, estes serão ignorados).

# Exemplo #

As seguintes frases satisfazem "(as),a":
  * "(125 s),red" (match de 125 e red)
  * " ( reds s) , 125" (match de reds e 125)
As seguintes frases não satisfazem "(as),a":
  * "1(24 s),red"
  * "(reds),32"
  * "((red s),32"