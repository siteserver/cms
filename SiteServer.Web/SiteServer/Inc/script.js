function isNull(obj) {
	if (typeof (obj) == "undefined")
		return true;

	if (obj == undefined)
		return true;

	if (obj == null)
		return true;

	return false;
}

function loopRows(oTable, callBack) {
	if (!oTable) return;
	callBack = callBack || function () {};
	var trs = oTable.rows;
	var i = 0,
		l = trs.length;
	var flag = i < l;

	while (flag ? i < l : i > l) {
		var cur = trs[i];
		try {
			callBack(cur, i);
		} catch (e) {
			if (e == 'break') {
				break;
			}
		}
		flag ? i++ : i--;
	}
}

function selectRows(layer, bcheck) {
	for (var i = 0; i < layer.childNodes.length; i++) {
		if (layer.childNodes[i].childNodes.length > 0) {
			selectRows(layer.childNodes[i], bcheck);
		} else {
			if (layer.childNodes[i].type == "checkbox") {
				layer.childNodes[i].checked = bcheck;
				var cb = $(layer.childNodes[i]);
				var tr = cb.closest('tr');
				if (!tr.hasClass("thead")) {
					cb.is(':checked') ? tr.addClass('table-active') : tr.removeClass('table-active');
				}
			}
		}
	}
}

function chkSelect(e) {
	var e = (e || event);
	var el = this;
	if (el.getElementsByTagName('input') && el.getElementsByTagName('input').length > 0) {
		if ($(el).hasClass('thead')) return;
		el.className = (el.className == 'table-active' ? '' : 'table-active');
		el.getElementsByTagName('input')[0].checked = (el.className == 'table-active');
	}
}

function showImage(obj, imageID, rootUrl, siteUrl) {
	if (obj.value == "") {
		document.getElementById(imageID).setAttribute('href', '../assets/icon/empty.gif');
		return false;
	}
	if (obj.value.search(/\.bmp|\.jpg|\.jpeg|\.gif|\.png$/i) != -1) {
		var imageUrl = obj.value;
		if (imageUrl) {
			if (imageUrl.charAt(0) == '~') {
				imageUrl = imageUrl.replace('~', rootUrl);
			} else if (imageUrl.charAt(0) == '@') {
				imageUrl = imageUrl.replace('@', siteUrl);
			}
		}
		if (imageUrl && imageUrl.substr(0, 2) == '//') {
			imageUrl = imageUrl.replace('//', '/');
		}
		document.getElementById(imageID).setAttribute('href', imageUrl);
	}
}

function DeleteAttachment(attributeName) {
	document.getElementById(attributeName).value = '';
}

function SelectAttachment(attributeName, attachmentUrl, fileViewUrl) {
	document.getElementById(attributeName).value = attachmentUrl;
}

function _toggleVisible(targetID, image, imageUrl, imageCollapsedUrl) {
	target = _get(targetID);
	try {
		$('#' + targetID).toggle();
	} catch (e) {
		if (target.style.display == "none") {
			target.style.display = "";
		} else {
			target.style.display = "none";
		}
	}

	if (!_isNull(imageCollapsedUrl) && imageCollapsedUrl != "") {
		if (target.style.display == "none") {
			image.src = imageCollapsedUrl;
		} else {
			image.src = imageUrl;
		}
	}
	return false;
}

function _toggleTab(no, totalNum) {
	$('#tab' + no).addClass("active");
	$('#column' + no).show();
	for (var i = 1; i <= totalNum; i++) {
		if (i != no) {
			$('#tab' + i).removeClass("active");
			$('#column' + i).hide();
		}
	}
}

function _checkCol(column, className, bcheck) {
	var elements = $('#' + column + ' .' + className);
	for (var i = 0; i < elements.length; i++) {
		_checkAll(elements[i], bcheck);
	}
}

function _checkFormAll(isChecked) {
	$(":checkbox").each(function () {
		$(this).attr("checked", isChecked);
		$(this)[0].checked = isChecked;
	});
}

function _checkAll(layer, bcheck) {
	for (var i = 0; i < layer.childNodes.length; i++) {
		if (layer.childNodes[i].childNodes.length > 0) {
			_checkAll(layer.childNodes[i], bcheck);
		} else {
			if (layer.childNodes[i].type == "checkbox") {
				layer.childNodes[i].checked = bcheck;
			}
		}
	}
}

function _getCheckBoxCollectionValue(checklist) {

	var retval = '';
	if (checklist) {
		if (!checklist.length) {
			if (checklist.checked) {
				retval = encodeURI(checklist.value) + '';
			}
		} else {
			var hasValue = false;
			for (var i = 0; i < checklist.length; i++) {
				if (checklist[i].checked) {
					hasValue = true;
					retval += encodeURI(checklist[i].value) + ',';
				}
			}
			if (hasValue) {
				retval = retval.substring(0, retval.length - 1);
			}
		}
	}
	return retval;
}

function _alertCheckBoxCollection(checklist, alertString) {
	var collectionValue = _getCheckBoxCollectionValue(checklist);
	if (collectionValue.length == 0) {
		swal({
			title: alertString,
			icon: 'warning',
			button: '关 闭'
		});
		return true;
	}
	return false;
}

function _confirmCheckBoxCollection(checklist, emptyAlertText, confirmAlertText, linkUrl) {
	var collectionValue = _getCheckBoxCollectionValue(checklist);
	if (collectionValue.length == 0) {
		swal({
			title: emptyAlertText,
			icon: 'warning',
			button: '关 闭'
		});
		return true;
	}
	
	swal({
		title: confirmAlertText,
		icon: 'warning',
		buttons: {
			cancel: {
				text: '取 消',
				visible: true,
				className: 'btn'
			},
			confirm: {
				text: '确 认',
				visible: true,
				className: 'btn btn-danger'
			}
		}
	})
	.then(function (isConfirm) {
		if (isConfirm) {
			{
				_goto(linkUrl);
			}
		}
	});
}

function _confirm(confirmAlertText, linkUrl) {
	swal({
		title: confirmAlertText,
		icon: 'warning',
		buttons: {
			cancel: {
				text: '取 消',
				visible: true,
				className: 'btn'
			},
			confirm: {
				text: '确 认',
				visible: true,
				className: 'btn btn-danger'
			}
		}
	})
	.then(function (isConfirm) {
		if (isConfirm) {
			{
				_goto(linkUrl);
			}
		}
	});
}

var _get = function (id) {
	return document.getElementById(id);
};

var _goto = function (url) {

	window.location.href = url;
};

function _refresh() {
	window.location.reload(true);
}

function _setCookie(name, value, hoursToExpire) {
	var expire = '';
	if (hoursToExpire != undefined) {
		var d = new Date();
		d.setTime(d.getTime() + (3600000 * parseFloat(hoursToExpire)));
		expire = '; expires=' + d.toGMTString();
	}

	window.document.cookie = name + "=" + escape(value) + expire + ";path=/";
}


function _getCookie(name) {
	var cookieString = "" + window.document.cookie;
	var search = name + "=";
	if (cookieString.length > 0) {
		offset = cookieString.indexOf(search);
		if (offset != -1) {
			offset += search.length;
			end = cookieString.indexOf(";", offset);
			if (end == -1) end = cookieString.length;
			return unescape(cookieString.substring(offset, end));
		}
	}
	return null;
}