using System;
using System.Data.SqlClient;
using System.Data;
using System.Reflection.Emit;
using System.Windows.Forms;

namespace stupid_project
{
    public partial class writeReview : Form
    {
        private string gameTitle;

        public writeReview(string gameTitle)
        {
            InitializeComponent();
            this.gameTitle = gameTitle;
        }

        private void writeReview_Load(object sender, EventArgs e)
        {
            string connStr = @"Server=.\SQLEXPRESS;Initial Catalog=gamestore_proj;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;";

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // First, get the gameID using the gameTitle
                string getGameIdQuery = "SELECT gameID FROM game WHERE title = @title";
                SqlCommand getGameIdCmd = new SqlCommand(getGameIdQuery, con);
                getGameIdCmd.Parameters.AddWithValue("@title", gameTitle);
                object result = getGameIdCmd.ExecuteScalar();

                if (result != null)
                {
                    int gameID = Convert.ToInt32(result);

                    // Now fetch reviews for this game and current user
                    string query = @"SELECT u.username, r.dateAdded, r.reviewText 
                             FROM review r 
                             INNER JOIN users u ON u.userID=r.userID
                             WHERE r.gameID = @gameid";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@userid", SessionManager.CurrentUserID);
                    cmd.Parameters.AddWithValue("@gameid", gameID);

                    SqlDataReader reader = cmd.ExecuteReader();
                    DataTable table = new DataTable();
                    table.Load(reader);
                    dataGridView1.DataSource = table;
                }
                else
                {
                    MessageBox.Show("Game not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                con.Close();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string review_text = textBox1.Text.Trim();

            if (string.IsNullOrEmpty(review_text))
            {
                MessageBox.Show("Please write a review before submitting.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connStr = @"Server=.\SQLEXPRESS;Initial Catalog=gamestore_proj;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;";

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // Get gameID
                string getGameIdQuery = "SELECT gameID FROM game WHERE title = @title";
                SqlCommand getGameIdCmd = new SqlCommand(getGameIdQuery, con);
                getGameIdCmd.Parameters.AddWithValue("@title", gameTitle);
                object result = getGameIdCmd.ExecuteScalar();

                if (result != null)
                {
                    int gameID = Convert.ToInt32(result);

                    // Insert review using parameters properly
                    string insertQuery = @"INSERT INTO review (userID, gameID, reviewText)
                                   VALUES (@userID, @gameID, @reviewText)";
                    SqlCommand cmd = new SqlCommand(insertQuery, con);
                    cmd.Parameters.AddWithValue("@userID", SessionManager.CurrentUserID);
                    cmd.Parameters.AddWithValue("@gameID", gameID);
                    cmd.Parameters.AddWithValue("@reviewText", review_text);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows > 0)
                    {
                        MessageBox.Show("Review submitted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBox1.Clear(); // clear after success
                        writeReview_Load(null, null); // refresh the DataGridView
                    }
                    else
                    {
                        MessageBox.Show("Something went wrong.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Game not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                con.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            dashboard dashForm = new dashboard(SessionManager.CurrentUserID);
            dashForm.Show();
        }
    }
}
