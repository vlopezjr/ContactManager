namespace ContactManagerTest
{
    partial class OrderForm
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
            this.btnEcotech = new System.Windows.Forms.Button();
            this.btnEmpty = new System.Windows.Forms.Button();
            this.btnAccountingCustomer = new System.Windows.Forms.Button();
            this.editContactControl1 = new ContactEditorControl.EditContactControl();
            this.SuspendLayout();
            // 
            // btnEcotech
            // 
            this.btnEcotech.Location = new System.Drawing.Point(12, 12);
            this.btnEcotech.Name = "btnEcotech";
            this.btnEcotech.Size = new System.Drawing.Size(141, 29);
            this.btnEcotech.TabIndex = 0;
            this.btnEcotech.Text = "SALES - Customer";
            this.btnEcotech.UseVisualStyleBackColor = true;
            this.btnEcotech.Click += new System.EventHandler(this.btnCustomer_Click);
            // 
            // btnEmpty
            // 
            this.btnEmpty.Location = new System.Drawing.Point(12, 47);
            this.btnEmpty.Name = "btnEmpty";
            this.btnEmpty.Size = new System.Drawing.Size(141, 24);
            this.btnEmpty.TabIndex = 1;
            this.btnEmpty.Text = "SALES - Empty";
            this.btnEmpty.UseVisualStyleBackColor = true;
            this.btnEmpty.Click += new System.EventHandler(this.btnEmpty_Click);
            // 
            // btnAccountingCustomer
            // 
            this.btnAccountingCustomer.Location = new System.Drawing.Point(12, 77);
            this.btnAccountingCustomer.Name = "btnAccountingCustomer";
            this.btnAccountingCustomer.Size = new System.Drawing.Size(141, 29);
            this.btnAccountingCustomer.TabIndex = 4;
            this.btnAccountingCustomer.Text = "ACCOUNTING - Customer";
            this.btnAccountingCustomer.UseVisualStyleBackColor = true;
            this.btnAccountingCustomer.Click += new System.EventHandler(this.btnAccountingCustomer_Click);
            // 
            // editContactControl1
            // 
            this.editContactControl1.Location = new System.Drawing.Point(198, 47);
            this.editContactControl1.Name = "editContactControl1";
            this.editContactControl1.Size = new System.Drawing.Size(163, 25);
            this.editContactControl1.TabIndex = 5;
            this.editContactControl1.Load += new System.EventHandler(this.editContactControl1_Load);
            // 
            // Test_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(386, 122);
            this.Controls.Add(this.editContactControl1);
            this.Controls.Add(this.btnAccountingCustomer);
            this.Controls.Add(this.btnEmpty);
            this.Controls.Add(this.btnEcotech);
            this.Name = "Test_Form";
            this.ShowIcon = false;
            this.Text = "Test Form";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnEcotech;
        private System.Windows.Forms.Button btnEmpty;
        private System.Windows.Forms.Button btnAccountingCustomer;
        private ContactEditorControl.EditContactControl editContactControl1;
    }
}

