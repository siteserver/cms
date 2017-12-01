var service = {};
service.baseUrl = '/api/wx_lottery/';
service.publishmentSystemID = utilService.getUrlVar('publishmentSystemID');

service.getUrl = function(action, id){
  if (id){
    return service.baseUrl + action + '/' + id + '?publishmentSystemID=' + service.publishmentSystemID;
  }
  return service.baseUrl + action + '?publishmentSystemID=' + service.publishmentSystemID;
};

service.getLotteryParameter = function(lotteryID, wxOpenID, success){
    $.getJSON(service.getUrl('GetLotteryParameter', lotteryID), {'wxOpenID' : wxOpenID}, success);
};

service.submitLottery = function(lotteryID, wxOpenID, success){
    $.getJSON(service.getUrl('SubmitLottery', lotteryID), {'wxOpenID' : wxOpenID}, success);
};

service.submitApplication = function(lotteryID, winnerID, realName, mobile, email, address, success){
    $.getJSON(service.getUrl('SubmitApplication', lotteryID), {'winnerID' : winnerID, 'realName' : realName, 'mobile' : mobile, 'email' : email, 'address' : address}, success);
};

service.submitAwardCode = function(lotteryID, winnerID, success){
    $.getJSON(service.getUrl('SubmitAwardCode', lotteryID), {'winnerID' : winnerID}, success);
};
 