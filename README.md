# Corrige Metadados de Data

## Apresenta��o
Algumas vezes pode acontecer de notarmos em nossa galeria de fotos algumas fotos com datas erradas. Voc� busca por uma foto do ver�o e se guia pela data, por�m a mesma n�o � encontrada. Com muito custo voc� localiza a foto, por�m, percebe que a mesma encontra-se com as informa��es de data referentes ao inverno. O que ser� que aconteceu?

H� in�meros motivos para isso ter ocorrido, podemos citar alguns

- Data incorreta do celular
- Edi��es na foto que alteram a data de atualiza��o para o dia em que a foto foi editada
- C�pia de arquivos de um diret�rio para outro
- Utiliza��o de programas para comprimir arquivos de m�dia

Estes e outros motivos acabam alterando os metadados das fotos e v�deos, provocando assim, uma desorganiza��o em sua biblioteca de fotos.

A ferramenta d� suporte aos principais arquivos de m�dia, s�o eles:
- .jpg (jpeg)
- .png
- .gif
- .3ga
- .mp4
- .avi

## A Solu��o
Pensando nisso, desenvolvi esta ferramenta para corrigir a data e hora destes arquivos.

### Funcionamento
Seu fincionamento � relativamente simples. A maioria destas fotos possuem em seus nomes de arquivo a data em que foi tirada a foto. Alguns contam com a hora tamb�m. Al�m disso, algumas fotos tem o metadado "Tirado em" com a informa��o exata de quando aquela imagem foi fotografada, dentre outras propriedades. A ferramenta funciona capturando estas informa��es e alterando o metadado que informa a data da �ltima modifica��o do arquivo (metadado este que geralmente � utilizado pelas galerias de fotos para se situarem no dia em que a foto foi tirada).

> **AVISO IMPORTANTE!** Isso far� com que todos os arquivos de fotos e v�deos presentes na pasta informada sejam modificados. Tenha sempre uma c�pia de seguran�a dos mesmos para que caso ocorra quaisquer erro, voc� possa recuper�-los.

### Como utilizar
Antes de mais nada, � preciso que voc� tenha o .Net Core 6.0 instalado em sua m�quina. Voc� pode baixa-lo diretamente do site oficial dispon�vel [AQUI](https://dotnet.microsoft.com/pt-br/download/dotnet/6.0).

Ap�s ter o .Net Core 6.0 instalado, seguiremos o passo a passo, que � relativamente simples.

1. Ao executar a ferramenta, a mesma pedir� que informe o caminho da pasta onde seus arquivos de imagem e v�deo se encontram. Voc� pode informar o diret�rio da seguinte forma
   ```
   C:\local\completo\onde\se\encontram\as\fotos\e\videos
   ```
   **DICA!** Substitua "C" pela letra da unidade onde se encontra a pasta com suas fotos e v�deos.
2. Ap�s informar a ferramenta o local onde as fotos e v�deos se encontram, ela ir� exibir uma contagem de arquivos de m�dias encontrados e te dar� duas op��es sendo elas
   - Apertar qualquer tecla do seu teclado para continuar
   - Apertar 0 para sair
3. Caso escolha continuar com a execu��o, ela come�ar� a trabalhar em seus arquivos, informando sobre o sucesso ou falha na opera��o de cada um deles
4. Ap�s a execu��o completa da ferramenta, ela ir� exibir na tela o total de arquivos corrigidos e o total de arquivos com falha. Caso haja falha, um arquivo de log ser� gerado. Falaremos mais dele mais adiante!
5. Assim que a ferramenta termina de executar e mostra os resultados, ela fica aguardando que o usu�rio pressione qualquer tecla para que a mesma seja finalizada por completo.

### Arquivo de log
Toda vez que uma tentativa de alterar a data e hora de um arquivo falha, uma mensagem detalhada sobre o erro � salva no arquivo log.txt que ser� armazenado juntamente com as fotos onde a ferramenta trabalhou.
A seguir, um exemplo de como seria a mensagem de erro.

> [2023-09-28 00:53:26] - Erro: Value cannot be null. (Parameter 's') - Arquivo: C:\local\completo\onde\se\encontram\as\fotos\e\videos\20d170927_210207 (1).jpg 
>
   > at System.DateTime.ParseExact(String s, String format, IFormatProvider provider)
   >
   > at Program.AlteraDataHoraArquivos(String pasta) in C:\Users\user\source\repos\CorrigiMetaDadosData\Program.cs:line 106

   ### Considera��es finais
   Acredito que esta ferramenta possa ajudar pessoas que, assim como eu, gostam de manter suas lembran�as organizadas, seja por fotos em papel, seja na galeria offline do celular ou em algum servi�o de armazenamento em nuvem como o Google Fotos.

   A ferramenta pode ser baixada na se��o de releases aqui no Github. Seu c�digo fonte est� dispon�vel e aberto ao p�blico. Sintam-se a vontade para clonar e sugerir melhorias e novas ideias.

   Obrigado por ter lido at� aqui, divirtam-se com o projeto e compartilhem com aqueles que voc� acha que ir�o gostar da ideia.