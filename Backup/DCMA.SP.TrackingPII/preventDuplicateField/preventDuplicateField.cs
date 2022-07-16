using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

namespace DCMA.SP.TrackingPII.preventDuplicateField
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class preventDuplicateField : SPItemEventReceiver
    {
       /// <summary>
       /// An item is being added.
       /// </summary>
       public override void ItemAdding(SPItemEventProperties properties)
       {
           base.ItemAdding(properties);
           //Lok for PII tracking list
           if (properties.ListTitle.Equals("PII Tracking List"))
           {

               try
               {

                   using (SPSite thisSite = new SPSite(properties.WebUrl))
                   {

                       SPWeb thisWeb = thisSite.OpenWeb();

                       SPList list = thisWeb.Lists[properties.ListId];

                       SPQuery query = new SPQuery();
                       //if the title already exists, do not add 
                       query.Query = @"<Where><Eq><FieldRef Name='Title' /><Value Type='Text'>" + properties.AfterProperties["Title"] + "</Value></Eq></Where>";

                       SPListItemCollection listItem = list.GetItems(query);

                       if (listItem.Count > 0)
                       {

                           properties.Cancel = true;

                           properties.Status = SPEventReceiverStatus.CancelNoError;

                           //properties.ErrorMessage = "Item with this Name already exists.";

                       }

                   }

               }

               catch (Exception ex)
               {

                   //PortalLog.LogString("Error occured in event ItemAdding(SPItemEventProperties properties)() @ AAA.BBB.PreventDuplicateItem class. Exception Message:" + ex.Message.ToString());

                   //throw new SPException("An error occured while processing the My List Feature. Please contact your Portal Administrator");

               }

           }

       }

    }

}


