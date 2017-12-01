// JavaScript Document

checkData = function(el, msgID, name)
{
	$("#" + msgID).hide();
	if ($(el).val() == '')
	{
		$("#" + msgID).html(name + "不能为空");
		$("#" + msgID).show();
		return false;
	}

	return true;
}

checkAdminName = function(value)
{
	$("#adminname_msg").hide();

	if ($('#TbAdminName').val() == '')
	{
		$("#adminname_msg").html("管理员用户名不能为空");
		$("#adminname_msg").show();
		return false;
	}

	return true;
}

checkPassword = function()
{
	$('#password_msg').hide();

	var strPassword = $('#TbAdminPassword').val();

	if ( strPassword == '' )
	{
		$("#password_msg").html("密码不能为空");
		$("#password_msg").show();
		return false;
	}

	if ( strPassword.length < 6 || strPassword.length>20 )
	{
		$('#password_msg').html('密码长度小于6个或者大于20个字符');
		$('#password_msg').show();
		return false;
	}

	setPasswordLevel(strPassword, $('#passwordLevel'))
	return true;
}

checkConfirmPassword = function()
{
	$('#confirmPassword_msg').hide();

	var strPassword = $('#TbAdminPassword').val();
	var confirmPassword = $('#TbComfirmAdminPassword').val();

	if ( confirmPassword == '' )
	{
		$("#confirmPassword_msg").html("确认密码不能为空");
		$("#confirmPassword_msg").show();
		return false;
	}

	else if ( strPassword != confirmPassword )
	{
		$('#confirmPassword_msg').html("确认密码与密码不一致");
		$('#confirmPassword_msg').show();
		return false;
	}

	return true;
}


/******************************************************************************************
 * 检查密码强度
 ******************************************************************************************/
checkPasswordLevel = function(strPassword)
{
	//check length
	var result = 0;
	if ( strPassword.length == 0)
		result += 0;
	else if ( strPassword.length<8 && strPassword.length >0 )
		result += 5;
	else if (strPassword.length>10)
		result += 25;
	else
		result += 10;
	//alert("检查长度:"+strPassword.length+"-"+result);

	//check letter
	var bHave = false;
	var bAll = false;
	var capital = strPassword.match(/[A-Z]{1}/);//找大写字母
	var small = strPassword.match(/[a-z]{1}/);//找小写字母
	if ( capital == null && small == null )
	{
		result += 0; //没有字母
		bHave = false;
	}
	else if ( capital != null && small != null )
	{
		result += 20;
		bAll = true;
	}
	else
	{
		result += 10;
		bAll = true;
	}
	//alert("检查字母："+result);

	//检查数字
	var bDigi = false;
	var digitalLen = 0;
	for ( var i=0; i<strPassword.length; i++)
	{

		if ( strPassword.charAt(i) <= '9' && strPassword.charAt(i) >= '0' )
		{
			bDigi = true;
			digitalLen += 1;
			//alert(strPassword[i]);
		}

	}
	if ( digitalLen==0 )//没有数字
	{
		result += 0;
		bDigi = false;
	}
	else if (digitalLen>2)//2个数字以上
	{
		result += 20 ;
		bDigi = true;
	}
	else
	{
		result += 10;
		bDigi = true;
	}
	//alert("数字个数：" + digitalLen);
	//alert("检查数字："+result);

	//检查非单词字符
	var bOther = false;
	var otherLen = 0;
	for (var i=0; i<strPassword.length; i++)
	{
		if ( (strPassword.charAt(i)>='0' && strPassword.charAt(i)<='9') ||
			(strPassword.charAt(i)>='A' && strPassword.charAt(i)<='Z') ||
			(strPassword.charAt(i)>='a' && strPassword.charAt(i)<='z'))
			continue;
		otherLen += 1;
		bOther = true;
	}
	if ( otherLen == 0 )//没有非单词字符
	{
		result += 0;
		bOther = false;
	}
	else if ( otherLen >1)//1个以上非单词字符
	{
		result +=25 ;
		bOther = true;
	}
	else
	{
		result +=10;
		bOther = true;
	}
	//alert("检查非单词："+result);

	//检查额外奖励
	if ( bAll && bDigi && bOther)
		result += 5;
	else if (bHave && bDigi && bOther)
		result += 3;
	else if (bHave && bDigi )
		result += 2;
	//alert("检查额外奖励："+result);

	var level = "";
	//根据分数来算密码强度的等级
	if ( result >=70 )
		level = "rank r7";
	else if ( result>=60)
		level = "rank r6";
	else if ( result>=50)
		level = "rank r5";
	else if ( result>=40)
		level = "rank r4";
	else if ( result>=30)
		level = "rank r3";
	else if ( result>20)
		level = "rank r2";
	else if ( result>10)
		level = "rank r1";
	else
		level = "rank r0";

//		alert("return:"+level);
	return level.toString();
}


/******************************************************************************************
 * 设置密码强度样式
 ******************************************************************************************/
setPasswordLevel = function(strPassword, levelObj)
{
	var level = "rank r0";
	level = checkPasswordLevel(strPassword);
	$(levelObj).removeClass().addClass(level);
}
