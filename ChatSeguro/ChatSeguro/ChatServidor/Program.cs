using EI.SI;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/// <summary>
/// Classe principal do servidor de chat.
/// Responsável por aceitar ligações de múltiplos clientes e reencaminhar mensagens.
/// </summary>
class Program
{
    // Porta onde o servidor vai escutar
    private const int PORT = 5000;

    // Lista de clientes ligados (thread-safe com lock)
    private static List<ClientHandler> clients = new List<ClientHandler>();
    private static readonly object clientsLock = new object();

    static void Main(string[] args)
    {
        Console.WriteLine("=== SERVIDOR DE CHAT ===");
        Console.WriteLine($"A iniciar na porta {PORT}...");

        TcpListener listener = new TcpListener(IPAddress.Any, PORT);
        listener.Start();

        Console.WriteLine("Servidor iniciado. À espera de clientes...\n");

        // Loop infinito para aceitar novos clientes
        while (true)
        {
            // Aceita uma nova ligação (bloqueia até alguém ligar)
            TcpClient tcpClient = listener.AcceptTcpClient();
            Console.WriteLine($"[+] Novo cliente ligado: {tcpClient.Client.RemoteEndPoint}");

            // Cria um handler para este cliente e arranca numa thread separada
            ClientHandler handler = new ClientHandler(tcpClient);

            lock (clientsLock)
            {
                clients.Add(handler);
            }

            Thread t = new Thread(handler.Handle);
            t.IsBackground = true;
            t.Start();
        }
    }

    /// <summary>
    /// Envia uma mensagem a todos os clientes ligados, exceto ao remetente.
    /// </summary>
    /// <param name="message">Mensagem a reencaminhar</param>
    /// <param name="sender">Cliente que enviou a mensagem (para não lhe reenviar)</param>
    public static void Broadcast(string message, ClientHandler sender)
    {
        lock (clientsLock)
        {
            foreach (ClientHandler client in clients)
            {
                if (client != sender && client.IsConnected)
                {
                    client.SendMessage(message);
                }
            }
        }
    }

    /// <summary>
    /// Remove um cliente da lista quando este se desliga.
    /// </summary>
    /// <param name="handler">Handler do cliente a remover</param>
    public static void RemoveClient(ClientHandler handler)
    {
        lock (clientsLock)
        {
            clients.Remove(handler);
        }
        Console.WriteLine($"[-] Cliente '{handler.Username}' desligado. Total: {clients.Count}");
    }
}

/// <summary>
/// Classe que gere a ligação individual com cada cliente.
/// Cada instância corre na sua própria thread.
/// </summary>
class ClientHandler
{
    private TcpClient tcpClient;
    private NetworkStream stream;
    private ProtocolSI protocolSI;

    // Nome do utilizador (definido quando entra no chat)
    public string Username { get; private set; } = "Desconhecido";

    // Indica se o cliente ainda está ligado
    public bool IsConnected { get; private set; } = true;

    /// <summary>
    /// Construtor - inicializa o handler com o TcpClient recebido.
    /// </summary>
    public ClientHandler(TcpClient client)
    {
        this.tcpClient = client;
        this.stream = client.GetStream();
        this.protocolSI = new ProtocolSI();
    }

    /// <summary>
    /// Método principal que corre na thread do cliente.
    /// Trata do registo do username e depois fica à escuta de mensagens.
    /// </summary>
    public void Handle()
    {
        try
        {
            // 1. Receber o username do cliente
            stream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);

            if (protocolSI.GetCmdType() == ProtocolSICmdType.USER_OPTION_1)
            {
                // USER_OPTION_1 é usado para enviar o username
                Username = protocolSI.GetStringFromData();
                Console.WriteLine($"[*] Cliente identificado como: '{Username}'");

                // Notificar todos os outros clientes
                Program.Broadcast($"[Sistema] '{Username}' entrou no chat.", this);

                // Confirmar ao cliente que foi aceite
                byte[] ok = protocolSI.Make(ProtocolSICmdType.ACK);
                stream.Write(ok, 0, ok.Length);
            }

            // 2. Loop de receção de mensagens
            while (IsConnected)
            {
                int bytesRead = stream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);

                if (bytesRead == 0) break; // Cliente desligou

                // Mensagem de chat normal
                if (protocolSI.GetCmdType() == ProtocolSICmdType.DATA)
                {
                    string message = protocolSI.GetStringFromData();
                    string fullMessage = $"[{Username}]: {message}";

                    Console.WriteLine($"  >> {fullMessage}");

                    // Reencaminhar a mensagem para todos os outros clientes
                    Program.Broadcast(fullMessage, this);
                }
                // Cliente pediu para sair
                else if (protocolSI.GetCmdType() == ProtocolSICmdType.EOT)
                {
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO] Cliente '{Username}': {ex.Message}");
        }
        finally
        {
            // Limpar e remover o cliente
            IsConnected = false;
            Program.Broadcast($"[Sistema] '{Username}' saiu do chat.", this);
            Program.RemoveClient(this);
            tcpClient.Close();
        }
    }

    /// <summary>
    /// Envia uma mensagem de texto para este cliente.
    /// </summary>
    /// <param name="message">Texto a enviar</param>
    public void SendMessage(string message)
    {
        try
        {
            byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, message);
            stream.Write(packet, 0, packet.Length);
        }
        catch
        {
            IsConnected = false;
        }
    }
}