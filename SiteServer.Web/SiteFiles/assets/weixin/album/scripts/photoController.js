var controller = {};

controller.isLoading = true;

controller.albumID = utilService.getUrlVar('albumID');
controller.wxOpenID = utilService.getUrlVar('wxOpenID');
controller.parentID = utilService.getUrlVar('parentID');

controller.isPoweredBy = false;
controller.poweredBy = '';
controller.title = '';
controller.contentInfoList = [];

controller.returnToAlbum = function(){
  location.href = 'index.html?publishmentSystemID=' + service.publishmentSystemID + '&albumID=' + controller.albumID + '&wxOpenID=' + controller.wxOpenID;
};

controller.getPhotoUrl = function(parentID){
  return 'photo.html?publishmentSystemID=' + service.publishmentSystemID + '&albumID=' + controller.albumID + '&wxOpenID=' + controller.wxOpenID + '&parentID=' + parentID;
};

controller.render = function(){
  $('.title').html(controller.title.substr(0, 9));
  for (var i = 0; i < controller.contentInfoList.length; i++) {
    var contentInfo = controller.contentInfoList[i];
      //var html = '<li><a href="' + contentInfo.largeImageUrl + '" data-gallery><img src="' + contentInfo.imageUrl + '"></a></li>';
    var html = '<li><a href="' + contentInfo.largeImageUrl + '" data-gallery><img src="' + contentInfo.imageUrl + '"><p style="text-align:center">' + contentInfo.title + '</p></a></li>';
    $('#tiles').append(html);
  }

  $('.modal-body').height($(window).height() - 200);

  var options = {
    autoResize: true, // This will auto-update the layout when the browser window is resized.
    container: $('#photoList'), // Optional, used for some extra CSS styling
    offset: 2, // Optional, the distance between grid items
    itemWidth:144 // Optional, the width of a grid item
  };
  
  // Get a reference to your grid items.
  var handler = $('#tiles li');
 
  // Call the layout function.
  handler.wookmark(options);
  window.setTimeout(function(){
    handler.wookmark(options);
  }, 500);

};

service.getPhotoParameter(controller.albumID, controller.parentID, controller.wxOpenID, function (data) {
  controller.isLoading = false;
  if (data.isSuccess){

    controller.isPoweredBy = data.isPoweredBy;
    controller.title = data.title;
    controller.contentInfoList = data.contentInfoList;

    controller.render();
    
  }else{
    notifyService.error(data.errorMessage);
  }
});