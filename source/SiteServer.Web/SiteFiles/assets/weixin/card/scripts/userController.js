var controller = {};
 
controller.poweredBy = '';
controller.isLoading = true;
controller.isSuccess = false;
controller.isPoweredBy = false;
  
controller.userInfo = {};
controller.userContactInfo = {};
 
controller.render = function () {
    utilService.render('controller', controller);
     
    new YMDselect('year', 'month', 'day');

    $("#province").ProvinceCity();
    $("#province select").attr("class", "fl form-control input-sm")
    
    if (controller.userContactInfo) {
        $("#gender").val(controller.userContactInfo.gender);
        $("#year").val(controller.userContactInfo.birthday.split(',')[0]);
        $("#month").append('<option value="' + controller.userContactInfo.birthday.split(',')[1] + '" selected="selected">' + controller.userContactInfo.birthday.split(',')[1] + '月</option>');
        $("#day").append('<option value="' + controller.userContactInfo.birthday.split(',')[2] + '" selected="selected">' + controller.userContactInfo.birthday.split(',')[2] + '日</option>');
        $("#province select").eq(0).val(controller.userContactInfo.position.split(',')[0]);
        $("#province select").eq(1).append('<option value="' + controller.userContactInfo.position.split(',')[1] + '" selected="selected">' + controller.userContactInfo.position.split(',')[1] + '</option>');
        $("#province select").eq(2).append('<option value="' + controller.userContactInfo.position.split(',')[2] + '" selected="selected">' + controller.userContactInfo.position.split(',')[2] + '</option>');

        $("#address").val(controller.userContactInfo.address);
    }

    $("#btnSubmit").click(function () {
        controller.editUser();
    });

    $('#loading').hide();
    $('#main').show();
}

controller.editUser = function () {

    var displayName = $('#displayName').val();
    var mobile = $('#mobile').val();
    var gender = $('#gender').val();
    var birthday = $('#year').val()+","+$('#month').val()+","+$('#day').val();
    var position = $("#province select").eq(0).val() + "," + $("#province select").eq(1).val() + "," + $("#province select").eq(2).val();
    var address = $('#address').val();
     
    service.editUser(displayName,mobile,gender,birthday,position,address, function (data) {
        controller.isSuccess = data.isSuccess;
        if (controller.isSuccess) {
             service.openMyCard();
           
        } else {
            notifyService.error(data.errorMessage);
        }

    });
};
   
controller.main = function () {

    service.getUser(function (data) {
        controller.isLoading = false;
        if (data.isSuccess) {
            controller.isPoweredBy = data.isPoweredBy;
            controller.poweredBy = data.poweredBy;
            controller.userInfo = data.userInfo;
            controller.userContactInfo = data.userContactInfo;

            controller.render();
         } else {
            notifyService.error(data.errorMessage);
        }
    });
}

controller.main();

 