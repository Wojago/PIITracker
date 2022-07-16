using System;
using System.Data;
using System.Text;
using Microsoft.Office.Server.Search.Internal.UI;
using Microsoft.Office.Server.Search.Administration;
using Microsoft.Office.Server.Search.Query;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System.ComponentModel;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using System.Web.UI;


namespace DCMA.SP.TrackingPII.CreatePIIListWP
{
    public partial class CreatePIIListWPUserControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        protected void ButtonPII_Click(object sender, EventArgs e)
        {

            try
            {
                //connect to the service application proxy
                SearchServiceApplicationProxy proxy = (SearchServiceApplicationProxy)SearchServiceApplicationProxy.GetProxy(SPServiceContext.GetContext(SPContext.Current.Site));
                FullTextSqlQuery query = new FullTextSqlQuery(proxy);
                query.ResultsProvider = SearchProvider.Default;
                query.ResultTypes = ResultType.RelevantResults;
                query.KeywordInclusion = KeywordInclusion.AnyKeyword;
                query.TrimDuplicates = true;
                query.EnableStemming = true;
                query.RowLimit = 10000;
                query.IgnoreAllNoiseQuery = true;
                string queryText = "select title, path, ContentClass from Scope()  WHERE \"scope\" = 'PII Scope' AND FREETEXT('SSN OR DOB OR SENSITIVE OR PII')";
                query.QueryText = queryText;
                ResultTableCollection results = query.Execute();
                if (results.Exists(ResultType.RelevantResults))
                {
                    Uri resultUri;
                    //Set up and load the data table
                    ResultTable relevant = results[ResultType.RelevantResults];
                    DataTable search = new DataTable();
                    search.Load(relevant);
                    //Create a new list in the current site to add the search results if list not available 
                    SPList newList = null;
                    SPSite site = SPContext.Current.Site;
                    SPWeb PIIWeb = site.OpenWeb();
                    SPListCollection lists = PIIWeb.Lists;
                    {
                        SPList dList = PIIWeb.Lists.TryGetList("PII Tracking List");
                        {
                            if (dList == null)
                                newList = creatPIIList(newList, PIIWeb, lists);

                            if (dList != null)
                            {
                                foreach (DataRow dr in search.Rows)
                                {
                                    if (dr["Path"] != null & Uri.TryCreate(dr["Path"].ToString(), UriKind.Absolute, out resultUri))
                                    {

                                        //Get site url from path in result collection  
                                        SPWeb web;
                                        using (SPSite siteCol = new SPSite(dr["Path"].ToString()))
                                        {
                                            web = siteCol.OpenWeb();
                                        }
                                        //Select content class for items and add the item to PII tracking list 
                                        if (dr["ContentClass"].ToString().StartsWith("STS_ListItem"))
                                        {
                                            SPListItem item = web.GetListItem(dr["Path"].ToString());
                                            if (item != null)
                                                createNewItem(PIIWeb, dr, web, item);
                                        }
                                        else
                                        {
                                            //Select content class for list and add to PII tracking list
                                            SPList list = web.GetList(dr["Path"].ToString());
                                            if (list != null)
                                                createNewListItem(PIIWeb, dr, web, list);
                                        }
                                        

                                    }
                                }
                                   // InfoLabel.Text = "Operation completed successfully!";

                                    Response.Redirect(PIIWeb.Url.ToString() + "/" + dList.RootFolder.Url.ToString(), false);

                                

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                InfoLabel.Text = "</br>The operation did not complete successfully. Please refresh the browser and try again or try later. If the problem persists, contact site administrator for additional support by referencing comments below.</br></br>*" + ex.Message;
                //throw;
            }
        }

        private static void createNewListItem(SPWeb PIIWeb, DataRow dr, SPWeb web, SPList list)
        {
            SPList PIIList = PIIWeb.Lists["PII Tracking List"];
            SPListItemCollection items = PIIList.Items;
            SPListItem newlistItem = PIIList.Items.Add();
            newlistItem["Title"] = list.Title;
            newlistItem["Type"] = "List";
            newlistItem["Source"] = dr["Path"].ToString();
            newlistItem["SiteURL"] = web.Url;
            SPFieldUrlValue url = new SPFieldUrlValue();
            string value = dr["Path"].ToString();
            value.Replace(" ", "%20");
            newlistItem["Path"] = value;
            newlistItem["lGuid"] = list.ID;
            newlistItem.Update();
        }

        private static void createNewItem(SPWeb PIIWeb, DataRow dr, SPWeb web, SPListItem item)
        {
            //int itemid = item.ID;
            SPList PIIList = PIIWeb.Lists["PII Tracking List"];
            SPListItemCollection items = PIIList.Items;
            SPListItem newitem = PIIList.Items.Add();
            newitem["Title"] = item.Name;
            newitem["Type"] = "Item";
            string value = dr["Path"].ToString();
            value.Replace(" ", "%20");
            newitem["SiteURL"] = web.Url;
            newitem["itemID"] = item.ID;
            newitem["Source"] = dr["Path"].ToString();
            newitem["Parent"] = item.ParentList;
            newitem["ParentID"] = item.ParentList.ID;
            newitem["Path"] = value;
            newitem.Update();
        }

        private static SPList creatPIIList(SPList newList, SPWeb PIIWeb, SPListCollection lists)
        {
            //create list and add fields if list is available
            lists.Add("PII Tracking List", "This list is created to track PII content and manage permissions", SPListTemplateType.GenericList);
            newList = PIIWeb.Lists["PII Tracking List"];
            newList.NoCrawl = true;
            newList.BreakRoleInheritance(false);
            newList.OnQuickLaunch = true;
            newList.Fields.Add("Title", SPFieldType.Text, false);
            newList.Fields.Add("SiteURL", SPFieldType.Text, false);
            newList.Fields.Add("Path", SPFieldType.URL, false);
            newList.Fields.Add("itemID", SPFieldType.Text, false);
            newList.Fields.Add("Permissions Status", SPFieldType.Text, false);
            newList.Fields.Add("Source", SPFieldType.Text, false);
            newList.Fields.Add("Type", SPFieldType.Text, false);
            newList.Fields.Add("Parent", SPFieldType.Text, false);
            newList.Fields.Add("ParentID", SPFieldType.Text, false);
            newList.Fields.Add("lGuid", SPFieldType.Text, false);
            newList.Fields.Add("BaseType", SPFieldType.Text, false);
            newList.Update();
            //select display fields 
            SPView view = newList.DefaultView;
            view.ViewFields.Add("Path");
            view.ViewFields.Add("Permissions Status");
            //Group by fields 
            string viewQuery = @" <GroupBy Collapse=""TRUE"" GroupLimit=""100""><FieldRef Name=""Created"" /><FieldRef Name=""SiteURL"" /></GroupBy>";
            view.Query = viewQuery;
            view.Update();
            return newList;
        }

    }

}


        
    

