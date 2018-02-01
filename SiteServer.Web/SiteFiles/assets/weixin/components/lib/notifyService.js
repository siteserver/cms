toastr.options = {
  "positionClass": "toast-bottom-full-width"
};

var notifyService = {
    successCallback: function (e) {
        toastr.success('操作成功');
    },
    errorCallback: function (e) {
        var error = e.data.exceptionMessage;
        if (!error){
            error = e.data.message;
        }
        if (error){
            toastr.error('操作失败：' + error);
        }else{
            toastr.error('操作失败');
        }
    },
    success: function (text) {
        if (!text){
            text = "操作成功";
        }
        toastr.success(text);
    },
    error: function (text) {
        if (!text){
            text = "操作失败";
        }
        toastr.error(text);
    }
};