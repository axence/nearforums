﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI;
using NearForums.Web.Controllers.Helpers;
using NearForums.Web.Extensions.FormsAuthenticationHelper;
using NearForums.Web.Extensions.FormsAuthenticationHelper.Impl;
using NearForums.Web.Extensions;
using NearForums.ServiceClient;

namespace NearForums.Web.Controllers
{

	[HandleError]
	public class FormsAuthenticationController : BaseController
	{
		/* This class uses code written by Troy Goode
		 * Source code licensed by MS-PL
		 * Website: https://github.com/TroyGoode/MembershipStarterKit
		 */

		// This constructor is used by the MVC framework to instantiate the controller using
		// the default forms authentication and membership providers.
		public FormsAuthenticationController()
			: this(null, null)
		{
		}

		// This constructor is not used by the MVC framework but is instead provided for ease
		// of unit testing this type. See the comments at the end of this file for more
		// information.
		public FormsAuthenticationController(IFormsAuthentication formsAuth, IMembershipService service)
		{
			FormsAuth = formsAuth ?? new FormsAuthenticationService();
			MembershipService = service ?? new AccountMembershipService();
		}

		public IFormsAuthentication FormsAuth
		{
			get;
			private set;
		}

		public IMembershipService MembershipService
		{
			get;
			private set;
		}

		[HttpGet]
		public ActionResult Login(string returnUrl)
		{
			if (!Config.AuthorizationProviders.FormsAuth.IsDefined)
			{
				return ResultHelper.ForbiddenResult(this);
			}
			if (this.User != null)
			{
				return Redirect(returnUrl);
			}
			ViewBag.ReturnUrl = returnUrl;

			return View();
		}

