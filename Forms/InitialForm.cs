using System;
using System.Windows.Forms;

namespace DeitApp.Forms
{
    public partial class InitialForm : Form
    {
        public InitialForm(Guid userId)
        {
            InitializeComponent();
            userInstance.id = userId;
        }

        private Users userInstance = new Users();

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(nameTextBox.Text) || String.IsNullOrEmpty(ageTextBox.Text) ||
                String.IsNullOrEmpty(weightTextBox.Text) || String.IsNullOrEmpty(heightTextBox.Text) ||
                comboBox1.SelectedIndex == -1 || comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("Заполнены не все поля");
                return;
            }

            try
            {
                userInstance.Name = nameTextBox.Text;
                userInstance.Age = Convert.ToInt32(ageTextBox.Text);
                userInstance.Weight = Convert.ToInt32(weightTextBox.Text);
                userInstance.Height = Convert.ToInt32(heightTextBox.Text);
                userInstance.Activity = UserService.activityLevels[comboBox1.SelectedIndex];
                userInstance.DietGoal = comboBox2.SelectedIndex;
                if (radioButton1.Checked)
                    userInstance.Sex = UserService.MALECOEF;
                else
                    userInstance.Sex = UserService.FEMALECOEF;
            }
            catch
            {
                MessageBox.Show("Данные введены неверно");
                return;
            }

            MessageBox.Show("Данные успешно сохранены");
            this.Close();
        }

        public Users GetUserInstance()
        {
            return userInstance;
        }

    }
}
