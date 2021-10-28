/*
' Copyright (c) 2021  Gafware
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using System;
using System.Web;
using System.Web.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;

namespace Gafware.Modules.KeepAlive
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The View class displays the content
    /// 
    /// Typically your view control would be used to display content or functionality in your module.
    /// 
    /// View may be the only control you have in your project depending on the complexity of your module
    /// 
    /// Because the control inherits from KeepAliveModuleBase you have access to any custom properties
    /// defined there, as well as properties from DNN such as PortalId, ModuleId, TabId, UserId and many more.
    /// 
    /// </summary>
    /// -----------------------------------------------------------------------------
    public partial class View : KeepAliveModuleBase, IActionable
    {
        public int TimeOut
        {
            get
            {
                return (Config.GetSetting("PersistentCookieTimeout") != null ? int.Parse(Config.GetSetting("PersistentCookieTimeout")) : 30);
            }
        }

        public string IsLoggedIn
        {
            get
            {
                DotNetNuke.Entities.Users.UserInfo currentUser = DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo();
                return (currentUser.UserID > -1).ToString().ToLower();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                //check if user has supplied custom value for expiration
                int persistentCookieTimeout = (int)FormsAuthentication.Timeout.TotalMinutes; // Session.Timeout;
                /*if (Config.GetSetting("PersistentCookieTimeout") != null)
                {
                    persistentCookieTimeout = int.Parse(Config.GetSetting("PersistentCookieTimeout"));
                }*/
                //only use if non-zero, otherwise leave as asp.net value
                if (persistentCookieTimeout != 0)
                {
                    //locate and update cookie
                    string authCookie = FormsAuthentication.FormsCookieName;
                    if (HttpContext.Current.Request.Cookies[authCookie] != null)
                    {
                        HttpContext.Current.Request.Cookies[authCookie].Expires = DateTime.Now.AddMinutes(persistentCookieTimeout).AddHours(DateTimeOffset.Now.Offset.Hours);
                        DotNetNuke.Entities.Users.UserInfo currentUser = DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo();
                        if (HttpContext.Current.Response.Cookies[authCookie] == null)
                        {
                            HttpContext.Current.Response.Cookies.Add(HttpContext.Current.Request.Cookies[authCookie]);
                        }
                        else
                        {
                            HttpContext.Current.Response.Cookies.Remove(authCookie);
                            HttpContext.Current.Response.Cookies.Add(HttpContext.Current.Request.Cookies[authCookie]);
                        }
                        FormsAuthenticationTicket newTicket = new FormsAuthenticationTicket(currentUser.Username, true, persistentCookieTimeout);
                        HttpContext.Current.Response.Cookies[authCookie].Value = FormsAuthentication.Encrypt(newTicket);                                
                        HttpContext.Current.Response.Cookies[authCookie].Expires = DateTime.Now.AddMinutes(persistentCookieTimeout).AddHours(DateTimeOffset.Now.Offset.Hours);
                    }
                    authCookie = "authentication";
                    if (HttpContext.Current.Request.Cookies[authCookie] != null)
                    {
                        HttpContext.Current.Request.Cookies[authCookie].Expires = DateTime.Now.AddMinutes(persistentCookieTimeout).AddHours(DateTimeOffset.Now.Offset.Hours);
                        DotNetNuke.Entities.Users.UserInfo currentUser = DotNetNuke.Entities.Users.UserController.Instance.GetCurrentUserInfo();
                        if (HttpContext.Current.Response.Cookies[authCookie] == null)
                        {
                            HttpContext.Current.Response.Cookies.Add(HttpContext.Current.Request.Cookies[authCookie]);
                        }
                        else
                        {
                            HttpContext.Current.Response.Cookies.Remove(authCookie);
                            HttpContext.Current.Response.Cookies.Add(HttpContext.Current.Request.Cookies[authCookie]);
                        }
                        FormsAuthenticationTicket newTicket = new FormsAuthenticationTicket(currentUser.Username, true, persistentCookieTimeout);
                        HttpContext.Current.Response.Cookies[authCookie].Value = FormsAuthentication.Encrypt(newTicket);                                
                        HttpContext.Current.Response.Cookies[authCookie].Expires = DateTime.Now.AddMinutes(persistentCookieTimeout).AddHours(DateTimeOffset.Now.Offset.Hours);
                    }
                }
            }
            catch (Exception) //Module failed to load
            {
               // Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        public ModuleActionCollection ModuleActions
        {
            get
            {
                return new ModuleActionCollection();
            }
        }

        protected void lnkRefresh_Click(object sender, EventArgs e)
        {
            // Do nothing
        }
    }
}