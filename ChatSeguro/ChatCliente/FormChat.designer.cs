namespace ChatCliente
{
    /// <summary>
    /// Designer do FormChat.
    /// Tudo num único formulário: campo de username + botão ligar no topo,
    /// área de chat no centro, e campo de envio em baixo.
    /// Após ligar, o username fica sempre visível na barra de topo.
    /// </summary>
    partial class FormChat
    {
        private System.ComponentModel.IContainer components = null;

        // Barra de topo
        private System.Windows.Forms.Panel panelTopo;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Button btnLigar;
        private System.Windows.Forms.Label lblUsernameAtivo;

        // Área do chat
        private System.Windows.Forms.TextBox txtChat;

        // Barra de envio
        private System.Windows.Forms.Panel panelRodape;
        private System.Windows.Forms.TextBox txtMensagem;
        private System.Windows.Forms.Button btnEnviar;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// Inicializa e posiciona todos os controlos do formulário.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelTopo = new System.Windows.Forms.Panel();
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.btnLigar = new System.Windows.Forms.Button();
            this.lblUsernameAtivo = new System.Windows.Forms.Label();
            this.txtChat = new System.Windows.Forms.TextBox();
            this.panelRodape = new System.Windows.Forms.Panel();
            this.txtMensagem = new System.Windows.Forms.TextBox();
            this.btnEnviar = new System.Windows.Forms.Button();

            this.panelTopo.SuspendLayout();
            this.panelRodape.SuspendLayout();
            this.SuspendLayout();

            // ── panelTopo ──
            this.panelTopo.BackColor = System.Drawing.Color.FromArgb(28, 28, 40);
            this.panelTopo.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTopo.Height = 52;
            this.panelTopo.Controls.Add(this.lblUsername);
            this.panelTopo.Controls.Add(this.txtUsername);
            this.panelTopo.Controls.Add(this.btnLigar);
            this.panelTopo.Controls.Add(this.lblUsernameAtivo);
            // linha divisória em baixo do topo
            this.panelTopo.Paint += (s, e) => {
                e.Graphics.DrawLine(
                    new System.Drawing.Pen(System.Drawing.Color.FromArgb(50, 50, 70)),
                    0, this.panelTopo.Height - 1,
                    this.panelTopo.Width, this.panelTopo.Height - 1);
            };

            // ── lblUsername ──
            this.lblUsername.Text = "Username:";
            this.lblUsername.ForeColor = System.Drawing.Color.FromArgb(180, 180, 200);
            this.lblUsername.Font = new System.Drawing.Font("Segoe UI", 9f);
            this.lblUsername.Location = new System.Drawing.Point(12, 17);
            this.lblUsername.AutoSize = true;

            // ── txtUsername ──
            this.txtUsername.Location = new System.Drawing.Point(88, 13);
            this.txtUsername.Size = new System.Drawing.Size(160, 24);
            this.txtUsername.Font = new System.Drawing.Font("Segoe UI", 10f);
            this.txtUsername.BackColor = System.Drawing.Color.FromArgb(38, 38, 55);
            this.txtUsername.ForeColor = System.Drawing.Color.White;
            this.txtUsername.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUsername.MaxLength = 20;
            this.txtUsername.Text = "User1";

            // ── btnLigar ──
            this.btnLigar.Text = "Ligar";
            this.btnLigar.Location = new System.Drawing.Point(260, 11);
            this.btnLigar.Size = new System.Drawing.Size(75, 28);
            this.btnLigar.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
            this.btnLigar.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.btnLigar.ForeColor = System.Drawing.Color.White;
            this.btnLigar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLigar.FlatAppearance.BorderSize = 0;
            this.btnLigar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLigar.Click += new System.EventHandler(this.btnLigar_Click);

            // ── lblUsernameAtivo (aparece depois de ligar, substitui os campos) ──
            this.lblUsernameAtivo.Text = "";
            this.lblUsernameAtivo.ForeColor = System.Drawing.Color.White;
            this.lblUsernameAtivo.Font = new System.Drawing.Font("Segoe UI", 12f, System.Drawing.FontStyle.Bold);
            this.lblUsernameAtivo.Location = new System.Drawing.Point(12, 13);
            this.lblUsernameAtivo.AutoSize = true;
            this.lblUsernameAtivo.Visible = false;

            // ── txtChat ──
            this.txtChat.Multiline = true;
            this.txtChat.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtChat.ReadOnly = true;
            this.txtChat.BackColor = System.Drawing.Color.FromArgb(18, 18, 26);
            this.txtChat.ForeColor = System.Drawing.Color.FromArgb(215, 215, 230);
            this.txtChat.Font = new System.Drawing.Font("Consolas", 10.5f);
            this.txtChat.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtChat.Dock = System.Windows.Forms.DockStyle.Fill;

            // ── panelRodape ──
            this.panelRodape.BackColor = System.Drawing.Color.FromArgb(28, 28, 40);
            this.panelRodape.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelRodape.Height = 58;
            this.panelRodape.Controls.Add(this.txtMensagem);
            this.panelRodape.Controls.Add(this.btnEnviar);
            this.panelRodape.Paint += (s, e) => {
                e.Graphics.DrawLine(
                    new System.Drawing.Pen(System.Drawing.Color.FromArgb(50, 50, 70)),
                    0, 0, this.panelRodape.Width, 0);
            };

            // ── txtMensagem ──
            this.txtMensagem.Location = new System.Drawing.Point(12, 13);
            this.txtMensagem.Size = new System.Drawing.Size(490, 30);
            this.txtMensagem.Font = new System.Drawing.Font("Segoe UI", 11f);
            this.txtMensagem.BackColor = System.Drawing.Color.FromArgb(38, 38, 55);
            this.txtMensagem.ForeColor = System.Drawing.Color.White;
            this.txtMensagem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMensagem.Enabled = false;
            this.txtMensagem.PlaceholderText = "Escreve uma mensagem...";
            this.txtMensagem.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMensagem_KeyDown);

            // ── btnEnviar ──
            this.btnEnviar.Text = "Enviar ➤";
            this.btnEnviar.Location = new System.Drawing.Point(515, 11);
            this.btnEnviar.Size = new System.Drawing.Size(95, 34);
            this.btnEnviar.Font = new System.Drawing.Font("Segoe UI", 9.5f, System.Drawing.FontStyle.Bold);
            this.btnEnviar.BackColor = System.Drawing.Color.FromArgb(0, 120, 215);
            this.btnEnviar.ForeColor = System.Drawing.Color.White;
            this.btnEnviar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEnviar.FlatAppearance.BorderSize = 0;
            this.btnEnviar.Enabled = false;
            this.btnEnviar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnEnviar.Click += new System.EventHandler(this.btnEnviar_Click);

            // ── FormChat ──
            this.Text = "Chat Seguro";
            this.ClientSize = new System.Drawing.Size(630, 520);
            this.MinimumSize = new System.Drawing.Size(500, 400);
            this.BackColor = System.Drawing.Color.FromArgb(18, 18, 26);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Controls.Add(this.txtChat);
            this.Controls.Add(this.panelRodape);
            this.Controls.Add(this.panelTopo);

            this.panelTopo.ResumeLayout(false);
            this.panelRodape.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}