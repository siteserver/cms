var service = {};
service.baseUrl = '/api/wx_collect/';
service.publishmentSystemID = utilService.getUrlVar('publishmentSystemID');
service.collectID = utilService.getUrlVar('collectID');
service.wxOpenID = utilService.getUrlVar('wxOpenID');
service.itemID = utilService.getUrlVar('itemID');

service.getUrl = function(action, id){
  if (id){
    return service.baseUrl + action + '/' + id + '?publishmentSystemID=' + service.publishmentSystemID;
  }
  return service.baseUrl + action + '?publishmentSystemID=' + service.publishmentSystemID;
};

service.getCollectParameter = function(success){
    $.getJSON(service.getUrl('GetCollectParameter', service.collectID), {'wxOpenID' : service.wxOpenID}, success);
};

service.submitUpload = function(title, description, mobile, smallUrl, largeUrl, success){
    $.post(service.getUrl('SubmitUpload', service.collectID), { 'wxOpenID': service.wxOpenID, 'title': title, 'description': description, 'mobile': mobile, 'smallUrl': smallUrl, 'largeUrl': largeUrl }, success);
};

service.submitVote = function(itemID, success){
    $.getJSON(service.getUrl('SubmitVote', service.collectID), { 'itemID': itemID }, success);
};

service.getUploadUrl = function(){
  return service.getUrl('Upload', service.collectID);
};

service.redirectToIndex = function(){
  location.href = 'index.html?collectID=' + service.collectID + '&publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID='+service.wxOpenID;
};

service.redirectToUpload = function(){
  location.href = 'upload.html?collectID=' + service.collectID + '&publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID='+service.wxOpenID;
};

service.redirectToVote = function(){
  location.href = 'vote.html?collectID=' + service.collectID + '&publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID='+service.wxOpenID;
};

service.redirectToItem = function(itemID){
  location.href = 'item.html?collectID=' + service.collectID + '&publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID=' + service.wxOpenID + '&itemID=' + itemID;
};

service.redirectToEnd = function(){
  location.href = 'end.html?collectID=' + service.collectID + '&publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID='+service.wxOpenID;
};