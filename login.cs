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
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();
        }


        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string password = textBox2.Text;

            string connStr = @"Server=.\SQLEXPRESS;Initial Catalog=gamestore_proj;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT userID, userRole FROM users WHERE username = @username AND pass_word = @password";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", password);

                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            string role = reader["userRole"].ToString();

                            // Set session data upon successful login
                            SessionManager.CurrentUserID = Convert.ToInt32(reader["userID"]);
                            SessionManager.CurrentUsername = username;
                            SessionManager.CurrentUserRole = reader["userRole"].ToString();

                            // Redirect based on role
                            if (role == "user")
                            {
                                dashboard userForm = new dashboard(SessionManager.CurrentUserID);
                                userForm.Show();
                            }
                            else
                            {
                                adminDash adminForm = new adminDash();
                                adminForm.Show();
                            }
                            

                            this.Hide(); // Hide login form
                        }
                        else
                        {
                            MessageBox.Show("Username or password is incorrect 🥲", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Something went wrong: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            
                this.Hide(); // Hide the login form
                register regForm = new register(); // Create an instance of the register form
                regForm.FormClosed += (s, args) => this.Show(); // Show login form again when register is closed (optional)
                regForm.Show(); // Show the register form

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void login_Load(object sender, EventArgs e)
        {

        }
    }
}
