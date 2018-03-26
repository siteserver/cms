function event_observe(attributeName, eventName, handler){
	element = document.getElementById(attributeName);
	
	var responder = function(event) {
	  handler.call(element, event);
	};
	
	if (element.addEventListener)
		element.addEventListener(eventName, responder, false);
	else
		element.attachEvent("on" + eventName, responder);
}

function checkAttributeValueByParam(attributeName, isRequire, value, errorMessage, displayName, minNum, maxNum, validateType, regExp)
{
	if($("#" + attributeName).is(":hidden")) {return true;};
    if (!isRequire) { isRequire = 'false'; }
    if (!errorMessage) { errorMessage = ''; }
    if (!displayName) { displayName = ''; }
	$("#" + attributeName + "_msg").hide();
	
	if (isRequire == "true" && (!value || value.length == 0))
	{
		if (errorMessage.length > 0){
			$("#" + attributeName + "_msg").html(errorMessage);
		}else{
			$("#" + attributeName + "_msg").html(displayName + "不能为空");	
		}
		$("#" + attributeName + "_msg").show();
		return false;
	}
	
	if (minNum > 0 && value.length < minNum )
	{
		if (errorMessage.length > 0){
			$("#" + attributeName + "_msg").html(errorMessage);
		}else{
			$("#" + attributeName + "_msg").html('长度不能小于' + minNum + '个字符');
		}
		$("#" + attributeName + "_msg").show();
		return false;
	}
	
	if (maxNum > 0 && value.length > maxNum )
	{
		if (errorMessage.length > 0){
			$("#" + attributeName + "_msg").html(errorMessage);
		}else{
			$("#" + attributeName + "_msg").html('长度不能大于' + maxNum + '个字符');
		}
		$("#" + attributeName + "_msg").show();
		return false;
	}
	
	if ( (value && value.length > 0) && (validateType && validateType != "None") ){
		if (validateType == "Custom"){
			if (regExp.length > 0){
				var isChecked = new RegExp(regExp,"gi").test(value);
				if (isChecked == false){
					if (errorMessage.length > 0){
						$("#" + attributeName + "_msg").html(errorMessage);
					}else{
						$("#" + attributeName + "_msg").html(displayName + '格式不正确');
					}
					$("#" + attributeName + "_msg").show();
					return false;
				}
			}
		}else{
			var regExpression;
			if (validateType == "Chinese"){
				regExpression = /^[\u0391-\uFFE5]+$/;
			}else if (validateType == "English"){
				regExpression = /^[A-Za-z]+$/;
			}else if (validateType == "Email"){
				regExpression = /^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/;
			}else if (validateType == "Url"){
				regExpression = /^http:\/\/[A-Za-z0-9]+\.[A-Za-z0-9]+[\/=\?%\-&_~`@[\]\':+!]*([^<>\"\"])*$/;
			}else if (validateType == "Phone"){
				regExpression = /^((\(\d{3}\))|(\d{3}\-))?(\(0\d{2,3}\)|0\d{2,3}-)?[1-9]\d{6,7}$/;
			}else if (validateType == "Mobile"){
				regExpression = /^((\(\d{3}\))|(\d{3}\-))?1\d{10}$/;
			}else if (validateType == "Integer"){
				regExpression = /^[-\+]?\d+$/;
			}else if (validateType == "Currency"){
				regExpression = /^\d+(\.\d+)?$/;
			}else if (validateType == "Zip"){
				regExpression = /^[1-9]\d{5}$/;
			}else if (validateType == "IdCard"){
				regExpression = /^\d{15}(\d{2}[A-Za-z0-9])?$/;
			}else if (validateType == "QQ"){
				regExpression = /^[1-9]\d{4,11}$/;
			}
			if (regExpression){
				var isChecked = regExpression.test(value);
				if (isChecked == false){
					if (errorMessage.length > 0){
						$("#" + attributeName + "_msg").html(errorMessage);
					}else{
						$("#" + attributeName + "_msg").html(displayName + '格式不正确');
					}
					$("#" + attributeName + "_msg").show();
					return false;
				}
			}
		}
	}

	return true;
}

function checkAttributeValue(event)
{
	var obj = event.srcElement || event.target;
	var attributeName = obj.id;
	
	var isRequire = obj.getAttribute("isRequire");
	var value = obj.value;
	var errorMessage = obj.getAttribute("errorMessage");
	var displayName = obj.getAttribute("displayName");
	var minNum = parseInt(obj.getAttribute("minNum"));
	var maxNum = parseInt(obj.getAttribute("maxNum"));
	var validateType = obj.getAttribute("validateType");
	var regExp = obj.getAttribute("regExp");
	
	return checkAttributeValueByParam(attributeName, isRequire, value, errorMessage, displayName, minNum, maxNum, validateType, regExp);
}

function checkAttributeValueById(attributeName)
{
	var obj = document.getElementById(attributeName);
	if (obj){
		var isRequire = obj.getAttribute("isRequire");
		var value = obj.value;
		var errorMessage = obj.getAttribute("errorMessage");
		var displayName = obj.getAttribute("displayName");
		var minNum = parseInt(obj.getAttribute("minNum"));
		var maxNum = parseInt(obj.getAttribute("maxNum"));
		var validateType = obj.getAttribute("validateType");
		var regExp = obj.getAttribute("regExp");
		
		return checkAttributeValueByParam(attributeName, isRequire, value, errorMessage, displayName, minNum, maxNum, validateType, regExp);
	}
	return true;
}

function checkFormValueById(formId){
	var isChecked = true;
	var myArray = document.getElementById(formId).getElementsByTagName('*');
	if (myArray && myArray.length > 0){
		for(i = 0; i < myArray.length; i++){
			var item = myArray.item(i);
			if(item.getAttribute("isValidate") == "true"){
				if (checkAttributeValueById(item.getAttribute("id")) == false){
					isChecked = false;
				}
			}
		}
	}
	
	return isChecked;
}