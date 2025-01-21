using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maxi.BackOffice.Agent.Domain.Model
{
    public enum GiactVerificationResponse
    {
        /// <summary>
        /// The account was found on your Private Bad Check List. See the Private Bad Checks List section.
        /// </summary>
        PrivateBadChecksList = 1,
        /// <summary>
        /// The item should be declined.
        /// </summary>
        Declined = 2,
        /// <summary>
        /// The item should be rejected. (The check number supplied has an active stop payment on file)
        /// </summary>
        RejectItem = 3,
        /// <summary>
        /// This item should be accepted with consideration for the understood risks.
        /// </summary>
        AcceptWithRisk = 4,
        /// <summary>
        /// This item should not be accepted with consideration for the understood risks.
        /// </summary>
        RiskAlert = 5,
        /// <summary>
        /// This item passes all verification checks.
        /// </summary>
        Pass = 6,
        /// <summary>
        /// This account is a non-demand deposit account.
        /// </summary>
        PassNdd = 7,
        /// <summary>
        /// There is negative data in the history of this account.
        /// </summary>
        NegativeData = 8,
        /// <summary>
        /// No data was found on this account or identity.
        /// </summary>
        NoData = 9,
        /// <summary>
        /// There was an error with the inquiry. See the ErrorMessage section for details.
        /// </summary>
        Error = 0

    }
}
