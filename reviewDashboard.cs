using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace stupid_project
{
    public partial class reviewDashboard : Form
    {
        public reviewDashboard()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            dashboard dashForm = new dashboard(SessionManager.CurrentUserID);
            dashForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // You can add some filter/search features here later if you want!
        }

        private void reviewDashboard_Load(object sender, EventArgs e)
        {
            string connStr = @"Server=.\SQLEXPRESS;Initial Catalog=gamestore_proj;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;";

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                string query = @"SELECT g.title AS [Game Title], g.gameDescription AS [Description] 
                                 FROM game g
                                 LEFT JOIN inventory i ON g.gameID = i.gameID";

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(reader);
                dataGridView1.DataSource = table;

                ApplyTableStyling();
            }
        }

        private void ApplyTableStyling()
        {
            // Header Styling
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.MidnightBlue;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Cell Styling
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dataGridView1.DefaultCellStyle.BackColor = Color.White;
            dataGridView1.DefaultCellStyle.ForeColor = Color.Black;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.LightSteelBlue;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;

            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.GhostWhite;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.GridColor = Color.LightGray;

            // Clickable title column styling
            if (dataGridView1.Columns.Contains("Game Title"))
            {
                dataGridView1.Columns["Game Title"].DefaultCellStyle.ForeColor = Color.RoyalBlue;
                dataGridView1.Columns["Game Title"].DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Underline);
            }

            // Center-align everything for aesthetic symmetry
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            dataGridView1.Cursor = Cursors.Hand;

            // Attach click event
            dataGridView1.CellClick += dataGridView1_CellClick;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridView1.Columns[e.ColumnIndex].Name == "Game Title")
            {
                string selectedGameTitle = dataGridView1.Rows[e.RowIndex].Cells["Game Title"].Value.ToString();
                writeReview reviewForm = new writeReview(selectedGameTitle);
                reviewForm.Show();
            }
        }
    }
}
