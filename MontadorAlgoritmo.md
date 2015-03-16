# Ideia #

Será usado um algoritmo de uma passada com pendências. Como haverá, no máximo, uma instrução por linha, podemos tratar cada linha individualmente. Os passos são os seguintes:

  1. Se um label for definida, acrescenta o byte em que isso ocorreu em uma tabela
  1. Se uma label for referenciada, verifica-se se ela existe. Se existir, monta a instrução com o valor adequado. Se não existir, acrescenta uma pendência para o byte atual e reserva espaço para a label
  1. Itera por todas as pendências, montando as linhas em que aparecem

Se no final do processo alguma label não for definida, o montador deve produzir um erro e não gerar o binário.