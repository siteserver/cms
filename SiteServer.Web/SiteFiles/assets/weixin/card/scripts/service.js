var service = {};
service.baseUrl = '/api/wx_card/';

service.cardID = utilService.getUrlVar('cardID');
service.userName = utilService.getUrlVar('userName')
service.publishmentSystemID = utilService.getUrlVar('publishmentSystemID');


service.getUrl = function(action, id){
    if (id) {
        return service.baseUrl + action + '/' + id + '?publishmentSystemID=' + service.publishmentSystemID ;
    }
    return service.baseUrl + action + '?publishmentSystemID=' + service.publishmentSystemID;
};

service.getCardParameter = function (cardID, success) {
    $.getJSON(service.getUrl('GetCardParameter', cardID),null, success);
};
 
service.getUser = function (success) {
    $.getJSON(service.getUrl('GetUser'),null, success);
};

service.editUser = function (displayName, mobile, gender, birthday,position,address, success) {
    $.getJSON(service.getUrl('EidtUser'), { 'displayName': displayName, 'mobile': mobile, 'gender': gender, 'birthday': birthday, "position": position, "address": address }, success);
};

service.register = function (cardID, userName,email, mobile,password, success) {
    $.getJSON(service.getUrl('Register'), { 'cardID': cardID, 'userName': userName, 'email': email, 'mobile': mobile, 'password': password }, success);
};
 
service.login = function (cardID,userName, password, isPersistent, success) {
    $.getJSON(service.getUrl('Login'), {'cardID':cardID ,'userName': userName, 'password': password, 'isPersistent': isPersistent }, success);
};

service.bindCard = function (cardID,cardSN, mobile, success) {
    $.getJSON(service.getUrl('BindCard', cardID), { 'cardSN': cardSN, 'mobile': mobile }, success);
};

service.consume = function (cardID,amount, operator,password, success) {
    $.getJSON(service.getUrl('Consume',cardID), { 'amount': amount, 'operator': operator, "password": password }, success);
};

service.exchange = function (cardID, credits, success) {
    $.getJSON(service.getUrl('Exchange', cardID), { 'credits': credits}, success);
};

 
service.sign = function (success) {
    $.getJSON(service.getUrl('Sign'), null, success);
};

service.getSignRecord = function (success) {
    $.getJSON(service.getUrl('GetSignRecord'), null, success);
};

service.getSignRule = function (success) {
    $.getJSON(service.getUrl('GetSignRule'), null, success);
};

service.getCardCashLog = function (cardID, success) {
    $.getJSON(service.getUrl('GetCardCashLog', cardID), null , success);
};

service.openLogin = function () {
    location.href = 'login.html?publishmentSystemID=' + service.publishmentSystemID + '&cardID=' + service.cardID;
};
 
service.openRegister = function () {
    location.href = 'register.html?publishmentSystemID=' + service.publishmentSystemID + '&cardID=' + service.cardID;
};
 
service.openMyCard = function () {
    location.href = 'myCard.html?publishmentSystemID=' + service.publishmentSystemID + '&cardID=' + service.cardID;
};



 