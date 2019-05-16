using ContactManager;
using CreateCustomer.API.DomainServices;
using System;
using System.Linq;
using System.Windows.Forms;

namespace ContactManagerTest
{
    public partial class OrderForm : Form
    {
        int contactOwnerKey = 29301; //ecotech
        //int contactKey = 149806; //daniel price

        //int contactOwnerKey = 4722;
        //int gregsContactKey = 20378; //greg
        //int mockContactOwnerKey = 99999999;


        public OrderForm()
        {
            InitializeComponent();
        }

        private void btnCustomer_Click(object sender, EventArgs e)
        {
            var contactManager = new ContactManagerForm();
            contactManager.Initialize(contactOwnerKey, "SALES");
            contactManager.ShowForm();
        }

        private void btnEmpty_Click(object sender, EventArgs e)
        {
            //var contactManager = new ContactManagerForm();
            //contactManager.Initialize(mockContactOwnerKey, "SALES");
            //contactManager.ShowDialog();
        }

        private void btnAccountingCustomer_Click(object sender, EventArgs e)
        {
            var contactManager = new ContactManagerForm();
            contactManager.Initialize(contactOwnerKey, "SALES");
            contactManager.ShowForm();
        }

        private void editContactControl1_Load(object sender, EventArgs e)
        {
            //contacts.SelectedContact = contacts.FirstOrDefault(c => c.ContactKey == gregsContactKey);

            //editContactControl1.Initialize(ref contacts);
        }
    }
}
