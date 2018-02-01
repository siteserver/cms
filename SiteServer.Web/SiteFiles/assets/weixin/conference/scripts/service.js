var service = {};
service.baseUrl = '/api/wx_conference/';
service.publishmentSystemID = utilService.getUrlVar('publishmentSystemID');

service.getUrl = function(action, id){
  if (id){
    return service.baseUrl + action + '/' + id + '?publishmentSystemID=' + service.publishmentSystemID;
  }
  return service.baseUrl + action + '?publishmentSystemID=' + service.publishmentSystemID;
};

service.getConferenceParameter = function(conferenceID, wxOpenID, success){
    $.getJSON(service.getUrl('GetConferenceParameter', conferenceID), {'wxOpenID' : wxOpenID}, success);
};

service.submitApplication = function(conferenceID, wxOpenID, realName, mobile, email, company, position, note, success){
    $.getJSON(service.getUrl('SubmitApplication', conferenceID), {'wxOpenID' : wxOpenID, 'realName' : realName, 'mobile' : mobile, 'email' : email, 'company' : company, 'position' : position, 'note' : note}, success);
};