		[HttpPost]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings",
			Justification = "Needs to take same parameter type as Controller.Redirect()")]
		public ActionResult Login(string userName, string password, bool rememberMe, string returnUrl)
		{
			if (!Config.AuthorizationProviders.FormsAuth.IsDefined)
			{
				return ResultHelper.ForbiddenResult(this);
			}
			if (!ValidateLogOn(userName, password))
			{
				ViewBag.ReturnUrl = returnUrl;
				return View();
			}

			FormsAuth.SignIn(userName, rememberMe);

			SecurityHelper.TryFinishMembershipLogin(base.Session, Membership.GetUser(userName));

			if (!String.IsNullOrEmpty(returnUrl))
			{
				return Redirect(returnUrl);
			}
			else
			{
				return RedirectToAction("List", "Forums");
			}
		}

		public ActionResult LogOff()
		{
			if (!Config.AuthorizationProviders.FormsAuth.IsDefined)
			{
				return ResultHelper.ForbiddenResult(this);
			}
			FormsAuth.SignOut();

			return RedirectToAction("List", "Forums");
		}

		public ActionResult Register()
		{
			if (!Config.AuthorizationProviders.FormsAuth.IsDefined)
			{
				return ResultHelper.ForbiddenResult(this);
			}
			ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

			return View();
		}

		public ActionResult ResetPassword()
		{
			if (!Config.AuthorizationProviders.FormsAuth.IsDefined)
			{
				return ResultHelper.ForbiddenResult(this);
			}

			return View();
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult ResetPassword(string email)
		{
			if (!Config.AuthorizationProviders.FormsAuth.IsDefined)
			{
				return ResultHelper.ForbiddenResult(this);
			}

			string userName = Membership.GetUserNameByEmail(email);

			if (userName != null)
			{
				MembershipUser membershipUser = Membership.GetUser(userName);
				User user = UsersServiceClient.GetByProviderId(AuthenticationProvider.Membership, membershipUser.ProviderUserKey.ToString());
				string guid = System.Guid.NewGuid().ToString().Replace("-",string.Empty);
				UsersServiceClient.UpdatePasswordResetGuid(user.Id, guid, DateTime.Now.AddDays(2)); //Expire after 2 days. Maybe could be defined in config
				//TODO: Send email with the GUID and verify guid expire date on web request!!
			}
			else
			{
				ModelState.AddModelError("_FORM", "There is no account associated with the provided email address.");
				return View();
			}
			return View();
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Register(string userName, string email, string password, string confirmPassword)
		{
			if (!Config.AuthorizationProviders.FormsAuth.IsDefined)
			{
				return ResultHelper.ForbiddenResult(this);
			}
			ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

			if (ValidateRegistration(userName, email, password, confirmPassword))
			{
				// Attempt to register the user
				MembershipCreateStatus createStatus = MembershipService.CreateUser(userName, password, email);

				if (createStatus == MembershipCreateStatus.Success)
				{
					FormsAuth.SignIn(userName, false /* createPersistentCookie */);

					SecurityHelper.TryFinishMembershipLogin(base.Session, Membership.GetUser(userName));
					return RedirectToAction("List", "Forums");
				}
				else
				{
					ModelState.AddModelError("_FORM", ErrorCodeToString(createStatus));
				}
			}

			// If we got this far, something failed, redisplay form
			return View();
		}

		[Authorize]
		public ActionResult ChangePassword()
		{

			ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

			return View();
		}

		[Authorize]
		[AcceptVerbs(HttpVerbs.Post)]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "Exceptions result in password not being changed.")]
		public ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
		{

            ViewData["PasswordLength"] = MembershipService.MinPasswordLength;

            if (!ValidateChangePassword(currentPassword, newPassword, confirmPassword))
            {
                return View();
            }

            try
            {
                if (MembershipService.ChangePassword(Membership.GetUser().UserName, currentPassword, newPassword))
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("_FORM", "The current password is incorrect or the new password is invalid.");
                    return View();
                }
            }
            catch
            {
                ModelState.AddModelError("_FORM", "The current password is incorrect or the new password is invalid.");
                return View();
            }
		}

        [Authorize]
		//[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult ChangePasswordSuccess()
		{
			return View();
		}

		#region Validation Methods
		[NonAction]
		private bool ValidateChangePassword(string currentPassword, string newPassword, string confirmPassword)
		{
			if (String.IsNullOrEmpty(currentPassword))
			{
				ModelState.AddModelError("currentPassword", "You must specify a current password.");
			}
			if (newPassword == null || newPassword.Length < MembershipService.MinPasswordLength)
			{
				ModelState.AddModelError("newPassword",
					String.Format(CultureInfo.CurrentCulture,
						 "You must specify a new password of {0} or more characters.",
						 MembershipService.MinPasswordLength));
			}

			if (!String.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
			{
				ModelState.AddModelError("_FORM", "The new password and confirmation password do not match.");
			}

			return ModelState.IsValid;
		}
		[NonAction]
		private bool ValidateLogOn(string userName, string password)
		{
			if (String.IsNullOrEmpty(userName))
			{
				ModelState.AddModelError("username", "You must specify a username.");
			}
			if (String.IsNullOrEmpty(password))
			{
				ModelState.AddModelError("password", "You must specify a password.");
			}
			if (!MembershipService.ValidateUser(userName, password))
			{
				ModelState.AddModelError("_FORM", "The username or password provided is incorrect.");
			}

			return ModelState.IsValid;
		}
		[NonAction]
		private bool ValidateRegistration(string userName, string email, string password, string confirmPassword)
		{
			if (String.IsNullOrEmpty(userName))
			{
				ModelState.AddModelError("username", "You must specify a username.");
			}
			if (String.IsNullOrEmpty(email))
			{
				ModelState.AddModelError("email", "You must specify an email address.");
			}
			if (password == null || password.Length < MembershipService.MinPasswordLength)
			{
				ModelState.AddModelError("password",
					String.Format(CultureInfo.CurrentCulture,
						 "You must specify a password of {0} or more characters.",
						 MembershipService.MinPasswordLength));
			}
			if (!String.Equals(password, confirmPassword, StringComparison.Ordinal))
			{
				ModelState.AddModelError("_FORM", "The new password and confirmation password do not match.");
			}
			return ModelState.IsValid;
		}
		[NonAction]
		private static string ErrorCodeToString(MembershipCreateStatus createStatus)
		{
			// See http://msdn.microsoft.com/en-us/library/system.web.security.membershipcreatestatus.aspx for
			// a full list of status codes.
			switch (createStatus)
			{
				case MembershipCreateStatus.DuplicateUserName:
					return "Username already exists. Please enter a different user name.";

				case MembershipCreateStatus.DuplicateEmail:
					return "A username for that e-mail address already exists. Please enter a different e-mail address.";

				case MembershipCreateStatus.InvalidPassword:
					return "The password provided is invalid. Please enter a valid password value.";

				case MembershipCreateStatus.InvalidEmail:
					return "The e-mail address provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidAnswer:
					return "The password retrieval answer provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidQuestion:
					return "The password retrieval question provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidUserName:
					return "The user name provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.ProviderError:
					return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

				case MembershipCreateStatus.UserRejected:
					return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

				default:
					return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
			}
		}
		#endregion
	}
}