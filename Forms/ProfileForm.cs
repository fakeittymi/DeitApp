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
    public partial class ProfileForm : Form
    {
        public ProfileForm(Users user)
        {
            InitializeComponent();
            this.user = user;
        }

        Users user;

        private void ProfileForm_Load(object sender, EventArgs e)
        {
            nameTextBox.Text = user.Name;
            ageTextBox.Text = user.Age.ToString();
            weightTextBox.Text = user.Weight.ToString();
            heightTextBox.Text = user.Height.ToString();
            for(int i = 0; i < UserService.activityLevels.Length; i++)
            {
                if (UserService.activityLevels[i] == user.Activity)
                {
                    comboBox1.SelectedIndex = i;
                    break;
                }
            }
            comboBox2.SelectedIndex = user.DietGoal;
            
        }

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
                user.Name = nameTextBox.Text;
                user.Age = Convert.ToInt32(ageTextBox.Text);
                user.Weight = Convert.ToInt32(weightTextBox.Text);
                user.Height = Convert.ToInt32(heightTextBox.Text);
                user.Activity = UserService.activityLevels[comboBox1.SelectedIndex];
                user.DietGoal = comboBox2.SelectedIndex;
                if (radioButton1.Checked)
                    user.Sex = UserService.MALECOEF;
                else
                    user.Sex = UserService.FEMALECOEF;
            }
            catch
            {
                MessageBox.Show("Данные введены неверно");
                return;
            }

            MessageBox.Show("Данные успешно сохранены");
            UserService userService = new UserService();
            userService.ActualizeUserData(user);
        }

        public Users GetUserInstance()
        {
            return user;
        }
    }
}
