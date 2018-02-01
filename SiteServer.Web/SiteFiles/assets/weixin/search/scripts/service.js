var service = {};
service.baseUrl = '/api/wx_search/';
service.publishmentSystemID = utilService.getUrlVar('publishmentSystemID');

service.getUrl = function(action, id){
  if (id){
    return service.baseUrl + action + '/' + id + '?publishmentSystemID=' + service.publishmentSystemID;
  }
  return service.baseUrl + action + '?publishmentSystemID=' + service.publishmentSystemID;
};

service.getSearchParameter = function(searchID, success){
    $.getJSON(service.getUrl('GetSearchParameter', searchID), null, success);
};

service.getRefreshParameter = function(searchID, imageIDCollection, textIDCollection, success){

    $.getJSON(service.getUrl('GetRefreshParameter', searchID), { 'imageIDCollection': imageIDCollection, 'textIDCollection': textIDCollection }, success);
};

service.getSearchResultParameter = function (searchID, keywords, success) {

    $.getJSON(service.getUrl('GetSearchResultParameter', searchID), { 'keywords': keywords }, success);
};
  
service.getMoreResultParameter = function (searchID, keywords, idCollection, success) {

    $.getJSON(service.getUrl('GetMoreResultParameter', searchID), { 'keywords': keywords, 'idCollection': idCollection }, success);
};
 