﻿//-----------------------------------------------------------------------
// <copyright file="GlobalEventsHandler.cs" company="Atomia AB">
//     Copyright (c) Atomia AB. All rights reserved.
// </copyright>
// <author> Ilija Nikolic </author>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Web;
using Atomia.Web.Base.Configs;
using Atomia.Web.Base.Helpers.General;
using Atomia.Web.Plugin.OrderServiceReferences.AtomiaBillingPublicService;
using Elmah;

namespace Atomia.Web.Plugin.PublicOrder.GeneralItems
{
    using Atomia.Web.Plugin.PublicOrder.Helpers.ActionTrail;

    /// <summary>
    /// Implemets Global.asax methods
    /// </summary>
    public class GlobalEventsHandler : GlobalEventsDefaultHandler
    {
        /// <summary>
        /// Handles the PreRequestHandlerExecute event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public override void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            if (HttpContext.Current != null && HttpContext.Current.Request.Params.AllKeys.Any(item => item == "theme"))
            {
                if (AppConfig.Instance.ThemesList.Cast<Theme>().Any(theme => theme.Name.Trim() == HttpContext.Current.Request.Params["theme"].Trim()))
                {
                    if (HttpContext.Current.Session != null)
                    {
                        HttpContext.Current.Session["Theme"] = HttpContext.Current.Request.Params["theme"];
                    }
                }
            }
        }

        /// <summary>
        /// Handles the BeginRequest event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public override void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContext.Current.Response.AppendHeader("Pragma", "no-cache");
            HttpContext.Current.Response.AppendHeader("Cache-Control", "no-cache");

            HttpContext.Current.Response.CacheControl = "no-cache";
            HttpContext.Current.Response.Expires = -1;

            HttpContext.Current.Response.ExpiresAbsolute = new DateTime(1900, 1, 1);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        }

        /// <summary>
        /// Handles the Start event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public override void Application_Start(object sender, EventArgs e)
        {
            try
            {
                using (AtomiaBillingPublicService service = new AtomiaBillingPublicService())
                {
                    service.Url = HttpContext.Current.Application["OrderApplicationPublicServiceURL"].ToString();

                    DomainSearch.Helpers.DomainSearchHelper.LoadProductsIntoSession(service, Guid.Empty, null, null);
                }
            }
            catch (Exception ex)
            {
                OrderPageLogger.LogOrderPageException(ex);
            }

            base.Application_Start(sender, e);
        }
    }
}
