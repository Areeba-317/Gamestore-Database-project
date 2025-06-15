using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stupid_project
{
    public partial class gameManagement : Form
    {
        public gameManagement()
        {
            InitializeComponent();
        }

        private void gameManagement_Load(object sender, EventArgs e)
        {
            string connStr = @"Server=.\SQLEXPRESS;Initial Catalog=gamestore_proj;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;";

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                string query;
                SqlCommand cmd;

                    query = @"SELECT g.title,g.genre, g.price, g.gameDescription, 
                                     ISNULL(i.stockQuantity, 0) AS stockQuantity
                              FROM game g
                              LEFT JOIN inventory i ON g.gameID = i.gameID";
                    cmd = new SqlCommand(query, con);
                
                SqlDataReader reader = cmd.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(reader);
                dataGridView1.DataSource = table;
                // Apply a modern, clean style to the DataGridView
                dataGridView1.EnableHeadersVisualStyles = false;
                dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.MidnightBlue;
                dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 10);
                dataGridView1.DefaultCellStyle.BackColor = Color.White;
                dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
                dataGridView1.DefaultCellStyle.SelectionBackColor = Color.LightSkyBlue;
                dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;
                dataGridView1.RowTemplate.Height = 30;
                dataGridView1.GridColor = Color.LightGray;
                dataGridView1.BackgroundColor = Color.White;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.BorderStyle = BorderStyle.None;
                dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.MultiSelect = false;

                // Rename the stock column header
                if (dataGridView1.Columns.Contains("stockQuantity"))
                {
                    dataGridView1.Columns["stockQuantity"].HeaderText = "Available 💾";
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            adminDash adminForm = new adminDash();
            adminForm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string title = textBox1.Text.Trim();
            string description = textBox2.Text.Trim();
            string genre = textBox3.Text.Trim();
            string priceText = textBox4.Text.Trim();
            string quantityText = textBox5.Text.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Please enter the title of the game to update.");
                return;
            }

            string connStr = @"Server=.\SQLEXPRESS;Initial Catalog=gamestore_proj;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;";
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();
                SqlTransaction transaction = con.BeginTransaction();

                try
                {
                    List<string> updateFields = new List<string>();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.Transaction = transaction;

                    if (!string.IsNullOrWhiteSpace(description))
                    {
                        updateFields.Add("gameDescription = @desc");
                        cmd.Parameters.AddWithValue("@desc", description);
                    }

                    if (!string.IsNullOrWhiteSpace(genre))
                    {
                        updateFields.Add("genre = @genre");
                        cmd.Parameters.AddWithValue("@genre", genre);
                    }

                    if (!string.IsNullOrWhiteSpace(priceText))
                    {
                        if (!decimal.TryParse(priceText, out decimal price))
                        {
                            MessageBox.Show("Invalid price value.");
                            return;
                        }
                        updateFields.Add("price = @price");
                        cmd.Parameters.AddWithValue("@price", price);
                    }

                    if (updateFields.Count > 0)
                    {
                        string updateGameQuery = "UPDATE game SET " + string.Join(", ", updateFields) + " WHERE title = @title";
                        cmd.CommandText = updateGameQuery;
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.ExecuteNonQuery();
                    }

                    if (!string.IsNullOrWhiteSpace(quantityText))
                    {
                        if (!int.TryParse(quantityText, out int quantity))
                        {
                            MessageBox.Show("Invalid quantity value.");
                            return;
                        }

                        string updateInventoryQuery = @"
                    UPDATE inventory 
                    SET stockQuantity = @quantity 
                    WHERE gameID = (SELECT gameID FROM game WHERE title = @title)";
                        SqlCommand cmdInv = new SqlCommand(updateInventoryQuery, con, transaction);
                        cmdInv.Parameters.AddWithValue("@quantity", quantity);
                        cmdInv.Parameters.AddWithValue("@title", title);
                        cmdInv.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    MessageBox.Show("Game updated successfully!");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error during update: " + ex.Message);
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text) ||
                string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("Please fill in all the fields.");
                return;
            }

            if (!decimal.TryParse(textBox4.Text, out decimal price))
            {
                MessageBox.Show("Please enter a valid number for the price.");
                return;
            }

            if (!int.TryParse(textBox5.Text, out int quantity))
            {
                MessageBox.Show("Please enter a valid number for the quantity.");
                return;
            }

            string title = textBox1.Text.Trim();
            string description = textBox2.Text.Trim();
            string genre = textBox3.Text.Trim();

            string connStr = @"Server=.\SQLEXPRESS;Initial Catalog=gamestore_proj;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;";

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                SqlTransaction transaction = con.BeginTransaction(); // just in case anything fails

                try
                {
                    // Insert into game table
                    string insertGameQuery = @"INSERT INTO game (title, gameDescription, price, genre)
                                       VALUES (@title, @desc, @price, @genre);
                                       SELECT SCOPE_IDENTITY();"; // get gameID

                    SqlCommand cmdGame = new SqlCommand(insertGameQuery, con, transaction);
                    cmdGame.Parameters.AddWithValue("@title", title);
                    cmdGame.Parameters.AddWithValue("@desc", description);
                    cmdGame.Parameters.AddWithValue("@price", price);
                    cmdGame.Parameters.AddWithValue("@genre", genre);

                    int gameID = Convert.ToInt32(cmdGame.ExecuteScalar());

                    // Insert into inventory
                    string insertInventoryQuery = "INSERT INTO inventory (gameID, stockQuantity) VALUES (@gameID, @quantity)";
                    SqlCommand cmdInventory = new SqlCommand(insertInventoryQuery, con, transaction);
                    cmdInventory.Parameters.AddWithValue("@gameID", gameID);
                    cmdInventory.Parameters.AddWithValue("@quantity", quantity);
                    cmdInventory.ExecuteNonQuery();

                    transaction.Commit();

                    MessageBox.Show("Game added successfully with inventory!");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }


    }
}
