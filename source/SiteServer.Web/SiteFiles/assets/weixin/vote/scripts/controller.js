var controller = {};

controller.isLoading = true;

controller.voteID = utilService.getUrlVar('voteID');
controller.wxOpenID = utilService.getUrlVar('wxOpenID');
controller.uniqueID = utilService.getUrlVar('uniqueID');
controller.from = utilService.getUrlVar('from');

controller.isPoweredBy = false;
controller.poweredBy = '';
controller.isVoted = false;
controller.totalVoteNum = 0;
controller.voteInfo = {};
controller.itemList = [];
controller.itemIDCollection = '';

controller.openCheck = function () {
    location.href = 'text.html?voteID=' + controller.voteID + '&publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID=';
};

controller.render = function () {
    utilService.render('controller', controller);

    $('.col-xs-12').click(function () {
        if (controller.voteInfo.contentIsCheckBox) {
            $(this).toggleClass('title_list_H');
        }
        else {
            $(this).addClass('title_list_H').siblings().removeClass('title_list_H').end();
        }
    });

    $('.col-md-12').click(function () {
        if (controller.voteInfo.contentIsCheckBox) {
            $(this).toggleClass('img-thumbnail-H');
        } else {
            $(this).addClass('img-thumbnail-H').siblings().removeClass('img-thumbnail-H').end();
        }
    });

    $('#loading').hide();
    $('#main').show();
};


controller.Auth = function () {
    var loginType = "WeixinMob";
    var returnUrl = location.href;
    var locationUrl = "http://gexia.com/home/authlogin.html?isWeixin=true&loginType=" + loginType + "&returnUrl=" + returnUrl;
    location.href = locationUrl;
};

function isPc() {
    var userAgentInfo = navigator.userAgent;
    var Agents = ["Android", "iPhone", "SymbianOS", "Windows Phone", "iPad", "iPod"];
    var flag = true;
    for (var v = 0; v < Agents.length; v++) {
        if (userAgentInfo.indexOf(Agents[v]) > 0) {
            flag = false;
            break;
        }
    }
    return flag;
}
controller.main = function () {
    if (controller.from != "") {
        controller.openCheck();
    }
    if (controller.uniqueID == "" && !isPc()) {
        //controller.Auth();
        //return;
    }
    else {
        controller.wxOpenID = controller.uniqueID;
    }
    service.getVoteParameter(controller.voteID, controller.wxOpenID, function (data) {
        controller.isLoading = false;
        if (data.isSuccess) {

            if (data.isEnd) {
                location.href = 'end.html?voteID=' + controller.voteID + '&publishmentSystemID=' + service.publishmentSystemID + '&wxOpenID=' + controller.wxOpenID;
            }
            else {

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

            }

        } else {
            notifyService.error(data.errorMessage);
        }
        document.title = data.voteInfo.title;
    });
};

controller.main();

controller.getPercentage = function (voteNum) {
    if (controller.totalVoteNum == 0) {
        controller.totalVoteNum = 1;
    }
    return Math.round((voteNum / controller.totalVoteNum) * 100) + '%';
};

controller.isItem = function (itemID) {
    if (controller.itemIDCollection) {
        var array = JSON.parse("[" + controller.itemIDCollection + "]");
        if (array && array.indexOf(itemID) != -1) {
            return true;
        }
    }
    return false;
};

controller.submitVote = function () {

    controller.itemIDCollection = '';
    $('.title_list_H,.img-thumbnail-H').each(function (i, val) {
        controller.itemIDCollection += $(val).attr('itemID') + ',';
    });

    if (!controller.itemIDCollection) {
        notifyService.error('操作失败，请至少选择一个项目进行投票');
        return;
    } else {
        controller.itemIDCollection = controller.itemIDCollection.substring(0, controller.itemIDCollection.length - 1);
    }

    var realName = $('#userName').val();

    $('#loading').show();
    $('#main').hide();
    var agentsType = IsPC();
    service.submitVote(controller.voteID, controller.wxOpenID, controller.itemIDCollection, realName, agentsType, function (data) {
        if (data.isSuccess) {
            controller.isVoted = true;
            controller.itemList = data.itemList;
            controller.totalVoteNum = 0;
            for (i = 0; i < controller.itemList.length; i++) {
                var item = controller.itemList[i];
                controller.totalVoteNum += item.voteNum;
            }
            notifyService.success('投票成功，感谢您的参与');
        } else {
            notifyService.error(data.errorMessage);
        }
        controller.render();
    });
};

function ChoseVoteType(obj) {
    //$("[name='checkbox']").removeAttr("checked");
    //obj.checked = true;
    //if (obj.id == "RealName") {
    if (obj.checked) {
        $("#VoteUserNameDiv").css("display", "");
    } else {
        $("#VoteUserNameDiv").css("display", "none");
    }
}

function IsPC() {
    var userAgentInfo = navigator.userAgent;
    var Agents = ["Android", "iPhone",
                "SymbianOS", "Windows Phone",
                "iPad", "iPod"];
    var flag = true;
    for (var v = 0; v < Agents.length; v++) {
        if (userAgentInfo.indexOf(Agents[v]) > 0) {
            flag = false;
            break;
        }
    }
    return flag;
}
