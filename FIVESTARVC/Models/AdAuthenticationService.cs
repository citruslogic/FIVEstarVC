﻿using System;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;
using Microsoft.Owin.Security;
using FIVESTARVC;


namespace FIVESTARVC.Models
{
    public class AdAuthenticationService
    {
        public class AuthenticationResult
        {
            public AuthenticationResult(string errorMessage = null)
            {
                ErrorMessage = errorMessage;
            }

            public string ErrorMessage { get; private set; }
            public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);
        }

        private readonly IAuthenticationManager authenticationManager;

        public AdAuthenticationService(IAuthenticationManager authenticationManager)
        {
            this.authenticationManager = authenticationManager;
        }


        /// <summary>
        /// Check if username and password matches existing account in AD. 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public AuthenticationResult SignIn(string username, string password)
        {
#if DEBUG
            // authenticates against your local machine - for development time
            ContextType authenticationType = ContextType.Machine;
#else
            // authenticates against your Domain AD
            ContextType authenticationType = ContextType.Domain;
#endif
            PrincipalContext principalContext = new PrincipalContext(authenticationType);
            bool isAuthenticated = false;
            UserPrincipal userPrincipal = null;
            try
            {
                userPrincipal = UserPrincipal.FindByIdentity(principalContext, username);
                if (userPrincipal != null)
                {
                    isAuthenticated = principalContext.ValidateCredentials(username, password, ContextOptions.Negotiate);
                }

                if (isAuthenticated)
                {

                    var identity = CreateIdentity(userPrincipal);

                    authenticationManager.SignOut(FIVESTARAuthentication.ApplicationCookie);
                    authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, identity);
                    return new AuthenticationResult();
                }

                if (userPrincipal.IsAccountLockedOut())
                {
                    // here can be a security related discussion weather it is worth 
                    // revealing this information
                    return new AuthenticationResult("Your account is locked.");
                }

                if (userPrincipal.Enabled.HasValue && userPrincipal.Enabled.Value == false)
                {
                    // here can be a security related discussion weather it is worth 
                    // revealing this information
                    return new AuthenticationResult("Your account is disabled.");
                }



                return new AuthenticationResult("Username or Password is not correct.");
            }
            catch (Exception)
            {
                //TODO log exception in your ELMAH like this:
                //Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                return new AuthenticationResult("Username or Password is not correct.");
            }

        }


        private ClaimsIdentity CreateIdentity(UserPrincipal userPrincipal)
        {
            var identity = new ClaimsIdentity(FIVESTARAuthentication.ApplicationCookie, ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            identity.AddClaim(new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "Active Directory"));
            identity.AddClaim(new Claim(ClaimTypes.Name, userPrincipal.SamAccountName));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userPrincipal.SamAccountName));
            if (!string.IsNullOrEmpty(userPrincipal.EmailAddress))
            {
                identity.AddClaim(new Claim(ClaimTypes.Email, userPrincipal.EmailAddress));
            }

            var groups = userPrincipal.GetAuthorizationGroups();
            foreach (var @group in groups)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, @group.Name));
            }

            // add your own claims if you need to add more information stored on the cookie

            return identity;
        }
    }
}