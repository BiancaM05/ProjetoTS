using System;
using System.Windows.Forms;

namespace ChatCliente
{
    /// <summary>
    /// Ponto de entrada da aplicação cliente.
    /// Abre diretamente o formulário do chat.
    /// </summary>
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormChat());
            
        }
    }
}