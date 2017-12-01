function inputCheckAttributeValueByParam(inputId, obj, value) {
    var isRequire = obj.getAttribute("isRequire");
    var errorMessage = obj.getAttribute("errorMessage");
    var displayName = obj.getAttribute("displayName");
    var minNum = parseInt(obj.getAttribute("minNum"));
    var maxNum = parseInt(obj.getAttribute("maxNum"));
    var validateType = obj.getAttribute("validateType");
    var regExp = obj.getAttribute("regExp");

    try {
        if ($("#" + inputId).is(":hidden")) return true;
    } catch (e) { }
    if (!isRequire) { isRequire = 'false'; }
    if (!errorMessage) { errorMessage = ''; }
    if (!displayName) { displayName = ''; }
    document.getElementById(inputId + "_msg").style.display = "none";

    if (isRequire == "true" && (!value || value.length === 0)) {
        if (errorMessage.length > 0) {
            document.getElementById(inputId + "_msg").innerHTML = errorMessage;
        } else {
            document.getElementById(inputId + "_msg").innerHTML = displayName + "不能为空";
        }
        document.getElementById(inputId + "_msg").style.display = "";
        return false;
    }

    if (minNum > 0 && value.length < minNum) {
        if (errorMessage.length > 0) {
            document.getElementById(inputId + "_msg").innerHTML = errorMessage;
        } else {
            document.getElementById(inputId + "_msg").innerHTML = displayName + '长度不能小于' + minNum + '个字符';
        }
        document.getElementById(inputId + "_msg").style.display = "";
        return false;
    }

    if (maxNum > 0 && value.length > maxNum) {
        if (errorMessage.length > 0) {
            document.getElementById(inputId + "_msg").innerHTML = errorMessage;
        } else {
            document.getElementById(inputId + "_msg").innerHTML = displayName + '长度不能大于' + maxNum + '个字符';
        }
        document.getElementById(inputId + "_msg").style.display = "";
        return false;
    }
    var isChecked = false;

    if ((value && value.length > 0) && (validateType && validateType != "None")) {
        if (validateType == "Custom") {
            if (regExp.length > 0) {
                isChecked = new RegExp(regExp, "gi").test(value);
                if (!isChecked) {
                    if (errorMessage.length > 0) {
                        document.getElementById(inputId + "_msg").innerHTML = errorMessage;
                    } else {
                        document.getElementById(inputId + "_msg").innerHTML = displayName + '格式不正确';
                    }
                    document.getElementById(inputId + "_msg").style.display = "";
                    return false;
                }
            }
        } else {
            var regExpression;
            if (validateType == "Chinese") {
                regExpression = /^[\u0391-\uFFE5]+$/;
            } else if (validateType == "English") {
                regExpression = /^[A-Za-z]+$/;
            } else if (validateType == "Email") {
                regExpression = /^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/;
            } else if (validateType == "Url") {
                regExpression = /^http:\/\/[A-Za-z0-9]+\.[A-Za-z0-9]+[\/=\?%\-&_~`@[\]\':+!]*([^<>\"\"])*$/;
            } else if (validateType == "Phone") {
                regExpression = /^((\(\d{3}\))|(\d{3}\-))?(\(0\d{2,3}\)|0\d{2,3}-)?[1-9]\d{6,7}$/;
            } else if (validateType == "Mobile") {
                regExpression = /^((\(\d{3}\))|(\d{3}\-))?1\d{10}$/;
            } else if (validateType == "Integer") {
                regExpression = /^[-\+]?\d+$/;
            } else if (validateType == "Currency") {
                regExpression = /^\d+(\.\d+)?$/;
            } else if (validateType == "Zip") {
                regExpression = /^[1-9]\d{5}$/;
            } else if (validateType == "IdCard") {
                regExpression = /^\d{15}(\d{2}[A-Za-z0-9])?$/;
            } else if (validateType == "QQ") {
                regExpression = /^[1-9]\d{4,11}$/;
            }
            if (regExpression) {
                isChecked = regExpression.test(value);
                if (!isChecked) {
                    if (errorMessage.length > 0) {
                        document.getElementById(inputId + "_msg").innerHTML = errorMessage;
                    } else {
                        document.getElementById(inputId + "_msg").innerHTML = displayName + '格式不正确';
                    }
                    document.getElementById(inputId + "_msg").style.display = "";
                    return false;
                }
            }
        }
    }

    return true;
}

function inputCheckAttributeValueById(inputId) {
    var obj = document.getElementById(inputId);
    if (obj) {
        var value = obj.value;
        return inputCheckAttributeValueByParam(inputId, obj, value);
    }
    return true;
}

function inputCheckAttributeValueByName(inputId) {
    var obj = document.getElementById(inputId);
    if (obj) {
        var value = $("input[name='" + inputId + "']:checked").val() || $("select[name='" + inputId + "']").val();
        return inputCheckAttributeValueByParam(inputId, obj, value);
    }
    return true;
}

function inputSubmit(e, formId, containerId, idList) {   
    var isChecked = true;

    var btnSubmit = $(e);
    var attr = btnSubmit.attr('clicking');
    if (attr) {
        return false;
    } else {
        btnSubmit.attr('clicking', 'true');
    }

    for (i = 0; i < idList.length; i++) { 
        var id = idList[i];
        var item = $('#' + id);

        if (item.attr("isValidate") == "true") {
            if (item.attr("isListItem") == "true") {
                if (inputCheckAttributeValueByName(id) === false) {
                    isChecked = false;
                }
            }
            else {
                if (inputCheckAttributeValueById(id) === false) {
                    isChecked = false;
                }
            }
        }
    }

    if (isChecked){
        if (document.forms[formId]) {
            $('#' + containerId + ' .stl_input_template').hide();
            $('#' + containerId + ' .stl_input_loading').show();
            document.forms[formId].submit();
        }
    } else {
        btnSubmit.removeAttr("clicking");
    }
}

function inputBlur(inputId) {
    $('#' + inputId).blur(function(e){
        inputCheckAttributeValueById(inputId);
    });
}

window.addEventListener('message', receiveMessage, false);

function receiveMessage(evt)
{
    var containerId = evt.data.containerId;
    var isSuccess = evt.data.isSuccess;

    $('#' + containerId + ' .stl_input_loading').hide();
    if (isSuccess){
        $('#' + containerId + ' .stl_input_yes').show();
    } else {
        $('#' + containerId + ' .stl_input_no').show();
    }
}