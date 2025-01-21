using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.Agent.Domain.Model
{
    public enum GiactAccountResponseCode
    {
        /// <summary>
        /// Description : Account Verified – The account was found to be an open and valid checking account. 
        /// Verification: Pass
        /// </summary>
        _1111 = 12,
        /// <summary>
        /// Description : AMEX – The account was found to be an American Express Travelers Cheque account.  
        /// Verification: Pass
        /// </summary>
        _2222 = 13,
        /// <summary>
        /// Code :_3333   Description : Non-Participant Provider – This account was reported with acceptable, positive data found in current or recent transactions.  Verification: Pass
        /// </summary>
        _3333 = 14,
        /// <summary>
        /// Description : Savings Account Verified – The account was found to be an open and valid savings account.  
        /// Verification: Pass
        /// </summary>
        _5555 = 15,
        /// <summary>        
        /// Description : No positive or negative information has been reported on the account. 
        /// Verification: NoData
        /// </summary>
        ND00 = 21,
        /// <summary>
        /// Description : This routing number can only be valid for US Government financial institutions.
        /// Verification: NoData
        /// </summary>
        ND01 = 22,
        /// <summary>
        /// Description :  Invalid Routing Number - The routing number supplied fails the validation test.
        /// Verification: Declined
        /// </summary>
        GS01 = 1,
        /// <summary>
        /// Description : Invalid Account Number - The account number supplied fails the validation test. 
        /// Verification: Declined
        /// </summary>
        GS02 = 2,
        /// <summary>
        /// Description :  Invalid Check Number - The check number supplied fails the validation test.
        /// Verification: Declined
        /// </summary>
        GS03 = 3,
        /// <summary>
        /// Description : Invalid Amount - The amount supplied fails the validation test.
        /// Verification: Declined 
        /// </summary>
        GS04 = 4,
        /// <summary>
        /// Description :  The routing number supplied is reported as not assigned to a financial institution         /// Verification: Unassigned Routing Number
        /// </summary>
        GN05 = 20 ,
        /// <summary>
        /// Description : The account was found as active in your Private Bad Checks List.
        /// Verification: PrivateBadChecksList
        /// </summary>
        GP01 = 5 ,
        /// <summary>
        /// Description : The routing number belongs to a reporting bank; however, no positive nor negativeinformation has been reported on the account number.
        /// Verification: Declined
        /// </summary>
        RT00 = 6,
        /// <summary>
        /// Description : This account should be declined based on the risk factor being reported.
        /// Verification: Declined
        /// </summary>
        RT01 = 7,
        /// <summary>
        /// Description : This item should be rejected based on the risk factor being reported.
        /// Verification: RejectItem
        /// </summary>
        RT02 = 8,
        /// <summary>
        /// Description : Current negative data exists on this account. Accept transaction with risk. (Example: Checking or savings accounts in NSF status, recent returns, or outstanding items)
        /// Verification: AcceptWithRisk
        /// </summary>
        RT03 = 9,
        /// <summary>
        /// Description : Non-Demand Deposit Account (post no debits), Credit Card Check, Line of Credit, Home Equity, Brokerage Check, or other type of non-demand deposit account.
        /// Verification: PassNdd
        /// </summary>
        RT04 = 10,
        /// <summary>
        /// Description : Negative information was found in this account's history.
        /// Verification: NegativeData
        /// </summary>
        GN01 = 19,

        /// <summary>
        /// Description : N/A
        /// Verification: N/A
        /// </summary>
        RT05 = 11,
        /// <summary>
        /// Description : N/A
        /// Verification: N/A
        /// </summary>
        _7777 = 16,
        /// <summary>
        /// Description : N/A
        /// Verification: N/A
        /// </summary>
        _8888 = 17,
        /// <summary>
        /// Description : N/A
        /// Verification: N/A
        /// </summary>
        _9999 = 18

    }
}
