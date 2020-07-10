function stlLoginCallback(serviceUrl, successCallback, failureCallback, successArgument, failureArgument){
	
	jQuery.get(serviceUrl, '', function(data, textStatus){
		data = eval('(' + data + ')');
		if (data.isLogin == 'true')
		{
			try{
				if (successCallback){
					eval(successCallback + "(" + successArgument + ")");
				}
			}
			catch(e){}
		}
		else
		{
			try{
				if (failureCallback){
					eval(failureCallback + "(" + failureArgument + ")");
				}
			}
			catch(e){}
		}
    });
}


function stlOpenLoginWindow(successCallback, successArgument){
	var windowUrl = stl_url_ss + '/login/loginFrame.aspx?successCallback=' + successCallback + '&successArgument=' + successArgument;
	stlOpenWindow(windowUrl, 500, 374);

}


function stlOpenMailSendWindow(publishmentSystemID, channelID, contentID){

	var windowUrl = stl_url_ss + '/mail/mailSendFrame.aspx?publishmentSystemID=' + publishmentSystemID + '&channelID=' + channelID + '&contentID=' + contentID + '&pageTitle=' + encodeURIComponent(document.title) + '&pageUrl=' + location.href;
	stlOpenWindow(windowUrl, 373, 335);

}

function stlOpenMailSubscribeWindow(publishmentSystemID){

	var windowUrl = stl_url_ss + '/mail/mailSubscribeFrame.aspx?publishmentSystemID=' + publishmentSystemID;
	stlOpenWindow(windowUrl, 373, 275);

}


function stlCommentFirstLogin(redirectUrl){
	setTimeout("JSONscriptQuery('"+url+"');", 1);
}


//检查是否登陆

function check_is_login(){

	try {

		var usrname = getCookie('usrname');

		if (usrname == "") {

			global_login_frm_show(this,true,window.location.href,1);

			return false;

		}

		else{

			return true;

		}

	} catch (e) {}

}

var login_success_callback_function = '';

//需要回调的登陆

function check_is_login_callback(){

	var ref_flag = 1;

	if(arguments.length==1){

		login_success_callback_function = arguments[0];

		ref_flag = 0;

	}

	try {

		var usrname = getCookie('usrname');

		if (usrname == "") {

			

			global_login_frm_show(this,true,window.location.href,ref_flag);

			return false;

		}

		else{

			if(arguments.length>=1){

				login_success_eval_callback(1);

			}

			else{

				return true;

			}

		}

	} catch (e) {}

}



//执行登陆的回调的回调

function login_success_eval_callback(flag){

	if(typeof(login_success_callback_function)!='undefined'){

		if(typeof(login_success_callback_function)=='function'){

			login_success_callback_function(flag);

		}

		else if(typeof(login_success_callback_function)=='string'){

			try{

				eval(login_success_callback_function);

			}

			catch(e){}

		}

	}

	login_success_callback_function = '';

}



//登陆过后的回调

function login_success_callback(json){

	update_user_status();

	login_success_eval_callback(0);//执行回调的回调

}