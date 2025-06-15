using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace stupid_project
{
    public partial class dashboard : Form
    {
        private int userID;
        public dashboard(int userID)
        {
            InitializeComponent();
            this.userID = userID;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            browseGames browseForm = new browseGames(); 
            browseForm.Show(); 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            cart cartForm = new cart(); // Create an instance of the form
            cartForm.Show(); // Show the register form
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            orderSummary summaryForm = new orderSummary(userID);
            summaryForm.Show();
        }

        private void dashboard_Load(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
            login loginForm = new login();
            loginForm.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
            reviewDashboard reviewForm = new reviewDashboard();
            reviewForm.Show();
        }
    }
}
