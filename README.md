# As quinze colegiais de Kirkman

> Quinze meninas de um colégio saem em fila de três, cinco filas por dia, sete dias
> seguidos. Como arrumar as filas para que **duas meninas nunca andem juntas duas
> vezes**?

Thomas Kirkman publicou isso em 1850, numa revista de passatempos. O que parecia
charada virou um ramo inteiro da matemática: a pergunta geral — quando dá para repartir
os pares de `v` pontos em trios sem repetir par nenhum — é a dos **sistemas triplos de
Steiner**, e a exigência de arrumar os trios em dias é a resolubilidade.

Aqui os dois são varridos, e a condição de existência não é citada: é medida.

```
dotnet test testes/Kirkman.Testes/Kirkman.Testes.csproj -c Release
dotnet run  --project ferramentas/Medidor/Medidor.csproj -c Release -- existencia
dotnet run  --project ferramentas/Medidor/Medidor.csproj -c Release -- simetrias
dotnet run  --project ferramentas/Medidor/Medidor.csproj -c Release -- escalas
dotnet run  --project ferramentas/Medidor/Medidor.csproj -c Release -- quinze
```

## A resposta

```
dia 1: ABC  DEF  GHI  JKL  MNO
dia 2: ADG  BEH  CJM  FKN  ILO
dia 3: AEJ  BFL  CHO  DIN  GKM
dia 4: AFO  BDM  CGL  EIK  HJN
dia 5: AHK  BGN  CFI  DJO  ELM
dia 6: AIM  BKO  CEN  DHL  FGJ
dia 7: ALN  BIJ  CDK  EGO  FHM
```

São 35 filas, e os 35 trios cobrem os 105 pares de meninas, cada um exatamente uma vez.
Os dois fatos são conferidos separadamente: que o conjunto é um sistema de Steiner, e que
cada dia reparte as quinze sem sobra.

## O juiz

A classe `Sistema` aplica a definição e nada mais: percorre os trios, conta em quantos
cada par aparece, e exige que todo par apareça **uma** vez. Dela sai de graça o número de
trios — cada trio cobre 3 pares, há `v(v−1)/2` pares, logo `v(v−1)/6` trios — e é a busca
que encontra os sistemas, sem nunca consultar condição de existência nenhuma.

## Os achados

### 1. Ter número inteiro de trios não basta, e engana em metade dos casos

```
pontos  v mod 6 e 1 ou 3  trios inteiros  sistemas  nos
3       sim              sim             1         2
4       nao              sim             0         3
5       nao              nao             0         7
6       nao              sim             0         13
7       sim              sim             30        156
8       nao              nao             0         79
9       sim              sim             840       8863
10      nao              sim             0         633
11      nao              nao             0         5036818
12      nao              sim             0         6331
13      sim              sim             -1        1290
14      nao              nao             0         75973
15      sim              sim             -1        36
```

A primeira coisa que qualquer um faz é a conta óbvia: `v(v−1)/6` precisa ser inteiro,
senão nem adianta tentar. Ela **elimina quatro dos oito tamanhos impossíveis e deixa
passar os outros quatro**: com 4, 6, 10 e 12 pontos o número de trios é inteiro e mesmo
assim não existe sistema nenhum.

A condição certa é `v ≡ 1` ou `v ≡ 3 (mod 6)`, e a busca concorda com ela nos treze
tamanhos, sem nunca tê-la consultado.

### 2. Provar que não existe custa mais com 11 pontos do que com 12

Na mesma tabela: 11 pontos custam **5.036.818 nós** para a busca voltar de mãos vazias.
Doze pontos custam **6.331** — oitocentas vezes menos, com um ponto a mais.

Com 12 o número de trios é inteiro e a impossibilidade só aparece no fim, mas a aritmética
aperta cedo e a árvore morre perto da raiz. Com 11 nem o número de trios fecha, e ainda
assim a busca passa cinco milhões de nós descobrindo isso trio por trio. **O tamanho do
problema não diz nada sobre o custo de provar que ele não tem solução.**

### 3. Duas contagens que não se falam dão o mesmo fatorial

```
pontos  sistemas  simetrias  produto  v!       classes
3       1         6          6        6        1
7       30        168        5040     5040     1
9       840       432        362880   362880   1
```

A coluna `sistemas` vem da busca, que enumera as repartições sobre pontos numerados. A
coluna `simetrias` vem de pegar **um** sistema e contar quantas das `v!` renomeações o
devolvem igual. Elas não compartilham uma linha de código.

O produto das duas dá `v!` exatamente, nas três linhas. Isso diz duas coisas de uma vez:
que as duas contagens estão certas, e que **todos os sistemas de um tamanho são o mesmo
sistema com outros nomes** — uma classe só, em 3, 7 e 9 pontos.

E o 168 tem nome: é a quantidade de simetrias do plano de Fano, o menor sistema
interessante que existe, com sete pontos e sete retas.

### 4. A escala das quinze custa dez milhões de nós; a das nove custa dezesseis

```
meninas  dias  conseguiu  nos
3        1     sim        2
9        4     sim        16
15       7     sim        10789596
```

De nove para quinze meninas, o custo de achar **uma** escala multiplica por seiscentos
mil. E não é o número de trios que explodiu — são 12 contra 35. É a exigência de que os
dias fechem: cada dia precisa ser uma repartição completa, e um erro no terceiro dia só
aparece no sexto.

## O que as medidas me corrigiram

- **Li a metade errada da matriz e a busca nunca mais terminou.** Eu guardava os pares
  cobertos só na metade de cima da matriz — índice menor primeiro — e, na hora de
  conferir, comparava `(a, c)` sem normalizar a ordem. Com `c < a` a leitura caía na
  metade vazia, o par era marcado duas vezes, e o desfazer de um ramo apagava a marca do
  outro: a busca voltava a um estado que ela achava já ter passado, e caía em recursão
  infinita até estourar a pilha. A tabela de simetrias, que tem a sua própria enumeração
  escrita com cuidado, funcionou perfeitamente ao lado — **a metade certa do programa
  rodando foi o que disfarçou a metade errada**.

- **Eu ia varrer 21 e 27 meninas junto.** A escala de 15 custa dez milhões de nós; as de
  21 e 27 não terminaram em minutos. Ficaram fora, e a tabela diz o que foi medido em vez
  de insinuar que o método escala.

- **Eu achava que a conta dos trios inteiros era quase a condição.** Ela deixa passar 4,
  6, 10 e 12 — metade dos tamanhos impossíveis até 15. Uma condição necessária que erra
  metade das vezes não é "quase suficiente", e eu ia escrever exatamente isso.

- **O 168 eu ia citar.** Ele saiu da medida, contando quantas das 5.040 renomeações de
  sete pontos devolvem o mesmo sistema. Citar um número que o programa calcula é o que
  este repositório existe para não fazer.

## Estrutura

```
src/Kirkman/
  Sistema.cs    o juiz: cada par em exatamente um trio, pela definicao
  Busca.cs      a busca que atende sempre o primeiro par descoberto
  Simetrias.cs  as renomeacoes que devolvem o mesmo sistema, e as classes
  Passeio.cs    as colegiais: os trios repartidos em dias completos
  Medidas.cs    as tabelas deste README
testes/Kirkman.Testes/
ferramentas/Medidor/
```

71 testes.
