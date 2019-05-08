function checkAttributeValue(event)
{
	var attributeName = this.id;

	$(attributeName + "_msg").style.display = "none";
	
	if (this.isRequire == "true" && (!this.value || this.value.length == 0))
	{
		if (this.errorMessage.length > 0){
			$(attributeName + "_msg").innerHTML = this.errorMessage;
		}else{
			$(attributeName + "_msg").innerHTML = this.displayName + "不能为空";	
		}
		$(attributeName + "_msg").style.display = "";
		return false;
	}
	
	if (this.minNum > 0 && this.value.length < this.minNum )
	{
		if (this.errorMessage.length > 0){
			$(attributeName + "_msg").innerHTML = this.errorMessage;
		}else{
			$(attributeName + "_msg").innerHTML = this.displayName + '长度不能小于' + this.minNum + '个字符';
		}
		$(attributeName + "_msg").style.display = "";
		return false;
	}
	
	if (this.maxNum > 0 && this.value.length > this.maxNum )
	{
		if (this.errorMessage.length > 0){
			$(attributeName + "_msg").innerHTML = this.errorMessage;
		}else{
			$(attributeName + "_msg").innerHTML = this.displayName + '长度不能大于' + this.maxNum + '个字符';
		}
		$(attributeName + "_msg").style.display = "";
		return false;
	}
	
	if ( (this.value && this.value.length > 0) && (this.validateType && this.validateType != "None") ){
		if (this.validateType == "Custom"){
			if (this.regExp.length > 0){
				var isChecked = new RegExp(this.regExp,"gi").test(this.value);
				if (isChecked == false){
					if (this.errorMessage.length > 0){
						$(attributeName + "_msg").innerHTML = this.errorMessage;
					}else{
						$(attributeName + "_msg").innerHTML = this.displayName + '格式不正确';
					}
					$(attributeName + "_msg").style.display = "";
					return false;
				}
			}
		}else{
			var regExpression;
			if (this.validateType == "Chinese"){
				regExpression = /^[\u0391-\uFFE5]+$/;
			}else if (this.validateType == "English"){
				regExpression = /^[A-Za-z]+$/;
			}else if (this.validateType == "Email"){
				regExpression = /^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/;
			}else if (this.validateType == "Url"){
				regExpression = /^http:\/\/[A-Za-z0-9]+\.[A-Za-z0-9]+[\/=\?%\-&_~`@[\]\':+!]*([^<>\"\"])*$/;
			}else if (this.validateType == "Phone"){
				regExpression = /^((\(\d{3}\))|(\d{3}\-))?(\(0\d{2,3}\)|0\d{2,3}-)?[1-9]\d{6,7}$/;
			}else if (this.validateType == "Mobile"){
				regExpression = /^((\(\d{3}\))|(\d{3}\-))?13\d{9}$/;
			}else if (this.validateType == "Integer"){
				regExpression = /^[-\+]?\d+$/;
			}else if (this.validateType == "Currency"){
				regExpression = /^\d+(\.\d+)?$/;
			}else if (this.validateType == "Zip"){
				regExpression = /^[1-9]\d{5}$/;
			}else if (this.validateType == "IdCard"){
				regExpression = /^\d{15}(\d{2}[A-Za-z0-9])?$/;
			}else if (this.validateType == "QQ"){
				regExpression = /^[1-9]\d{4,11}$/;
			}
			if (regExpression){
				var isChecked = regExpression.test(this.value);
				if (isChecked == false){
					if (this.errorMessage.length > 0){
						$(attributeName + "_msg").innerHTML = this.errorMessage;
					}else{
						$(attributeName + "_msg").innerHTML = this.displayName + '格式不正确';
					}
					$(attributeName + "_msg").style.display = "";
					return false;
				}
			}
		}
	}

	return true;
}

function checkAttributeValueById(attributeName)
{
	$(attributeName + "_msg").style.display = "none";
	var obj = $(attributeName);
	
	if (obj.isRequire == "true" && (!obj.value || obj.value.length == 0))
	{
		if (obj.errorMessage.length > 0){
			$(attributeName + "_msg").innerHTML = obj.errorMessage;
		}else{
			$(attributeName + "_msg").innerHTML = obj.displayName + "不能为空";	
		}
		$(attributeName + "_msg").style.display = "";
		return false;
	}
	
	if (obj.minNum > 0 && obj.value.length < obj.minNum )
	{
		if (obj.errorMessage.length > 0){
			$(attributeName + "_msg").innerHTML = obj.errorMessage;
		}else{
			$(attributeName + "_msg").innerHTML = obj.displayName + '长度不能小于' + obj.minNum + '个字符';
		}
		$(attributeName + "_msg").style.display = "";
		return false;
	}
	
	if (obj.maxNum > 0 && obj.value.length > obj.maxNum )
	{
		if (obj.errorMessage.length > 0){
			$(attributeName + "_msg").innerHTML = obj.errorMessage;
		}else{
			$(attributeName + "_msg").innerHTML = obj.displayName + '长度不能大于' + obj.maxNum + '个字符';
		}
		$(attributeName + "_msg").style.display = "";
		return false;
	}
	
	if ( (this.value && this.value.length > 0) && (obj.validateType && obj.validateType != "None") ){
		if (obj.validateType == "Custom"){
			if (obj.regExp.length > 0){
				var isChecked = new RegExp(obj.regExp,"gi").test(obj.value);
				if (isChecked == false){
					if (obj.errorMessage.length > 0){
						$(attributeName + "_msg").innerHTML = obj.errorMessage;
					}else{
						$(attributeName + "_msg").innerHTML = obj.displayName + '格式不正确';
					}
					$(attributeName + "_msg").style.display = "";
					return false;
				}
			}
		}else{
			var regExpression;
			if (obj.validateType == "Chinese"){
				regExpression = /^[\u0391-\uFFE5]+$/;
			}else if (obj.validateType == "English"){
				regExpression = /^[A-Za-z]+$/;
			}else if (obj.validateType == "Email"){
				regExpression = /^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/;
			}else if (obj.validateType == "Url"){
				regExpression = /^http:\/\/[A-Za-z0-9]+\.[A-Za-z0-9]+[\/=\?%\-&_~`@[\]\':+!]*([^<>\"\"])*$/;
			}else if (obj.validateType == "Phone"){
				regExpression = /^((\(\d{3}\))|(\d{3}\-))?(\(0\d{2,3}\)|0\d{2,3}-)?[1-9]\d{6,7}$/;
			}else if (obj.validateType == "Mobile"){
				regExpression = /^((\(\d{3}\))|(\d{3}\-))?13\d{9}$/;
			}else if (obj.validateType == "Integer"){
				regExpression = /^[-\+]?\d+$/;
			}else if (obj.validateType == "Currency"){
				regExpression = /^\d+(\.\d+)?$/;
			}else if (obj.validateType == "Zip"){
				regExpression = /^[1-9]\d{5}$/;
			}else if (obj.validateType == "IdCard"){
				regExpression = /^\d{15}(\d{2}[A-Za-z0-9])?$/;
			}else if (obj.validateType == "QQ"){
				regExpression = /^[1-9]\d{4,8}$/;
			}
			if (regExpression){
				var isChecked = regExpression.test(obj.value);
				if (isChecked == false){
					if (obj.errorMessage.length > 0){
						$(attributeName + "_msg").innerHTML = obj.errorMessage;
					}else{
						$(attributeName + "_msg").innerHTML = obj.displayName + '格式不正确';
					}
					$(attributeName + "_msg").style.display = "";
					return false;
				}
			}
		}
	}

	return true;
}


function checkFormValue(event){
	var isChecked = true;
	var myArray = Form.getElements(this.id);
	myArray.each(function(item) {
		if(item.isValidate == "true"){
			if (checkAttributeValueById(item.id) == false){
				isChecked = false;
			}
		}
	});
	if (isChecked == false){
		Event.stop(event);
	}
}

function checkFormValueById(formId){
	var isChecked = true;
	var myArray = Form.getElements(formId);
	myArray.each(function(item) {
		if(item.isValidate == "true"){
			if (checkAttributeValueById(item.id) == false){
				isChecked = false;
			}
		}
	});
	return isChecked;
}