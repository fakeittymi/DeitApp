using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeitApp.Forms
{
    public partial class FoodForm : Form
    {
        public FoodForm(Users user)
        {
            InitializeComponent();
            this.user = user;
        }

        Users user;

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar)))
            {

                if (e.KeyChar != (char)Keys.Back)
                {
                    e.Handled = true;
                }

            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar)))
            {
                if (e.KeyChar != ',')
                {
                    if (e.KeyChar != (char)Keys.Back)
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar)))
            {
                if (e.KeyChar != ',')
                {
                    if (e.KeyChar != (char)Keys.Back)
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar)))
            {
                if (e.KeyChar != ',')
                {
                    if (e.KeyChar != (char)Keys.Back)
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        private void FoodForm_Load(object sender, EventArgs e)
        {
            using (BalancedDietEntities db = new BalancedDietEntities())
            {
                var bindingList = db.RestProducts.Select(p => p.Name).ToList();
                comboBox1.DataSource = bindingList;
                comboBox1.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                comboBox1.AutoCompleteSource = AutoCompleteSource.ListItems;

                bindingList = db.UserWLProducts.Where(p1 => p1.UserId == user.id).Select(p2 => p2.Name).ToList();
                comboBox3.DataSource = bindingList;
                comboBox3.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                comboBox3.AutoCompleteSource = AutoCompleteSource.ListItems;

                bindingList = db.SuggestionProducts.Select(p => p.Name).ToList();
                comboBox4.DataSource = bindingList;
                comboBox4.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                comboBox4.AutoCompleteSource = AutoCompleteSource.ListItems;

                bindingList.Clear();
                var prIdsList = db.UserBLProducts.Where(p1 => p1.UserId == user.id).Select(p2 => p2.ProductId).ToList();
                foreach (var prId in prIdsList)
                {
                    string prName = db.SuggestionProducts.Where(p => p.id == prId).FirstOrDefault().Name;
                    if (!bindingList.Contains(prName))
                        bindingList.Add(prName);
                }
                comboBox5.DataSource = bindingList;
                comboBox5.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                comboBox5.AutoCompleteSource = AutoCompleteSource.ListItems;
            }
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            comboBox1.DroppedDown = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (BalancedDietEntities db = new BalancedDietEntities())
            {
                if (String.IsNullOrEmpty(comboBox1.Text))
                {
                    MessageBox.Show("Введено неправильное название продукта");
                    return;
                }

                RestProducts chosenProduct = db.RestProducts.Where(p => p.Name == comboBox1.Text).FirstOrDefault();

                if (db.UserWLProducts.Where(p => p.UserId == user.id && p.Name == comboBox1.Text).FirstOrDefault() != null)
                {
                    listBox1.Items.Clear();
                    listBox1.Items.Add($"Продукт {chosenProduct.Name} уже имеется в вашем списке");
                    return;
                }

                if (chosenProduct != null)
                {
                    UserWLProducts userWLProduct = new UserWLProducts();
                    userWLProduct.id = Guid.NewGuid();
                    userWLProduct.UserId = user.id;
                    userWLProduct.Name = chosenProduct.Name;
                    userWLProduct.Calories = chosenProduct.Calories;
                    userWLProduct.Proteins = chosenProduct.Proteins;
                    userWLProduct.Fats = chosenProduct.Fats;
                    userWLProduct.Carbons = chosenProduct.Carbons;
                    userWLProduct.Category = chosenProduct.Category;
                    db.UserWLProducts.Add(userWLProduct);
                    db.SaveChanges();
                    var bindingList = db.UserWLProducts.Where(p1 => p1.UserId == user.id).Select(p2 => p2.Name).ToList();
                    comboBox3.DataSource = bindingList;
                    listBox1.Items.Add($"Продукт {userWLProduct.Name} добавлен");
                }
                else
                {
                    MessageBox.Show("Введено неправильное название продукта");
                }
            }
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            using (BalancedDietEntities db = new BalancedDietEntities())
            {
                listBox1.Items.Clear();
                RestProducts chosenProduct = db.RestProducts.Where(p => p.Name == comboBox1.Text).FirstOrDefault();
                listBox1.Items.Add("Белки " + chosenProduct.Proteins + " г");
                listBox1.Items.Add("Жиры " + chosenProduct.Fats + " г");
                listBox1.Items.Add("Углеводы " + chosenProduct.Carbons + " г");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (BalancedDietEntities db = new BalancedDietEntities())
            {
                try
                {
                    UserWLProducts userWLProduct = new UserWLProducts();
                    userWLProduct.id = Guid.NewGuid();
                    userWLProduct.UserId = user.id;
                    userWLProduct.Name = textBox1.Text;
                    userWLProduct.Calories = Convert.ToInt32(textBox2.Text);
                    userWLProduct.Proteins = (float)Convert.ToDouble(textBox3.Text);
                    userWLProduct.Fats = (float)Convert.ToDouble(textBox4.Text);
                    userWLProduct.Carbons = (float)Convert.ToDouble(textBox5.Text);
                    string category = ProductManager.FOODCATEGORIES[comboBox2.SelectedIndex];
                    userWLProduct.Category = category;
                    db.UserWLProducts.Add(userWLProduct);
                    //if (category.Contains("Grass") || category.Contains("Vegets"))
                    //{
                    //    category = category.Replace("lunch", "dnr");
                    //    userWLProduct.id = Guid.NewGuid();
                    //    userWLProduct.Category = category;
                    //    db.UserWLProducts.Add(userWLProduct);
                    //    db.SaveChanges();
                    //}
                    //if (category.Contains("Fruits"))
                    //{
                    //    category = category.Replace("bf", "dnr");
                    //    userWLProduct.id = Guid.NewGuid();
                    //    userWLProduct.Category = category;
                    //    db.UserWLProducts.Add(userWLProduct);
                    //    db.SaveChanges();
                    //}
                    db.SaveChanges();
                    var bindingList = db.UserWLProducts.Where(p1 => p1.UserId == user.id).Select(p2 => p2.Name).ToList();
                    comboBox3.DataSource = bindingList;
                    MessageBox.Show("Продукт добавлен");
                }
                catch
                {
                    MessageBox.Show("Введены неверные значения");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (BalancedDietEntities db = new BalancedDietEntities())
            {
                if (String.IsNullOrEmpty(comboBox3.Text))
                {
                    MessageBox.Show("Введено неправильное название продукта");
                    return;
                }

                UserWLProducts chosenProduct = db.UserWLProducts.Where(p => p.Name == comboBox3.Text).FirstOrDefault();

                if (chosenProduct != null)
                {
                    db.UserWLProducts.Remove(chosenProduct);
                    db.SaveChanges();
                    var bindingList = db.UserWLProducts.Where(p1 => p1.UserId == user.id).Select(p2 => p2.Name).ToList();
                    comboBox3.DataSource = bindingList;
                    listBox2.Items.Add($"Продукт {chosenProduct.Name} удален");
                }
                else
                {
                    MessageBox.Show("Введено неправильное название продукта");
                }
            }
        }

        private void comboBox3_TextChanged(object sender, EventArgs e)
        {
            comboBox3.DroppedDown = false;
        }

        private void comboBox3_SelectedValueChanged(object sender, EventArgs e)
        {
            using (BalancedDietEntities db = new BalancedDietEntities())
            {
                listBox2.Items.Clear();
                UserWLProducts chosenProduct = db.UserWLProducts.Where(p => p.Name == comboBox3.Text).FirstOrDefault();
                listBox2.Items.Add("Белки " + chosenProduct.Proteins + " г");
                listBox2.Items.Add("Жиры " + chosenProduct.Fats + " г");
                listBox2.Items.Add("Углеводы " + chosenProduct.Carbons + " г");
            }
        }

        private void comboBox4_TextChanged(object sender, EventArgs e)
        {
            comboBox4.DroppedDown = false;
        }

        private void comboBox5_TextChanged(object sender, EventArgs e)
        {
            comboBox4.DroppedDown = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (BalancedDietEntities db = new BalancedDietEntities())
            {
                if (String.IsNullOrEmpty(comboBox4.Text))
                {
                    MessageBox.Show("Введено неправильное название продукта");
                    return;
                }

                List<SuggestionProducts> chosenProducts = db.SuggestionProducts.Where(p => p.Name == comboBox4.Text).ToList();
                if (chosenProducts.Count != 0)
                {
                    foreach (var chosenProduct in chosenProducts)
                    {                       
                        if (chosenProduct != null)
                        {
                            UserBLProducts prod = new UserBLProducts();
                            prod.id = Guid.NewGuid();
                            prod.UserId = user.id;
                            prod.ProductId = chosenProduct.id;
                            db.UserBLProducts.Add(prod);
                            db.SaveChanges();
                        }
                        else
                        {
                            MessageBox.Show("Введено неправильное название продукта");
                        }
                    }
                    listBox3.Items.Add($"Продукт {comboBox4.Text} убран");
                    var bl = db.UserBLProducts.Where(p => p.UserId == user.id).Select(p2 => p2.ProductId).ToList();
                    List<string> bindingList = new List<string>();
                    bindingList = db.SuggestionProducts.Where(p => !bl.Contains(p.id)).Select(p2 => p2.Name).ToList();
                    comboBox4.DataSource = bindingList;

                    bindingList.Clear();
                    var prIdsList = db.UserBLProducts.Where(p1 => p1.UserId == user.id).Select(p2 => p2.ProductId).ToList();
                    foreach (var prId in prIdsList)
                    {
                        string prName = db.SuggestionProducts.Where(p => p.id == prId).FirstOrDefault().Name;
                        if (!bindingList.Contains(prName))
                            bindingList.Add(prName);
                    }
                    comboBox5.DataSource = bindingList;
                }
                else
                {
                    MessageBox.Show("Введено неправильное название продукта");
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (BalancedDietEntities db = new BalancedDietEntities())
            {              
                if (String.IsNullOrEmpty(comboBox5.Text))
                {
                    MessageBox.Show("Введено неправильное название продукта");
                    return;
                }

                List<Guid> chosenIds = db.SuggestionProducts.Where(p => p.Name == comboBox5.Text).Select(p => p.id).ToList();

                if (chosenIds.Count != 0)
                {
                    foreach (var chosenProduct in chosenIds)
                    {
                        UserBLProducts prod = db.UserBLProducts.Where(p => p.ProductId == chosenProduct).FirstOrDefault();
                        if (prod != null)
                            db.UserBLProducts.Remove(prod);
                    }
                    db.SaveChanges();
                }
                else
                {
                    MessageBox.Show("Введено неправильное название продукта");
                    return;
                }

                listBox4.Items.Add($"Продукт {comboBox5.Text} добавлен обратно в базу");

                List<string> bindingList = new List<string>();
                var prIdsList = db.UserBLProducts.Where(p1 => p1.UserId == user.id).Select(p2 => p2.ProductId).ToList();
                foreach (var prId in prIdsList)
                {
                    string prName = db.SuggestionProducts.Where(p => p.id == prId).FirstOrDefault().Name;
                    if (!bindingList.Contains(prName))
                        bindingList.Add(prName);
                }
                comboBox5.DataSource = bindingList;
                comboBox5.Text = "";
            }
        }
    }
}
