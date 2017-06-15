var agendaController = {};
var guestController = {};

guestController.selectImageClick = function(itemIndex){
  eval(guestController.selectImageClickString.replace("itemPicUrl_", "itemPicUrl_" + itemIndex));
};

guestController.uploadImageClick = function(itemIndex){
  eval(guestController.uploadImageClickString.replace("itemPicUrl_", "itemPicUrl_" + itemIndex));
};

guestController.cuttingImageClick = function(itemIndex){
  eval(guestController.cuttingImageClickString.replace("itemPicUrl_", "itemPicUrl_" + itemIndex));
};

guestController.previewImageClick = function(itemIndex){
  eval(guestController.previewImageClickString.replace("itemPicUrl_", "itemPicUrl_" + itemIndex));
};

agendaController.setItems = function(){
  agendaController.items.splice(0);

  $('.itemDateTime').each(function(i, val){
    var item = {
      dateTime: $($('.itemDateTime').get(i)).val(),
      title: $($('.itemTitle').get(i)).val(),
      summary: $($('.itemSummary').get(i)).val()
    };
    agendaController.items.push(item);
  });

  agendaController.agendaCount = agendaController.items.length;
};

agendaController.removeItem = function(itemIndex) {
  agendaController.setItems();
  agendaController.items.splice(itemIndex, 1);
  agendaController.agendaCount--;

  utilService.render('agendaController', agendaController);
};

agendaController.addItem = function(item){
  agendaController.setItems();
  agendaController.items.push(item);
  agendaController.agendaCount++;

  utilService.render('agendaController', agendaController);
};

guestController.setItems = function(){
  guestController.items.splice(0);

  $('.itemDisplayName').each(function(i, val){
    var item = {
      displayName: $($('.itemDisplayName').get(i)).val(),
      position: $($('.itemPosition').get(i)).val(),
      picUrl: $($('.itemPicUrl').get(i)).val()
    };
    guestController.items.push(item);
  });

  guestController.guestCount = guestController.items.length;
};

guestController.removeItem = function(itemIndex) {
  guestController.setItems();
  guestController.items.splice(itemIndex, 1);
  guestController.guestCount--;

  utilService.render('guestController', guestController);
};

guestController.addItem = function(item){
  debugger;
  guestController.setItems();
  guestController.items.push(item);
  guestController.guestCount++;

  utilService.render('guestController', guestController);
};

$(document).ready(function(e) {
  utilService.render('agendaController', agendaController);
  utilService.render('guestController', guestController);
});