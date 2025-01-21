using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.Agent.Domain.Model
{
    public enum GiactFundsConfirmationResult
    {
        /// <summary>
        /// This item's bank is not a participant in the network.
        /// </summary>
        NonParticipatingBank,
        /// <summary>
        /// This item has an invalid account number.
        /// </summary>
        InvalidAccountNumber,
        /// <summary>
        /// The bank is reporting this account as closed.
        /// </summary>
        AccountClosed,
        /// <summary>
        /// There are not sufficient funds available for this item's amount.
        /// </summary>
        InsufficientFunds,
        /// <summary>
        /// There are sufficient funds available for this item's amount.
        /// </summary>
        SufficientFunds
    }
}
