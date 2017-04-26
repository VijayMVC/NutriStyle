using System;
using System.Security.Principal;

using Microsoft.ReportingServices.Interfaces;

namespace Microsoft.Samples.ReportingServices.AnonymousSecurity
{

    public class AuthenticationExtension : IAuthenticationExtension
    {

        /// <summary>

        /// You must implement SetConfiguration as required by IExtension

        /// </summary>

        /// <param name="configuration">Configuration data as an XML

        /// string that is stored along with the Extension element in

        /// the configuration file.</param>

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]

        public void SetConfiguration(String configuration)
        {

            // No configuration data is needed for this extension

        }



        /// <summary>

        /// You must implement LocalizedName as required by IExtension

        /// </summary>

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]

        public string LocalizedName
        {

            get
            {

                return null;

            }

        }



        /// <summary>

        /// Indicates whether a supplied username and password are valid.

        /// </summary>

        /// <param name="userName">The supplied username</param>

        /// <param name="password">The supplied password</param>

        /// <param name="authority">Optional. The specific authority to use to

        /// authenticate a user. For example, in Windows it would be a Windows

        /// Domain</param>

        /// <returns>true when the username and password are valid</returns>

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]

        public bool LogonUser(string userName, string password, string authority)
        {

            return true;

        }



        /// <summary>

        /// Required by IAuthenticationExtension. The report server calls the

        /// GetUserInfo methodfor each request to retrieve the current user

        /// identity.

        /// </summary>

        /// <param name="userIdentity">represents the identity of the current

        /// user. The value of IIdentity may appear in a user interface and

        /// should be human readable</param>

        /// <param name="userId">represents a pointer to a unique user identity

        /// </param>

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]

        public void GetUserInfo(out IIdentity userIdentity, out IntPtr userId)
        {

            userIdentity = new GenericIdentity("dummy user");

            userId = IntPtr.Zero;

        }



        /// <summary>

        /// The IsValidPrincipalName method is called by the report server when

        /// the report server sets security on an item. This method validates

        /// that the user name is valid for Windows.The principal name needs to

        /// be a user, group, or builtin account name.

        /// </summary>

        /// <param name="principalName">A user, group, or built-in account name

        /// </param>

        /// <returns>true when the principle name is valid</returns>

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]

        public bool IsValidPrincipalName(string principalName)
        {

            return true;

        }

    }

}
