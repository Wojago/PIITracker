
function enable() {
    var items = SP.ListOperation.Selection.getSelectedItems();
    var itemCount = CountDictionary(items);
    return (itemCount == 1);
}

function getListProperties() {
    var currentItem = '';
    var context = SP.ClientContext.get_current();
    web = context.get_web();
    context.load(web);
    var currentlistid = SP.ListOperation.Selection.getSelectedList();
    var currentList = web.get_lists().getById(currentlistid);
    //var url = SP.ClientContext.get_current().get_url();
    var selectedItems = SP.ListOperation.Selection.getSelectedItems(context);
    var count = CountDictionary(selectedItems);
    if (count != 1) {
        alert('More than one item or no items have been selected. Please select exactly one item.');
    }
    //var siteUrl = window.location.protocol + "//" + window.location.host + _spPageContextInfo.siteServerRelativeUrl;
    var i = '';
    for (i in selectedItems) {
        currentItem = currentList.getItemById(selectedItems[i].id);
        context.load(currentItem);
        var selId = selectedItems[i].id;
    }
    context.executeQueryAsync(Function.createDelegate(this, onQuerySucceeded),
    Function.createDelegate(this, onQueryFailed));
    function onQuerySucceeded(sender, args) {
        //var theweb = web.get_url();


        if (currentItem.get_item('Type') == 'List') {

            var pageUrl = SP.Utilities.Utility.getLayoutsPageUrl('/DCMA.SP.TrackingPII/RemovePerms.aspx?listId=' + currentItem.get_item('lGuid') + '&webUrl=' + currentItem.get_item('SiteURL') + '&type=' + currentItem.get_item('Type') + '&cID=' + selId);

            var options1 = SP.UI.$create_DialogOptions();
            options1.width = 600;
            options1.height = 100;
            options1.resizable = 1;
            options1.scroll = 1;
            options1.url = pageUrl;
            //options1.url = '/_layouts/DCMA.SP.TrackingPII/RemovePerms.aspx?listId=' + currentItem.get_item('lGuid') + '&webUrl=' + currentItem.get_item('SiteURL') + '&type=' + currentItem.get_item('Type') + '&pURL=' + siteUrl + '&cID=' + selId;
            SP.UI.ModalDialog.showModalDialog(options1);
        } 
   

        if (currentItem.get_item('Type') == 'Item') {
            var options = SP.UI.$create_DialogOptions(); options.width = 600;
            options.height = 100;
            options.resizable = 1;
            options.scroll = 1;
            var page1Url = SP.Utilities.Utility.getLayoutsPageUrl('/DCMA.SP.TrackingPII/RemovePerms.aspx?itemId=' + currentItem.get_item('itemID') + '&plName=' + currentItem.get_item('Parent') + '&type=' + currentItem.get_item('Type') + '&webUrl=' + currentItem.get_item('SiteURL') + '&cID=' + selId);

            //options.url = '{SiteUrl}/_layouts/DCMA.SP.TrackingPII/RemovePerms.aspx?itemId=' + currentItem.get_item('itemID') + '&plName=' + currentItem.get_item('Parent') + '&type=' + currentItem.get_item('Type') + '&webUrl=' + currentItem.get_item('SiteURL') + '&pURL=' + siteUrl + '&cID=' + selId;

            options.url = page1Url;

            SP.UI.ModalDialog.showModalDialog(options);

        } 
    }
    function onQueryFailed(sender, args) {
        alert('failed' + args.toString());
    }
}
//This function is used to display permissions on selected PII item.It uses column value of the selected item and gets information of the PII content source
//On success function it passes information in URL to code behind file
function viewPermissions() {
    var currentItem = '';
    var context = SP.ClientContext.get_current();
    web = context.get_web();
    context.load(web);
    var currentlistid = SP.ListOperation.Selection.getSelectedList();
    var currentList = web.get_lists().getById(currentlistid);
    //var url = SP.ClientContext.get_current().get_url();
    var selectedItems = SP.ListOperation.Selection.getSelectedItems(context);
    var count = CountDictionary(selectedItems);
    var i = '';
    for (i in selectedItems) {
        currentItem = currentList.getItemById(selectedItems[i].id);
        context.load(currentItem);
    }
    context.executeQueryAsync(Function.createDelegate(this, onQuerySucceeded),
    Function.createDelegate(this, onQueryFailed));
    function onQuerySucceeded(sender, args) {
        var turl = currentItem.get_item('SiteURL')

        if (currentItem.get_item('Type') == 'List') {
            //window.open(currentItem.get_item('SiteURL') + '/_layouts/user.aspx?obj=' + currentItem.get_item('lGuid') + ',' + 'list&list=' + currentItem.get_item('lGuid'));
            var options = SP.UI.$create_DialogOptions();
            options.width = 500;
            options.height = 700;
            options.resizable = 1;
            options.scroll = 1;
            options.url = turl + '/_layouts/user.aspx?obj=' + currentItem.get_item('lGuid') + ',' + 'list&list=' + currentItem.get_item('lGuid');
           SP.UI.ModalDialog.showModalDialog(options);
       }
       //function DialogClosedCallback() { SP.UI.ModalDialog.RefreshPage(SP.UI.DialogResult.OK); } 
        if (currentItem.get_item('Type') == 'Item') {
            var options = SP.UI.$create_DialogOptions();
            options.width = 500;
            options.height = 700;
            options.resizable = 1;
            options.scroll = 1;
            options.url = turl + '/_layouts/User.aspx?obj=' + currentItem.get_item('ParentID') + ',' + currentItem.get_item('itemID') + ',' + 'LISTITEM&List=' + currentItem.get_item('ParentID')

            SP.UI.ModalDialog.showModalDialog(options);
        }
       // function DialogClosedCallback() { SP.UI.ModalDialog.RefreshPage(SP.UI.DialogResult.OK); } 
    }
    function onQueryFailed(sender, args) {
        alert('failed' + args.toString());
    }
}

