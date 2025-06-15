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
    public partial class adminDash : Form
    {
        //private int userID;
        public adminDash()
        {
            InitializeComponent();
            //this.userID = userID;
        }
        

        private void adminDash_Load(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Close();
            login loginForm = new login();
            loginForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            gameManagement gmForm = new gameManagement();
            gmForm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            users userForm = new users();
            userForm.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
            analytics analyticForm = new analytics();
            analyticForm.Show();

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
