var controller = {};

controller.isLoading = true;

controller.isPoweredBy = false;
controller.poweredBy = '';
controller.collectInfo = {};
controller.itemList = [];
controller.itemInfo = {};
controller.votedItemIDCollection = '';
controller.itemIDCollectionWithRank = [];

controller.render = function(){
  utilService.render('controller', controller);

  if (service.itemID){
      $(".j_tp_btn").click(function(){
        $(".mark_fixed").show();
        $(".share_img").show();
      });

      $(".mark_fixed, .share_img").click(function(){
        $(".mark_fixed").hide();
        $(".share_img").hide();
      });
  }else if ($('#upload').length > 0){
    new AjaxUpload('upload', {
       action: service.getUploadUrl(),
       name: "Upload",
       data: {},
       onSubmit: function(file, ext) {
         var reg = /^(jpg|jpeg|png|gif)$/i;
         if (ext && reg.test(ext)) {
           //$('#img_upload_txt_').text('上传中... ');
         } else {
           notifyService.error('只允许上传JPG,PNG,GIF图片');
           return false;
         }
       },
       onComplete: function(file, response) {
         if (response) {
           response = $.parseJSON($(response).text());
           if (response.isSuccess) {
             $('#preview_imageUrl').attr('src', response.largeUrl);
             $('#largeUrl').val(response.largeUrl);
             $('#smallUrl').val(response.smallUrl);
           } else {
             notifyService.error(response.errorMessage);
           }
         }
       }
      });
  }

  $('#loading').hide();
  $('#main').show();
};

controller.main = function () {
    service.getCollectParameter(function (data) {
        controller.isLoading = false;
        if (data.isSuccess) {

            if (data.isEnd) {
                service.redirectToEnd();
            }
            else {

                controller.isPoweredBy = data.isPoweredBy;
                controller.poweredBy = data.poweredBy;
                controller.votedItemIDCollection = data.votedItemIDCollection;
                controller.itemIDCollectionWithRank = data.itemIDCollectionWithRank;
                controller.collectInfo = data.collectInfo;
                controller.itemList = data.itemList;

                for (i = 0; i < controller.itemList.length; i++) {
                    var item = controller.itemList[i];
                    if (service.itemID == item.id) {
                        controller.itemInfo = item;
                    }
                }

                controller.render();

            }

        } else {
            notifyService.error(data.errorMessage);
        }
        document.title = data.collectInfo.title;
    });
};

controller.main();

controller.isVoted = function(itemID){
  if (controller.votedItemIDCollection){
      var array = JSON.parse("[" + controller.votedItemIDCollection + "]");
      if (array && array.indexOf(itemID) != -1){
        return true;
      }
  }
  return false;
};

controller.getRank = function(itemID){
  if (controller.itemIDCollectionWithRank){
      for(i = 0; i < controller.itemIDCollectionWithRank.length; i ++) {
        var obj = controller.itemIDCollectionWithRank[i];
        var array = JSON.parse("[" + obj.itemIDCollection + "]");
        if (array && array.indexOf(itemID) != -1){
          return obj.rank;
        }
      }
  }
  return 0;
};

controller.submitUpload = function(){

  var title = $('#title').val();
  var description = $('#description').val();
  var mobile = $('#mobile').val();
  var smallUrl = $('#smallUrl').val();
  var largeUrl = $('#largeUrl').val();

  if (!title){
    notifyService.error('操作失败，请填写标题');
    $('#title').focus();
    return;
  }
  if (!description){
    notifyService.error('操作失败，请填写参赛宣言');
    $('#description').focus();
    return;
  }
  if (!mobile){
    notifyService.error('操作失败，请填写手机号码');
    $('#mobile').focus();
    return;
  }
  if (!smallUrl || !largeUrl){
    notifyService.error('操作失败，请上传照片');
    return;
  }

  $('#loading').show();
  $('#main').hide();

  service.submitUpload(title, description, mobile, smallUrl, largeUrl, function (data) {
    if (data.isSuccess){
        if (data.isCheck){
          notifyService.success('恭喜，提交已成功，我们将尽快审核您的参赛信息');
          setTimeout(function(){
            service.redirectToIndex();
          },1000); 
        }else{
          notifyService.success('恭喜，提交已成功，系统将跳转至您的参赛页面');
          setTimeout(function(){
            service.redirectToItem(data.itemID);
          },1000); 
        }
    }else{
        notifyService.error(data.errorMessage);
        $('#main').show();
        $('#loading').hide();
    }
  });
};

controller.submitVote = function(itemID){

  if (controller.isVoted(itemID)){
    notifyService.error('您已投票，可以分享到朋友圈为TA拉票');
    return;
  }

  $('#loading').show();
  $('#main').hide();

  service.submitVote(itemID, function (data) {
    if (data.isSuccess){
        notifyService.success('投票成功，感谢您的参与');
        setTimeout(function(){
          if (service.itemID){
            service.redirectToItem(service.itemID);
          }else{
            service.redirectToVote();
          }
        },1000); 
    }else{
        notifyService.error(data.errorMessage);
        controller.render();
    }
  });
};