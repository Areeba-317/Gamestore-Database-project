using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace stupid_project
{
    public partial class analytics : Form
    {
        public analytics()
        {
            InitializeComponent();
        }
        private string connectionString = "Server=.\\SQLEXPRESS;Initial Catalog=gamestore_proj;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;"; 


        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            adminDash adminForm = new adminDash();
            adminForm.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string selectedReport = comboBox1.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedReport))
            {
                MessageBox.Show("Please select a report type!");
                return;
            }

            string query = null;

            if (selectedReport == "Top Selling Games")
            {
                query = @"SELECT g.title AS GameTitle, SUM(o.quantity) AS TotalSold
                  FROM orders o
                  JOIN game g ON o.gameID = g.gameID
                  GROUP BY g.title
                  ORDER BY TotalSold DESC;";
            }
            

            if (!string.IsNullOrEmpty(query))
            {
                LoadReportData(query);
            }
            else
            {
                MessageBox.Show("No query found for the selected report!");
            }
        }

        private void LoadReportData(string query)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dataGridView1.DataSource = dt;
                // Beautify the DataGridView ✨
                dataGridView1.EnableHeadersVisualStyles = false;
                dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.MidnightBlue;
                dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

                dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 10);
                dataGridView1.DefaultCellStyle.BackColor = Color.White;
                dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
                dataGridView1.DefaultCellStyle.SelectionBackColor = Color.LightSteelBlue;
                dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;

                dataGridView1.GridColor = Color.LightGray;
                dataGridView1.BorderStyle = BorderStyle.None;
                dataGridView1.RowHeadersVisible = false;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AllowUserToResizeRows = false;
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.BackgroundColor = Color.White;


                
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void analytics_Load(object sender, EventArgs e)
        {

        }
    }
}
