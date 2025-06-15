using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace stupid_project
{
    public partial class browseGames : Form
    {
        public browseGames()
        {
            InitializeComponent();
        }

        private void browseGames_Load(object sender, EventArgs e)
        {
            // Load the games based on the currently selected genre in the ComboBox
            if (comboBox1.SelectedItem != null)
            {
                LoadGamesByGenre(comboBox1.SelectedItem.ToString());
            }
            else
            {
                // Fallback to "all" if nothing is selected yet
                LoadGamesByGenre("all");
            }

            // Beautify the table
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.RowTemplate.Height = 40;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 11);
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
            dataGridView1.EnableHeadersVisualStyles = false;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Filter the games when the selected genre changes
            string selectedGenre = comboBox1.SelectedItem.ToString();
            LoadGamesByGenre(selectedGenre);
        }

        private void LoadGamesByGenre(string genre)
        {
            string connStr = @"Server=.\SQLEXPRESS;Initial Catalog=gamestore_proj;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;";

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                string query;
                SqlCommand cmd;

                if (genre == "all")
                {
                    query = @"SELECT g.title, g.price, g.gameDescription, 
                                     ISNULL(i.stockQuantity, 0) AS stockQuantity
                              FROM game g
                              LEFT JOIN inventory i ON g.gameID = i.gameID";
                    cmd = new SqlCommand(query, con);
                }
                else
                {
                    query = @"SELECT g.title, g.price, g.gameDescription, 
                                     ISNULL(i.stockQuantity, 0) AS stockQuantity
                              FROM game g
                              LEFT JOIN inventory i ON g.gameID = i.gameID
                              WHERE g.genre = @gamegenre";
                    cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@gamegenre", genre);
                }

                SqlDataReader reader = cmd.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(reader);
                dataGridView1.DataSource = table;

                // Rename the stock column header
                if (dataGridView1.Columns.Contains("stockQuantity"))
                {
                    dataGridView1.Columns["stockQuantity"].HeaderText = "Available 💾";
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignore clicks on header row or outside the bounds
            if (e.RowIndex < 0 || e.RowIndex >= dataGridView1.Rows.Count)
                return;

            // Get the clicked row
            DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];

            // Get the game title and description (adjust column names if needed)
            string title = selectedRow.Cells["title"].Value.ToString();
            string description = selectedRow.Cells["gameDescription"].Value.ToString();

            // Show it in a MessageBox
            MessageBox.Show(description, $"📖 About '{title}'", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            dashboard dashForm = new dashboard(SessionManager.CurrentUserID); // Create an instance of the form
            dashForm.Show(); // Show the register form
        }

        private void label4_Click(object sender, EventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void label5_Click(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Get the game name and quantity from textboxes
            string gameTitle = textBox1.Text.Trim();
            string quantityText = textBox2.Text.Trim();

            // Check if both textboxes are filled
            if (string.IsNullOrEmpty(gameTitle) || string.IsNullOrEmpty(quantityText))
            {
                MessageBox.Show("Please enter both the game name and quantity.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if the quantity is a valid number
            if (!int.TryParse(quantityText, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Please enter a valid quantity.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string connStr = @"Server=.\SQLEXPRESS;Initial Catalog=gamestore_proj;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;";

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // Retrieve the game details based on the game title
                string query = "SELECT gameID, price FROM game WHERE title = @gameTitle";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@gameTitle", gameTitle);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read()) // If the game exists in the database
                {
                    int gameID = (int)reader["gameID"];
                    decimal price = (decimal)reader["price"];

                    // Calculate the total amount (price * quantity)
                    decimal totalAmount = price * quantity;

                    // Close the reader to move on to the next query
                    reader.Close();

                    // Insert the order into the orders table with quantity
                    string insertOrderQuery = "INSERT INTO orders (userID, orderDate, totalAmount, orderStatus, gameID, quantity) VALUES (@userID, @orderDate, @totalAmount, @orderStatus, @gameID, @quantity)";
                    SqlCommand insertCmd = new SqlCommand(insertOrderQuery, con);
                    insertCmd.Parameters.AddWithValue("@userID", SessionManager.CurrentUserID);
                    insertCmd.Parameters.AddWithValue("@orderDate", DateTime.Now);
                    insertCmd.Parameters.AddWithValue("@totalAmount", totalAmount);
                    insertCmd.Parameters.AddWithValue("@orderStatus", "Pending");
                    insertCmd.Parameters.AddWithValue("@gameID", gameID);
                    insertCmd.Parameters.AddWithValue("@quantity", quantity); // 🔥 This line makes your trigger work

                    insertCmd.ExecuteNonQuery();

                    MessageBox.Show("Game added to your cart!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Game not found. Please check the game title.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

    }
}
