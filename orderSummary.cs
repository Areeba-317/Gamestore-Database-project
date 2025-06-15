using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace stupid_project
{
    public partial class orderSummary : Form
    {
        private int userID;

        public orderSummary(int userID)
        {
            InitializeComponent();
            this.userID = userID;
        }

        private void orderSummary_Load(object sender, EventArgs e)
        {
            string connStr = @"Server=.\SQLEXPRESS;Initial Catalog=gamestore_proj;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;";
            SqlConnection con = new SqlConnection(connStr);
            con.Open();

            string query = @"
                SELECT 
                    g.title AS [Game Title], 
                    o.orderDate AS [Date], 
                    o.totalAmount AS [Total Amount], 
                    o.quantity AS [Quantity], 
                    o.orderStatus AS [Status]
                FROM orders o 
                INNER JOIN game g ON g.gameID = o.gameID  
                WHERE (o.orderStatus = 'Completed' OR o.orderStatus = 'Cancelled') 
                AND o.userID = @id";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id", userID);

            SqlDataReader reader = cmd.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            dataGridView1.DataSource = table;

            con.Close();

            StyleDataGrid(); // Apply that glam styling
        }

        private void StyleDataGrid()
        {
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.DarkSlateBlue;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dataGridView1.DefaultCellStyle.BackColor = Color.White;
            dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.Lavender;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;

            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.GhostWhite;
            dataGridView1.GridColor = Color.LightGray;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.BackgroundColor = Color.White;

            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            dashboard dashForm = new dashboard(SessionManager.CurrentUserID);
            dashForm.Show();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Optional: Interactions for click events if needed in the future
        }
    }
}
