# Objetivo #

O objetivo do projeto é criar um montador e um simulador para as máquinas teóricas usadas nas disciplinas de Arquitetura de Computadores do Instituto de Informática da UFRGS.
Inicialmente, o programa deverá suportar as seguintes máquinas:

  * Ahmes
  * Cromag
  * Péricles
  * Pitágoras
  * Quéops
  * Ramses
  * Reg
  * Volta

Além disso, o montador e o simulador devem estar integrados, deforma que seja possível montar um programa e executá-lo imediatamente.

# Ideia #

Como são muitas máquinas e todas são parecidas, a ideia é criar uma linguagem de descrição dessas máquinas e um interpretador, de forma que para adicionar uma nova máquina tanto no simulador quanto no montador não seja necessário alterar o código fonte, mas apenas os arquivos que a descrevem.