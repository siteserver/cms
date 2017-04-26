var service = {};
service.baseUrl = '/api/wx_album/';
service.publishmentSystemID = utilService.getUrlVar('publishmentSystemID');

service.getUrl = function(action, id){
  if (id){
    return service.baseUrl + action + '/' + id + '?publishmentSystemID=' + service.publishmentSystemID;
  }
  return service.baseUrl + action + '?publishmentSystemID=' + service.publishmentSystemID;
};

service.getAlbumParameter = function(albumID, wxOpenID, success){
    $.getJSON(service.getUrl('GetAlbumParameter', albumID), {'wxOpenID' : wxOpenID}, success);
};

service.getPhotoParameter = function(albumID, parentID, wxOpenID, success){
    $.getJSON(service.getUrl('GetPhotoParameter', albumID), {'wxOpenID': wxOpenID, 'parentID': parentID}, success);
};
 