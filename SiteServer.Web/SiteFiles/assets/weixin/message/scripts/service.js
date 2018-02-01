var service = {};
service.baseUrl = '/api/wx_message/';
service.publishmentSystemID = utilService.getUrlVar('publishmentSystemID');

service.getUrl = function(action, id){
  if (id){
    return service.baseUrl + action + '/' + id + '?publishmentSystemID=' + service.publishmentSystemID;
  }
  return service.baseUrl + action + '?publishmentSystemID=' + service.publishmentSystemID;
};

service.getMessageParameter = function(messageID, wxOpenID, success){
    $.getJSON(service.getUrl('GetMessageParameter', messageID), {'wxOpenID' : wxOpenID}, success);
};

service.submitContent = function(messageID, wxOpenID, displayName, color, content, success){
    $.getJSON(service.getUrl('SubmitContent', messageID), {'wxOpenID' : wxOpenID, 'displayName' : displayName, 'color' : color, 'content' : content}, success);
};

service.submitReply = function(messageID, replyContentID, wxOpenID, displayName, content, success){
    $.getJSON(service.getUrl('SubmitReply', messageID), {'replyContentID' : replyContentID, 'wxOpenID' : wxOpenID, 'displayName' : displayName, 'content' : content}, success);
};

service.submitLike = function(messageID, contentID, wxOpenID, success){
    $.getJSON(service.getUrl('SubmitLike', messageID), {'contentID' : contentID, 'wxOpenID' : wxOpenID}, success);
};

service.more = function(messageID, startNum, success){
    $.getJSON(service.getUrl('More', messageID), {'startNum' : startNum}, success);
};