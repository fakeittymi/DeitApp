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
    public partial class AuthForm : Form
    {
        public AuthForm()
        {
            InitializeComponent();      
        }

        private void logInBtn_Click(object sender, EventArgs e)
        {
            string login = loginTextBox.Text;
            string password = pasTextBox.Text;
            if (String.IsNullOrEmpty(login) || String.IsNullOrEmpty(password))
            {
                MessageBox.Show("Данные введены некорректно");
                return;
            }

            AuthManager auth = new AuthManager();
            if (auth.Authorize(login, password))
            {
                MessageBox.Show("Успешно!");
                MainForm form = new MainForm(auth.GetUserId());
                form.FormClosed += (sender1, e1) =>
                {
                    if (form.closedByButton)
                    {
                        pasTextBox.Text = "";
                        this.Show();
                    }
                    else
                    {
                        Application.Exit();
                    }
                };
                form.Show();
                this.Hide();           
            }
            else
            {
                MessageBox.Show("Логин или пароль введены неверно");
                pasTextBox.Text = "";
            }
        }

        private void registerBtn_Click(object sender, EventArgs e)
        {
            RegistryForm regForm = new RegistryForm();
            regForm.Show();
        }
    }
}
