﻿using Goldmint.Common;
using Goldmint.CoreLogic.Services.Localization;
using Goldmint.CoreLogic.Services.Notification.Impl;
using Goldmint.WebApplication.Core.Policies;
using Goldmint.WebApplication.Core.Response;
using Goldmint.WebApplication.Core.Tokens;
using Goldmint.WebApplication.Models.API.v1.OAuthModels;
using Goldmint.WebApplication.Services.OAuth;
using Goldmint.WebApplication.Services.OAuth.Impl;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Goldmint.WebApplication.Core;

namespace Goldmint.WebApplication.Controllers.v1 {

	[Route("api/v1/oauth")]
	public class OAuthController : BaseController {

		/// <summary>
		/// Create redirect
		/// </summary>
		[AnonymousAccess]
		[HttpGet, Route("google")]
		[ProducesResponseType(typeof(RedirectView), 200)]
		public async Task<APIResponse> Google() {

			var provider = HttpContext.RequestServices.GetRequiredService<GoogleProvider>();

			return APIResponse.Success(
				new RedirectView() {
					Redirect = await provider.GetRedirect(
						Url.Link("OAuthGoogleCallback", new { }),
						null
					),
				}
			);
		}

		/// <summary>
		/// On callback
		/// </summary>
		[AnonymousAccess]
		[HttpGet, Route("googleCallback", Name = "OAuthGoogleCallback")]
		[ProducesResponseType(302)]
		[ApiExplorerSettings(IgnoreApi = true)]
		public async Task<IActionResult> GoogleCallback(string error, string code, string state) {
			try {

				if (!string.IsNullOrWhiteSpace(error) || string.IsNullOrWhiteSpace(code)) {
					throw new Exception("Invalid callback result");
				}

				var provider = HttpContext.RequestServices.GetRequiredService<GoogleProvider>();
				var userInfo = await provider.GetUserInfo(
					Url.Link("OAuthGoogleCallback", new { }),
					state,
					code
				);

				return await ProcessOAuthCallback(
					LoginProvider.Google,
					userInfo
				);
			}
			catch {
				return Redirect("/");
			}
		}

		// ---

		[NonAction]
		public async Task<RedirectResult> ProcessOAuthCallback(LoginProvider provider, UserInfo userInfo) {

			var audience = JwtAudience.Cabinet;

			// find user with this ext login
			var user = await UserManager.FindByLoginAsync(provider.ToString(), userInfo.Id);
			var userLocale = GetUserLocale();

			// exists
			if (user != null) {

				// try to sign in
				var signResult = await SignInManager.CanSignInAsync(user);

				// check rights
				var accessRightsMask = Core.UserAccount.ResolveAccessRightsMask(HttpContext.RequestServices, audience, user);

				if (signResult && accessRightsMask != null) {

					var agent = GetUserAgentInfo();

					// notification
					await EmailComposer.FromTemplate(await TemplateProvider.GetEmailTemplate(EmailTemplate.SignedIn, userLocale))
						.ReplaceBodyTag("IP", agent.Ip)
						.Initiator(agent.Ip, agent.Agent, DateTime.UtcNow)
						.Send(user.Email, user.UserName, EmailQueue)
					;

					// activity
					var userActivity = CoreLogic.User.CreateUserActivity(
						user: user,
						type: Common.UserActivityType.Auth,
						comment: "Signed in with social network",
						ip: agent.Ip,
						agent: agent.Agent,
						locale: userLocale
					);
					DbContext.UserActivity.Add(userActivity);
					await DbContext.SaveChangesAsync();

					// tfa required
					if (user.TwoFactorEnabled) {

						var tokenForTfa = JWT.CreateAuthToken(
							appConfig: AppConfig,
							user: user,
							audience: audience,
							area: JwtArea.Tfa,
							rightsMask: accessRightsMask.Value
						);
						return Redirect(
							this.MakeAppLink(audience, fragment: AppConfig.Apps.Cabinet.RouteOAuthTfaPage.Replace(":token", tokenForTfa))
						);
					}

					// new jwt salt
					UserAccount.GenerateJwtSalt(user, audience);
					DbContext.SaveChanges();

					// ok
					var token = JWT.CreateAuthToken(
						appConfig: AppConfig, 
						user: user, 
						audience: audience,
						area: JwtArea.Authorized,
						rightsMask: accessRightsMask.Value
					);
					return Redirect(
						this.MakeAppLink(audience, fragment: AppConfig.Apps.Cabinet.RouteOAuthAuthorized.Replace(":token", token))
					);
				}

				// never should get here
				return Redirect("/");
			}

			// doesnt exists yet
			else {

				var email = userInfo.Email;

				// try create and sign in
				var cuaResult = await Core.UserAccount.CreateUserAccount(HttpContext.RequestServices, email);
				if (cuaResult.User != null) {

					// user created and external login attached
					if (await CreateExternalLogin(cuaResult.User, provider, userInfo)) {

						var accessRightsMask = Core.UserAccount.ResolveAccessRightsMask(HttpContext.RequestServices, audience, cuaResult.User);
						if (accessRightsMask != null) {

							// jwt salt
							var jwtSalt = UserAccount.CurrentJwtSalt(cuaResult.User, audience);

							// send dpa
							var tokenForDpa = Core.Tokens.JWT.CreateSecurityToken(
								appConfig: AppConfig,
								entityId: cuaResult.User.UserName,
								audience: audience,
								securityStamp: jwtSalt,
								area: JwtArea.Dpa,
								validFor: TimeSpan.FromDays(1)
							);
							await Core.UserAccount.ResendUserDpaDocument(
								locale: userLocale,
								services: HttpContext.RequestServices,
								user: cuaResult.User,
								email: cuaResult.User.Email,
								redirectUrl: this.MakeAppLink(audience, fragment: AppConfig.Apps.Cabinet.RouteDpaSigned.Replace(":token", tokenForDpa))
							);

							// dpa is unsigned
							if (!CoreLogic.User.HasSignedDpa(cuaResult.User.UserOptions)) {
								return Redirect(
									this.MakeAppLink(audience, fragment: AppConfig.Apps.Cabinet.RouteDpaRequired)
								);
							}

							// ok
							var token = JWT.CreateAuthToken(
								appConfig: AppConfig,
								user: cuaResult.User,
								audience: audience,
								area: JwtArea.Authorized,
								rightsMask: accessRightsMask.Value
							);
							return Redirect(
								this.MakeAppLink(audience, fragment: AppConfig.Apps.Cabinet.RouteOAuthAuthorized.Replace(":token", token))
							);
						}
					}

					// failed
					return Redirect("/");
				}

				// redirect to error OR email input
				return Redirect(
					this.MakeAppLink(audience, fragment: AppConfig.Apps.Cabinet.RouteEmailTaken)
				);
			}
		}

		[NonAction]
		private async Task<bool> CreateExternalLogin(DAL.Models.Identity.User user, LoginProvider provider, UserInfo userInfo) {

			// attach login
			var res = await UserManager.AddLoginAsync(user, new UserLoginInfo(provider.ToString(), userInfo.Id, provider.ToString()));

			// sign in
			if (res.Succeeded && await SignInManager.CanSignInAsync(user)) {
				return true;
			}
			return false;
		}

	}
}
