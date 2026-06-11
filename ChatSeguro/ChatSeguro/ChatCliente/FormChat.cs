using EI.SI;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace ChatCliente
{
    /// <summary>
    /// Formulário principal do cliente de chat.
    /// Permite definir um username, ligar ao servidor e trocar mensagens.
    /// O username fica guardado e visível durante toda a conversa.
    /// </summary>
    public partial class FormChat : Form
    {
        private const string SERVER_IP = "127.0.0.1";
        private const int SERVER_PORT = 5000;

        private TcpClient tcpClient;
        private NetworkStream stream;
        private ProtocolSI protocolSI;
        private Thread receiveThread;
        private bool isConnected = false;

        // Username guardado após ligar — usado em todas as mensagens
        private string username = "";

        /// <summary>
        /// Construtor — inicializa os componentes visuais.
        /// </summary>
        public FormChat()
        {
            InitializeComponent();
            this.FormClosing += FormChat_FormClosing;
        }

        // ─────────────────────────────────────────────
        //  LIGAÇÃO AO SERVIDOR
        // ─────────────────────────────────────────────

        /// <summary>
        /// Tenta ligar ao servidor com o username introduzido.
        /// Envia o username via USER_OPTION_1 e aguarda ACK de confirmação.
        /// Após ligação, o username fica guardado e visível na barra de topo.
        /// </summary>
        private void btnLigar_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text.Trim();

            if (string.IsNullOrEmpty(user))
            {
                MessageBox.Show("Introduz um username!", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Criar ligação TCP ao servidor
                tcpClient = new TcpClient(SERVER_IP, SERVER_PORT);
                stream = tcpClient.GetStream();
                protocolSI = new ProtocolSI();

                // Enviar o username ao servidor (USER_OPTION_1 = identificação)
                byte[] packet = protocolSI.Make(ProtocolSICmdType.USER_OPTION_1, user);
                stream.Write(packet, 0, packet.Length);

                // Aguardar ACK do servidor
                stream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);

                if (protocolSI.GetCmdType() == ProtocolSICmdType.ACK)
                {
                    // Guardar o username para usar em todas as mensagens
                    username = user;
                    isConnected = true;

                    // Atualizar UI: mostrar username no topo, desativar campos de login
                    lblUsernameAtivo.Text = $"👤  {username}";
                    lblUsernameAtivo.Visible = true;
                    btnLigar.Enabled = false;
                    txtUsername.Enabled = false;
                    btnEnviar.Enabled = true;
                    txtMensagem.Enabled = true;
                    txtMensagem.Focus();

                    AppendMessage($"✓ Ligado como '{username}'. Bem-vindo!\n");

                    // Arrancar thread de receção de mensagens
                    receiveThread = new Thread(ReceiveMessages);
                    receiveThread.IsBackground = true;
                    receiveThread.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao ligar: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ─────────────────────────────────────────────
        //  ENVIO DE MENSAGENS
        // ─────────────────────────────────────────────

        /// <summary>
        /// Envia a mensagem ao clicar no botão Enviar.
        /// </summary>
        private void btnEnviar_Click(object sender, EventArgs e)
        {
            EnviarMensagem();
        }

        /// <summary>
        /// Permite enviar mensagem carregando Enter na caixa de texto.
        /// </summary>
        private void txtMensagem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                EnviarMensagem();
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// Lê o texto da caixa, cria um pacote DATA com o protocolo SI e envia ao servidor.
        /// Mostra a mensagem localmente com o username guardado.
        /// </summary>
        private void EnviarMensagem()
        {
            if (!isConnected) return;

            string mensagem = txtMensagem.Text.Trim();
            if (string.IsNullOrEmpty(mensagem)) return;

            try
            {
                byte[] packet = protocolSI.Make(ProtocolSICmdType.DATA, mensagem);
                stream.Write(packet, 0, packet.Length);

                // Mostrar localmente com o username guardado
                AppendMessage($"[{username}]: {mensagem}");
                txtMensagem.Clear();
                txtMensagem.Focus();
            }
            catch (Exception ex)
            {
                AppendMessage($"[ERRO ao enviar]: {ex.Message}");
            }
        }

        // ─────────────────────────────────────────────
        //  RECEÇÃO DE MENSAGENS (thread separada)
        // ─────────────────────────────────────────────

        /// <summary>
        /// Corre numa thread separada.
        /// Fica em loop à espera de mensagens do servidor e mostra-as no chat.
        /// </summary>
        private void ReceiveMessages()
        {
            try
            {
                while (isConnected)
                {
                    int bytes = stream.Read(protocolSI.Buffer, 0, protocolSI.Buffer.Length);
                    if (bytes == 0) break;

                    if (protocolSI.GetCmdType() == ProtocolSICmdType.DATA)
                    {
                        string msg = protocolSI.GetStringFromData();
                        AppendMessage(msg);
                    }
                }
            }
            catch { }
            finally
            {
                isConnected = false;
                AppendMessage("\n[Sistema] Ligação ao servidor perdida.");
            }
        }

        // ─────────────────────────────────────────────
        //  UTILITÁRIOS
        // ─────────────────────────────────────────────

        /// <summary>
        /// Adiciona uma linha ao chat de forma thread-safe via Invoke.
        /// </summary>
        private void AppendMessage(string message)
        {
            if (txtChat.InvokeRequired)
            {
                txtChat.Invoke(new Action(() => AppendMessage(message)));
                return;
            }
            txtChat.AppendText(message + Environment.NewLine);
            txtChat.ScrollToCaret();
        }

        /// <summary>
        /// Ao fechar a janela, envia EOT ao servidor para desligar corretamente.
        /// </summary>
        private void FormChat_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isConnected && stream != null)
            {
                try
                {
                    byte[] eot = protocolSI.Make(ProtocolSICmdType.EOT);
                    stream.Write(eot, 0, eot.Length);
                }
                catch { }
                tcpClient?.Close();
            }
        }
    }
}