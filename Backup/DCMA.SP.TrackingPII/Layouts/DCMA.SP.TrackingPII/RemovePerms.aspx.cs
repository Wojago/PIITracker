using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace DCMA.SP.TrackingPII.Layouts.DCMA.SP.TrackingPII
{
    public partial class RemovePerms : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                //Get and store data from query string
                string listGuid = Request.Params["listId"];
                int itemId = Convert.ToInt32(Request.Params["itemId"]);
                string type = Request.Params["type"];
                int cid = Convert.ToInt32(Request.Params["cID"]);
                //string purl = Request.Params["pURL"];
                string parent = Request.Params["plName"];
                string webUrl = Request.Params["WebUrl"];
                if (type != null && type == "List")
                {
                    if (listGuid == null && webUrl == null) return;

                        SPList pList = null;
                        using (SPSite site = new SPSite(webUrl))
                        using (SPWeb web = site.OpenWeb())
                        {
                            web.AllowUnsafeUpdates = true;
                            pList = web.Lists[new Guid(listGuid)];
                            SPGroup PIIgroup = null;
                            //Create PII Members group and add to the site collection with Full Control permisions
                            try
                            {
                                PIIgroup = web.SiteGroups["PII Members"];
                            }
                            catch
                            {
                            }
                            if (PIIgroup == null)
                            {
                                web.SiteGroups.Add("PII Members", web.CurrentUser, null, "A group to manage PII list");

                            }
                            PIIgroup = web.SiteGroups["PII Members"];
                            SPRoleAssignment roleAssignment = new SPRoleAssignment(PIIgroup);
                            SPRoleDefinition permissionRole = web.RoleDefinitions["Full Control"];
                            roleAssignment.RoleDefinitionBindings.Add(permissionRole);
                            if (!pList.HasUniqueRoleAssignments)
                            {
                                pList.BreakRoleInheritance(false); //Break existing pemrissions, add the new group 
                            }
                            pList.RoleAssignments.Add(roleAssignment);
                            pList.Update();
                            web.Update();
                            web.AllowUnsafeUpdates = false;
                            try
                            {
                                SPWeb lweb = SPContext.Current.Web;
                                lweb.AllowUnsafeUpdates = true;

                                //Update list column once permission is updated
                                SPList PIIList = lweb.Lists.TryGetList("PII Tracking List");
                                SPListItem cSataus = PIIList.GetItemById(cid);
                                cSataus["Permissions Status"] = "Updated";
                                cSataus.Update();
                            }
                            catch (Exception)
                            {

                                //ListPermLiteral.Text = "You have successfully updated permissions on the " + " " + pList.Title + " " + "list, but the permissions column in the list is not updated with the term 'Permissions Updated'. Please update manually. ";
                            }
                            
                        }
                        ListPermLiteral.Text = "You have successfully updated permissions on the " + " " + pList.Title + " " + "list.";
                    
                }
                SPListItem item = null;

                if (type != null && type == "Item")
                {
                    if (parent == null || webUrl == null) return;
                        using (SPSite msite = new SPSite(webUrl))
                        using (SPWeb mweb = msite.OpenWeb())
                        {
                            mweb.AllowUnsafeUpdates = true;
                            SPList iList = mweb.Lists.TryGetList(parent);
                            item = iList.GetItemById(itemId);
                            SPGroup pgroup = null;
                            //Create PII Members group and add to the site collection with Full Control permisions
                            try
                            {
                                pgroup = mweb.SiteGroups["PII Members"];
                            }
                            catch
                            {
                            }
                            if (pgroup == null)
                            {
                                mweb.SiteGroups.Add("PII Members", mweb.CurrentUser, null, "A group to manage PII list");
                            }
                            pgroup = mweb.SiteGroups["PII Members"];
                            SPRoleAssignment roleAssignment = new SPRoleAssignment(pgroup);
                            SPRoleDefinition permissionRole = mweb.RoleDefinitions["Full Control"];
                            roleAssignment.RoleDefinitionBindings.Add(permissionRole);
                            if (!item.HasUniqueRoleAssignments)
                            {
                                item.BreakRoleInheritance(false); // Break existing pemrission, add new the group 
                            }
                            item.RoleAssignments.Add(roleAssignment);
                            item.Update();
                            mweb.Update();
                            mweb.AllowUnsafeUpdates = false;
                             //Update list column once permission has been updated
                            try
                            {
                                SPWeb iweb = SPContext.Current.Web;
                                iweb.AllowUnsafeUpdates = true;

                                SPList PIIList = iweb.Lists.TryGetList("PII Tracking List");
                                SPListItem iSataus = PIIList.GetItemById(cid);
                                iSataus["Permissions Status"] = "Updated";
                                iSataus.Update();
                            }
                            catch (Exception)
                            {
                                //ListPermLiteral.Text = "You have successfully updated permissions on the " + " " + item.Title + " " + "list, but the permissions column in the list is not updated with the term 'Permissions Updated'. Please update manually. ";  
                            }
                            
                        }
                        ListPermLiteral.Text = "You have successfully updated permissions on the" + " " + item.Name + " " + "item.";

                }

                }
                catch (Exception ex)
                {
                    //throw;
                    ListPermLiteral.Text = "The operation did not complete successfully. Please refresh the browser and try again. If the problem persists contact site administrator for additional support. </br></br>*" + ex.Message;  
                }


            }
        }
    }
}
