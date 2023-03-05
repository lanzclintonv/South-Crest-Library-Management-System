using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace SouthCrestLMS
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            (new Login()).Show();
            this.Close();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'southCrestLibManSysDataSet3.Borrower_info' table. You can move, or remove it, as needed.
            this.borrower_infoTableAdapter.Fill(this.southCrestLibManSysDataSet3.Borrower_info);
            // TODO: This line of code loads data into the 'southCrestLibManSysDataSet2.Record_info' table. You can move, or remove it, as needed.
            this.record_infoTableAdapter.Fill(this.southCrestLibManSysDataSet2.Record_info);
            // TODO: This line of code loads data into the 'southCrestLibManSysDataSet1.Transaction_info' table. You can move, or remove it, as needed.
            this.transaction_infoTableAdapter.Fill(this.southCrestLibManSysDataSet1.Transaction_info);
            // TODO: This line of code loads data into the 'southCrestLibManSysDataSet.Book_info' table. You can move, or remove it, as needed.
            this.book_infoTableAdapter.Fill(this.southCrestLibManSysDataSet.Book_info);

        }

        LibManSysDataContext db;
        string editPin1;
        string editPin2;
        int returnPin;
        private void btnAdd1_Click(object sender, EventArgs e)
        {
            bool allow = AllCheckBook();
            if(allow)
            {
                db = new LibManSysDataContext();
                Book_info book = new Book_info();
                book.BookID = txtBID1.Text;
                book.Qty = Convert.ToInt32(nudQty1.Value);
                book.Title = txtTitle1.Text;
                book.Author = txtAuthor1.Text;
                book.Publisher = txtPublisher1.Text;
                book.ISBN = txtISBN1.Text;
                book.Category = cbCategory1.Text;
                book.DateAdded = DateTime.Now;
                if (nudQty1.Value == 0)
                {
                    book.Status = "Not Available";
                }
                else
                {

                    book.Status = "Available";
                }
                db.Book_infos.InsertOnSubmit(book);
                db.SubmitChanges();
                RefreshData();
                Clear1();
            }
        }

        private void bntEdit1_Click(object sender, EventArgs e)
        {
            db = new LibManSysDataContext();
            editPin1 = dgvBook.CurrentRow.Cells[0].Value.ToString();
            var query = db.Book_infos.Where(w => w.BookID == editPin1).FirstOrDefault();
            if (query.Status == "Available")
            {
                txtBID1.Text = Convert.ToString(query.BookID);
                txtTitle1.Text = query.Title;
                txtAuthor1.Text = query.Author;
                txtPublisher1.Text = query.Publisher;
                txtISBN1.Text = Convert.ToString(query.ISBN);
                cbCategory1.Text = query.Category;
                nudQty1.Value = query.Qty;
                btnSubmitChanges1.Visible = true;
                btnAdd1.Enabled = false;
                btnDelete1.Enabled = false;
                txtBID1.ReadOnly = true;
            }
            else
            {
                MessageBox.Show("This book cannot be edited!", "Book is borrowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            RefreshData();
        }

        private void btnDelete1_Click(object sender, EventArgs e)
        {
            db = new LibManSysDataContext();
            string pIn = dgvBook.CurrentRow.Cells[0].Value.ToString();
            var query = db.Book_infos.Where(w => w.BookID == pIn).FirstOrDefault();
            if (query.Status == "Available")
            {
                if (MessageBox.Show("Are You Sure You Want To Delete This?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    db.Book_infos.DeleteOnSubmit(query);
                    db.SubmitChanges();
                }
            }
            else
            {
                MessageBox.Show("This book cannot be deleted!", "Book is borrowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            RefreshData();
        }

        private void btnClear2_Click(object sender, EventArgs e)
        {
            Clear1();
        }

        private void btnSubmitChanges1_Click(object sender, EventArgs e)
        {

            bool allow = AllCheckBook();
            if (allow)
            {
                if (nudQty1.Value == 0)
                {
                    MessageBox.Show("You cannot set the quantity to 0", "Restrictions", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                else
                {
                    db = new LibManSysDataContext();
                    var book = db.Book_infos.Where(w => w.BookID == editPin1).FirstOrDefault();
                    book.BookID = txtBID1.Text;
                    book.Title = txtTitle1.Text;
                    book.Author = txtAuthor1.Text;
                    book.Publisher = txtPublisher1.Text;
                    book.ISBN = txtISBN1.Text;
                    book.Category = cbCategory1.Text;
                    book.Qty = Convert.ToInt32(nudQty1.Value);
                    db.SubmitChanges();
                    btnSubmitChanges1.Visible = false;
                    btnAdd1.Enabled = true;
                    btnDelete1.Enabled = true;
                    txtBID1.ReadOnly = false;
                    Clear1();
                    RefreshData();
                    editPin1 = "";
                }
            }
        }

        private void btnSearch1_Click(object sender, EventArgs e)
        {
            db = new LibManSysDataContext();
            string finder = txtSearch1.Text;
            var borrower = db.Borrower_infos.Where(w => w.ID == finder).FirstOrDefault();
            if(borrower == null)
            {
                MessageBox.Show("Please check again your ID and try again!", "Person cannot be found!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (borrower.Status == "OK")
                {
                    txtID1.Text = borrower.ID;
                    txtSurname1.Text = borrower.Surname;
                    txtFirstname1.Text = borrower.Firstname;
                    txtContactNo1.Text = borrower.CallNo;
                    txtAddress1.Text = borrower.Address;
                    txtGrade1.Text = borrower.Grade;
                    txtSection1.Text = borrower.Section;
                    txtType1.Text = borrower.Type;
                }
                else
                {
                    MessageBox.Show("This person cannot borrow at the moment!", "Currently borrowing a book", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            

        }

        private void btnBorrow_Click(object sender, EventArgs e)
        {
            if(txtID1.Text == "" || txtBID2.Text == "")
            {
                MessageBox.Show("Please choose a borrower and a book!", "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                db = new LibManSysDataContext();
                var query1 = db.Book_infos.Where(w => w.BookID == txtBID2.Text).FirstOrDefault();
                var query2 = db.Borrower_infos.Where(w => w.ID == txtID1.Text).FirstOrDefault();
                if(query1.Qty == 0)
                {
                    MessageBox.Show("Book is unavailable at the moment", "No more stocks", MessageBoxButtons.OK, MessageBoxIcon.Error);
                 }
                else
                {
                    Transaction_info borrow = new Transaction_info();
                    borrow.BookID = query1.BookID;
                    borrow.ID = query2.ID;
                    borrow.BorrowDate = DateTime.Now;
                    borrow.DueDate = DateTime.Now.AddDays(1);
                    borrow.FineAmount = 0;
                    borrow.FineStatus = "Not Returned";
                    db.Transaction_infos.InsertOnSubmit(borrow);
                    db.SubmitChanges();
                    query1.Qty--;
                    if (query1.Qty == 0)
                    {
                        query1.Status = "Not Available";
                    }
                    else
                    {
                        query1.Status = "Still Available";
                    }
                    query2.Status = "Omitted";
                    db.SubmitChanges();
                }
                RefreshData();
                Clear3();
                
            }
        }

        private void btnClear1_Click(object sender, EventArgs e)
        {
            Clear3();
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
           

        }

        private void btnClear3_Click(object sender, EventArgs e)
        {
            Clear4();
        }

        private void btnAdd2_Click(object sender, EventArgs e)
        {
            bool allow = AllCheckBorrower();
            if(allow)
            {
                db = new LibManSysDataContext();
                Borrower_info borrower = new Borrower_info();
                borrower.ID = txtID.Text;
                borrower.Type = cbType.Text;
                borrower.Surname = txtSurname3.Text;
                borrower.Firstname = txtFirstname3.Text;
                borrower.Grade = cbGrade.Text;
                borrower.Section = txtSection3.Text;
                borrower.CallNo = txtContactNo3.Text;
                borrower.Address = txtAddress3.Text;
                borrower.Status = "OK";
                db.Borrower_infos.InsertOnSubmit(borrower);
                db.SubmitChanges();
                RefreshData();
                Clear2();
            }
        }

        private void btnEdit2_Click(object sender, EventArgs e)
        {
            db = new LibManSysDataContext();
            editPin2 = dgvBorrower.CurrentRow.Cells[0].Value.ToString();
            var query = db.Borrower_infos.Where(w => w.ID == editPin2).FirstOrDefault();
            if(query.Status == "OK")
            {
                txtID.Text = query.ID;
                cbType.Text = query.Type;
                txtSurname3.Text = query.Surname;
                txtFirstname3.Text = query.Firstname;
                cbGrade.Text = query.Grade;
                txtSection3.Text = query.Section;
                txtContactNo3.Text = query.CallNo;
                txtAddress3.Text = query.Address;
                btnAdd2.Enabled = false;
                btnDelete2.Enabled = false;
                btnSubmitChanges2.Visible = true;
                txtID.ReadOnly = true;
                RefreshData();
            }
            else
            {
                MessageBox.Show("Borrower cannot be edited!", "Currently borrowing!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
        }

        private void btnDelete2_Click(object sender, EventArgs e)
        {
            db = new LibManSysDataContext();
            string pIn = dgvBorrower.CurrentRow.Cells[0].Value.ToString();
            var query = db.Borrower_infos.Where(w => w.ID == pIn).FirstOrDefault();
            if (query.Status == "OK")
            {
                if (MessageBox.Show("Are You Sure You Want To Delete This?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    db.Borrower_infos.DeleteOnSubmit(query);
                    db.SubmitChanges();
                }
            }
            else
            {
                MessageBox.Show("Borrower cannot be deleted!", "Currently borrowing!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            RefreshData();
        }
        private void dgvBorrow_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            db = new LibManSysDataContext();
            string borrowPin = dgvBorrow.CurrentRow.Cells[0].Value.ToString();
            var borrow = db.Book_infos.Where(w => w.BookID == borrowPin).FirstOrDefault();
            txtBID2.Text = borrow.BookID;
            txtTitle2.Text = borrow.Title;
            txtAuthor2.Text = borrow.Author;
            txtPublisher2.Text = borrow.Publisher;
            txtISBN2.Text = borrow.ISBN;
            txtCategory1.Text = borrow.Category;
        }
        private void btnClear4_Click(object sender, EventArgs e)
        {
            Clear2();
        }
        private void dgvReturn_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            db = new LibManSysDataContext();
            returnPin = Convert.ToInt32(dgvReturn.CurrentRow.Cells[0].Value.ToString());
            var borrow = db.Transaction_infos.Where(w => w.ReferenceID == returnPin).FirstOrDefault();
            txtBID3.Text = borrow.BookID;
            txtTitle3.Text = borrow.Book_info.Title;
            txtAuthor3.Text = borrow.Book_info.Author;
            txtPublisher3.Text = borrow.Book_info.Publisher;
            txtISBN3.Text = borrow.Book_info.ISBN;
            txtCategory2.Text = borrow.Book_info.Category;
            txtID2.Text = borrow.ID;
            txtSurname2.Text = borrow.Borrower_info.Surname;
            txtFirstname2.Text = borrow.Borrower_info.Firstname;
            txtContactNo2.Text = borrow.Borrower_info.CallNo;
            txtAddress2.Text = borrow.Borrower_info.Address;
            txtGrade2.Text = borrow.Borrower_info.Grade;
            txtSection2.Text = borrow.Borrower_info.Section;
            txtType2.Text = borrow.Borrower_info.Type;
            var ReturnDate = dtpReturnDate.Value;
            if(borrow.DueDate.Date == ReturnDate.Date || borrow.BorrowDate.Date == ReturnDate.Date)
            {
                txtFine.Text = "0";
                btnPay.Enabled = true;
            }
            else
            {
                var days = ReturnDate.Subtract(borrow.DueDate);
                int rate = Convert.ToInt32(days.TotalDays) -1;
                int fine = rate * 5;
                if(fine < 0)
                {
                    txtFine.Text = "Invalid Date";
                }
                else
                {
                    txtFine.Text = fine.ToString();
                }
                
            }
        }
        private void btnSubmitChanges2_Click(object sender, EventArgs e)
        {
            bool allow = AllCheckBorrower();
            if(allow)
            {
                db = new LibManSysDataContext();
                var borrower = db.Borrower_infos.Where(w => w.ID == editPin2).FirstOrDefault();
                borrower.ID = txtID.Text;
                borrower.Type = cbType.Text;
                borrower.Surname = txtSurname3.Text;
                borrower.Firstname = txtFirstname3.Text;
                borrower.Grade = cbGrade.Text;
                borrower.Section = txtSection3.Text;
                borrower.CallNo = txtContactNo3.Text;
                borrower.Address = txtAddress3.Text;
                borrower.Status = "OK";
                db.SubmitChanges();
                btnAdd2.Enabled = true;
                btnDelete2.Enabled = true;
                btnSubmitChanges2.Visible = false;
                txtID.ReadOnly = false;
                editPin2 = "";
                RefreshData();
                Clear2();
            }
            
        }
        private void btnPay_Click(object sender, EventArgs e)
        {
            btnPay.Enabled = false;
            var borrow = db.Transaction_infos.Where(w => w.ReferenceID == returnPin).FirstOrDefault();
            borrow.FineStatus = "Paid";
            if (txtID2.Text == "" || txtBID3.Text == "")
            {
                MessageBox.Show("Please choose a book to return!", "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                db = new LibManSysDataContext();
                var query = db.Transaction_infos.Where(w => w.ReferenceID == returnPin).FirstOrDefault();
                var books = db.Book_infos.Where(w => w.BookID == query.BookID).FirstOrDefault();
                var borrowers = db.Borrower_infos.Where(w => w.ID == query.ID).FirstOrDefault();
                string id = query.BookID;
                int refID = query.ReferenceID;
                bool finder = true;
                Record_info record = new Record_info();
                record.ReferenceID = refID;
                record.ReturnDate = DateTime.Now;
                //record.Fine = 0;
                record.BookID = txtBID3.Text;
                record.ID = txtID2.Text;
                db.Transaction_infos.DeleteOnSubmit(query);
                db.SubmitChanges();
                db.Record_infos.InsertOnSubmit(record);
                db.SubmitChanges();
                RefreshData();
                Clear4();
                books.Qty++;
                try
                {
                    var comp = db.Transaction_infos.Where(w => w.BookID == id).FirstOrDefault();
                    if (comp.BookID != id)
                    {
                        finder = false;
                    }
                }
                catch
                {
                    finder = false;
                }
                if (finder == true)
                {
                    books.Status = "Still Available";
                }
                else
                {
                    books.Status = "Available";
                }
                borrowers.Status = "OK";
                db.SubmitChanges();
                RefreshData();
            }
        }
        private void txtPay_ValueChanged(object sender, EventArgs e)
        {
            int fine = Convert.ToInt32(txtFine.Text);
            int pay = Convert.ToInt32(txtPay.Value);
            int change = pay - fine;
            if (change >= 0)
            {
                txtChange.Text = (change).ToString();
                btnPay.Enabled = true;
            }
            else
            {
                txtChange.Text = "Insufficient";
                btnPay.Enabled = false;
            }
        }
        private void dtpReturnDate_ValueChanged(object sender, EventArgs e)
        {
            db = new LibManSysDataContext();
            returnPin = Convert.ToInt32(dgvReturn.CurrentRow.Cells[0].Value.ToString());
            var ReturnDate = dtpReturnDate.Value;
            var borrow = db.Transaction_infos.Where(w => w.ReferenceID == returnPin).FirstOrDefault(); txtBID3.Text = borrow.BookID;
            txtTitle3.Text = borrow.Book_info.Title;
            txtAuthor3.Text = borrow.Book_info.Author;
            txtPublisher3.Text = borrow.Book_info.Publisher;
            txtISBN3.Text = borrow.Book_info.ISBN;
            txtCategory2.Text = borrow.Book_info.Category;
            txtID2.Text = borrow.ID;
            txtSurname2.Text = borrow.Borrower_info.Surname;
            txtFirstname2.Text = borrow.Borrower_info.Firstname;
            txtContactNo2.Text = borrow.Borrower_info.CallNo;
            txtAddress2.Text = borrow.Borrower_info.Address;
            txtGrade2.Text = borrow.Borrower_info.Grade;
            txtSection2.Text = borrow.Borrower_info.Section;
            txtType2.Text = borrow.Borrower_info.Type;
            if (borrow.DueDate.Date == ReturnDate.Date || borrow.BorrowDate.Date == ReturnDate.Date)
            {
                txtFine.Text = "0";
                btnPay.Enabled = true;
            }
            else
            {
                var days = ReturnDate.Subtract(borrow.DueDate);
                int rate = Convert.ToInt32(days.TotalDays) - 1;
                int fine = rate * 5;
                if (fine < 0)
                {
                    txtFine.Text = "Invalid Date";
                }
                else
                {
                    txtFine.Text = fine.ToString();
                }
            }
        }
        //Functions
        #region
        private bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }

        private bool IsSpecialChar(string str)
        {
            var regexItem = new Regex("^[a-zA-Z0-9 ,]*$");
            if (!regexItem.IsMatch(str))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool IsProperName(string str)
        {
            var regexItem = new Regex("^[a-zA-Z ,]*$");
            if (!regexItem.IsMatch(str))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private bool IsEmptyBook()
        {
            if (txtBID1.Text == "" || txtTitle1.Text == "" || txtPublisher1.Text == "" || txtISBN1.Text == "" || cbCategory1.Text == "")
            {
                MessageBox.Show("Please enter all of the fields!", "Missing fields!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool IsEmptyBorrower()
        {
            if (txtID.Text == "" || txtSurname3.Text == "" || txtFirstname3.Text == "" || txtSection3.Text == "" || cbType.Text == ""
                || cbGrade.Text == "" || txtContactNo3.Text == "" || txtAddress3.Text == "")
            {
                MessageBox.Show("Please enter all of the fields!", "Missing fields!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool CheckDigitsBook()
        {
            if (IsDigitsOnly(txtBID1.Text) && IsDigitsOnly(txtISBN1.Text))
            {
                return true;
            }
            else
            {
                MessageBox.Show("Book ID and ISBN should only contain numbers.");
                return false;
            }
        }

        private bool CheckDigitsBorrower()
        {
            if (IsDigitsOnly(txtID.Text) && IsDigitsOnly(txtContactNo3.Text))
            {
                return true;
            }
            else
            {
                MessageBox.Show("ID and Contact No should only contain numbers.");
                return false;
            }
        }

        private bool CheckCharBook()
        {
            if (IsSpecialChar(txtAuthor1.Text) && IsSpecialChar(txtPublisher1.Text))
            {
                return true;
            }
            else
            {
                MessageBox.Show("Special Characters are not allowed!", "Special Characters Detected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool CheckNameBook()
        {
            if(IsProperName(txtAuthor1.Text))
            {
                return true;
            }
            else
            {
                MessageBox.Show("Numbers and special characters are not allowed in Names", "Invalid Names", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool CheckNameBorrower()
        {
            if (IsProperName(txtFirstname3.Text) && IsProperName(txtSurname3.Text))
            {
                return true;
            }
            else
            {
                MessageBox.Show("Numbers and special characters are not allowed in Names", "Invalid Names", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool CheckCharBorrower()
        {
            if (IsSpecialChar(txtID.Text) && IsSpecialChar(txtSurname3.Text) && IsSpecialChar(txtSurname3.Text)
                && IsSpecialChar(txtFirstname3.Text) && IsSpecialChar(txtSection3.Text) && IsSpecialChar(txtContactNo3.Text))
            {
                return true;
            }
            else
            {
                MessageBox.Show("Special Characters are not allowed!", "Special Characters Detected", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool AllCheckBook()
        {
            if (IsEmptyBook())
            {
                if (CheckDigitsBook())
                {
                    if (CheckCharBook())
                    {
                        if (CheckNameBook())
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private bool AllCheckBorrower()
        {
            if (IsEmptyBorrower())
            {
                if (CheckDigitsBorrower())
                {
                    if (CheckCharBorrower())
                    {
                        if(CheckNameBook())
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void RefreshData()
        {
            // TODO: This line of code loads data into the 'southCrestLibManSysDataSet3.Borrower_info' table. You can move, or remove it, as needed.
            this.borrower_infoTableAdapter.Fill(this.southCrestLibManSysDataSet3.Borrower_info);
            // TODO: This line of code loads data into the 'southCrestLibManSysDataSet2.Record_info' table. You can move, or remove it, as needed.
            this.record_infoTableAdapter.Fill(this.southCrestLibManSysDataSet2.Record_info);
            // TODO: This line of code loads data into the 'southCrestLibManSysDataSet1.Transaction_info' table. You can move, or remove it, as needed.
            this.transaction_infoTableAdapter.Fill(this.southCrestLibManSysDataSet1.Transaction_info);
            // TODO: This line of code loads data into the 'southCrestLibManSysDataSet.Book_info' table. You can move, or remove it, as needed.
            this.book_infoTableAdapter.Fill(this.southCrestLibManSysDataSet.Book_info);
        }
        
        private void Clear1()
        {
            txtBID1.Text = "";
            txtTitle1.Text = "";
            txtAuthor1.Text = "";
            txtPublisher1.Text = "";
            txtISBN1.Text = "";
            cbCategory1.Text = "";
            nudQty1.Value = 1;
        }
        
        private void Clear2()
        {
            txtID.Text = "";
            cbType.Text = "";
            txtSurname3.Text = "";
            txtFirstname3.Text = "";
            cbGrade.Text = "";
            txtSection3.Text = "";
            txtContactNo3.Text = "";
            txtAddress3.Text = "";
        }

        private void Clear3()
        {
            txtID1.Text = "";
            txtSurname1.Text = "";
            txtFirstname1.Text = "";
            txtContactNo1.Text = "";
            txtAddress1.Text = "";
            txtGrade1.Text = "";
            txtSection1.Text = "";
            txtBID2.Text = "";
            txtTitle2.Text = "";
            txtAuthor2.Text = "";
            txtPublisher2.Text = "";
            txtISBN2.Text = "";
            txtCategory1.Text = "";
            txtSearch1.Text = "";
            txtType1.Text = "";
        }

        private void Clear4()
        {
            txtID2.Text = "";
            txtSurname2.Text = "";
            txtFirstname2.Text = "";
            txtContactNo2.Text = "";
            txtAddress2.Text = "";
            txtGrade2.Text = "";
            txtSection2.Text = "";
            txtBID3.Text = "";
            txtTitle3.Text = "";
            txtAuthor3.Text = "";
            txtPublisher3.Text = "";
            txtISBN3.Text = "";
            txtCategory2.Text = "";
            txtType2.Text = "";
            txtFine.Text = "";
            txtPay.Value = 0;
            txtChange.Text = "";
        }
        #endregion

        private void SupplementData()
        {
            db = new LibManSysDataContext();
            var data = db.Record_infos.Where(w => w.ReferenceID == 1).FirstOrDefault();
        }

        



        

        





    }
}
