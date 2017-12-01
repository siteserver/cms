var controller = {};

controller.conferenceID = utilService.getUrlVar('conferenceID');
controller.wxOpenID = utilService.getUrlVar('wxOpenID');

controller.openConference = function(){
    location.href = 'index.html?publishmentSystemID=' + service.publishmentSystemID + '&conferenceID=' + controller.conferenceID + '&wxOpenID=' + controller.wxOpenID + '&r=' + Math.round(Math.random() * 10);
    //location.href = 'index.html?publishmentSystemID=' + service.publishmentSystemID + '&conferenceID=' + controller.conferenceID + '&wxOpenID=' + ' ' + '&r=' + Math.round(Math.random() * 10);
};

controller.submitApplication = function(){

  var realName = $('#realName').val();
  var mobile = $('#mobile').val();
  var email = $('#email').val();
  var company = $('#company').val();
  var position = $('#position').val();
  var note = $('#note').val();

  $('#loading').show();
  $('#main').hide();

  service.submitApplication(controller.conferenceID, controller.wxOpenID, realName, mobile, email, company, position, note, function (data) {
    if (data.isSuccess){

        $('#loading').hide();
        $('#main').show();

        $('.alert-success').show();
        $('form').hide();

        window.setTimeout(function(){
          controller.openConference();
        }, 2000);

    }else{
        notifyService.error(data.errorMessage);
    }
  });
};