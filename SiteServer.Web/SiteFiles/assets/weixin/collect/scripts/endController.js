var controller = {};

controller.isLoading = true;

controller.voteID = utilService.getUrlVar('voteID');
controller.wxOpenID = utilService.getUrlVar('wxOpenID');
 
controller.isPoweredBy = false;
controller.poweredBy = '';
controller.isVoted = false;
controller.totalVoteNum = 0;
controller.voteInfo = {};
controller.itemList = [];
controller.itemIDCollection = '';

controller.render = function(){
  utilService.render('controller', controller);
  

  $('#loading').hide();
  $('#main').show();
 
};

controller.main = function () {
    service.getVoteParameter(controller.voteID, controller.wxOpenID, function (data) {
        controller.isLoading = false;
        if (data.isSuccess) {

            controller.isPoweredBy = data.isPoweredBy;
            controller.poweredBy = data.poweredBy;
            controller.isVoted = data.isVoted;
            controller.voteInfo = data.voteInfo;
            controller.itemList = data.itemList;
            controller.totalVoteNum = 0;

            for (i = 0; i < controller.itemList.length; i++) {
                var item = controller.itemList[i];
                controller.totalVoteNum += item.voteNum;
            }

            controller.render();

        } else {
            notifyService.error(data.errorMessage);
        }
    });
};

controller.main();

controller.getPercentage = function(voteNum){
  if (controller.totalVoteNum == 0){
    controller.totalVoteNum = 1;
  }
  return Math.round((voteNum / controller.totalVoteNum) * 100) + '%';
};

controller.isItem = function(itemID){
  if (controller.itemIDCollection){
      var array = JSON.parse("[" + controller.itemIDCollection + "]");
      if (array && array.indexOf(itemID) != -1){
        return true;
      }
  }
  return false;
};
 