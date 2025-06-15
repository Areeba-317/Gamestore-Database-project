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
    public partial class payment : Form
    {
        public payment()
        {
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a payment method!");
                return;
            }

            string paymentMethod = comboBox1.SelectedItem.ToString();
            string connStr = @"Server=.\SQLEXPRESS;Initial Catalog=gamestore_proj;Integrated Security=True;Encrypt=False;TrustServerCertificate=True;";

            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();
                    string query = @"
                INSERT INTO payment (orderID, paymentStatus, paymentMethod)
                SELECT orderID, 'Completed', @method
                FROM orders
                WHERE orderStatus = 'Pending';";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@method", paymentMethod);
                        int rowsAffected = cmd.ExecuteNonQuery();

                        MessageBox.Show($"{rowsAffected} payment(s) completed!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Oops! Something went wrong:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            cart cartForm = new cart(); // Create an instance of the form
            cartForm.Show(); // Show the register form
        }
    }
}
