var itemController = {};

itemController.items = [];

itemController.removeItem = function (itemIndex) {
  itemController.items.splice(itemIndex, 1);

  utilService.render('itemController', itemController);
};

itemController.addItem = function (item) {
  itemController.syncItems();

  item.navigationType = 'Url';
  item.imageCssClass = 'fa fa-angle-double-down';
  itemController.items.push(item);
  
  utilService.render('itemController', itemController);
};

itemController.changeItem = function (e, itemIndex) {
  itemController.syncItems();
  
  item = itemController.items[itemIndex];
  item.navigationType = $(e).val();
  item.pageTitle = '';

  utilService.render('itemController', itemController);
};

$(document).ready(function (e) {
  utilService.render('itemController', itemController);

  $("#tbNavImageCssColor").change(function(){
       $("i").css("color","#"+$(this).val());
  });
  var navImageCssColor=$("#navImageCssColor").val();
  $("i").css("color","#"+navImageCssColor);

  $('#cbIsNavigation').click(function(){
    if (document.getElementById("cbIsNavigation").checked){
      $(".isNavigation").show();
    }else{
      $(".isNavigation").hide();
    }
  });

  if (document.getElementById("cbIsNavigation") && document.getElementById("cbIsNavigation").checked){
    $(".isNavigation").show();
  }else{
    $(".isNavigation").hide();
  }

});

itemController.syncItems = function(){
  itemController.items.splice(0);
  $('.itemID').each(function(i, val){
    var item = {
      id: $($('.itemID').get(i)).val(),
      imageCssClass: $($('.itemImageCssClass').get(i)).val(),
      keywordType: $($('.itemKeywordType').get(i)).val(),
      functionID: $($('.itemFunctionID').get(i)).val(),
      channelID: $($('.itemChannelID').get(i)).val(),
      contentID: $($('.itemContentID').get(i)).val(),
      title: $($('.itemTitle').get(i)).val(),
      navigationType: $($('.itemNavigationType').get(i)).val(),
      url: $($('.itemUrl').get(i)).val()
    };
    itemController.items.push(item);
  });
};

var selectFunction = function(itemIndex, keywordType, functionID, pageTitle){
  itemController.syncItems();

  item = itemController.items[itemIndex];
  item.keywordType = keywordType;
  item.functionID = functionID;
  item.pageTitle = pageTitle;

  utilService.render('itemController', itemController);
};

var selectContent = function(itemIndex, pageTitle, channelID, contentID){
  itemController.syncItems();

  item = itemController.items[itemIndex];
  item.channelID = channelID;
  item.contentID = contentID;
  item.pageTitle = '内容页：' + pageTitle;

  utilService.render('itemController', itemController);
};

var selectChannel = function(itemIndex, pageTitle, channelID){
  itemController.syncItems();

  item = itemController.items[itemIndex];
  item.channelID = channelID;
  item.contentID = 0;
  item.pageTitle = '栏目页：' + pageTitle;

  utilService.render('itemController', itemController);
};

var selectImageCssClass= function (itemIndex, cssClass)
{
  itemController.syncItems();

  item = itemController.items[itemIndex];
  item.imageCssClass = cssClass;

  utilService.render('itemController', itemController);
};