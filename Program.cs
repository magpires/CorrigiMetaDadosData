using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Este console irá corrigir a data dos arquivos de imagem e vídeo que tiverem as mesmas incorretas.");
        Console.WriteLine("Arquivos nos formatos jpg, png, mp4, gif e avi serão corrigidos.");
        Console.WriteLine("O console extrai a data correta dos arquivos baseando-se no nome dos mesmos, que geralmente contém esta informação.\n");
        Console.WriteLine("Informe o caminho completo da pasta onde as fotos que terão suas datas corrigidas se encontram:");
        string diretorio = Console.ReadLine();
        Console.WriteLine("");

        if (Directory.Exists(diretorio))
        {
            try
            {
                AlteraDataHoraArquivos(diretorio);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao executar o aplicativo: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("O caminho especificado não existe.");
        }
    }

    static string ExtrairDataDoNome(string nomeArquivo)
    {
        Match match = Regex.Match(nomeArquivo, @"\d{4}-?\d{2}-?\d{2}");
        if (match.Success)
        {
            return match.Value.Replace("-", "");
        }
        else
        {
            return null;
        }
    }

    static void AlteraDataHoraArquivos(string pasta)
    {
        var arquivos = Directory.GetFiles(pasta, "*.*")
            .Where(f => f.ToLower().EndsWith(".jpg")
                || f.ToLower().EndsWith(".jpeg")
                || f.ToLower().EndsWith(".png")
                || f.ToLower().EndsWith(".gif")
                || f.ToLower().EndsWith(".mp4")
                || f.ToLower().EndsWith(".avi"));

        Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] - Total de arquivos encontrados: {arquivos.Count()}");
        Console.WriteLine("Pressione qualquer tecla para continuar ou 0 para sair.");
        string opcao = Console.ReadLine();

        if (opcao == "0")
            Environment.Exit(0);

        Console.WriteLine("");
        int totalArquivosCorrigidos = 0;
        int totalArquivosComFalha = 0;
        int index = 1;

        foreach (var arquivo in arquivos)
        {
            try
            {
                FileInfo arquivoInfo = new FileInfo(arquivo);
                string dataString = ExtrairDataDoNome(arquivoInfo.Name);
                DateTime dataCorrigida = DateTime.ParseExact(dataString, "yyyyMMdd", null);
                DateTime novaData = new DateTime(dataCorrigida.Year, dataCorrigida.Month, dataCorrigida.Day, arquivoInfo.CreationTime.Hour, arquivoInfo.CreationTime.Minute, arquivoInfo.CreationTime.Second);

                var datasDiferentes = (dataCorrigida.Date != arquivoInfo.CreationTime.Date) || (dataCorrigida.Date != arquivoInfo.LastWriteTime.Date);

                if (datasDiferentes)
                {
                    arquivoInfo.CreationTime = novaData;
                    arquivoInfo.LastWriteTime = novaData;
                    Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] - {index} de {arquivos.Count()} Data de criação e atualização corrigida com sucesso para o arquivo {arquivoInfo.Name}");
                    totalArquivosCorrigidos++;
                }
            }
            catch (Exception e)
            {
                using (StreamWriter logWriter = new StreamWriter($@"{pasta}\log.txt", true))
                {
                    Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] - Falha ao alterar o arquivo {arquivo}");
                    logWriter.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] - Erro: {e.Message} - Arquivo: {arquivo} \n\n {e.StackTrace}\n");
                    totalArquivosComFalha++;
                }
            }
            index++;
        }

        Console.WriteLine("");
        Console.WriteLine("Operação concluída");
        Console.WriteLine($"Total de arquivos corrigidos: {totalArquivosCorrigidos} de {arquivos.Count()}");
        Console.WriteLine($"Total de arquivos com falha: {totalArquivosComFalha} de {arquivos.Count()}");

        var teveFalha = totalArquivosComFalha > 0;

        if (teveFalha) 
        {
            Console.WriteLine("");
            Console.WriteLine("Consultar o arquivo log.txt para mais detalhes\n");
        }

        Console.WriteLine("Pressione qualquer tecla para encerrar...");
        Console.ReadKey();
    }
}
