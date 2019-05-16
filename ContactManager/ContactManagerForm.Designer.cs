
using System;

namespace ContactManager
{
    partial class ContactManagerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContactManagerForm));
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.dgvContacts = new System.Windows.Forms.DataGridView();
            this.Key = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Primary = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Shared = new System.Windows.Forms.DataGridViewImageColumn();
            this.Active = new System.Windows.Forms.DataGridViewImageColumn();
            this.Title = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Phone = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Cell = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Fax = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Email = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CreditMemo = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Statement = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Invoice = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.DebitMemo = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.contactBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnNew = new System.Windows.Forms.Button();
            this.chkShowDeleted = new System.Windows.Forms.CheckBox();
            this.chkShared = new System.Windows.Forms.CheckBox();
            this.lblShared = new System.Windows.Forms.Label();
            this.linklblNavigate = new System.Windows.Forms.LinkLabel();
            this.lblAccountInfo = new System.Windows.Forms.Label();
            this.panelContact = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripLabel();
            this.rdoActive = new System.Windows.Forms.RadioButton();
            this.rdoDeleted = new System.Windows.Forms.RadioButton();
            this.grpStatus = new System.Windows.Forms.GroupBox();
            this.lblCount = new System.Windows.Forms.Label();
            this.pictureBoxShared = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvContacts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.contactBindingSource)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.grpStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxShared)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(101, 406);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(84, 29);
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "&Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(587, 405);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(84, 29);
            this.btnClose.TabIndex = 13;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // dgvContacts
            // 
            this.dgvContacts.AllowUserToAddRows = false;
            this.dgvContacts.AllowUserToOrderColumns = true;
            this.dgvContacts.AllowUserToResizeRows = false;
            this.dgvContacts.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvContacts.AutoGenerateColumns = false;
            this.dgvContacts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dgvContacts.BackgroundColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvContacts.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvContacts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvContacts.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Key,
            this.Primary,
            this.Shared,
            this.Active,
            this.Title,
            this.nameDataGridViewTextBoxColumn,
            this.Phone,
            this.Cell,
            this.Fax,
            this.Email,
            this.CreditMemo,
            this.Statement,
            this.Invoice,
            this.DebitMemo});
            this.dgvContacts.DataSource = this.contactBindingSource;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.ActiveCaption;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvContacts.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvContacts.Location = new System.Drawing.Point(9, 25);
            this.dgvContacts.MultiSelect = false;
            this.dgvContacts.Name = "dgvContacts";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvContacts.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvContacts.RowHeadersVisible = false;
            this.dgvContacts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvContacts.Size = new System.Drawing.Size(662, 208);
            this.dgvContacts.TabIndex = 0;
            this.dgvContacts.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvContacts_CellContentClick);
            this.dgvContacts.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvContacts_CellFormatting);
            this.dgvContacts.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dgvContacts_RowPostPaint);
            this.dgvContacts.SelectionChanged += new System.EventHandler(this.dgvContacts_SelectionChanged);
            // 
            // Key
            // 
            this.Key.DataPropertyName = "Key";
            this.Key.HeaderText = "Key";
            this.Key.Name = "Key";
            this.Key.ReadOnly = true;
            this.Key.Visible = false;
            this.Key.Width = 31;
            // 
            // Primary
            // 
            this.Primary.DataPropertyName = "IsPrimary";
            this.Primary.HeaderText = "Primary";
            this.Primary.Name = "Primary";
            this.Primary.ReadOnly = true;
            this.Primary.Visible = false;
            this.Primary.Width = 47;
            // 
            // Shared
            // 
            this.Shared.HeaderText = "Shared";
            this.Shared.Name = "Shared";
            this.Shared.ReadOnly = true;
            this.Shared.Width = 47;
            // 
            // Active
            // 
            this.Active.HeaderText = "Active";
            this.Active.Name = "Active";
            this.Active.ReadOnly = true;
            this.Active.Width = 43;
            // 
            // Title
            // 
            this.Title.DataPropertyName = "Title";
            this.Title.HeaderText = "Title";
            this.Title.Name = "Title";
            this.Title.ReadOnly = true;
            this.Title.Width = 52;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Name";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.ReadOnly = true;
            this.nameDataGridViewTextBoxColumn.Width = 60;
            // 
            // Phone
            // 
            this.Phone.DataPropertyName = "Phone";
            this.Phone.HeaderText = "Phone";
            this.Phone.Name = "Phone";
            this.Phone.ReadOnly = true;
            this.Phone.Width = 63;
            // 
            // Cell
            // 
            this.Cell.DataPropertyName = "MobilePhone";
            this.Cell.HeaderText = "Cell";
            this.Cell.Name = "Cell";
            this.Cell.ReadOnly = true;
            this.Cell.Width = 49;
            // 
            // Fax
            // 
            this.Fax.DataPropertyName = "Fax";
            this.Fax.HeaderText = "Fax";
            this.Fax.Name = "Fax";
            this.Fax.ReadOnly = true;
            this.Fax.Width = 49;
            // 
            // Email
            // 
            this.Email.DataPropertyName = "Email";
            this.Email.HeaderText = "Email";
            this.Email.Name = "Email";
            this.Email.ReadOnly = true;
            this.Email.Width = 57;
            // 
            // CreditMemo
            // 
            this.CreditMemo.DataPropertyName = "CreditMemo";
            this.CreditMemo.HeaderText = "CreditMemo";
            this.CreditMemo.Name = "CreditMemo";
            this.CreditMemo.Visible = false;
            this.CreditMemo.Width = 69;
            // 
            // Statement
            // 
            this.Statement.DataPropertyName = "Statement";
            this.Statement.HeaderText = "Statement";
            this.Statement.Name = "Statement";
            this.Statement.Visible = false;
            this.Statement.Width = 61;
            // 
            // Invoice
            // 
            this.Invoice.DataPropertyName = "Invoice";
            this.Invoice.HeaderText = "Invoice";
            this.Invoice.Name = "Invoice";
            this.Invoice.Visible = false;
            this.Invoice.Width = 48;
            // 
            // DebitMemo
            // 
            this.DebitMemo.DataPropertyName = "DebitMemo";
            this.DebitMemo.HeaderText = "DebitMemo";
            this.DebitMemo.Name = "DebitMemo";
            this.DebitMemo.Visible = false;
            this.DebitMemo.Width = 67;
            // 
            // contactBindingSource
            // 
            this.contactBindingSource.DataSource = typeof(CreateCustomer.API.Entities.Contact);
            // 
            // btnNew
            // 
            this.btnNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNew.Location = new System.Drawing.Point(11, 406);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(84, 29);
            this.btnNew.TabIndex = 10;
            this.btnNew.Text = "&New";
            this.btnNew.UseVisualStyleBackColor = true;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // chkShowDeleted
            // 
            this.chkShowDeleted.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkShowDeleted.AutoSize = true;
            this.chkShowDeleted.Location = new System.Drawing.Point(578, 239);
            this.chkShowDeleted.Name = "chkShowDeleted";
            this.chkShowDeleted.Size = new System.Drawing.Size(93, 17);
            this.chkShowDeleted.TabIndex = 26;
            this.chkShowDeleted.Text = "Show Deleted";
            this.chkShowDeleted.UseVisualStyleBackColor = true;
            this.chkShowDeleted.CheckedChanged += new System.EventHandler(this.chkShowDeleted_CheckedChanged);
            // 
            // chkShared
            // 
            this.chkShared.AutoSize = true;
            this.chkShared.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.chkShared.Enabled = false;
            this.chkShared.Location = new System.Drawing.Point(275, 247);
            this.chkShared.Name = "chkShared";
            this.chkShared.Size = new System.Drawing.Size(15, 14);
            this.chkShared.TabIndex = 8;
            this.chkShared.UseVisualStyleBackColor = true;
            this.chkShared.CheckedChanged += new System.EventHandler(this.chkShared_CheckedChanged);
            // 
            // lblShared
            // 
            this.lblShared.AutoSize = true;
            this.lblShared.BackColor = System.Drawing.Color.Transparent;
            this.lblShared.Font = new System.Drawing.Font("Century Gothic", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblShared.Location = new System.Drawing.Point(283, 266);
            this.lblShared.Name = "lblShared";
            this.lblShared.Size = new System.Drawing.Size(37, 13);
            this.lblShared.TabIndex = 29;
            this.lblShared.Text = "Shared";
            // 
            // linklblNavigate
            // 
            this.linklblNavigate.AutoSize = true;
            this.linklblNavigate.Location = new System.Drawing.Point(323, 236);
            this.linklblNavigate.Name = "linklblNavigate";
            this.linklblNavigate.Size = new System.Drawing.Size(81, 13);
            this.linklblNavigate.TabIndex = 30;
            this.linklblNavigate.TabStop = true;
            this.linklblNavigate.Text = "Navigate to HQ";
            this.linklblNavigate.Visible = false;
            this.linklblNavigate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linklblNavigate_LinkClicked);
            // 
            // lblAccountInfo
            // 
            this.lblAccountInfo.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.lblAccountInfo.AutoSize = true;
            this.lblAccountInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccountInfo.ForeColor = System.Drawing.Color.Green;
            this.lblAccountInfo.Location = new System.Drawing.Point(233, 5);
            this.lblAccountInfo.Name = "lblAccountInfo";
            this.lblAccountInfo.Size = new System.Drawing.Size(100, 13);
            this.lblAccountInfo.TabIndex = 31;
            this.lblAccountInfo.Text = "{{Account Info}}";
            // 
            // panelContact
            // 
            this.panelContact.Location = new System.Drawing.Point(-2, 233);
            this.panelContact.Name = "panelContact";
            this.panelContact.Size = new System.Drawing.Size(304, 177);
            this.panelContact.TabIndex = 32;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.toolStrip1.Location = new System.Drawing.Point(0, 445);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(678, 25);
            this.toolStrip1.TabIndex = 33;
            this.toolStrip1.Text = "toolStrip";
            // 
            // statusLabel
            // 
            this.statusLabel.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 22);
            // 
            // rdoActive
            // 
            this.rdoActive.AutoSize = true;
            this.rdoActive.Location = new System.Drawing.Point(6, 18);
            this.rdoActive.Name = "rdoActive";
            this.rdoActive.Size = new System.Drawing.Size(55, 17);
            this.rdoActive.TabIndex = 34;
            this.rdoActive.TabStop = true;
            this.rdoActive.Text = "&Active";
            this.rdoActive.UseVisualStyleBackColor = true;
            this.rdoActive.CheckedChanged += new System.EventHandler(this.rdoActive_CheckedChanged);
            this.rdoActive.KeyUp += new System.Windows.Forms.KeyEventHandler(this.rdoActive_KeyUp);
            // 
            // rdoDeleted
            // 
            this.rdoDeleted.AutoSize = true;
            this.rdoDeleted.Location = new System.Drawing.Point(6, 40);
            this.rdoDeleted.Name = "rdoDeleted";
            this.rdoDeleted.Size = new System.Drawing.Size(62, 17);
            this.rdoDeleted.TabIndex = 35;
            this.rdoDeleted.TabStop = true;
            this.rdoDeleted.Text = "&Deleted";
            this.rdoDeleted.UseVisualStyleBackColor = true;
            this.rdoDeleted.CheckedChanged += new System.EventHandler(this.rdoDeleted_CheckedChanged);
            this.rdoDeleted.KeyUp += new System.Windows.Forms.KeyEventHandler(this.rdoInactive_KeyUp);
            // 
            // grpStatus
            // 
            this.grpStatus.Controls.Add(this.rdoActive);
            this.grpStatus.Controls.Add(this.rdoDeleted);
            this.grpStatus.Location = new System.Drawing.Point(262, 285);
            this.grpStatus.Name = "grpStatus";
            this.grpStatus.Size = new System.Drawing.Size(79, 69);
            this.grpStatus.TabIndex = 36;
            this.grpStatus.TabStop = false;
            this.grpStatus.Text = "Status";
            // 
            // lblCount
            // 
            this.lblCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(614, 9);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(0, 13);
            this.lblCount.TabIndex = 37;
            // 
            // pictureBoxShared
            // 
            this.pictureBoxShared.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxShared.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBoxShared.BackgroundImage")));
            this.pictureBoxShared.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBoxShared.Location = new System.Drawing.Point(287, 239);
            this.pictureBoxShared.Name = "pictureBoxShared";
            this.pictureBoxShared.Size = new System.Drawing.Size(30, 30);
            this.pictureBoxShared.TabIndex = 28;
            this.pictureBoxShared.TabStop = false;
            this.pictureBoxShared.Click += new System.EventHandler(this.pictureBoxShared_Click);
            // 
            // ContactManagerForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(678, 470);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.grpStatus);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.linklblNavigate);
            this.Controls.Add(this.lblShared);
            this.Controls.Add(this.chkShared);
            this.Controls.Add(this.pictureBoxShared);
            this.Controls.Add(this.lblAccountInfo);
            this.Controls.Add(this.chkShowDeleted);
            this.Controls.Add(this.dgvContacts);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.panelContact);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1000, 538);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(576, 450);
            this.Name = "ContactManagerForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Contact Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ContactManagerForm_FormClosing);
            this.Load += new System.EventHandler(this.ContactManager_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvContacts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.contactBindingSource)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.grpStatus.ResumeLayout(false);
            this.grpStatus.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxShared)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridView dgvContacts;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.CheckBox chkShowDeleted;
        private System.Windows.Forms.BindingSource contactBindingSource;
        private System.Windows.Forms.CheckBox chkShared;
        private System.Windows.Forms.Label lblShared;
        private System.Windows.Forms.PictureBox pictureBoxShared;
        private System.Windows.Forms.LinkLabel linklblNavigate;
        private System.Windows.Forms.Label lblAccountInfo;
        private System.Windows.Forms.Panel panelContact;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel statusLabel;
        private System.Windows.Forms.RadioButton rdoActive;
        private System.Windows.Forms.RadioButton rdoDeleted;
        private System.Windows.Forms.GroupBox grpStatus;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn Key;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Primary;
        private System.Windows.Forms.DataGridViewImageColumn Shared;
        private System.Windows.Forms.DataGridViewImageColumn Active;
        private System.Windows.Forms.DataGridViewTextBoxColumn Title;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Phone;
        private System.Windows.Forms.DataGridViewTextBoxColumn Cell;
        private System.Windows.Forms.DataGridViewTextBoxColumn Fax;
        private System.Windows.Forms.DataGridViewTextBoxColumn Email;
        private System.Windows.Forms.DataGridViewCheckBoxColumn CreditMemo;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Statement;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Invoice;
        private System.Windows.Forms.DataGridViewCheckBoxColumn DebitMemo;
    }
}