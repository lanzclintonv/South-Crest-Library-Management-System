using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SouthCrestLMS
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (IsvalidUser(txtUsername.Text, txtPassword.Text))
            {
                (new Main()).Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Username or Password is invalid!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private bool IsvalidUser(string userName, string password)
        {
            LibManSysDataContext context = new LibManSysDataContext();
            var q = from p in context.User_infos
                    where p.Username == userName
                    && p.Password == password
                    select p;
            if (q.Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
