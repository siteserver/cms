var service = {};
service.baseUrl = '/api/wx_vote/';
service.publishmentSystemID = utilService.getUrlVar('publishmentSystemID');

service.getUrl = function (action, id) {
    if (id) {
        return service.baseUrl + action + '/' + id + '?publishmentSystemID=' + service.publishmentSystemID;
    }
    return service.baseUrl + action + '?publishmentSystemID=' + service.publishmentSystemID;
};

service.getVoteParameter = function (voteID, wxOpenID, success) {
    $.getJSON(service.getUrl('GetVoteParameter', voteID), { 'wxOpenID': wxOpenID }, success);
};

service.submitVote = function (voteID, wxOpenID, itemIDCollection, realName, agentsType, success) {
    $.getJSON(service.getUrl('SubmitVote', voteID), { 'wxOpenID': wxOpenID, 'itemIDCollection': itemIDCollection, 'realName': realName, 'agentsType': agentsType }, success);
};