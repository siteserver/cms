var itemController = {};

itemController.selectImageClick = function(itemIndex){
    eval(itemController.selectImageClickString.replace("itemImageUrl_", "itemImageUrl_" + itemIndex).replace("return false;",""));
};

itemController.uploadImageClick = function(itemIndex){
    eval(itemController.uploadImageClickString.replace("itemImageUrl_", "itemImageUrl_" + itemIndex).replace("return false;", ""));
};

itemController.cuttingImageClick = function(itemIndex){
    eval(itemController.cuttingImageClickString.replace("itemImageUrl_", "itemImageUrl_" + itemIndex).replace("return false;", ""));
};

itemController.previewImageClick = function(itemIndex){
    eval(itemController.previewImageClickString.replace("itemImageUrl_", "itemImageUrl_" + itemIndex).replace("return false;", ""));
};

itemController.setItems = function(){
  itemController.items.splice(0);

  $('.itemID').each(function(i, val){
    var item = {
      id: $($('.itemID').get(i)).val(),
      title: $($('.itemTitle').get(i)).val(),
      imageUrl: $($('.itemImageUrl').get(i)).val(),
      navigationUrl: $($('.itemNavigationUrl').get(i)).val(),
      voteNum: $($('.itemVoteNum').get(i)).val()
    };
    itemController.items.push(item);
  });

  itemController.itemCount = itemController.items.length;
};

itemController.removeItem = function(itemIndex) {
  itemController.setItems();
  itemController.items.splice(itemIndex, 1);
  itemController.itemCount--;

  utilService.render('itemController', itemController);
};

itemController.addItem = function(item){
  itemController.setItems();
  itemController.items.push(item);
  itemController.itemCount++;

  utilService.render('itemController', itemController);
};

itemController.isImageOptionChange = function(ele){
  if ($(ele).val() == 'false'){
    itemController.isImageOption = false;
  }else{
    itemController.isImageOption = true;
  }
  utilService.render('itemController', itemController);
};

$(document).ready(function(e) {
  utilService.render('itemController', itemController);
  $('#ddlContentIsImageOption').val(itemController.isImageOption + '');
});