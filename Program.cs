using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Text;
using System.Reflection;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Este console irá corrigir a data dos arquivos de imagem e vídeo que tiverem as mesmas incorretas.");
        Console.WriteLine("Arquivos nos formatos jpg, png, mp4, 3gp, gif e avi serão corrigidos.");
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
        // Tenta extrair a hora no formato yyyyMMddHHmmss
        Match match = Regex.Match(nomeArquivo, @"\d{4}-?\d{2}-?\d{2}[-_ ]?\d{2}[.:]?\d{2}[.:]?\d{2}");
        if (match.Success)
        {
            // Remove caracteres especiais e formata para "yyyyMMddHHmmss"
            string dataHoraFormatada = Regex.Replace(match.Value, @"[^0-9]", "");
            return dataHoraFormatada;
        }
        else
        {
            // Caso não consiga, tentará extrair no formato yyyyMMdd (sem a hora)
            match = Regex.Match(nomeArquivo, @"\d{4}-?\d{2}-?\d{2}");
            if (match.Success)
            {
                // Remove caracteres especiais e formata para "yyyyMMddHHmmss" com a hora zerada
                return match.Value.Replace("-", "") + "000000";
            }
            else
            {
                return null;
            }
        }
    }

    static DateTime ObtemTiradoEmJpg(DateTime dataCorrigida, string caminhoArquivo)
    {
        Image image = Image.FromFile(caminhoArquivo);
        int[] propertyIds = image.PropertyIdList;

        var possuiCriadoEm = propertyIds.Contains(36867);

        if (possuiCriadoEm)
        {
            PropertyItem propriedadeTiradoEm = image.GetPropertyItem(36867);
            string dataStringTiradoEm = System.Text.Encoding.ASCII.GetString(propriedadeTiradoEm.Value).Replace("\0", "");
            DateTime dataObjetoTiradoEm = DateTime.ParseExact(dataStringTiradoEm, "yyyy:MM:dd HH:mm:ss", null);
            image.Dispose();
            return dataObjetoTiradoEm;
        }

        return DateTime.MinValue;
    }

    static string AtualizaTiradoEmJpg(DateTime dataCorrigida, string caminhoArquivo, string extensao)
    {
        Image image = Image.FromFile(caminhoArquivo);
        int[] propertyIds = image.PropertyIdList;

        var possuiCriadoEm = propertyIds.Contains(36867);

        if (possuiCriadoEm)
        {
            PropertyItem propriedadeTiradoEmAntiga = image.GetPropertyItem(36867);
            PropertyItem novaPropriedade = image.GetPropertyItem(36867);
            novaPropriedade.Id = propriedadeTiradoEmAntiga.Id;
            novaPropriedade.Len = propriedadeTiradoEmAntiga.Len;
            novaPropriedade.Type = propriedadeTiradoEmAntiga.Type;
            novaPropriedade.Value = Encoding.ASCII.GetBytes(dataCorrigida.ToString("yyyy:MM:dd HH:mm:ss"));
            image.SetPropertyItem(novaPropriedade);
            image.Save(caminhoArquivo.Replace(extensao, "_novo" + extensao));
            image.Dispose();

            return caminhoArquivo.Replace(extensao, "_novo" + extensao);
        }

        return caminhoArquivo;
    }

    static string AtualizarDataHoraMidiaCriadaMp43Gp(string caminhoExifTool, string caminhoArquivo, DateTime novaDataHora)
    {
        DateTime novaDataHoraUtc = novaDataHora.ToUniversalTime();

        using (Process processo = new Process())
        {
            processo.StartInfo.FileName = caminhoExifTool;
            processo.StartInfo.Arguments = $"-CreateDate=\"{novaDataHoraUtc:yyyy:MM:dd HH:mm:ss}\" -overwrite_original \"{caminhoArquivo}\"";
            processo.StartInfo.UseShellExecute = false;
            processo.StartInfo.RedirectStandardOutput = true;
            processo.StartInfo.RedirectStandardError = true;
            processo.StartInfo.CreateNoWindow = true;

            processo.Start();
            processo.WaitForExit();

            string saida = processo.StandardOutput.ReadToEnd();
            string saidaErro = processo.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(saidaErro))
            {
                return saidaErro;
            }

            return null;
        }
    }

    static DateTimeOffset ExtrairDataCriacao3GpMp4(string caminhoExifTool, string caminhoArquivo)
    {
        using (Process processo = new Process())
        {
            processo.StartInfo.FileName = caminhoExifTool;
            processo.StartInfo.Arguments = $"-CreateDate -n -s3 -d \"%Y-%m-%d %H:%M:%S\" \"{caminhoArquivo}\"";
            processo.StartInfo.UseShellExecute = false;
            processo.StartInfo.RedirectStandardOutput = true;
            processo.StartInfo.CreateNoWindow = true;

            processo.Start();
            string resultado = processo.StandardOutput.ReadToEnd();
            processo.WaitForExit();

            if (resultado == "0000:00:00 00:00:00\r\n")
                return DateTimeOffset.MinValue;

            int posicaoOffset = resultado.IndexOf('-');

            if (posicaoOffset >= 0)
            {
                resultado = resultado.Substring(0, posicaoOffset).Trim();
            }

            DateTimeOffset dataExtraiada = DateTimeOffset.ParseExact(resultado.Trim(), "yyyy:MM:dd HH:mm:ss", null);
            TimeSpan offsetOriginal = dataExtraiada.Offset;
            dataExtraiada = dataExtraiada.Add(offsetOriginal);

            return dataExtraiada;
        }
    }

    static void AlteraDataHoraArquivos(string pasta)
    {
        var arquivos = Directory.GetFiles(pasta, "*.*")
            .Where(f => f.ToLower().EndsWith(".jpg")
                || f.ToLower().EndsWith(".jpeg")
                || f.ToLower().EndsWith(".png")
                || f.ToLower().EndsWith(".gif")
                || f.ToLower().EndsWith(".3gp")
                || f.ToLower().EndsWith(".mp4")
                || f.ToLower().EndsWith(".avi")).ToArray();

        Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] - Total de arquivos encontrados: {arquivos.Count()}");
        Console.WriteLine("Pressione qualquer tecla para continuar ou 0 para sair.");
        var opcao = Console.ReadKey();

        if (opcao.KeyChar == '0')
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
                DateTime dataCorrigida = DateTime.ParseExact(dataString, "yyyyMMddHHmmss", null);

                var datasDiferentes = dataCorrigida != arquivoInfo.LastWriteTime;
                bool datasVideoDiferentes = false;
                bool datasJpgDiferentes = false;
                var datasUtcDiferentes = dataCorrigida.ToUniversalTime() != arquivoInfo.LastWriteTimeUtc;

                var arquivoImagemJpg = arquivoInfo.Extension.ToLower() == ".jpg"
                                    || arquivoInfo.Extension.ToLower() == ".jpeg";

                var arquivoVideo3GpMp4 = arquivoInfo.Extension.ToLower() == ".mp4" || arquivoInfo.Extension.ToLower() == ".3gp";
                var diretorioAtual = AppContext.BaseDirectory;
                var caminhoExifTool = Path.Combine(diretorioAtual,"Utils", "Exiftool", "exiftool.exe");
                string caminhoCompletoExifTool = Path.GetFullPath(caminhoExifTool);
                DateTimeOffset dataAntigaVideo = new DateTimeOffset();
                DateTime dataAntigaJpg = new DateTime();

                if (arquivoVideo3GpMp4)
                {
                    dataAntigaVideo = ExtrairDataCriacao3GpMp4(caminhoCompletoExifTool, arquivo);
                    datasVideoDiferentes = (dataAntigaVideo != DateTimeOffset.MinValue) && (dataAntigaVideo.Date != dataCorrigida.Date);
                }

                if (arquivoImagemJpg)
                {
                    dataAntigaJpg = ObtemTiradoEmJpg(dataCorrigida, arquivoInfo.FullName);
                    datasJpgDiferentes = (dataAntigaJpg != DateTime.MinValue) && (dataAntigaJpg.Date != dataCorrigida.Date);
                }

                if ((datasDiferentes && datasUtcDiferentes) || datasVideoDiferentes || datasJpgDiferentes)
                {
                    if(arquivoVideo3GpMp4)
                    {
                        var saidaErro = AtualizarDataHoraMidiaCriadaMp43Gp(caminhoCompletoExifTool, arquivo, dataCorrigida);

                        if (saidaErro != null)
                        {
                            var mensagem = $"A data do arquivo de vídeo {arquivoInfo.Name} foi corrigida, mas não foi possível corrigir o metadado 'Mídia Criada' do mesmo, devido a um erro do " +
                                                $"ExifTool.";

                            if (datasDiferentes)
                                arquivoInfo.LastWriteTime = dataCorrigida;

                            if (datasUtcDiferentes)
                                arquivoInfo.LastWriteTimeUtc = dataCorrigida.ToUniversalTime();

                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] - {index} de {arquivos.Count()} - {mensagem} Consulte o arquivo log.txt para mais detalhes!");
                            Console.ResetColor();

                            using (StreamWriter logWriter = new StreamWriter($@"{pasta}\log.txt", true))
                            {
                                logWriter.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] - {mensagem}" +
                                                    $"O erro erro lançado foi\n\n{saidaErro}");
                            }
                            totalArquivosComFalha++;
                            continue;
                        }
                    }

                    if (arquivoImagemJpg)
                    {
                        var caminhoNovoJpgTiradoEmCorrigido = AtualizaTiradoEmJpg(dataCorrigida, arquivoInfo.FullName, arquivoInfo.Extension);

                        if (caminhoNovoJpgTiradoEmCorrigido != arquivoInfo.FullName)
                        {
                            arquivoInfo.Delete();
                            FileInfo arquivoInfoCorrigido = new FileInfo(caminhoNovoJpgTiradoEmCorrigido);
                            arquivoInfoCorrigido.MoveTo(arquivoInfo.FullName);
                            arquivoInfo = new FileInfo(arquivo);
                        }

                        datasDiferentes = dataCorrigida != arquivoInfo.LastWriteTime;
                        datasUtcDiferentes = dataCorrigida.ToUniversalTime() != arquivoInfo.LastWriteTimeUtc;
                    }

                    if (datasDiferentes)
                        arquivoInfo.LastWriteTime = dataCorrigida;

                    if (datasUtcDiferentes)
                        arquivoInfo.LastWriteTimeUtc = dataCorrigida.ToUniversalTime();

                    if (datasDiferentes || datasUtcDiferentes || datasVideoDiferentes)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] - {index} de {arquivos.Count()} - Data corrigida com sucesso para o arquivo {arquivoInfo.Name}");
                        Console.ResetColor();
                        totalArquivosCorrigidos++;
                    }
                }
            }
            catch (Exception e)
            {
                using (StreamWriter logWriter = new StreamWriter($@"{pasta}\log.txt", true))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] - Falha ao alterar o arquivo {arquivo}");
                    Console.ResetColor();
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
