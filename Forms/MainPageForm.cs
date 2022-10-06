using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DeitApp.ProductManager;

namespace DeitApp.Forms
{
    public partial class MainPageForm : Form
    {
        public MainPageForm(Users user)
        {
            InitializeComponent();
            this.user = user;
        }

        Users user;

        private void MainPageForm_Load(object sender, EventArgs e)
        {
            ProductManager productManager = new ProductManager(user);
            DailyNorm daily = productManager.CalculateDailyNorm();
            label2.Text = $"{daily.calories} килокалорий";
            label3.Text = $"{(int)daily.proteins} г. белков";
            label4.Text = $"{(int)daily.fats} г. жиров";
            label5.Text = $"{(int)daily.carbons} г. углеводов";

            ViewRecomendations();
        }

        private void ViewRecomendations()
        {
            dataGridView1.Rows.Clear();
            using (BalancedDietEntities db = new BalancedDietEntities())
            {
                List<DietProducts> userRecomendation = db.DietProducts.Where(p => p.UserId == user.id).ToList();
                if (userRecomendation.Count != 0)
                {
                    for (int i = 1; i <= (userRecomendation.Count / 18); i++)
                    {
                        List<DietProducts> daySortedRec = userRecomendation.Where(p => p.DayNumber == i).ToList();
                        foreach (var product in daySortedRec)
                        {
                            if (product.Category.Contains("bf"))
                            {
                                product.Category = "Завтрак";
                                continue;
                            }

                            if (product.Category.Contains("lunch"))
                            {
                                product.Category = "Обед";
                                continue;
                            }

                            if (product.Category.Contains("dnr"))
                            {
                                product.Category = "Ужин";
                                continue;
                            }
                        }
                        foreach (var row in daySortedRec.OrderBy(r => r.Category))
                        {
                            dataGridView1.Rows.Add(row.Name, (int)row.Mass, row.Calories, row.DayNumber, row.Category);
                        }
                    }

                    dataGridView1.Visible = true;
                    label7.Visible = false;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {           
            ProductManager productManager = new ProductManager(user);
            int dayCount = (int)numericUpDown1.Value;
            productManager.GenerateRecomendation(dayCount);
            ViewRecomendations();
        }
    }
}
