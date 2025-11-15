// JogoDaVelha_Console_CSharp_Melhorado.cs
// Projeto: Console Tic-Tac-Toe Aprimorado (Windows, Visual Studio 2022)
// NuGet: Install-Package Spectre.Console -Version 0.44.0
//        Install-Package NAudio -Version 2.2.0

using NAudio.Wave;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JogoDaVelhaConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "🎮 Jogo da Velha - Edição Premium";
            Anciador.Iniciar();
        }
    }

    // ═══════════════════════════════════════════════════════════════════
    // ANCIADOR - Menu Principal
    // ═══════════════════════════════════════════════════════════════════
    public static class Anciador
    {
        public static void Iniciar()
        {
            var placarGlobal = new Placar();
            while (true)
            {
                Console.Clear();
                ExibirCabecalho();

                var opcao = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("\n[bold cyan]┌─────────────────────────────────┐[/]\n[bold cyan]│[/]  [yellow]Escolha uma opção:[/]          [bold cyan]│[/]\n[bold cyan]└─────────────────────────────────┘[/]")
                        .HighlightStyle(new Style(Color.Green, decoration: Decoration.Bold))
                        .AddChoices(new[] {
                            "🎮 Jogar contra amigo (local)",
                            "🤖 Jogar contra IA",
                            "🌐 Jogar online (servidor/cliente)",
                            "👁️  Assistir partida (espectador)",
                            "📊 Placar e estatísticas",
                            "⚙️  Configurações",
                            "🚪 Sair"
                        }));

                if (opcao.Contains("amigo")) Modos.JogadorContraJogadorLocal(placarGlobal);
                else if (opcao.Contains("IA")) Modos.JogadorContraMaquina(placarGlobal);
                else if (opcao.Contains("online")) Modos.Online(placarGlobal);
                else if (opcao.Contains("Assistir")) Modos.AssistirPartida(placarGlobal);
                else if (opcao.Contains("Placar")) { placarGlobal.MostrarPlacarCompleto(); PausarTela(); }
                else if (opcao.Contains("Configurações")) Configuracoes.Mostrar();
                else if (opcao.Contains("Sair")) break;
            }

            AnsiConsole.MarkupLine("\n[bold green]Obrigado por jogar! Até a próxima! 👋[/]\n");
        }

        private static void ExibirCabecalho()
        {
            var panel = new Panel(
                new FigletText("JOGO DA VELHA")
                    .Centered()
                    .Color(Color.Cyan1))
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Blue),
                Padding = new Padding(2, 1)
            };
            AnsiConsole.Write(panel);

            AnsiConsole.MarkupLine("[dim]Versão 2.0 - Edição Premium | Desenvolvido com ❤️[/]\n");
        }

        private static void PausarTela()
        {
            AnsiConsole.MarkupLine("\n[dim]Pressione qualquer tecla para continuar...[/]");
            Console.ReadKey(true);
        }
    }

    // ═══════════════════════════════════════════════════════════════════
    // CONFIGURAÇÕES
    // ═══════════════════════════════════════════════════════════════════
    public static class Configuracoes
    {
        public static bool TomHumilhante { get; set; } = false;
        public static int NivelSons { get; set; } = 70;
        public static bool AnimacoesAtivadas { get; set; } = true;

        public static void Mostrar()
        {
            while (true)
            {
                Console.Clear();
                var panel = new Panel("[bold yellow]⚙️  CONFIGURAÇÕES[/]")
                {
                    Border = BoxBorder.Rounded,
                    BorderStyle = new Style(Color.Yellow)
                };
                AnsiConsole.Write(panel);

                var tabela = new Table()
                    .Border(TableBorder.Rounded)
                    .BorderColor(Color.Grey)
                    .AddColumn(new TableColumn("[bold]Opção[/]").Centered())
                    .AddColumn(new TableColumn("[bold]Status[/]").Centered())
                    .AddRow("Tom de jogo", TomHumilhante ? "[red]Provocador 😈[/]" : "[green]Respeitoso 😊[/]")
                    .AddRow("Animações", AnimacoesAtivadas ? "[green]Ativadas ✓[/]" : "[red]Desativadas ✗[/]")
                    .AddRow("Sons", $"[cyan]Volume: {NivelSons}%[/]");

                AnsiConsole.Write(tabela);

                var escolha = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("\n[cyan]O que deseja alterar?[/]")
                        .AddChoices(new[] {
                            "🎭 Alternar tom (provocador/respeitoso)",
                            "✨ Ativar/desativar animações",
                            "🔊 Ajustar volume",
                            "↩️  Voltar"
                        }));

                if (escolha.Contains("tom")) TomHumilhante = !TomHumilhante;
                else if (escolha.Contains("animações")) AnimacoesAtivadas = !AnimacoesAtivadas;
                else if (escolha.Contains("volume"))
                {
                    NivelSons = AnsiConsole.Prompt(
                        new TextPrompt<int>("[yellow]Volume (0-100):[/]")
                            .DefaultValue(70)
                            .ValidationErrorMessage("[red]Digite um número entre 0 e 100[/]")
                            .Validate(v => v >= 0 && v <= 100));
                }
                else break;
            }
        }
    }

    // ═══════════════════════════════════════════════════════════════════
    // MODOS DE JOGO
    // ═══════════════════════════════════════════════════════════════════
    public static class Modos
    {
        public static void JogadorContraJogadorLocal(Placar placar)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[bold green]🎮 MODO LOCAL - 2 JOGADORES[/]").RuleStyle("green"));

            var nome1 = AnsiConsole.Ask<string>("\n[cyan]Nome do Jogador 1:[/]");
            var nome2 = AnsiConsole.Ask<string>("[cyan]Nome do Jogador 2:[/]");

            var simbolo1 = AnsiConsole.Prompt(
                new SelectionPrompt<char>()
                    .Title($"[yellow]{nome1}, escolha seu símbolo:[/]")
                    .AddChoices('X', 'O'));

            var jogador1 = new Jogador(nome1, simbolo1);
            var jogador2 = new Jogador(nome2, simbolo1 == 'X' ? 'O' : 'X');

            var jogo = new JogoDaVelha(jogador1, jogador2, placar);
            jogo.IniciarPartida();
        }

        public static void JogadorContraMaquina(Placar placar)
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[bold blue]🤖 MODO IA - VOCÊ vs COMPUTADOR[/]").RuleStyle("blue"));

            var nome = AnsiConsole.Ask<string>("\n[cyan]Seu nome:[/]");
            var simbolo = AnsiConsole.Prompt(
                new SelectionPrompt<char>()
                    .Title("[yellow]Escolha seu símbolo:[/]")
                    .AddChoices('X', 'O'));

            var nivel = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Escolha o nível de dificuldade:[/]")
                    .AddChoices("😊 Fácil", "😐 Médio", "😈 Difícil"));

            IA.NivelDificuldade nivelIA = nivel.Contains("Fácil") ? IA.NivelDificuldade.Facil :
                                          nivel.Contains("Médio") ? IA.NivelDificuldade.Medio :
                                          IA.NivelDificuldade.Dificil;

            var jogador = new Jogador(nome, simbolo);
            var maquina = new Jogador("🤖 IA", simbolo == 'X' ? 'O' : 'X');

            var jogo = new JogoDaVelha(jogador, maquina, placar, nivelIA);
            jogo.IniciarPartida();
        }

        public static void Online(Placar placar)
        {
            var escolha = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Modo online:[/]")
                    .AddChoices("🖥️  Criar servidor", "🔌 Conectar a servidor", "↩️  Voltar"));

            if (escolha.Contains("Criar"))
            {
                var servidor = new Servidor(placar);
                servidor.Iniciar();
            }
            else if (escolha.Contains("Conectar"))
            {
                var ip = AnsiConsole.Ask<string>("[cyan]IP do servidor:[/]", "127.0.0.1");
                var porta = AnsiConsole.Ask<int>("[cyan]Porta:[/]", 5000);
                var cliente = new Cliente(ip, porta, placar);
                cliente.Iniciar();
            }
        }

        public static void AssistirPartida(Placar placar)
        {
            AnsiConsole.MarkupLine("\n[yellow]👁️  Modo espectador:[/]");
            AnsiConsole.MarkupLine("[dim]Conecte-se a um servidor para assistir partidas ao vivo.[/]\n");
            AnsiConsole.MarkupLine("[dim]Pressione qualquer tecla...[/]");
            Console.ReadKey(true);
        }
    }

    // ═══════════════════════════════════════════════════════════════════
    // PLACAR E ESTATÍSTICAS
    // ═══════════════════════════════════════════════════════════════════
    public class Placar
    {
        private Dictionary<string, EstatisticaConfronto> estatisticas = new Dictionary<string, EstatisticaConfronto>();

        public void RegistrarResultado(string nomeA, string nomeB, Resultado resultado)
        {
            var chave = Chave(nomeA, nomeB);
            if (!estatisticas.ContainsKey(chave))
                estatisticas[chave] = new EstatisticaConfronto(nomeA, nomeB);
            estatisticas[chave].Registrar(resultado);
        }

        private string Chave(string a, string b)
        {
            return string.Compare(a, b) <= 0 ? $"{a}|{b}" : $"{b}|{a}";
        }

        public void MostrarPlacarCompleto()
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[bold yellow]📊 PLACAR E ESTATÍSTICAS[/]").RuleStyle("yellow"));

            if (estatisticas.Count == 0)
            {
                AnsiConsole.MarkupLine("\n[dim]Nenhuma partida registrada ainda. Jogue para ver suas estatísticas![/]\n");
                return;
            }

            var tabela = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Yellow)
                .AddColumn(new TableColumn("[bold cyan]Jogador A[/]").Centered())
                .AddColumn(new TableColumn("[bold yellow]vs[/]").Centered())
                .AddColumn(new TableColumn("[bold magenta]Jogador B[/]").Centered())
                .AddColumn(new TableColumn("[bold green]Vitórias A[/]").Centered())
                .AddColumn(new TableColumn("[bold red]Vitórias B[/]").Centered())
                .AddColumn(new TableColumn("[bold grey]Empates[/]").Centered())
                .AddColumn(new TableColumn("[bold white]Total[/]").Centered());

            foreach (var kv in estatisticas.OrderByDescending(x => x.Value.TotalPartidas))
            {
                kv.Value.AdicionarNaTabela(tabela);
            }

            AnsiConsole.Write(tabela);
        }
    }

    public class EstatisticaConfronto
    {
        public string JogadorA { get; }
        public string JogadorB { get; }
        private int vitoriasA = 0, vitoriasB = 0, empates = 0;

        public int TotalPartidas => vitoriasA + vitoriasB + empates;

        public EstatisticaConfronto(string a, string b)
        {
            JogadorA = a;
            JogadorB = b;
        }

        public void Registrar(Resultado res)
        {
            if (res == Resultado.Empate) empates++;
            else if (res == Resultado.VitoriaA) vitoriasA++;
            else if (res == Resultado.VitoriaB) vitoriasB++;
        }

        public void AdicionarNaTabela(Table tabela)
        {
            tabela.AddRow(
                $"[cyan]{JogadorA}[/]",
                "[yellow]⚔️[/]",
                $"[magenta]{JogadorB}[/]",
                $"[green]{vitoriasA}[/]",
                $"[red]{vitoriasB}[/]",
                $"[grey]{empates}[/]",
                $"[white]{TotalPartidas}[/]"
            );
        }
    }

    public enum Resultado { VitoriaA, VitoriaB, Empate }

    // ═══════════════════════════════════════════════════════════════════
    // JOGADOR
    // ═══════════════════════════════════════════════════════════════════
    public class Jogador
    {
        public string Nome { get; set; }
        public char Simbolo { get; set; }

        public Jogador(string nome, char simbolo)
        {
            Nome = nome;
            Simbolo = simbolo;
        }
    }

    // ═══════════════════════════════════════════════════════════════════
    // JOGO DA VELHA - LÓGICA PRINCIPAL
    // ═══════════════════════════════════════════════════════════════════
    public class JogoDaVelha
    {
        private char[] tab = new char[9];
        private Jogador jogadorA, jogadorB;
        private Placar placar;
        private IA.NivelDificuldade nivelIA;
        private bool contraMaquina;
        private List<string> historico = new List<string>();

        public JogoDaVelha(Jogador a, Jogador b, Placar placarGlobal, IA.NivelDificuldade nivel = IA.NivelDificuldade.Medio)
        {
            jogadorA = a;
            jogadorB = b;
            placar = placarGlobal;
            nivelIA = nivel;
            contraMaquina = b.Nome.Contains("IA") || a.Nome.Contains("IA");

            for (int i = 0; i < 9; i++) tab[i] = ' ';
        }

        public void IniciarPartida()
        {
            Jogador vez = jogadorA;
            int numeroJogada = 0;

            while (true)
            {
                DesenharTabuleiro(vez);
                int jogada = -1;

                if (contraMaquina && vez.Nome.Contains("IA"))
                {
                    AnsiConsole.Status()
                        .Start("[yellow]🤖 IA está pensando...[/]", ctx =>
                        {
                            Thread.Sleep(800);
                            jogada = IA.CalcularJogada(tab, vez.Simbolo, nivelIA);
                        });
                    Sons.TocarSom("jogada.wav");
                }
                else
                {
                    jogada = SolicitarJogada(vez);
                    if (jogada == -1) continue;
                }

                if (tab[jogada] != ' ')
                {
                    Sons.TocarSom("erro.wav");
                    AnsiConsole.MarkupLine("\n[red]❌ Casa ocupada! Escolha outra posição.[/]");
                    Thread.Sleep(1000);
                    continue;
                }

                tab[jogada] = vez.Simbolo;
                numeroJogada++;
                historico.Add($"Jogada {numeroJogada}: {vez.Nome} marcou posição {jogada + 1}");
                Sons.TocarSom("marcar.wav");

                var resultado = VerificarResultado();
                if (resultado.HasValue)
                {
                    FinalizarPartida(resultado.Value, vez);
                    return;
                }

                vez = vez == jogadorA ? jogadorB : jogadorA;
            }
        }

        private int SolicitarJogada(Jogador jogador)
        {
            var posicoes = new List<string>();
            for (int i = 0; i < 9; i++)
            {
                if (tab[i] == ' ')
                    posicoes.Add((i + 1).ToString());
            }

            if (posicoes.Count == 0) return -1;

            var escolha = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"\n[bold yellow]👉 {jogador.Nome}, escolha sua jogada:[/]")
                    .PageSize(9)
                    .AddChoices(posicoes));

            return int.Parse(escolha) - 1;
        }

        private void DesenharTabuleiro(Jogador vezAtual, (Resultado Valor, int[] Linha)? ultimaVitoria = null)
        {
            Console.Clear();

            // Cabeçalho
            var panel = new Panel($"[bold cyan]{jogadorA.Nome}[/] ([yellow]{jogadorA.Simbolo}[/]) [bold white]vs[/] [bold magenta]{jogadorB.Nome}[/] ([yellow]{jogadorB.Simbolo}[/])")
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Blue),
                Padding = new Padding(1, 0)
            };
            AnsiConsole.Write(panel);

            AnsiConsole.MarkupLine($"\n[dim]Vez de:[/] [bold yellow]{vezAtual.Nome}[/] [yellow]({vezAtual.Simbolo})[/]\n");

            // Desenhar tabuleiro em grid visual
            var grid = new Table()
                .Border(TableBorder.Heavy)
                .BorderColor(Color.Cyan1)
                .HideHeaders();

            for (int i = 0; i < 3; i++)
                grid.AddColumn(new TableColumn("").Width(10).Centered());

            for (int r = 0; r < 3; r++)
            {
                var celulas = new List<string>();
                for (int c = 0; c < 3; c++)
                {
                    int idx = r * 3 + c;
                    bool destaque = ultimaVitoria.HasValue && Array.Exists(ultimaVitoria.Value.Linha, x => x == idx);
                    celulas.Add(FormatarCelula(idx, destaque));
                }
                grid.AddRow(celulas.ToArray());
            }

            AnsiConsole.Write(grid);

            // Histórico (últimas 3 jogadas)
            if (historico.Count > 0)
            {
                AnsiConsole.MarkupLine("\n[dim]═══ Histórico (últimas jogadas) ═══[/]");
                foreach (var h in historico.TakeLast(3))
                    AnsiConsole.MarkupLine($"[dim]• {h}[/]");
            }
        }

        private string FormatarCelula(int idx, bool destaque)
        {
            var valor = tab[idx];
            string display;

            if (valor == ' ')
                display = $"[dim grey]{idx + 1}[/]";
            else if (valor == 'X')
                display = "[bold red]X[/]";
            else
                display = "[bold blue]O[/]";

            if (destaque)
                return $"[on yellow] {display} [/]";
            else
                return $" {display} ";
        }

        private (Resultado Valor, int[] Linha)? VerificarResultado()
        {
            int[][] linhas = new int[][] {
                new[]{0,1,2}, new[]{3,4,5}, new[]{6,7,8},
                new[]{0,3,6}, new[]{1,4,7}, new[]{2,5,8},
                new[]{0,4,8}, new[]{2,4,6}
            };

            foreach (var l in linhas)
            {
                if (tab[l[0]] != ' ' && tab[l[0]] == tab[l[1]] && tab[l[1]] == tab[l[2]])
                {
                    var vencedor = tab[l[0]] == jogadorA.Simbolo ? Resultado.VitoriaA : Resultado.VitoriaB;
                    return (vencedor, l);
                }
            }

            if (tab.All(c => c != ' '))
                return (Resultado.Empate, new int[0]);

            return null;
        }

        private void FinalizarPartida((Resultado Valor, int[] Linha) resultado, Jogador ultimoJogador)
        {
            DesenharTabuleiro(ultimoJogador, resultado);

            if (resultado.Valor == Resultado.Empate)
            {
                AnsiConsole.MarkupLine("\n[bold yellow]🤝 EMPATE! Ninguém venceu desta vez.[/]\n");
                placar.RegistrarResultado(jogadorA.Nome, jogadorB.Nome, Resultado.Empate);
                Sons.TocarSom("empate.wav");
                if (Configuracoes.AnimacoesAtivadas)
                    Animacoes.ExibirAnimacaoEmpate();
            }
            else
            {
                var vencedor = resultado.Valor == Resultado.VitoriaA ? jogadorA : jogadorB;
                var perdedor = vencedor == jogadorA ? jogadorB : jogadorA;

                AnsiConsole.MarkupLine($"\n[bold green]🏆 VITÓRIA DE {vencedor.Nome.ToUpper()}![/]\n");
                placar.RegistrarResultado(jogadorA.Nome, jogadorB.Nome, resultado.Valor);
                Sons.TocarSom("vitoria.wav");

                if (Configuracoes.AnimacoesAtivadas)
                {
                    if (Configuracoes.TomHumilhante)
                        Animacoes.ExibirAnimacaoDerrotaEVitoriaHumilhante(perdedor.Nome, vencedor.Nome);
                    else
                        Animacoes.ExibirAnimacaoVitoriaCarinhosa(vencedor.Nome);
                }
            }

            AnsiConsole.MarkupLine("\n[dim]Pressione qualquer tecla para continuar...[/]");
            Console.ReadKey(true);
        }
    }

    // ═══════════════════════════════════════════════════════════════════
    // ANIMAÇÕES
    // ═══════════════════════════════════════════════════════════════════
    public static class Animacoes
    {
        public static void ExibirAnimacaoVitoriaCarinhosa(string nomeVencedor)
        {
            AnsiConsole.Write(new Rule($"[bold green]🎉 PARABÉNS {nomeVencedor}! 🎉[/]").RuleStyle("green"));

            var mensagens = new[] {
                $"💪 {nomeVencedor}, que jogada espetacular!",
                $"⭐ Excelente estratégia, {nomeVencedor}!",
                $"🏆 Domínio total! {nomeVencedor} é campeão!"
            };

            AnsiConsole.MarkupLine($"\n[bold green]{mensagens[new Random().Next(mensagens.Length)]}[/]\n");

            // Animação de confete
            for (int i = 0; i < 5; i++)
            {
                var simbolos = new[] { "🎊", "🎉", "⭐", "✨", "🏆" };
                var emoji = simbolos[i % simbolos.Length];
                var linha = string.Concat(Enumerable.Repeat(emoji, i + 1));
                AnsiConsole.MarkupLine($"[bold yellow]{linha}[/]");
                Thread.Sleep(100);
            }
        }

        public static void ExibirAnimacaoDerrotaEVitoriaHumilhante(string nomeDerrotado, string nomeVencedor)
        {
            var provocacoes = new[] {
                $"😅 {nomeDerrotado}, hoje não foi seu dia! {nomeVencedor} dominou!",
                $"🤔 {nomeDerrotado}, que tal revisar a estratégia? {nomeVencedor} arrasou!",
                $"😬 Opa! {nomeDerrotado} vacilou e {nomeVencedor} aproveitou!"
            };

            AnsiConsole.MarkupLine($"\n[bold red]{provocacoes[new Random().Next(provocacoes.Length)]}[/]");
            AnsiConsole.MarkupLine("\n[dim]( ͡° ͜ʖ ͡°) < Melhor sorte na próxima![/]\n");
        }

        public static void ExibirAnimacaoEmpate()
        {
            AnsiConsole.MarkupLine("[bold yellow]Estratégias equilibradas! Ambos jogaram muito bem! 🤝[/]\n");

            for (int i = 0; i < 3; i++)
            {
                AnsiConsole.MarkupLine($"[dim]{'='}[/]");
                Thread.Sleep(100);
            }
        }
    }

    // ═══════════════════════════════════════════════════════════════════
    // SONS
    // ═══════════════════════════════════════════════════════════════════
    public static class Sons
    {
        private static string PastaSons => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sons");

        public static void TocarSom(string nomeArquivo)
        {
            try
            {
                var caminho = Path.Combine(PastaSons, nomeArquivo);
                if (!File.Exists(caminho)) return;

                Task.Run(() =>
                {
                    using (var audioFile = new AudioFileReader(caminho))
                    using (var output = new WaveOutEvent())
                    {
                        output.Init(audioFile);
                        output.Volume = Configuracoes.NivelSons / 100f;
                        output.Play();
                        while (output.PlaybackState == PlaybackState.Playing)
                            Thread.Sleep(50);
                    }
                });
            }
            catch { /* Ignorar erros de som */ }
        }
    }

    // ═══════════════════════════════════════════════════════════════════
    // IA (INTELIGÊNCIA ARTIFICIAL)
    // ═══════════════════════════════════════════════════════════════════
    public static class IA
    {
        public enum NivelDificuldade { Facil, Medio, Dificil }

        public static int CalcularJogada(char[] tab, char simbolo, NivelDificuldade nivel)
        {
            var vazios = Enumerable.Range(0, 9).Where(i => tab[i] == ' ').ToList();
            if (vazios.Count == 0) return -1;

            var rnd = new Random();

            if (nivel == NivelDificuldade.Facil)
                return vazios[rnd.Next(vazios.Count)];

            if (nivel == NivelDificuldade.Medio)
            {
                int win = JogadaVitoriaOuBloqueio(tab, simbolo);
                if (win != -1) return win;

                int block = JogadaVitoriaOuBloqueio(tab, simbolo == 'X' ? 'O' : 'X');
                if (block != -1) return block;

                return vazios[rnd.Next(vazios.Count)];
            }

            return MinimaxMelhorJogada(tab, simbolo);
        }

        private static int JogadaVitoriaOuBloqueio(char[] tab, char chk)
        {
            int[][] linhas = new int[][] {
                new[]{0,1,2}, new[]{3,4,5}, new[]{6,7,8},
                new[]{0,3,6}, new[]{1,4,7}, new[]{2,5,8},
                new[]{0,4,8}, new[]{2,4,6}
            };

            foreach (var l in linhas)
            {
                int cont = 0, vazio = -1;
                foreach (var idx in l)
                {
                    if (tab[idx] == chk) cont++;
                    if (tab[idx] == ' ') vazio = idx;
                }
                if (cont == 2 && vazio != -1) return vazio;
            }
            return -1;
        }

        private static int MinimaxMelhorJogada(char[] tab, char simbolo)
        {
            int melhor = -1;
            int melhorPont = int.MinValue;

            for (int i = 0; i < 9; i++)
            {
                if (tab[i] == ' ')
                {
                    tab[i] = simbolo;
                    int pont = Minimax(tab, 0, false, simbolo);
                    tab[i] = ' ';

                    if (pont > melhorPont)
                    {
                        melhorPont = pont;
                        melhor = i;
                    }
                }
            }

            if (melhor == -1)
            {
                var vazios = Enumerable.Range(0, 9).Where(i => tab[i] == ' ').ToList();
                return vazios[new Random().Next(vazios.Count)];
            }

            return melhor;
        }

        private static int Minimax(char[] tab, int profundidade, bool maximizando, char jogadorMax)
        {
            var resultado = VerificarVencedor(tab, jogadorMax);
            if (resultado.HasValue) return resultado.Value - profundidade;

            if (tab.All(c => c != ' ')) return 0;

            char jogadorAtual = maximizando ? jogadorMax : (jogadorMax == 'X' ? 'O' : 'X');

            if (maximizando)
            {
                int melhor = int.MinValue;
                for (int i = 0; i < 9; i++)
                {
                    if (tab[i] == ' ')
                    {
                        tab[i] = jogadorAtual;
                        int pont = Minimax(tab, profundidade + 1, false, jogadorMax);
                        tab[i] = ' ';
                        melhor = Math.Max(melhor, pont);
                    }
                }
                return melhor;
            }
            else
            {
                int melhor = int.MaxValue;
                for (int i = 0; i < 9; i++)
                {
                    if (tab[i] == ' ')
                    {
                        tab[i] = jogadorAtual;
                        int pont = Minimax(tab, profundidade + 1, true, jogadorMax);
                        tab[i] = ' ';
                        melhor = Math.Min(melhor, pont);
                    }
                }
                return melhor;
            }
        }

        private static int? VerificarVencedor(char[] tab, char jogadorMax)
        {
            int[][] linhas = new int[][] {
                new[]{0,1,2}, new[]{3,4,5}, new[]{6,7,8},
                new[]{0,3,6}, new[]{1,4,7}, new[]{2,5,8},
                new[]{0,4,8}, new[]{2,4,6}
            };

            foreach (var l in linhas)
            {
                if (tab[l[0]] != ' ' && tab[l[0]] == tab[l[1]] && tab[l[1]] == tab[l[2]])
                {
                    return tab[l[0]] == jogadorMax ? 10 : -10;
                }
            }
            return null;
        }
    }

    // ═══════════════════════════════════════════════════════════════════
    // SERVIDOR (MODO ONLINE)
    // ═══════════════════════════════════════════════════════════════════
    public class Servidor
    {
        private TcpListener listener;
        private List<TcpClient> clientes = new List<TcpClient>();
        private List<string> mensagensChat = new List<string>();
        private Placar placar;
        private bool rodando = false;

        public Servidor(Placar p)
        {
            placar = p;
        }

        public void Iniciar()
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[bold green]🖥️  SERVIDOR - MODO ONLINE[/]").RuleStyle("green"));

            int porta = AnsiConsole.Ask<int>("\n[cyan]Porta para o servidor:[/]", 5000);

            try
            {
                listener = new TcpListener(IPAddress.Any, porta);
                listener.Start();
                rodando = true;

                var panel = new Panel($"[bold green]✓[/] Servidor iniciado!\n[dim]Porta:[/] [yellow]{porta}[/]\n[dim]IP Local:[/] [yellow]{ObterIPLocal()}[/]")
                {
                    Border = BoxBorder.Rounded,
                    BorderStyle = new Style(Color.Green)
                };
                AnsiConsole.Write(panel);

                AnsiConsole.MarkupLine("\n[yellow]Aguardando conexões...[/]");
                AnsiConsole.MarkupLine("[dim]Digite mensagens para enviar ao chat ou '/sair' para encerrar[/]\n");

                Task.Run(() => AceitarLoop());

                while (rodando)
                {
                    var cmd = Console.ReadLine();
                    if (cmd == "/sair")
                    {
                        rodando = false;
                        listener.Stop();
                        break;
                    }
                    BroadcastMensagem($"[SERVIDOR] {cmd}");
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Erro ao iniciar servidor: {ex.Message}[/]");
                Thread.Sleep(2000);
            }
        }

        private async Task AceitarLoop()
        {
            while (rodando)
            {
                try
                {
                    var cliente = await listener.AcceptTcpClientAsync();
                    clientes.Add(cliente);
                    AnsiConsole.MarkupLine($"[green]✓ Novo cliente conectado! Total: {clientes.Count}[/]");
                    Task.Run(() => TratarCliente(cliente));
                }
                catch { break; }
            }
        }

        private void TratarCliente(TcpClient c)
        {
            var ns = c.GetStream();
            var buffer = new byte[4096];

            try
            {
                while (rodando)
                {
                    int bytesLidos = ns.Read(buffer, 0, buffer.Length);
                    if (bytesLidos == 0) break;

                    var msg = Encoding.UTF8.GetString(buffer, 0, bytesLidos);
                    BroadcastMensagem(msg);
                }
            }
            catch { }
            finally
            {
                clientes.Remove(c);
                c.Close();
                AnsiConsole.MarkupLine($"[red]✗ Cliente desconectado. Total: {clientes.Count}[/]");
            }
        }

        private void BroadcastMensagem(string msg)
        {
            mensagensChat.Add(msg);
            var dados = Encoding.UTF8.GetBytes(msg);

            foreach (var c in clientes.ToList())
            {
                try
                {
                    c.GetStream().Write(dados, 0, dados.Length);
                }
                catch
                {
                    clientes.Remove(c);
                }
            }

            AnsiConsole.MarkupLine($"[cyan]💬 {msg}[/]");
        }

        private string ObterIPLocal()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                        return ip.ToString();
                }
            }
            catch { }
            return "127.0.0.1";
        }
    }

    // ═══════════════════════════════════════════════════════════════════
    // CLIENTE (MODO ONLINE)
    // ═══════════════════════════════════════════════════════════════════
    public class Cliente
    {
        private string ip;
        private int porta;
        private Placar placar;
        private TcpClient cliente;
        private NetworkStream ns;
        private bool conectado = false;

        public Cliente(string ip, int porta, Placar p)
        {
            this.ip = ip;
            this.porta = porta;
            this.placar = p;
        }

        public void Iniciar()
        {
            Console.Clear();
            AnsiConsole.Write(new Rule("[bold blue]🔌 CLIENTE - CONECTANDO AO SERVIDOR[/]").RuleStyle("blue"));

            try
            {
                AnsiConsole.Status()
                    .Start($"[yellow]Conectando a {ip}:{porta}...[/]", ctx =>
                    {
                        cliente = new TcpClient();
                        cliente.Connect(ip, porta);
                        ns = cliente.GetStream();
                        conectado = true;
                    });

                if (conectado)
                {
                    var panel = new Panel($"[bold green]✓[/] Conectado com sucesso!\n[dim]Servidor:[/] [yellow]{ip}:{porta}[/]")
                    {
                        Border = BoxBorder.Rounded,
                        BorderStyle = new Style(Color.Green)
                    };
                    AnsiConsole.Write(panel);

                    AnsiConsole.MarkupLine("\n[yellow]💬 Chat ativo[/]");
                    AnsiConsole.MarkupLine("[dim]Digite suas mensagens ou '/sair' para desconectar[/]\n");

                    Task.Run(() => ReceberLoop());

                    while (conectado)
                    {
                        var txt = Console.ReadLine();
                        if (txt == "/sair")
                        {
                            cliente.Close();
                            conectado = false;
                            break;
                        }

                        var dados = Encoding.UTF8.GetBytes($"[CLIENTE] {txt}");
                        ns.Write(dados, 0, dados.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"\n[red]❌ Erro ao conectar: {ex.Message}[/]");
                AnsiConsole.MarkupLine("[dim]Verifique se o servidor está rodando e tente novamente.[/]");
                Thread.Sleep(3000);
            }
        }

        private void ReceberLoop()
        {
            var buffer = new byte[4096];

            try
            {
                while (conectado)
                {
                    int bytesLidos = ns.Read(buffer, 0, buffer.Length);
                    if (bytesLidos == 0) break;

                    var msg = Encoding.UTF8.GetString(buffer, 0, bytesLidos);
                    AnsiConsole.MarkupLine($"[green]📩 {msg}[/]");
                }
            }
            catch { }
            finally
            {
                conectado = false;
                cliente?.Close();
                AnsiConsole.MarkupLine("\n[red]Desconectado do servidor.[/]");
            }
        }
    }
}