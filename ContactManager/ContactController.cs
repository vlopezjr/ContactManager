using CreateCustomer.API.DomainServices;
using CreateCustomer.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ContactManager
{
    public class ContactController
    {
        private CustomerService service;
        private Module module;
        private AccountType accountType;

        public ContactController(Module _module, AccountType _accountType, CustomerService _service)
        {
            service = _service;
            module = _module;
            accountType = _accountType;
        }






        //SAVE
        internal void SaveOrUpdateContact(ref Contact currentContact, ref Customer currentCustomer)
        {
            switch (accountType)
            {
                case AccountType.StandAlone:
                    SaveContactForBranchAndStandAlone(ref currentContact);
                    break;
                case AccountType.Branch:
                    SaveContactForBranchAndStandAlone(ref currentContact);
                    break;
                case AccountType.HQ:
                    SaveContactForHQAccount(ref currentContact, ref currentCustomer);
                    break;
                default:
                    break;
            }
        }

        private void SaveContactForHQAccount(ref Contact currentContact, ref Customer currentCustomer)
        {
            try
            {
                //is new / wants to be shared 
                if (currentContact.IsNew && currentContact.Shared)
                {
                    //add to all branches
                    currentContact.Key = service.AddContact(currentContact);

                    foreach (var branch in currentCustomer.Branches)
                    {
                        var contact = new Contact
                        {
                            Name = currentContact.Name,
                            Phone = currentContact.Phone,
                            PhoneExt = currentContact.PhoneExt,
                            Fax = currentContact.Fax,
                            FaxExt = currentContact.FaxExt,
                            MobilePhone = currentContact.MobilePhone,
                            Email = currentContact.Email,
                            CntctOwnerKey = branch.Key,
                            ParentKey = currentContact.Key,
                            Shared = true,
                            EMailFormat = 3,
                            EntityType = 501,
                            CreateDate = currentContact.IsNew ? DateTime.Now : currentContact.CreateDate,
                            CreateUserID = currentContact.IsNew ? Environment.UserName : currentContact.CreateUserID,
                            Title = currentContact.Title,
                            Module = module.ToString(),
                            Deleted = currentContact.Deleted,
                            IsDirty = false
                        };

                        service.AddContact(contact);
                    }
                }

                //is new / not shared
                else if (currentContact.IsNew && !currentContact.Shared)
                {
                    /*add new to single branch*/
                    currentContact.Key = service.AddContact(currentContact);
                }

                // not new / shared
                else if (!currentContact.IsNew && currentContact.Shared)
                {
                    foreach (var branch in currentCustomer.Branches)
                    {
                        //find possible match
                        var currentContactKey = currentContact.Key;
                        var possibleMatch = branch.Contacts.FirstOrDefault(c => c.ParentKey == currentContactKey);
                        if (possibleMatch != null)
                        {
                            //if found update those
                            possibleMatch.Name = currentContact.Name;
                            possibleMatch.Email = currentContact.Email;
                            possibleMatch.Title = currentContact.Title;
                            possibleMatch.Phone = currentContact.Phone;
                            possibleMatch.Phone = currentContact.Phone;
                            possibleMatch.PhoneExt = currentContact.PhoneExt;
                            possibleMatch.Fax = currentContact.Fax;
                            possibleMatch.FaxExt = currentContact.FaxExt;
                            possibleMatch.MobilePhone = currentContact.MobilePhone;
                            possibleMatch.UpdateDate = currentContact.IsNew ? currentContact.UpdateDate : DateTime.Now;
                            possibleMatch.UpdateUserID = currentContact.IsNew ? currentContact.UpdateUserID : Environment.UserName;
                            possibleMatch.Shared = currentContact.Shared;
                            possibleMatch.Deleted = currentContact.Shared ? (short)0 : (short)1;
                            possibleMatch.CreditMemo = currentContact.CreditMemo;
                            possibleMatch.Invoice = currentContact.Invoice;
                            possibleMatch.Statement = currentContact.Statement;
                        }
                        else
                        {
                            //if not found - create new
                            var contact = new Contact
                            {
                                Name = currentContact.Name,
                                Phone = currentContact.Phone,
                                PhoneExt = currentContact.PhoneExt,
                                Fax = currentContact.Fax,
                                FaxExt = currentContact.FaxExt,
                                MobilePhone = currentContact.MobilePhone,
                                Email = currentContact.Email,
                                CntctOwnerKey = branch.Key,
                                ParentKey = currentContact.Key,
                                Shared = currentContact.Shared,
                                EMailFormat = 3,
                                EntityType = 501,
                                CreateDate = currentContact.IsNew ? DateTime.Now : currentContact.CreateDate,
                                CreateUserID = currentContact.IsNew ? Environment.UserName : currentContact.CreateUserID,
                                Title = currentContact.Title,
                                Module = module.ToString(),
                                Deleted = currentContact.Shared ? (short)0 : (short)1,
                                IsDirty = false,
                                CreditMemo = currentContact.CreditMemo,
                                Invoice = currentContact.Invoice,
                                Statement = currentContact.Statement,
                                DebitMemo = currentContact.DebitMemo
                            };

                            service.AddContact(contact);
                        }
                    }

                    service.UpdateCustomer(currentCustomer);

                    DeleteRestoreForHQAccount(ref currentContact, ref currentCustomer);
                }

                //not new / being unshared
                else if (!currentContact.IsNew && !currentContact.Shared)
                {
                    foreach (var branch in currentCustomer.Branches)
                    {
                        var currentContactKey = currentContact.Key;
                        var possibleMatch = branch.Contacts.FirstOrDefault(c => c.ParentKey == currentContactKey);
                        if (possibleMatch != null)
                        {
                            possibleMatch.Shared = false;
                            possibleMatch.Deleted = 1;

                            possibleMatch.CreditMemo = false;
                            possibleMatch.Invoice = false;
                            possibleMatch.Statement =  false;
                            possibleMatch.DebitMemo = false;
                        }
                    }

                    service.UpdateCustomer(currentCustomer);

                    DeleteRestoreForHQAccount(ref currentContact, ref currentCustomer);
                }
            }
            catch (Exception ex)
            {
                EmailSender.EmailUnexpectedException(ex, currentContact.Customer, "Module: SaveContactForHQAccount");
                MessageBox.Show("Error saving, please try restarting.");
            }
        }

        private void SaveContactForBranchAndStandAlone(ref Contact currentContact)
        {
            try
            {
                if(currentContact.ParentKey != null && !currentContact.IsDeleted)
                {
                    currentContact.ParentKey = null;
                }

                if (currentContact.IsNew)
                    currentContact.Key = service.AddContact(currentContact);
                else
                    service.UpdateContact(currentContact);
            }
            catch (Exception ex)
            {
                EmailSender.EmailUnexpectedException(ex, currentContact.Customer ?? null, "Module: SaveContactForBranchAndStandAlone");
                MessageBox.Show("Error saving, please try restarting.");
            }
        }



        private void DeleteRestoreForHQAccount(ref Contact currentContact, ref Customer currentCustomer)
        {
            int currentContactsKey = currentContact.Key; //can't use ref in lambda

            try
            {
                /*needs to be deleted & is shared*/
                if (currentContact.IsDeleted) 
                {
                    currentContact.Deleted = 1;

                    currentContact.CCCreditMemo = 0;
                    currentContact.CCInvoice = 0;
                    currentContact.CCCustStmnt = 0;
                    currentContact.CCDebitMemo = 0;

                    foreach (var branch in currentCustomer.Branches)
                    {
                        var possibleMatch = branch.Contacts.FirstOrDefault(c => c.ParentKey == currentContactsKey);
                        if (possibleMatch != null)
                        {
                            possibleMatch.Name = possibleMatch.Name.Replace("~", "");
                            possibleMatch.Deleted = 1;
                            possibleMatch.Shared = false;

                            possibleMatch.CCCreditMemo = 0;
                            possibleMatch.CCInvoice = 0;
                            possibleMatch.CCCustStmnt = 0;
                            possibleMatch.CCDebitMemo = 0;
                        }
                    }

                    service.UpdateCustomer(currentCustomer);
                }


                /*needs to be restored and is shared*/
                else if (!currentContact.IsDeleted && currentContact.Shared)
                {
                    /*restore in all branches*/
                    currentContact.Deleted = 0;
                    currentContact.Name = currentContact.Name.Replace("~", "");

                    foreach (var branch in currentCustomer.Branches)
                    {
                        var possibleMatch = branch.Contacts.FirstOrDefault(c => c.ParentKey == currentContactsKey);
                        if (possibleMatch != null)
                        {
                            possibleMatch.Name = possibleMatch.Name.Replace("~", "").TrimEnd();
                            possibleMatch.Deleted = 0;
                            possibleMatch.Shared = true;
                        }
                        else //not found then create a new contact for the branch
                        {
                            var contact = new Contact
                            {
                                Name = currentContact.Name,
                                Phone = currentContact.Phone,
                                PhoneExt = currentContact.PhoneExt,
                                Fax = currentContact.Fax,
                                FaxExt = currentContact.FaxExt,
                                MobilePhone = currentContact.MobilePhone,
                                Email = currentContact.Email,
                                CntctOwnerKey = branch.Key,
                                ParentKey = currentContact.Key,
                                Shared = currentContact.Shared,
                                EMailFormat = 3,
                                EntityType = 501,
                                CreateDate = currentContact.IsNew ? DateTime.Now : currentContact.CreateDate,
                                CreateUserID = currentContact.IsNew ? Environment.UserName : currentContact.CreateUserID,
                                Title = currentContact.Title,
                                Module = module.ToString(),
                                Deleted = currentContact.Deleted,
                                IsDirty = false
                            };

                            service.AddContact(contact);
                        }
                    }

                    service.UpdateCustomer(currentCustomer);
                }

                /*needs to be restored and is not shared*/
                else if (!currentContact.IsDeleted && !currentContact.Shared)
                {
                    currentContact.Deleted = 0;
                    currentContact.Name = currentContact.Name.Replace("~", "");

                    service.UpdateContact(currentContact);
                }
            }
            catch (Exception ex)
            {
                EmailSender.EmailUnexpectedException(ex, currentContact.Customer, "Module: RestoreOrDeleteCurrentContactHQ");
                MessageBox.Show("Error, please restart.");
            }
        }

        private void DeleteRestoreForBranchAndStandAlone(ref Contact currentContact)
        {
            try
            {
                if (currentContact.IsDeleted)
                {
                    currentContact.Deleted = 0;
                    currentContact.Name = currentContact.Name.Replace("~", "");
                }
                else
                {
                    currentContact.Deleted = 1;
                }

                service.UpdateContact(currentContact);
            }
            catch (Exception ex)
            {
                EmailSender.EmailUnexpectedException(ex, currentContact.Customer, "Module: RestoreOrDeleteCurrentContactBRAndSA");
                MessageBox.Show("Error, please restart.");
            }
        }








        //DOC TRANSMITTAL
        internal void UpdatePrimaryDocTransmittal(ref Customer currentCustomer, ref List<Contact> allContacts, TranType tranType, bool turningOnDT)
        {
            switch (accountType)
            {
                case AccountType.StandAlone:
                    UpdatePrimaryDocTransmittalForStandAlone(ref allContacts, ref currentCustomer, tranType, turningOnDT);
                    break;
                case AccountType.HQ:
                    UpdatePrimaryDocTransmittalForHQ(ref allContacts, ref currentCustomer, tranType, turningOnDT);
                    break;
                default:
                    break;
            }
        }

        private void UpdatePrimaryDocTransmittalForStandAlone(ref List<Contact> allContacts, ref Customer currentCustomer, TranType tranType, bool turningOnDT)
        {
            if (turningOnDT)
            {
                var primaryAddrKey = currentCustomer.PrimaryAddrKey;
                var primaryCustAddr = currentCustomer.CustAddresses.Find(c => c.Key == primaryAddrKey);

                var primaryContact = currentCustomer.Contacts.FirstOrDefault(c => c.IsPrimary);
                switch (tranType)
                {
                    case TranType.CreditMemo:
                        primaryContact.CreditMemo = true;
                        break;

                    case TranType.Invoice:
                        primaryContact.Invoice = true;
                        primaryCustAddr.InvcFormKey = 145;
                        break;

                    case TranType.Statement:
                        primaryContact.Statement = true;
                        break;

                    case TranType.DebitMemo:
                        primaryContact.DebitMemo = true;
                        break;

                    default:
                        break;
                }

                //turn on for customer
                var docTransmittal = currentCustomer.DocTransmittals.Find(c => c.TranType == (int)tranType);
                docTransmittal.EMail = 1;
                docTransmittal.IncludeCC = 1;

            }
            else
            {
                foreach (var contact in allContacts) //turn off for all contacts
                {
                    switch (tranType)
                    {
                        case TranType.CreditMemo:
                            contact.CreditMemo = false;
                            break;

                        case TranType.Invoice:
                            contact.Invoice = false;
                            break;

                        case TranType.Statement:
                            contact.Statement = false;
                            break;

                        case TranType.DebitMemo:
                            contact.DebitMemo = false;
                            break;

                        default:
                            break;
                    }
                }

                //turn off for customer
                var docTransmittal = currentCustomer.DocTransmittals.Find(c => c.TranType == (int)tranType);
                docTransmittal.EMail = 0;
                docTransmittal.IncludeCC = 0;

                //set the custaddr's email invoice form key
                if (tranType == TranType.Invoice)
                {
                    var primaryAddrKey = currentCustomer.PrimaryAddrKey;
                    var primaryCustAddr = currentCustomer.CustAddresses.Find(c => c.Key == primaryAddrKey);
                    primaryCustAddr.InvcFormKey = 79; 
                }
            }

            //save
            service.UpdateCustomer(currentCustomer);
        }

        private void UpdatePrimaryDocTransmittalForHQ(ref List<Contact> allContacts, ref Customer currentCustomer, TranType tranType, bool turningOnDT)
        {
            //update HQ account + primary doc transmittal
            service.UpdateHQPrimaryDocTransmittal(currentCustomer.Key, (int)tranType, turningOnDT);
        }





         

        //NON PRIMARY DOC TRANSMITTAL
        internal void UpdateNonPrimaryDocTransmittal(ref Customer currentCustomer, ref List<Contact> allContacts, TranType tranType, bool action, int contactKey)
        {
            switch (accountType)
            {
                case AccountType.StandAlone:
                    UpdateNonPrimaryDocTransmittalForBranchAndStandAlone(ref currentCustomer, ref allContacts, tranType, action, contactKey);
                    break;

                case AccountType.Branch:
                    UpdateNonPrimaryDocTransmittalForBranchAndStandAlone(ref currentCustomer, ref allContacts, tranType, action, contactKey);
                    break;

                case AccountType.HQ:
                    UpdateNonPrimaryDocTransmittalForHQ(ref currentCustomer, tranType, action, contactKey);
                    break;

                default:
                    break;
            }
        }

        private void UpdateNonPrimaryDocTransmittalForBranchAndStandAlone(ref Customer currentCustomer, ref List<Contact> allContacts, TranType tranType, bool action, int contactKey)
        {
            var contactToUpdate = allContacts.FirstOrDefault(c => c.Key == contactKey);
            switch (tranType)
            {
                case TranType.CreditMemo:
                    contactToUpdate.CreditMemo = action;
                    break;

                case TranType.Invoice:
                    contactToUpdate.Invoice = action;
                    break;

                case TranType.Statement:
                    contactToUpdate.Statement = action;
                    break;

                case TranType.DebitMemo:
                    contactToUpdate.DebitMemo = action;
                    break;

                default:
                    break;
            }

            service.UpdateContact(contactToUpdate);
        }

        private void UpdateNonPrimaryDocTransmittalForHQ(ref Customer currentCustomer, TranType tranType, bool action, int contactKey)
        {
            //propagate changes to all branches
            service.UpdateHQNonPrimaryDocTransmittal(currentCustomer.Key, (int)tranType, action, contactKey);
        }






        //PRIMARY SWAP
        internal void SwapPrimary(ref Customer currentCustomer, ref Contact newPrimaryContact, ref Contact oldPrimaryContact)
        {
            switch (accountType)
            {
                case AccountType.StandAlone:
                    SwapPrimaryForStandAlone(ref currentCustomer, ref newPrimaryContact, ref oldPrimaryContact);
                    break;

                case AccountType.HQ:
                    SwapPrimaryForHQ(ref currentCustomer, ref newPrimaryContact, ref oldPrimaryContact);
                    break;

                default:
                    break;
            }
        }

        private void SwapPrimaryForStandAlone(ref Customer currentCustomer, ref Contact newPrimaryContact, ref Contact oldPrimaryContact)
        {
            //swap the primary key for the customer
            currentCustomer.PrimaryCntctKey = newPrimaryContact.Key;
            var primaryAddrKey = currentCustomer.PrimaryAddrKey;
            var primaryCustAddress = currentCustomer.CustAddresses.First(c => c.Key == primaryAddrKey);
            primaryCustAddress.DfltCntctKey = newPrimaryContact.Key;

            newPrimaryContact.Deleted = 0;

            //swap the doc transmittal settings from original primary
            newPrimaryContact.CreditMemo = oldPrimaryContact.CreditMemo;
            newPrimaryContact.Invoice = oldPrimaryContact.Invoice;
            newPrimaryContact.Statement = oldPrimaryContact.Statement;
            newPrimaryContact.DebitMemo = oldPrimaryContact.DebitMemo;

            //reset the original primary doc transmittal settings
            oldPrimaryContact.CreditMemo = false;
            oldPrimaryContact.Invoice = false;
            oldPrimaryContact.Statement = false;
            oldPrimaryContact.DebitMemo = false;

            //save
            service.UpdateExplicitlyWithDependencies(currentCustomer);
            MessageBox.Show($"{newPrimaryContact.Name} is now the primary contact.");
        }

        private void SwapPrimaryForHQ(ref Customer currentCustomer, ref Contact newPrimaryContact, ref Contact oldPrimaryContact)
        {
            if (newPrimaryContact.Shared)
            {
                //swap primary contact key for HQ account 
                currentCustomer.PrimaryCntctKey = newPrimaryContact.Key;
                var primaryAddrKey = currentCustomer.PrimaryAddrKey;
                var primaryCustAddress = currentCustomer.CustAddresses.First(c => c.Key == primaryAddrKey);
                primaryCustAddress.DfltCntctKey = newPrimaryContact.Key;

                newPrimaryContact.Deleted = 0;

                //swap the doc transmittal settings from original primary and reset old primary dt
                newPrimaryContact.CreditMemo = oldPrimaryContact.CreditMemo;
                newPrimaryContact.Invoice = oldPrimaryContact.Invoice;
                newPrimaryContact.Statement = oldPrimaryContact.Statement;
                newPrimaryContact.DebitMemo = oldPrimaryContact.DebitMemo;

                oldPrimaryContact.CreditMemo = false;
                oldPrimaryContact.Invoice = false;
                oldPrimaryContact.Statement = false;
                oldPrimaryContact.DebitMemo = false;

                //swap dt / reset old primary dt / set new branch primary's parent key
                foreach (var branch in currentCustomer.Branches)
                {
                    //get branch current primary
                    var branchPrimary = branch.Contacts.First(c => c.Key == branch.PrimaryCntctKey);

                    //attemp to find the new primary
                    int parentKey = newPrimaryContact.Key;
                    var branchNewPrimaryContact = branch.Contacts.FirstOrDefault(c => c.ParentKey == parentKey);

                    //if found then set doc transmittal / branch keys / parent keys
                    if (branchNewPrimaryContact != null)
                    {
                        var branchPrimaryName = branchPrimary.Name;
                        var branchNewPrimaryName = branchNewPrimaryContact.Name;

                        branch.PrimaryCntctKey = branchNewPrimaryContact.Key;
                        var branchCustAddress = branch.CustAddresses.First(c => c.Key == branch.PrimaryAddrKey);
                        branchCustAddress.DfltCntctKey = branchNewPrimaryContact.Key;

                        branchNewPrimaryContact.Deleted = 0;

                        branchNewPrimaryContact.Invoice = newPrimaryContact.Invoice;
                        branchNewPrimaryContact.CreditMemo = newPrimaryContact.CreditMemo;
                        branchNewPrimaryContact.Statement = newPrimaryContact.Statement;
                        branchNewPrimaryContact.DebitMemo = newPrimaryContact.DebitMemo;

                        branchPrimary.CCInvoice = 0;
                        branchPrimary.CCCreditMemo = 0;
                        branchPrimary.CCCustStmnt = 0;
                        branchPrimary.CCDebitMemo = 0;

                    }
                    else //if not found - create new
                    {
                        branch.Contacts.Add(new Contact
                        {
                            Name = newPrimaryContact.Name,
                            Email = newPrimaryContact.Email,
                            Phone = newPrimaryContact.Phone,
                            PhoneExt = newPrimaryContact.PhoneExt,
                            Fax = newPrimaryContact.Fax,
                            FaxExt = newPrimaryContact.FaxExt,
                            MobilePhone = newPrimaryContact.MobilePhone,
                            CntctOwnerKey = branch.Key,
                            ParentKey = newPrimaryContact.Key,
                            Shared = newPrimaryContact.Shared,
                            EMailFormat = 3,
                            EntityType = 501,
                            CreateDate = DateTime.Now,
                            CreateUserID = Environment.UserName,
                            Title = newPrimaryContact.Title,
                            Module = module.ToString(),
                            Deleted = 0,
                            IsDirty = false,
                            CreditMemo = newPrimaryContact.CreditMemo,
                            Invoice = newPrimaryContact.Invoice,
                            Statement = newPrimaryContact.Statement,
                            DebitMemo = newPrimaryContact.DebitMemo
                        });
                    }
                }

                //save
                service.UpdateExplicitlyWithDependencies(currentCustomer);

                MessageBox.Show($"{newPrimaryContact.Name} is now the primary contact.");
            }
            else
            {
                MessageBox.Show($"{newPrimaryContact.Name} needs to be marked shared before making them the primary contact.");
            }
        }
    }
}
