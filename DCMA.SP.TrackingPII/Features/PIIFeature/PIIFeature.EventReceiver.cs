using System;
using System.Web;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.Office.Server.Search.Administration;
using Microsoft.SharePoint.Administration;


namespace DCMA.SP.TrackingPII.Features.PIIFeature
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("c92999f5-6958-4ea3-b6ad-3988993805a6")]
    public class PIIScopeFeatureEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.
        private string scopeName = "PII Scope";
        private string displayGroupName = "PII Display Group";
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {

                //SPSite site = properties.Feature.Parent as SPSite;
                using (SPSite site = new SPSite(((SPSite)properties.Feature.Parent).Url))
                {

                    Uri scopeUri = site.WebApplication.GetResponseUri(SPUrlZone.Default, site.ServerRelativeUrl);

                    // remote scopes class retrieves information via search web service so we run this as the search service account

                    RemoteScopes remoteScopes = new RemoteScopes(SPServiceContext.GetContext(site));

                    // see if there is an existing scope

                    Scope scope = (from s

                                   in remoteScopes.GetScopesForSite(new Uri(site.Url)).Cast<Scope>()

                                   where s.Name == scopeName

                                   select s).FirstOrDefault();

                    // only add if the scope doesn't exist already

                    if (scope == null)
                    {
                        scope = remoteScopes.AllScopes.Create(scopeName, "", new Uri(site.Url), true, "results.aspx", ScopeCompilationType.AlwaysCompile);

                        scope.Rules.CreateUrlRule(ScopeRuleFilterBehavior.Require, UrlScopeRuleType.Folder, scopeUri.AbsoluteUri);

                    }

                    // see if there is an existing display group         

                    ScopeDisplayGroup displayGroup = (from d

                                                      in remoteScopes.GetDisplayGroupsForSite(new Uri(site.Url)).Cast<ScopeDisplayGroup>()

                                                      where d.Name == displayGroupName

                                                      select d).FirstOrDefault();

                    // add if the display group doesn't exist

                    if (displayGroup == null)

                        displayGroup = remoteScopes.AllDisplayGroups.Create(displayGroupName, "", new Uri(site.Url), true);

                    // add scope to display group if not already added

                    if (!displayGroup.Contains(scope))
                    {
                        displayGroup.Add(scope);

                        displayGroup.Default = scope;

                        displayGroup.Update();

                    }

                    // optionally force a scope compilation so this is available immediately

                    remoteScopes.StartCompilation();
                }
            });

          
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {

            SPSite site = properties.Feature.Parent as SPSite;

            // remotescopes class retrieves information via search web service so we run this as the search service account

            RemoteScopes remoteScopes = new RemoteScopes(SPServiceContext.GetContext(site));

            // delete scope if found

            Scope scope = (from s

                           in remoteScopes.GetScopesForSite(new Uri(site.Url)).Cast<Scope>()

                           where s.Name == scopeName

                           select s).FirstOrDefault();

            if (scope != null)

                scope.Delete();

            // delete display group if found

            ScopeDisplayGroup displayGroup = (from d

                                              in remoteScopes.GetDisplayGroupsForSite(new Uri(site.Url)).Cast<ScopeDisplayGroup>()

                                              where d.Name == displayGroupName

                                              select d).FirstOrDefault();

            if (displayGroup != null)

                displayGroup.Delete();

        }

        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
