using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeitApp
{
    public partial class MainForm : Form
    {
        public MainForm(Guid userId)
        {
            InitializeComponent();
            this.userId = userId;
        }

        public bool closedByButton = false;
        Guid userId;
        Users currentUser = new Users();
        Button currentBtn;
        Form activeForm;

        private void ActivateButton(object btnSender)
        {
            if (btnSender != null)
            {
                if (currentBtn != (Button)btnSender)
                {
                    DisableButton();
                    currentBtn = (Button)btnSender;
                    Color color = new Color();
                    color = Color.FromArgb(132, 11, 96);
                    currentBtn.BackColor = color;
                    currentBtn.ForeColor = Color.White;
                }
            }
        }

        private void DisableButton()
        {
            foreach (Control previousBtn in panelSideMenu.Controls)
            {
                if (previousBtn.GetType() == typeof(Button))
                {
                    previousBtn.BackColor = Color.FromArgb(51, 51, 76);
                    previousBtn.ForeColor = Color.Gainsboro;
                }
            }
        }

        private void OpenChildFom(Form childForm, object btnSender)
        {
            if (activeForm != null)
            {
                activeForm.Close();
            }
            ActivateButton(btnSender);
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            this.panelMain.Controls.Add(childForm);
            this.panelMain.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenChildFom(new Forms.MainPageForm(currentUser), sender);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenChildFom(new Forms.FoodForm(currentUser), sender);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Forms.ProfileForm profileForm = new Forms.ProfileForm(currentUser);
            OpenChildFom(profileForm, sender);
            profileForm.FormClosed += (sender1, e1) =>
            {
                currentUser = profileForm.GetUserInstance();
                labelGreetings.Text = $"Привет, {currentUser.Name}";
            };

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                using (BalancedDietEntities db = new BalancedDietEntities())
                {
                    currentUser = db.Users.Find(userId);
                }

                if (currentUser == null)
                {
                    Forms.InitialForm initialForm = new Forms.InitialForm(userId);
                    activeForm = initialForm;
                    initialForm.TopLevel = false;
                    initialForm.FormBorderStyle = FormBorderStyle.None;
                    initialForm.Dock = DockStyle.Fill;
                    this.panelMain.Controls.Add(initialForm);
                    this.panelMain.Tag = initialForm;
                    initialForm.BringToFront();
                    initialForm.FormClosed += (sender1, e1) =>
                    {
                        using (BalancedDietEntities db = new BalancedDietEntities())
                        {
                            currentUser = initialForm.GetUserInstance();
                            db.Users.Add(currentUser);
                            db.SaveChanges();
                        }                       
                        button1.Enabled = true;
                        button2.Enabled = true;
                        button3.Enabled = true;
                        labelGreetings.Text = $"Привет, {currentUser.Name}";
                    };
                    initialForm.Show();
                }
                else
                {
                    button1.Enabled = true;
                    button2.Enabled = true;
                    button3.Enabled = true;
                    labelGreetings.Text = $"Привет, {currentUser.Name}";
                    OpenChildFom(new Forms.MainPageForm(currentUser), button1);
                }
               
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы действительно хотите выйти?", "Предупреждение!", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                closedByButton = true;
                this.Close();
            }
        }
    }
}
