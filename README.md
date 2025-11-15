🏆 Jogo da Velha — Edição Premium (Console em C#)

Um Jogo da Velha completo, moderno e extremamente divertido, desenvolvido em C# (.NET 9) com interface estilizada no console, multiplayer online, IA inteligente, chat integrado, efeitos sonoros e animações especiais.

Criado para ser um dos jogos de console mais avançados já feitos em C#.

🎮 Recursos Principais
✔️ Modos de Jogo

🧍 Jogador vs Máquina

Níveis: Fácil • Médio • Difícil (Minimax)

👥 Jogador vs Jogador (local)

🌐 Jogador vs Jogador (Online via Servidor TCP)

👁️ Modo Espectador

Jogadores e espectadores podem abrir o chat com F1

🎨 Interface Moderna (Console)

Desenvolvido com Spectre.Console, trazendo:

FigletText estilizado

Tabelas coloridas

Painéis com bordas modernas

Layout dividido (jogo à esquerda, chat à direita)

Emojis, animações e efeitos visuais

Barra de status com número de espectadores 👁️

🔊 Efeitos Sonoros (NAudio)

O jogo possui sons personalizados para:

Marcações no tabuleiro

Jogadas inválidas

Vitória

Derrota

Empate

Mensagens especiais

🧠 Inteligência Artificial

A IA possui comportamento adaptativo:

⚡ Fácil

Escolhe jogadas aleatórias.

🧩 Médio

Tenta bloquear jogadas e ganhar quando possível.

🧠 Difícil

Algoritmo Minimax completo → praticamente impossível de derrotar.

📊 Sistema de Placar e Estatísticas

Histórico de partidas

Vitórias, Derrotas e Empates

Estatísticas individuais por adversário

Persistência local

🌐 Modo Online com Chat

Servidor próprio (TCP)

Vários jogadores podem entrar

Sistema detecta se já existe uma partida em andamento

Espectadores entram automaticamente se o jogo estiver ativo

Chat global com F1

🛠 Tecnologias Usadas
Tecnologia	Uso
C# (.NET 9)	Jogo e lógica principal
Spectre.Console	Interface moderna no terminal
NAudio	Sons e efeitos
TCP Sockets	Multiplayer online
Minimax Algorithm	IA no nível difícil
📦 Instalação
1. Clone o repositório
git clone https://github.com/LeonardoGomesFerreira/Jogo-da-Velha.git

2. Instale os pacotes NuGet
Install-Package Spectre.Console -Version 0.44.0
Install-Package NAudio -Version 2.2.0

3. Coloque seus sons na pasta /Sons
marcar.wav  
erro.wav  
vitoria.wav  
derrota.wav  
empate.wav  
mensagem.wav

4. Execute o jogo
dotnet run

🗂 Estrutura do Projeto
/JogoDaVelha
 ├── Program.cs
 ├── MenuPrincipal.cs
 ├── JogoLocal.cs
 ├── JogoOnline.cs
 ├── Espectador.cs
 ├── Chat.cs
 ├── Tabuleiro.cs
 ├── IA.cs
 ├── Sons.cs
 ├── Animacoes.cs
 ├── Placar.cs
 ├── ServidorTCP.cs
 ├── ClienteTCP.cs
 └── /Sons

🖼 Layout (Simulação Visual)
╔══════════════════════ JOGO DA VELHA ═══════════════════════╗   👁️ 5 online
║  X | O | X                                                  ║
║ ---+---+---                                                 ║   CHAT
║  O | X |                                                    ║  [Jogador1] Opa!
║ ---+---+---                                                 ║  [Jogador2] Bora jogar!
║    |   |                                                    ║
╚═════════════════════════════════════════════════════════════╝

🚀 Roadmap
🔥 Futuras melhorias:

Replays das partidas

Skins para o tabuleiro

Sistema de temas (claro/escuro)

Ranking online

Sala de espera (lobby)

🤝 Contribuições

Sinta-se livre para abrir issues ou enviar pull requests.

📜 Licença

Este projeto está licenciado sob a MIT License.
