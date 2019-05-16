using ContactManager.CPMail;
using CreateCustomer.API.Entities;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace ContactManager
{
    internal class EmailSender
    {
        internal static void EmailUnexpectedException(Exception ex, Customer currentCustomer, string module = "")
        {
            if (Debugger.IsAttached) { throw new Exception(ex.Message + " " + module); }

            if (currentCustomer == null)
                currentCustomer = new Customer();

            string custKey = currentCustomer == null ? "cust is null" : currentCustomer.Key.ToString();
            string summary = string.Format($@"Unexpected exception occured. {Environment.NewLine} {Environment.NewLine}Current CustKey: {custKey}{Environment.NewLine} {Environment.NewLine}Exception message: {ex.Message + " " + module}{Environment.NewLine} {Environment.NewLine}Exception stack trace: {ex.StackTrace}");

            ServiceSoapClient client = new ServiceSoapClient();
            client.Email("op@caseparts.com", "dev@caseparts.com", "ContactManager Error", summary, false, "", "");
        }

        internal static void EmailHQPrimaryUnshared(Customer currentCustomer)
        {
            if (currentCustomer.IsStandAlone) return;

            Contact primaryUnsharedContact = currentCustomer.Contacts.FirstOrDefault(c => c.Key == currentCustomer.PrimaryCntctKey && !c.Shared);

            if (primaryUnsharedContact != null)
            {
                string custKey = currentCustomer == null ? "cust is null" : currentCustomer.Key.ToString();
                string summary = string.Format($"Primary contact is unshared on this account. {Environment.NewLine} {Environment.NewLine}Current CustKey: {custKey} {Environment.NewLine} {Environment.NewLine}Contact Key: {primaryUnsharedContact.Key} - Contact Name: {primaryUnsharedContact.Name} {Environment.NewLine}");

                if (Debugger.IsAttached) { MessageBox.Show(summary); return; }

                ServiceSoapClient client = new ServiceSoapClient();
                client.Email("op@caseparts.com", "dev@caseparts.com", "ContactManager Error", summary, false, "", "");
            }
        }


        internal static void EmailContactsWithNullModules(ref Customer currentCustomer, string role)
        {
            string custKey = currentCustomer == null ? "cust = null" : currentCustomer.Key.ToString();
            string summary = string.Format($@"These contacts have titles = null. {Environment.NewLine} {Environment.NewLine} 
                                              Current CustKey: {custKey} {Environment.NewLine} {Environment.NewLine}");

            var contactsWithNullTitles = currentCustomer.Contacts.Where(c => c.Module == null).ToList();
            contactsWithNullTitles.ForEach(c =>
            {
                c.Module = role;
                summary += string.Format($"Contact Key: {c.Key} - Contact Name: {c.Name} {Environment.NewLine}");
            });


            if (contactsWithNullTitles.Count > 0)
            {
                if (Debugger.IsAttached) { MessageBox.Show(summary); return; }

                ServiceSoapClient client = new ServiceSoapClient();
                client.Email("op@caseparts.com", "dev@caseparts.com", "ContactManager Error", summary, false, "", "");
            }
        }

        internal static void EmailNonAuthorizedUser(Contact currentContact)
        {
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            string userId = userName.Substring(userName.IndexOf(@"\") + 1);


            string summary = $"{userId} has created a new contact: {currentContact.Name} for accounting.";

            if (Debugger.IsAttached) { MessageBox.Show(summary); return; }

            ServiceSoapClient client = new ServiceSoapClient();
            client.Email("op@caseparts.com", "pams@caseparts.com, valeriem@caseparts.com, dev@caseparts.com", "ContactManager - Non Authorized User", summary, false, "", "");
        }
    }
}
