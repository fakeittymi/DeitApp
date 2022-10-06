using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeitApp.Forms
{
    public partial class RegistryForm : Form
    {
        public RegistryForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string login = loginTextBox.Text;
            string pas1 = pas1TextBox.Text;
            string pas2 = pas2TextBox.Text;

            if (String.IsNullOrEmpty(login) || String.IsNullOrEmpty(pas1) || String.IsNullOrEmpty(pas2))
            {
                MessageBox.Show("Пожалуйста, заполните все поля");
                return;
            }

            if (pas1 != pas2)
            {
                MessageBox.Show("Пароли не совпадают");
                return;
            }

            if (pas1.Length < 5 && pas1.Length > 16)
            {
                MessageBox.Show("Пароль должен быть от 6 до 16 символов");
                return;
            }    

            AuthManager auth = new AuthManager();
            if (auth.Register(login, pas1))
            {
                MessageBox.Show("Аккаунт успешно создан");
                this.Close();
            }
            else
            {
                MessageBox.Show("Пользователь с таким логином уже существует");
                return;
            }
        }
    }
}
