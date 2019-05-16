using ContactManager.Properties;
using CPUserControls.ContactModule;
using CreateCustomer.API.DomainServices;
using CreateCustomer.API.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices.AccountManagement;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ContactManager
{
    public partial class ContactManagerForm : Form
    {
        #region PRIVATE FIELDS
        private Contact currentContact = new Contact();
        private Customer currentCustomer;
        private Contact cachedContact;

        private List<Contact> allContacts = new List<Contact>();
        private List<Contact> activeContacts = new List<Contact>();
        private List<Contact> deletedContacts = new List<Contact>();

        private CustomerService service;

        private ContactControl contactControl;
        private ContactController controller;
        private AccountType accountType;

        private int contactOwnerKey;
        private Module module;
        private bool loadingDataGrid;
        private bool isInSuperGroup;
        private bool isInPamsGroup;
        private bool primarySwapRequiredBeforeClose;
        private string userName;
        #endregion


        public ContactManagerForm()
        {
            InitializeComponent();
        }



        #region API
        public void Initialize(int _contactOwnerKey, Module _module, bool _primarySwapRequiredBeforeClose = false)
        {
            if (contactControl == null)
                contactControl = new ContactControl() { Dock = DockStyle.Fill };

            var failed = NotifyIfContactIsDirty();
            if (failed) return;

            primarySwapRequiredBeforeClose = _primarySwapRequiredBeforeClose;
            contactOwnerKey = _contactOwnerKey;
            module = _module;

            try
            {
                CheckPermissions();

                service = new CustomerService();
                currentCustomer = service.LoadCustomerWithDependenciesByKey(contactOwnerKey);

                EmailSender.EmailContactsWithNullModules(ref currentCustomer, module.ToString());
                EmailSender.EmailHQPrimaryUnshared(currentCustomer);


                allContacts = currentCustomer.Contacts.Where(c => c.Module == module.ToString() &&
                                                                ((c.ParentKey == null) || (c.Deleted == 0 && c.ParentKey != null)))
                                                                .OrderByDescending(c => c.Key == currentCustomer.PrimaryCntctKey)
                                                                .ThenByDescending(i => i.Shared)
                                                                .ThenBy(c => c.Name)
                                                                .ToList();

                activeContacts = allContacts.Where(c => c.IsDeleted == false).ToList();
                deletedContacts = allContacts.Where(c => c.IsDeleted == true).ToList();

                if (currentCustomer.IsStandAlone)
                    accountType = AccountType.StandAlone;
                if (currentCustomer.IsBranch)
                    accountType = AccountType.Branch;
                if (currentCustomer.IsHQ)
                    accountType = AccountType.HQ;

                controller = new ContactController(module, accountType, service);
            }
            catch (Exception ex)
            {
                EmailSender.EmailUnexpectedException(ex, currentCustomer, "Module: Initilize");
            }
        }

        private void CheckPermissions()
        {
            userName = Environment.UserName;

            using (var context = new PrincipalContext(ContextType.Domain, "caseparts"))
            {
                var arSuperGroup = GroupPrincipal.FindByIdentity(context, "ARSuper");
                var custCreationGroup = GroupPrincipal.FindByIdentity(context, "CustCreation");
                var devGroup = GroupPrincipal.FindByIdentity(context, "Developers");

                UserPrincipal user = UserPrincipal.FindByIdentity(context, userName);

                if (user != null)
                {
                    isInSuperGroup = user.IsMemberOf(arSuperGroup);
                    isInPamsGroup = user.IsMemberOf(custCreationGroup);
                }
                else
                {
                    isInSuperGroup = false;
                    isInPamsGroup = false;
                }

                if (user.IsMemberOf(devGroup))
                {
                    isInSuperGroup = true;
                    isInPamsGroup = true;
                }

                arSuperGroup.Dispose();
                custCreationGroup.Dispose();
            }
        }



        public void ShowForm(bool _modal = true)
        {
            if (_modal)
                ShowDialog();
            else
                Show();
        }

        public override void Refresh()
        {
            ContactManager_Load(this, new EventArgs());
        }

        bool statusExternallySet;
        string externalText;
        Color externalColor;
        public void SetStatusLabel(Color foreColor, string message)
        {
            externalText = message;
            externalColor = foreColor;
            statusExternallySet = true;
        }
        #endregion







        //-----------------------LOAD----------------------------
        private void ContactManager_Load(object sender, EventArgs e)
        {
            /*visibility based off permissions*/
            ShowAccountingControls();

            /*set user interface*/
            SetCountLabel();
            SetFormHeaderText(module.ToString());
            SetDefaultFilterCheckboxes();
            SetGridDataBindings(activeContacts);
            SetImageColumns();
            HighlightAlienContacts();
            ResizeForm();
            SetCustomerLabel();
            SetCurrentContactFromSelectedRow();
            SetFormToCurrentContact();
            SetControlsBasedOnPermissions();

            /*subscribe to the contact editor control's events*/
            contactControl.Done += ContactControl_Done;
            contactControl.Invalid += () => btnSave.Enabled = false;
            contactControl.EnterPressed += () => btnSave.PerformClick();
            contactControl.StatusChanged += ContactControl_StatusChanged;
            contactControl.HideStatus();
            panelContact.Controls.Add(contactControl);

            contactControl.SetStatus(Color.Green, $"Permissions set for {userName}");

            if (statusExternallySet)
            {
                statusLabel.Text = externalText;
                statusLabel.ForeColor = externalColor;
            }

            if (primarySwapRequiredBeforeClose)
            {
                btnClose.Enabled = false;
            }
        }


        private void HighlightAlienContacts()
        {
            if (currentCustomer.IsHQ) return;

            var contactsThatDontBelong = activeContacts.Where(c => c.Deleted == 0 && c.ParentKey != null && c.Shared == false);
            foreach (var contact in contactsThatDontBelong)
            {
                var index = GetCurrentList().FindIndex(c => c.Key == contact.Key);

                bool contactIsInDataGridView = index != -1;
                if (contactIsInDataGridView)
                {
                    var row = dgvContacts.Rows[index];

                    row.DefaultCellStyle.BackColor = Color.Yellow;
                    row.DefaultCellStyle.SelectionBackColor = Color.DarkGoldenrod;
                }
            }
        }

        private void SetDefaultFilterCheckboxes()
        {
            chkShowDeleted.CheckedChanged -= chkShowDeleted_CheckedChanged;
            chkShowDeleted.Checked = false;
            chkShowDeleted.CheckedChanged += chkShowDeleted_CheckedChanged;
        }

        private void SetImageColumns()
        {
            foreach (DataGridViewRow row in dgvContacts.Rows)
            {
                var boundContact = (Contact)row.DataBoundItem;

                row.Cells["Active"].Value = boundContact.IsDeleted ? Resources.singlex16x16 : Resources.singlecheckmark;
                row.Cells["Shared"].Value = boundContact.Shared ? Resources.singlecheckmark : Resources.singlex16x16;
            }
        }

        private void SetCountLabel()
        {
            var activeList = GetCurrentList();
            lblCount.Text = "Count: " + activeList.Count.ToString();
            lblCount.ForeColor = activeList.Count > 0 ? Color.Green : Color.Red;
        }

        private void ContactControl_StatusChanged(Color statusColor, string statusMessage)
        {
            statusLabel.Text = statusMessage;
            statusLabel.ForeColor = statusColor;
        }

        private void ContactControl_Done(Contact contact)
        {
            SetContactProperties(contact);
            btnSave.Enabled = true;
        }

        private void SetFormHeaderText(string activeRole)
        {
            Text = Text.Replace("(SO)", "").Replace("(AR)", "");
            Text = "(" + activeRole + ") " + Text.TrimStart();
        }

        private void SetCustomerLabel()
        {
            lblAccountInfo.Text = currentCustomer.Name + "(" + (currentCustomer.IsHQ ? "HQ" : "Branch") + ")";
            if (currentCustomer.IsStandAlone)
                lblAccountInfo.Visible = false;
        }

        private void SetGridDataBindings(List<Contact> _contacts)
        {
            loadingDataGrid = true;
            dgvContacts.DataSource = null;
            dgvContacts.DataSource = _contacts;
            loadingDataGrid = false;
        }

        private void ResizeForm()
        {
            int width = 0;
            foreach (DataGridViewColumn column in dgvContacts.Columns)
                if (column.Visible == true)
                    width += column.Width;

            width += 55;
            Size = new Size(width, this.Height);
        }

        public void SetCurrentContactFromSelectedRow()
        {
            if (dgvContacts.SelectedRows.Count == 0)
            {
                return;
            }
            else
            {
                int selectedIndex = dgvContacts.SelectedRows[0].Index;

                List<Contact> activeList = GetCurrentList();
                currentContact = activeList[selectedIndex];
            }
        }

        private void SetFormToCurrentContact()
        {
            var activeList = GetCurrentList();
            if (activeList.Count == 0)
            {
                SetContactToNewContact();
                ClearForm();
                return;
            }

            chkShared.CheckedChanged -= chkShared_CheckedChanged;
            rdoActive.CheckedChanged -= rdoActive_CheckedChanged;
            rdoDeleted.CheckedChanged -= rdoDeleted_CheckedChanged;

            contactControl.FullName = currentContact.Name;
            contactControl.Email = currentContact.Email;
            contactControl.Title = currentContact.Title;
            contactControl.Phone = currentContact.Phone;
            contactControl.PhoneExt = currentContact.PhoneExt;
            contactControl.Fax = currentContact.Fax;
            contactControl.FaxExt = currentContact.FaxExt;
            contactControl.Cell = currentContact.MobilePhone;
            contactControl.Refresh();

            chkShared.Checked = currentContact.Shared;

            rdoActive.Checked = currentContact.Deleted == null ? true : currentContact.Deleted == 0;
            rdoDeleted.Checked = !rdoActive.Checked;

            chkShared.CheckedChanged += chkShared_CheckedChanged;
            rdoActive.CheckedChanged += rdoActive_CheckedChanged;
            rdoDeleted.CheckedChanged += rdoDeleted_CheckedChanged;

            dgvContacts.Focus();
        }


        #region CONTROL VISIBILITY
        private void ShowAccountingControls()
        {
            //datagrid columns
            if (module == Module.AR)
            {
                var creditMemoColumn = dgvContacts.Columns["CreditMemo"];
                creditMemoColumn.Visible = true;

                var invoiceColumn = dgvContacts.Columns["Invoice"];
                invoiceColumn.Visible = true;

                var statementColumn = dgvContacts.Columns["Statement"];
                statementColumn.Visible = true;

                var debitMemoColumn = dgvContacts.Columns["DebitMemo"];
                debitMemoColumn.Visible = true;

                var primaryColumn = dgvContacts.Columns["Primary"];
                primaryColumn.Visible = true;

            }

            if ((module == Module.AR) && (currentCustomer.IsStandAlone))
            {
                var sharedColumn = dgvContacts.Columns["Shared"];
                sharedColumn.Visible = false;
            }
            else if (module == Module.AR)
            {
                var sharedColumn = dgvContacts.Columns["Shared"];
                sharedColumn.Visible = true;
            }


            //show deleted contacts checkbox
            if (!isInPamsGroup || module != Module.AR)
            {
                chkShowDeleted.Visible = false;
            }
            else
            {
                chkShowDeleted.Visible = true;
            }


            //change status - active delete
            if (!isInPamsGroup) 
            {
                grpStatus.Visible = false;
            }
            else if (module != Module.AR)
            {
                grpStatus.Visible = false;
            }
            else
            {
                grpStatus.Visible = true;
            }


            //shared controls
            if (currentCustomer.IsStandAlone || module != Module.AR)
            {
                lblShared.Visible = false;
                pictureBoxShared.Visible = false;
                chkShared.Visible = false;
            }
            else
            {
                lblShared.Visible = true;
                pictureBoxShared.Visible = true;
                chkShared.Visible = true;
            }


            //navigate link
            if (!isInSuperGroup || module != Module.AR)
            {
                linklblNavigate.Visible = false;
            }
            else if ((currentCustomer.IsHQ && cachedBranchKey != 0) || (currentCustomer.IsBranch))
            {
                linklblNavigate.Visible = true;
            }
            else
            {
                linklblNavigate.Visible = false;
            }

            //buttons
            if ((!isInPamsGroup && module == Module.AR) || (!isInSuperGroup && currentCustomer.IsHQ))
            {
                btnNew.Visible = false;
                btnSave.Visible = false;
            }
        }
        #endregion





        #region PERMISSIONS CONTROL STATE
        private void SetControlsBasedOnPermissions()
        {
            SetFormState();

            if (GetCurrentList().Count == 0)
            {
                SetContactToNewContact();
                ClearForm();
            }
            else
            {
                SetShareControlsState();
                SetStatusControlsState();
                SetNewButtonState();
                SetGridCellsState();
                SetDocTransmittalCellsState();
                DisableSharedContactsOnDataGrid();
            }
        }


        private void SetFormState()
        {
            if ((currentCustomer.IsHQ && !isInSuperGroup) || (!isInPamsGroup) || (currentCustomer.IsBranch && currentContact.Shared))
            {
                contactControl.EnableDisableForm(false);
            }
            else
            {
                contactControl.EnableDisableForm(true);
            }
        }


        private void SetShareControlsState()
        {
            if ((!isInSuperGroup && currentCustomer.IsHQ) || (currentContact.IsPrimary) || (currentCustomer.IsBranch))
            {
                lblShared.Enabled = false;
                pictureBoxShared.Enabled = false;
                chkShared.Enabled = false;
            }
            else
            {
                lblShared.Enabled = true;
                pictureBoxShared.Enabled = true;
                chkShared.Enabled = true;
            }
        }


        private void SetStatusControlsState()
        {
            if (currentContact.IsPrimary || (currentCustomer.IsBranch && currentContact.Shared) || (currentCustomer.IsHQ && !isInSuperGroup))
            {
                grpStatus.Enabled = false;
            }
            else
            {
                grpStatus.Enabled = true;
            }
        }


        private void SetNewButtonState()
        {
            if (currentCustomer.IsHQ && !isInSuperGroup)
            {
                btnNew.Enabled = false;
            }
        }


        private void SetGridCellsState()
        {
            if (dgvContacts.Rows.Count == 0) return;

            if ((currentCustomer.IsHQ && !isInSuperGroup) || (!isInPamsGroup)) //disable all cells
            {
                foreach (DataGridViewRow row in dgvContacts.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        cell.Style.BackColor = Color.LightGray;
                        cell.ReadOnly = true;
                        if (cell is DataGridViewCheckBoxCell)
                            (cell as DataGridViewCheckBoxCell).FlatStyle = FlatStyle.Flat;
                    }
                }

                dgvContacts.ReadOnly = true;
            }
            else if (currentCustomer.IsHQ && isInSuperGroup) //disable the primary cell for not shared contacts only
            {
                var activeList = GetCurrentList();
                foreach (var contact in activeList)
                {
                    if (!contact.Shared)
                    {
                        var row = dgvContacts.Rows.Cast<DataGridViewRow>().First(c => (int)c.Cells["Key"].Value == contact.Key);

                        var chkCell = (DataGridViewCheckBoxCell)row.Cells["Primary"];
                        chkCell.Style.BackColor = Color.LightGray;
                        chkCell.FlatStyle = FlatStyle.Flat;
                        chkCell.ReadOnly = true;
                    }
                }
            }
            else if (currentCustomer.IsHQ || currentCustomer.IsBranch || !isInPamsGroup) //disable primary cell for all
            {
                foreach (DataGridViewRow row in dgvContacts.Rows)
                {
                    var chkCell = (DataGridViewCheckBoxCell)row.Cells["Primary"];
                    chkCell.Style.BackColor = Color.LightGray;
                    chkCell.FlatStyle = FlatStyle.Flat;
                    chkCell.ReadOnly = true;
                }
            }
        }


        private void SetDocTransmittalCellsState()
        {
            if (!isInPamsGroup) return;
            if (!currentCustomer.IsHQ)
            {
                var primaryContact = allContacts.FirstOrDefault(c => c.Key == currentCustomer.PrimaryCntctKey);

                EnableDisableDocTransmittalCells("CreditMemo", primaryContact.CreditMemo);
                EnableDisableDocTransmittalCells("Statement", primaryContact.Statement);
                EnableDisableDocTransmittalCells("Invoice", primaryContact.Invoice);
                EnableDisableDocTransmittalCells("DebitMemo", primaryContact.DebitMemo);
            }
            else if (isInSuperGroup)
            {
                var primaryContact = allContacts.FirstOrDefault(c => c.Key == currentCustomer.PrimaryCntctKey);

                EnableDisableDocTransmittalCells("CreditMemo", primaryContact.CreditMemo);
                EnableDisableDocTransmittalCells("Statement", primaryContact.Statement);
                EnableDisableDocTransmittalCells("Invoice", primaryContact.Invoice);
                EnableDisableDocTransmittalCells("DebitMemo", primaryContact.DebitMemo);
            }
        }


        private void DisableSharedContactsOnDataGrid()
        {
            if (currentCustomer.IsBranch)
            {
                IEnumerable<Contact> sharedContacts = GetCurrentList().Where(c => c.Key == currentCustomer.PrimaryCntctKey || c.Shared == true);
                List<DataGridViewRow> sharedContactRows = new List<DataGridViewRow>();
                foreach (var contact in sharedContacts)
                {
                    sharedContactRows
                        .Add(dgvContacts.Rows.Cast<DataGridViewRow>().FirstOrDefault(c => (int)c.Cells["Key"].Value == contact.Key));
                }

                foreach (DataGridViewRow row in sharedContactRows)
                {
                    row.DefaultCellStyle.BackColor = Color.LightGray;
                    row.ReadOnly = true;

                    var creditmemoCheckCell = (DataGridViewCheckBoxCell)row.Cells["CreditMemo"];
                    creditmemoCheckCell.Style.BackColor = Color.LightGray;
                    creditmemoCheckCell.FlatStyle = FlatStyle.Flat;
                    creditmemoCheckCell.ReadOnly = true;

                    var invoiceCheckCell = (DataGridViewCheckBoxCell)row.Cells["Invoice"];
                    invoiceCheckCell.Style.BackColor = Color.LightGray;
                    invoiceCheckCell.FlatStyle = FlatStyle.Flat;
                    invoiceCheckCell.ReadOnly = true;

                    var statementCheckCell = (DataGridViewCheckBoxCell)row.Cells["Statement"];
                    statementCheckCell.Style.BackColor = Color.LightGray;
                    statementCheckCell.FlatStyle = FlatStyle.Flat;
                    statementCheckCell.ReadOnly = true;

                    var debitMemoCheckCell = (DataGridViewCheckBoxCell)row.Cells["DebitMemo"];
                    debitMemoCheckCell.Style.BackColor = Color.LightGray;
                    debitMemoCheckCell.FlatStyle = FlatStyle.Flat;
                    debitMemoCheckCell.ReadOnly = true;
                }
            }
        }
        #endregion






        #region CREATE NEW CONTACT
        private void btnNew_Click(object sender, EventArgs e)
        {
            var failed = NotifyIfContactIsDirty();
            if (failed) return;

            SetContactToNewContact();
            DisableSaveButton();
            contactControl.EnableDisableForm(true);
            ClearForm();
            contactControl.ValidateManually();
        }

        private void SetContactToNewContact()
        {
            currentContact = new Contact();
        }

        private void ClearForm()
        {
            contactControl.ClearForm();
            contactControl.CreateNewContact();
            chkShared.CheckedChanged -= chkShared_CheckedChanged;
            chkShared.Checked = false;
            chkShared.CheckedChanged += chkShared_CheckedChanged;
        }

        private void DisableSaveButton()
        {
            btnSave.Enabled = false;
        }
        #endregion






        #region SAVE CONTACT
        private void btnSave_Click(object sender, EventArgs e)
        {
            /*contact save*/
            NotifyIfEmailEmptyAndSave();

            /*update lists*/
            AddContactToLocalLists();
            RefreshContactLists();

            /*user interface*/
            SetGridDataBindings(GetCurrentList());
            HighlightAlienContacts();
            SetImageColumns();
            SetCountLabel();
            ResizeForm();
            SelectDGVRowFromCurrentContact();
            SetFormToCurrentContact();
            SetControlsBasedOnPermissions();

            DisableSaveButton();
        }

        private void NotifyIfEmailEmptyAndSave()
        {
            bool contactHasDocTransmittalOn = (currentContact.CreditMemo || currentContact.Statement || currentContact.Invoice || currentContact.DebitMemo);
            bool contactHasNoEmail = string.IsNullOrEmpty(currentContact.Email);

            if ((contactHasNoEmail) && (contactHasDocTransmittalOn))
            {
                var result = MessageBox.Show($"Saving this contact without an email will remove doc transmittal. {Environment.NewLine} {Environment.NewLine}Are you sure you want to proceed? ", "Warning", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    RemoveDocTransmittal();
                    controller.SaveOrUpdateContact(ref currentContact, ref currentCustomer);
                    statusLabel.Text = "Contact has been saved.";
                    statusLabel.ForeColor = Color.Green;
                }
            }
            else
            {
                controller.SaveOrUpdateContact(ref currentContact, ref currentCustomer);
                statusLabel.Text = "Contact has been saved.";
                statusLabel.ForeColor = Color.Green;
            }
        }



        private void RemoveDocTransmittal()
        {
            if (currentContact.Key == currentCustomer.PrimaryCntctKey)
            {
                controller.UpdatePrimaryDocTransmittal(ref currentCustomer, ref allContacts, TranType.CreditMemo, false);
                controller.UpdatePrimaryDocTransmittal(ref currentCustomer, ref allContacts, TranType.Invoice, false);
                controller.UpdatePrimaryDocTransmittal(ref currentCustomer, ref allContacts, TranType.Statement, false);
                controller.UpdatePrimaryDocTransmittal(ref currentCustomer, ref allContacts, TranType.DebitMemo, false);

                EnableDisableDocTransmittalCells("CreditMemo", false);
                EnableDisableDocTransmittalCells("Statement", false);
                EnableDisableDocTransmittalCells("Invoice", false);
                EnableDisableDocTransmittalCells("DebitMemo", false);
            }
            else
            {
                controller.UpdateNonPrimaryDocTransmittal(ref currentCustomer, ref allContacts, TranType.CreditMemo, false, currentContact.Key);
                controller.UpdateNonPrimaryDocTransmittal(ref currentCustomer, ref allContacts, TranType.Invoice, false, currentContact.Key);
                controller.UpdateNonPrimaryDocTransmittal(ref currentCustomer, ref allContacts, TranType.Statement, false, currentContact.Key);
                controller.UpdateNonPrimaryDocTransmittal(ref currentCustomer, ref allContacts, TranType.DebitMemo, false, currentContact.Key);
            }

            currentContact.CreditMemo = false;
            currentContact.Invoice = false;
            currentContact.Statement = false;
            currentContact.DebitMemo = false;
        }

        private void SetContactProperties(Contact contactFromForm)
        {
            currentContact.CntctOwnerKey = contactOwnerKey;
            currentContact.EMailFormat = 3;
            currentContact.EntityType = 501; //customer
            currentContact.CreateDate = currentContact.IsNew ? DateTime.Now : currentContact.CreateDate;
            currentContact.CreateUserID = currentContact.IsNew ? userName : currentContact.CreateUserID;
            currentContact.UpdateDate = currentContact.IsNew ? currentContact.UpdateDate : DateTime.Now;
            currentContact.UpdateUserID = currentContact.IsNew ? currentContact.UpdateUserID : userName;
            currentContact.Deleted = currentContact.Deleted == null ? 0 : currentContact.Deleted;
            currentContact.IsDirty = false;
            contactControl.CurrentContactIsDirty = false;
            currentContact.Module = module.ToString();

            //set properties from form to avoid nulls in the database
            currentContact.Email = contactFromForm.Email ?? "";
            currentContact.Name = contactFromForm.Name ?? "";
            currentContact.Title = contactFromForm.Title ?? "";
            currentContact.Phone = contactFromForm.Phone ?? "";
            currentContact.PhoneExt = contactFromForm.PhoneExt ?? "";
            currentContact.Fax = contactFromForm.Fax ?? "";
            currentContact.FaxExt = contactFromForm.FaxExt ?? "";
            currentContact.MobilePhone = contactFromForm.MobilePhone ?? "";
        }

        private void AddContactToLocalLists()
        {
            var index = allContacts.FindIndex(c => c.Key == currentContact.Key);
            if (index == -1) /*returns -1 if not found*/
            {
                allContacts.Add(currentContact);
            }
        }

        private void RefreshContactLists()
        {
            allContacts = currentCustomer.Contacts.Where(c => c.Module == module.ToString() &&
                                                            ((c.ParentKey == null) || (c.Deleted == 0 && c.ParentKey != null)))
                                                            .OrderByDescending(c => c.Key == currentCustomer.PrimaryCntctKey)
                                                            .ThenByDescending(i => i.Shared)
                                                            .ThenBy(c => c.Name)
                                                            .ToList();

            activeContacts = allContacts.Where(c => c.IsDeleted == false).ToList();
            deletedContacts = allContacts.Where(c => c.IsDeleted == true).ToList();
        }


        private void SelectDGVRowFromCurrentContact()
        {
            loadingDataGrid = true;

            List<Contact> activeList = GetCurrentList();

            int index = activeList.FindIndex(c => c.Key == currentContact.Key);
            if (index == -1)
            {
                SetCurrentContactFromSelectedRow();
            }
            else
            {
                dgvContacts.Rows[index].Selected = true;
                dgvContacts.FirstDisplayedScrollingRowIndex = index;
            }

            loadingDataGrid = false;
        }

        private List<Contact> GetCurrentList()
        {
            if (chkShowDeleted.Checked)
            {
                return deletedContacts;
            }
            else
            {
                return activeContacts;
            }
        }
        #endregion







        //----------------SELECTION CHANGED------------------
        private void dgvContacts_SelectionChanged(object sender, EventArgs e)
        {
            if (loadingDataGrid) return;

            /*check if dirty*/
            var failed = NotifyIfContactIsDirty();
            if (failed) return;

            /*set contact*/
            SetCurrentContactFromSelectedRow();
            CloneCurrentContact();

            /*user interface*/
            SetFormToCurrentContact();
            SetControlsBasedOnPermissions();
            btnSave.Enabled = false;
        }

        private bool NotifyIfContactIsDirty()
        {
            if (contactControl.CurrentContactIsDirty)
            {
                var result = MessageBox.Show("Do you want to save changes made?", "Warning", MessageBoxButtons.YesNo);
                switch (result)
                {
                    case DialogResult.Yes:
                        if (contactControl.ValidateManually())
                        {
                            btnSave.Enabled = true;
                            btnSave.PerformClick();
                            return false;
                        }
                        else
                        {
                            MessageBox.Show("Form is invalid. Please fix errors before saving.");
                            return true;
                        }

                    case DialogResult.No:
                        RestoreProperties();
                        dgvContacts.Refresh();
                        return false;
                }
            }

            return false;
        }

        private void RestoreProperties()
        {
            if (cachedContact == null) return;
            currentContact.Name = cachedContact.Name;
            currentContact.Title = cachedContact.Title;
            currentContact.Phone = cachedContact.Phone;
            currentContact.PhoneExt = cachedContact.PhoneExt;
            currentContact.Fax = cachedContact.Fax;
            currentContact.FaxExt = cachedContact.FaxExt;
            currentContact.MobilePhone = cachedContact.MobilePhone;
            currentContact.Email = cachedContact.Email;
            currentContact.Shared = cachedContact.Shared;
            currentContact.IsDirty = false;
            contactControl.CurrentContactIsDirty = false;
        }

        private void CloneCurrentContact()
        {
            cachedContact = new Contact();
            cachedContact.Name = currentContact.Name;
            cachedContact.Title = currentContact.Title;
            cachedContact.Phone = currentContact.Phone;
            cachedContact.PhoneExt = currentContact.PhoneExt;
            cachedContact.Fax = currentContact.Fax;
            cachedContact.FaxExt = currentContact.FaxExt;
            cachedContact.MobilePhone = currentContact.MobilePhone;
            cachedContact.Email = currentContact.Email;
            cachedContact.Shared = currentContact.Shared;
        }

        private void chkShared_CheckedChanged(object sender, EventArgs e)
        {
            if (currentContact.Deleted == 1)
            {
                currentContact.Shared = false;
                chkShared.CheckedChanged -= chkShared_CheckedChanged;
                chkShared.Checked = false;
                chkShared.CheckedChanged += chkShared_CheckedChanged;
                statusLabel.Text = "Cannot share a deleted contact. Must activate contact first";
                statusLabel.ForeColor = Color.Red;
            }
            else
            {
                currentContact.Shared = chkShared.Checked;
                currentContact.IsDirty = true;

                btnSave.Enabled = contactControl.ValidateManually();

            }
        }

        private void pictureBoxShared_Click(object sender, EventArgs e)
        {
            chkShared.Checked = !chkShared.Checked;
        }

        private void dgvContacts_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var row = dgvContacts.Rows[e.RowIndex];

            /*set selection backcolor to current backcolor*/
            row.Cells["CreditMemo"].Style.SelectionBackColor = row.Cells["CreditMemo"].Style.BackColor;
            row.Cells["Invoice"].Style.SelectionBackColor = row.Cells["Invoice"].Style.BackColor;
            row.Cells["Statement"].Style.SelectionBackColor = row.Cells["Statement"].Style.BackColor;
            row.Cells["DebitMemo"].Style.SelectionBackColor = row.Cells["DebitMemo"].Style.BackColor;
        }

        private void chkShowDeleted_CheckedChanged(object sender, EventArgs e)
        {
            /*display correct list*/
            SetGridDataBindings(GetCurrentList());
            HighlightAlienContacts();
            SetImageColumns();
            SetCountLabel();
            ResizeForm();

            /*set contact*/
            SetCurrentContactFromSelectedRow();

            /*user interface*/
            SetFormToCurrentContact();
            SetControlsBasedOnPermissions();

            contactControl.SetStatus(Color.Green, "");
        }

        private void rdoActive_CheckedChanged(object sender, EventArgs e)
        {
            currentContact.IsDirty = true;

            if (rdoActive.Checked)
            {
                currentContact.Deleted = 0;

                btnSave.Enabled = contactControl.IsValid();
            }
        }

        private void rdoDeleted_CheckedChanged(object sender, EventArgs e)
        {
            currentContact.IsDirty = true;

            if (rdoDeleted.Checked)
            {
                currentContact.Deleted = 1;
                chkShared.Checked = false;

                if (contactControl.IsValid())
                {
                    btnSave.Enabled = true;
                }
            }
        }

        private void rdoActive_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSave.PerformClick();
            }
        }

        private void rdoInactive_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSave.PerformClick();
            }
        }




        #region CLOSE FORM
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (contactControl.CurrentContactIsDirty)
            {
                var result = MessageBox.Show("Do you want to save before leaving?", "Warning", MessageBoxButtons.YesNoCancel);
                switch (result)
                {
                    case DialogResult.Yes:
                        NotifyIfEmailEmptyAndSave();
                        break;

                    case DialogResult.No:
                        RestoreProperties();
                        break;
                }
            }

            Close();
        }

        private void ContactManagerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (primarySwapRequiredBeforeClose)
            {
                e.Cancel = true;

                string warning = "Branch is now a stand alone account. Must choose new primary contact";
                SetStatusLabel(Color.Red, warning);
            }
        }
        #endregion






        #region Document Transmittal
        private void dgvContacts_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            /*prevent exceptions*/
            if (e.RowIndex > dgvContacts.Rows.Count) return; //out of bounds
            if (e.ColumnIndex > dgvContacts.Columns.Count) return; //out of bounds
            if (dgvContacts.Rows.Count == 0) return; //no rows
            if (dgvContacts.Rows[e.RowIndex].Cells[e.ColumnIndex].GetType() != typeof(DataGridViewCheckBoxCell)) return; //not a checkbox


            Contact primaryContact = allContacts.FirstOrDefault(c => c.Key == currentCustomer.PrimaryCntctKey);
            Contact targetContact = (Contact)dgvContacts.Rows[e.RowIndex].DataBoundItem;
            DataGridViewRow currentRow = dgvContacts.Rows[e.RowIndex];


            /*conditions*/
            bool isPrimary = (bool)currentRow.Cells["Primary"].EditedFormattedValue == true;
            bool hasEmail = !string.IsNullOrEmpty(targetContact.Email.TrimEnd());
            bool cellIsEnabled = dgvContacts.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly == false;
            bool isDocTransmittalCell = dgvContacts.Columns[e.ColumnIndex].Name == "CreditMemo" ||
                                        dgvContacts.Columns[e.ColumnIndex].Name == "Invoice" ||
                                        dgvContacts.Columns[e.ColumnIndex].Name == "Statement" ||
                                        dgvContacts.Columns[e.ColumnIndex].Name == "DebitMemo";


            if ((isDocTransmittalCell) && (cellIsEnabled) && (isPrimary) && (hasEmail))
            {
                DataGridViewCheckBoxCell currentCheckCell = (DataGridViewCheckBoxCell)dgvContacts.Rows[0].Cells[e.ColumnIndex];

                bool action = (bool)currentCheckCell.EditedFormattedValue;
                string colName = dgvContacts.Columns[e.ColumnIndex].Name;

                switch (colName)
                {
                    case "CreditMemo":
                        controller.UpdatePrimaryDocTransmittal(ref currentCustomer, ref allContacts, TranType.CreditMemo, action);
                        primaryContact.CreditMemo = action;
                        EnableDisableDocTransmittalCells("CreditMemo", action, true);
                        break;

                    case "Statement":
                        controller.UpdatePrimaryDocTransmittal(ref currentCustomer, ref allContacts, TranType.Statement, action);
                        primaryContact.Statement = action;
                        EnableDisableDocTransmittalCells("Statement", action, true);
                        break;

                    case "Invoice":
                        controller.UpdatePrimaryDocTransmittal(ref currentCustomer, ref allContacts, TranType.Invoice, action);
                        primaryContact.Invoice = action;
                        EnableDisableDocTransmittalCells("Invoice", action, true);
                        break;

                    case "DebitMemo":
                        controller.UpdatePrimaryDocTransmittal(ref currentCustomer, ref allContacts, TranType.DebitMemo, action);
                        primaryContact.DebitMemo = action;
                        EnableDisableDocTransmittalCells("DebitMemo", action, true);
                        break;

                    default:
                        break;
                }
                return;
            }


            if ((isDocTransmittalCell) && (cellIsEnabled) && (!isPrimary) && (hasEmail))
            {
                var targetContactChkCell = (DataGridViewCheckBoxCell)dgvContacts.Rows[e.RowIndex].Cells[e.ColumnIndex];

                switch (dgvContacts.Columns[e.ColumnIndex].Name)
                {
                    case "CreditMemo":
                        controller.UpdateNonPrimaryDocTransmittal(ref currentCustomer, ref allContacts, TranType.CreditMemo, (bool)targetContactChkCell.EditedFormattedValue, targetContact.Key);
                        break;

                    case "Statement":
                        controller.UpdateNonPrimaryDocTransmittal(ref currentCustomer, ref allContacts, TranType.Statement, (bool)targetContactChkCell.EditedFormattedValue, targetContact.Key);
                        break;

                    case "Invoice":
                        controller.UpdateNonPrimaryDocTransmittal(ref currentCustomer, ref allContacts, TranType.Invoice, (bool)targetContactChkCell.EditedFormattedValue, targetContact.Key);
                        break;

                    case "DebitMemo":
                        controller.UpdateNonPrimaryDocTransmittal(ref currentCustomer, ref allContacts, TranType.DebitMemo, (bool)targetContactChkCell.EditedFormattedValue, targetContact.Key);
                        break;

                    default:
                        break;
                }

                return;
            }

            //on fail due to email
            if ((isDocTransmittalCell) && (cellIsEnabled) && (!hasEmail))
            {
                MessageBox.Show($"{currentContact.Name} needs an email before you can set them up for doc transmittal.");

                contactControl.HighlightEmail();

                currentContact.CreditMemo = false;
                currentContact.Invoice = false;
                currentContact.Statement = false;
                currentContact.DebitMemo = false;

                dgvContacts.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = false;
                dgvContacts.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 0;
                return;
            }

            /*document transmittal fail to edit*/
            if ((isDocTransmittalCell) && (!cellIsEnabled) && (targetContact.IsDeleted))
            {
                statusLabel.Text = "Cannot set document transmittal for deleted contacts";
                statusLabel.ForeColor = Color.Red;
            }
            else if ((isDocTransmittalCell) && (!cellIsEnabled) && (!targetContact.IsDeleted) && (currentCustomer.IsHQ) && (isInSuperGroup))
            {
                statusLabel.Text = "Document transmittal must be set on the primary before additional contacts.";
                statusLabel.ForeColor = Color.Red;
            }
            else if ((isDocTransmittalCell) && (!cellIsEnabled) && (!targetContact.IsDeleted) && !(isInSuperGroup || isInPamsGroup))
            {
                statusLabel.Text = $"{userName} is not allowed to set up document transmittal for this contact.";
                statusLabel.ForeColor = Color.Red;
            }

            /*primary swapping*/
            bool isPrimaryCell = dgvContacts.Columns[e.ColumnIndex].Name == "Primary";

            if ((isPrimaryCell) && (isInPamsGroup))
            {
                //email required if doc transmittal set on
                if ((primaryContact.CreditMemo || primaryContact.Invoice || primaryContact.Statement || primaryContact.DebitMemo) && (!hasEmail))
                {
                    MessageBox.Show($"New primary contact: {targetContact.Name} - will inherit the doc transmittal from the old primary. New primary must have email set up before setting up doc transmittal");

                    contactControl.HighlightEmail();

                    dgvContacts.Rows[e.RowIndex].Cells["Primary"].Value = false;
                    dgvContacts.Rows[e.RowIndex].Cells["Primary"].Value = 0;
                }
                else
                {
                    targetContact = allContacts.FirstOrDefault(c => c.Key == targetContact.Key);

                    controller.SwapPrimary(ref currentCustomer, ref targetContact, ref primaryContact);

                    RefreshContactLists();
                    SetGridDataBindings(GetCurrentList());
                    HighlightAlienContacts();
                    SetCountLabel();
                    SetImageColumns();
                    EnableDisableDocTransmittalCells("CreditMemo", allContacts.First(c => c.IsPrimary).CreditMemo);
                    EnableDisableDocTransmittalCells("Statement", allContacts.First(c => c.IsPrimary).Statement);
                    EnableDisableDocTransmittalCells("Invoice", allContacts.First(c => c.IsPrimary).Invoice);
                    EnableDisableDocTransmittalCells("DebitMemo", allContacts.First(c => c.IsPrimary).DebitMemo);
                    SetControlsBasedOnPermissions();
                    dgvContacts.Rows[0].Selected = true;
                    SetFormToCurrentContact();
                    SetControlsBasedOnPermissions();

                    if (primarySwapRequiredBeforeClose)
                    {
                        btnClose.Enabled = true;
                        primarySwapRequiredBeforeClose = false;
                    }
                }
            }
            else if ((isPrimaryCell) && (!isInPamsGroup))
            {
                statusLabel.Text = $"{userName} is not allowed to swap primary contacts for accounts.";
                statusLabel.ForeColor = Color.Red;
            }

        }


        private void EnableDisableDocTransmittalCells(string cellName, bool enable, bool uncheck = false)
        {
            IEnumerable<DataGridViewRow> nonPrimaryRows = dgvContacts.Rows.Cast<DataGridViewRow>().Where(c => (bool)c.Cells["Primary"].Value == false);

            if (enable)
            {
                foreach (DataGridViewRow row in nonPrimaryRows)
                {
                    bool isDeleted = GetCurrentList()[row.Index].IsDeleted;
                    if (isDeleted)
                    {
                        var chkCell = (DataGridViewCheckBoxCell)row.Cells[cellName];
                        chkCell.Style.BackColor = Color.LightGray;
                        chkCell.FlatStyle = FlatStyle.Flat;
                        chkCell.Value = uncheck ? 0 : Convert.ToInt16(chkCell.EditedFormattedValue) == 0 ? 0 : 1;
                        chkCell.ReadOnly = true;
                    }
                    else
                    {
                        var chkCell = (DataGridViewCheckBoxCell)row.Cells[cellName];
                        chkCell.Style.BackColor = row.DefaultCellStyle.BackColor;
                        chkCell.FlatStyle = FlatStyle.Standard;
                        chkCell.ReadOnly = false;
                    }
                }
            }
            else
            {
                foreach (DataGridViewRow row in nonPrimaryRows)
                {
                    var chkCell = (DataGridViewCheckBoxCell)row.Cells[cellName];
                    chkCell.Style.BackColor = Color.LightGray;
                    chkCell.FlatStyle = FlatStyle.Flat;
                    chkCell.Value = uncheck ? 0 : Convert.ToInt16(chkCell.EditedFormattedValue) == 0 ? 0 : 1;
                    chkCell.ReadOnly = true;
                }
            }
        }
        #endregion






        #region Phone Formatting
        private void dgvContacts_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var contact = (Contact)dgvContacts.Rows[e.RowIndex].DataBoundItem;

            switch (dgvContacts.Columns[e.ColumnIndex].Name)
            {
                case "Phone":
                    e.Value = FormatPhone(contact.Phone, contact.PhoneExt);
                    break;

                case "Mobile":
                    e.Value = FormatPhone(contact.MobilePhone, "");
                    break;

                case "Fax":
                    e.Value = FormatPhone(contact.Fax, contact.FaxExt);
                    break;

                default:
                    break;
            }
        }

        private string FormatPhone(string number, string numberExtension)
        {
            if (number != null) number = number.Replace(" ", "");
            if (numberExtension != null) numberExtension = numberExtension.Replace(" ", "");

            if (number == null || number == string.Empty)
                return string.Empty;
            else if (numberExtension == null || numberExtension == string.Empty)
                return string.Format("{0:(###) ###-####}", Convert.ToInt64(number));
            else
                return string.Format("{0:(###) ###-####}", Convert.ToInt64(number)) + " x" + numberExtension;
        }
        #endregion





        #region Navigation Between HQ & BR

        private int cachedBranchKey;
        private void linklblNavigate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var failed = NotifyIfContactIsDirty();
            if (failed) return;

            //if in HQ - try to go back to branch
            if (currentCustomer.IsHQ)
            {
                //check to see if branch was saved
                if (cachedBranchKey != 0)
                {
                    //refresh
                    Initialize(cachedBranchKey, Module.AR);
                    Refresh();

                    linklblNavigate.Text = "Navigate to HQ";
                }
            }
            else if (currentCustomer.IsBranch) //if in branch
            {
                //get hq account
                if (currentCustomer.Parents.Count != 0)
                {
                    //cache current account
                    cachedBranchKey = currentCustomer.Key;

                    //refresh  with HQ
                    Customer HQ = currentCustomer.Parents[0];
                    Initialize(HQ.Key, Module.AR);
                    Refresh();

                    linklblNavigate.Text = "Navigate back to branch";
                }
            }
        }
        #endregion
    }

    public enum TranType
    {
        Invoice = 501,
        CreditMemo = 502,
        Statement = 522,
        DebitMemo = 503
    }

    public enum AccountType
    {
        StandAlone,
        Branch,
        HQ
    }

    public enum Module
    {
        AR,
        SO
    }

}
