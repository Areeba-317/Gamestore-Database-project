using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace stupid_project
{
    public partial class cart : Form
    {
        public cart()
        {
            InitializeComponent();
        }

        private void cart_Load(object sender, EventArgs e)
        {
            LoadCartData();
        }

        private void LoadCartData()
        {
            string connStr = @"Server=.\SQLEXPRESS;Initial Catalog=gamestore_proj;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;";
            SqlConnection con = new SqlConnection(connStr);
            con.Open();

            string query = @"SELECT o.orderID, g.title, o.quantity, o.totalAmount 
                             FROM orders o 
                             INNER JOIN game g ON g.gameID = o.gameID 
                             WHERE o.orderStatus = 'Pending' AND o.userID = @id";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", SessionManager.CurrentUserID);

            SqlDataReader reader = cmd.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            dataGridView1.DataSource = table;
            con.Close();

            StyleDataGrid();

            // Add the Delete button column if it doesn't exist
            if (!dataGridView1.Columns.Contains("Delete"))
            {
                DataGridViewButtonColumn deleteButton = new DataGridViewButtonColumn();
                deleteButton.Name = "Delete";
                deleteButton.HeaderText = "Delete";
                deleteButton.Text = "❌ Delete";
                deleteButton.UseColumnTextForButtonValue = true;
                deleteButton.DefaultCellStyle.ForeColor = Color.Red;
                deleteButton.DefaultCellStyle.BackColor = Color.MistyRose;
                deleteButton.DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                deleteButton.FlatStyle = FlatStyle.Popup;
                dataGridView1.Columns.Add(deleteButton);
            }
        }

        private void StyleDataGrid()
        {
            // ✨ Beautify the DataGridView
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.MidnightBlue;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dataGridView1.DefaultCellStyle.BackColor = Color.White;
            dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.LightCyan;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;

            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.AliceBlue;
            dataGridView1.GridColor = Color.LightGray;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.BackgroundColor = Color.White;

            // Center align content
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "Delete" && e.RowIndex >= 0)
            {
                int orderID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["orderID"].Value);

                var confirmResult = MessageBox.Show("Are you sure you want to remove this item from your cart?",
                                                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirmResult == DialogResult.Yes)
                {
                    DeleteCartItem(orderID);
                    LoadCartData();
                }
            }
        }

        private void DeleteCartItem(int orderID)
        {
            string connStr = @"Server=.\SQLEXPRESS;Initial Catalog=gamestore_proj;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;";
            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                string updateOrderStatusQuery = "UPDATE orders SET orderStatus = 'Cancelled' WHERE orderID = @id";
                SqlCommand updateCmd = new SqlCommand(updateOrderStatusQuery, con);
                updateCmd.Parameters.AddWithValue("@id", orderID);
                updateCmd.ExecuteNonQuery();

                string deleteQuery = "DELETE FROM orders WHERE orderID = @id";
                SqlCommand deleteCmd = new SqlCommand(deleteQuery, con);
                deleteCmd.Parameters.AddWithValue("@id", orderID);
                deleteCmd.ExecuteNonQuery();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            dashboard dashForm = new dashboard(SessionManager.CurrentUserID);
            dashForm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            payment payForm = new payment();
            payForm.Show();
        }
    }
}
