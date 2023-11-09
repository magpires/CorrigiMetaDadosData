# Corrige Metadados de Data

## Apresentação
Algumas vezes pode acontecer de notarmos em nossa galeria de fotos algumas fotos com datas erradas. Você busca por uma foto do verão e se guia pela data, porém a mesma não é encontrada. Com muito custo você localiza a foto, porém, percebe que a mesma encontra-se com as informações de data referentes ao inverno. O que será que aconteceu?

Há inúmeros motivos para isso ter ocorrido, podemos citar alguns

- Data incorreta do celular
- Edições na foto que alteram a data de atualização para o dia em que a foto foi editada
- Cópia de arquivos de um diretório para outro
- Utilização de programas para comprimir arquivos de mídia

Estes e outros motivos acabam alterando os metadados das fotos e vídeos, provocando assim, uma desorganização em sua biblioteca de fotos.

## A Solução
Pensando nisso, desenvolvi esta ferramenta para corrigir a data e hora destes arquivos.

A ferramenta dá suporte aos principais arquivos de mídia, são eles:
- .jpg (jpeg)
- .png
- .gif
- .3ga
- .mp4
- .avi

### Funcionamento
Seu fincionamento é relativamente simples. A maioria destas fotos possuem em seus nomes de arquivo a data em que foi tirada a foto. Alguns contam com a hora também. Além disso, algumas fotos tem o metadado "Tirado em" com a informação exata de quando aquela imagem foi fotografada, dentre outras propriedades. Arquivos de vídeo contam com o metadado "Mídia criada". A ferramenta funciona capturando estas informações e alterando o metadado que informa a data da última modificação do arquivo (metadado este que geralmente é utilizado pelas galerias de fotos para se situarem no dia em que a foto foi tirada).

> **AVISO IMPORTANTE!** Isso fará com que todos os arquivos de fotos e vídeos presentes na pasta informada sejam modificados. Tenha sempre uma cópia de segurança dos mesmos para que caso ocorra quaisquer erro, você possa recuperá-los.

### Como utilizar
Antes de mais nada, é preciso que você tenha o .Net Core 6.0 instalado em sua máquina. Você pode baixa-lo diretamente do site oficial disponível [AQUI](https://dotnet.microsoft.com/pt-br/download/dotnet/6.0).

Após ter o .Net Core 6.0 instalado, seguiremos o passo a passo, que é relativamente simples.

1. Ao executar a ferramenta, a mesma pedirá que informe o caminho da pasta onde seus arquivos de imagem e vídeo se encontram. Você pode informar o diretório da seguinte forma
   ```
   C:\local\completo\onde\se\encontram\as\fotos\e\videos
   ```
   **DICA!** Substitua "C" pela letra da unidade onde se encontra a pasta com suas fotos e vídeos.
2. Após informar a ferramenta o local onde as fotos e vídeos se encontram, ela irá exibir uma contagem de arquivos de mídias encontrados e te dará duas opções sendo elas
   - Apertar qualquer tecla do seu teclado para continuar
   - Apertar 0 para sair
3. Caso escolha continuar com a execução, ela começará a trabalhar em seus arquivos, informando sobre o sucesso ou falha na operação de cada um deles
4. Após a execução completa da ferramenta, ela irá exibir na tela o total de arquivos corrigidos e o total de arquivos com falha. Caso haja falha, um arquivo de log será gerado. Falaremos mais dele mais adiante!
5. Assim que a ferramenta termina de executar e mostra os resultados, ela fica aguardando que o usuário pressione qualquer tecla para que a mesma seja finalizada por completo.

### Arquivo de log
Toda vez que uma tentativa de alterar a data e hora de um arquivo falha, uma mensagem detalhada sobre o erro é salva no arquivo ```log.txt``` que será armazenado juntamente com as fotos onde a ferramenta trabalhou.
A seguir, um exemplo de como seria a mensagem de erro.

> [2023-09-28 00:53:26] - Erro: Value cannot be null. (Parameter 's') - Arquivo: C:\local\completo\onde\se\encontram\as\fotos\e\videos\20d170927_210207 (1).jpg 
>
   > at System.DateTime.ParseExact(String s, String format, IFormatProvider provider)
   >
   > at Program.AlteraDataHoraArquivos(String pasta) in C:\Users\user\source\repos\CorrigiMetaDadosData\Program.cs:line 106

### Considerações finais
Acredito que esta ferramenta possa ajudar pessoas que, assim como eu, gostam de manter suas lembranças organizadas, seja por fotos em papel, seja na galeria offline do celular ou em algum serviço de armazenamento em nuvem como o Google Fotos.

A ferramenta pode ser baixada na seção de releases aqui no Github. Seu código fonte está disponível e aberto ao público. Sintam-se a vontade para clonar e sugerir melhorias e novas ideias.

Obrigado por ter lido até aqui, divirtam-se com o projeto e compartilhem com aqueles que você acha que irão gostar da ideia.

### Informações adicionais e créditos
Esta ferramenta utiliza o **ExifTool by Phil Harvey** para fixar o metadado "Mídia Criada" de arquivos de vídeo. 
Sou imensamente grato e faço, com o maior prazer, a menção honrosa pois sem sua ferramenta, não seria possível que eu lançasse
a segunda versão de minha ferramenta. O link para o site do projeto pode ser acessado clicando [AQUI](https://exiftool.org/)!