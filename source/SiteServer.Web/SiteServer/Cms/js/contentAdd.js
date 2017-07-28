function translateNodeAdd(name, value){
  $('#translateContainer').append("<div id='translate_" + value + "' class='addr_base addr_normal'><b>" + name + "</b> <a class='addr_del' href='javascript:;' onClick=\"translateNodeRemove('" + value + "')\"></a></div>");
  $('#translateCollection').val(value + ',' + $('#translateCollection').val());
  $('#translateType').show();
}
function translateNodeRemove(value){
  $('#translate_' + value).remove();
  var val = '';
  var values = $('#translateCollection').val().split(",");
  for (i=0;i<values.length ;i++ )
  {
    if (values[i] && value != values[i]){val = values[i] + ',';}
  }
  $('#translateCollection').val(val);
  if (val == ''){
    $('#translateType').hide();
  }
}
$(document).keypress(function(e){
  if(e.ctrlKey && e.which == 13 || e.which == 10) {
    e.preventDefault();
    $("#Submit").click();
  } else if (e.shiftKey && e.which==13 || e.which == 10) {
    e.preventDefault();
    $("#Submit").click();
  }
});

var isSaving = false;
function autoSave() {
  if(!$('#Title').val()) return;
  if (isSaving) return;

  isSaving = true;
  var options = {
      beforeSubmit: function() {
          return true;
      },
      url: location.href + '&isAjaxSubmit=True',
      type: 'POST',
      success: function(data) {
        isSaving = false;
        var obj = eval("(" + data + ")");
        if (obj.success == 'true'){
          $('#savedContentID').val(obj.savedContentID);
          var now = new Date();
          var hours = now.getHours() >= 10 ? now.getHours() : '0' + now.getHours();
          var minutes = now.getMinutes() >= 10 ? now.getMinutes() : '0' + now.getMinutes();
          var seconds = now.getSeconds() >= 10 ? now.getSeconds() : '0' + now.getSeconds();
          showTips('自动保存草稿成功（' + hours + ':' + minutes + ':' + seconds + '）', 'success');
        }
      }
  };

  if (UE){
      $.each(UE.instants,function(index,editor){
          editor.sync();
      });
  }
  $('#myForm').ajaxSubmit(options);
}

var isPreviewSaving = false;
function previewSave() {
    if (!$('#Title').val()) return;
    if (isPreviewSaving) return;

    isPreviewSaving = true;
    var options = {
        beforeSubmit: function () {
            return true;
        },
        url: location.href + '&isAjaxSubmit=True&isPreview=True',
        type: 'POST',
        success: function (data) {
            isPreviewSaving = false;
            var obj = eval("(" + data + ")");
            if (obj.success == 'true') {
              window.open(previewUrl + '&isPreview=true&previewID=' + obj.savedContentID);
            }
        }
    };

    if (UE) {
        $.each(UE.instants, function (index, editor) {
            editor.sync();
        });
    }
    $('#myForm').ajaxSubmit(options);
}
