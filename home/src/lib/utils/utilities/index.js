import _ from 'lodash';
import moment from 'moment';
import 'moment/locale/zh-cn';
export function getValidateCode() {
    var validateCode = "";
    var s = [
        '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
    ];
    for (var i = 0; i < 4; i++) {
        validateCode += s[_.random(s.length - 1)];
    }
    return validateCode;
}
export function toDate(o) {
    if (typeof o === 'object')
        return o;
    if (typeof o === 'string') {
        if (moment(o).isValid()) {
            return moment(o).toDate();
        }
    }
    return new Date();
}
export function dateDiff(start, end) {
    const startDate = toDate(start);
    const endDate = toDate(end);
    var datediff = endDate.getTime() - startDate.getTime();
    const d = (datediff / (24 * 60 * 60 * 1000));
    return d;
}
export function toDateString(date) {
    let str = '';
    if (typeof date === 'object') {
        let now = moment(date).format('YYYY-MM-DD');
        str = now;
    }
    else if (typeof date === 'string') {
        str = date;
    }
    str = str.replace(/\//g, '-');
    if (str.indexOf(' ') !== -1) {
        str = str.substr(0, str.indexOf(' '));
    }
    return str;
}
export function toTimeString(date) {
    let str = '';
    if (typeof date === 'object') {
        let now = moment(date).format('hh:mm:ss');
        str = now;
    }
    else if (typeof date === 'string') {
        str = date;
    }
    str = str.replace(/\//g, '-');
    if (str.indexOf(' ') !== -1) {
        str = str.substr(str.indexOf(' '));
    }
    return str;
}
export function isMobile(str) {
    const regPartton = /^1[3-9]+\d{9}$/;
    return regPartton.test(str);
}
export function isEmail(str) {
    var regPartton = /^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$/;
    return regPartton.test(str);
}
export function isError(errors) {
    for (let key in errors) {
        if (errors.hasOwnProperty(key) && errors[key]) {
            return true;
        }
    }
    return false;
}
;
export function stopAndPrevent(e) {
    stop(e);
    prevent(e);
}
export function stop(e) {
    if (e) {
        e.stopPropagation();
    }
}
export function prevent(e) {
    if (e) {
        e.preventDefault();
    }
}
export function getContent(str, maxNum) {
    let content = (str || '').substr(0, maxNum);
    if ((str || '').length > maxNum) {
        content += '...';
    }
    return content;
}
export function getFileName(attachUrl, len) {
    len = len || 5;
    try {
        let fileName = attachUrl.substr(attachUrl.lastIndexOf('/') + 1);
        if (fileName.lastIndexOf('_') !== -1) {
            fileName = fileName.substr(0, fileName.lastIndexOf('_'));
        }
        return getContent(fileName, len);
    }
    catch (e) { }
    return '查看';
}
export function getQueryStringValue(search, key) {
    return decodeURIComponent(search.replace(new RegExp("^(?:.*[&\\?]" + encodeURIComponent(key).replace(/[\.\+\*]/g, "\\$&") + "(?:\\=([^&]*))?)?.*$", "i"), "$1"));
}
export function findIgnoreCase(val, arr) {
    val = toLower(val);
    for (var o of arr) {
        if (toLower(o) === val) {
            return o;
        }
    }
    return undefined;
}
export function parseInt(val) {
    return _.parseInt(val);
}
export function parseBool(val) {
    return toLower(val) === 'true';
}
export function toLower(val) {
    if (!val)
        return '';
    return _.toLower(val);
}
export function lowerFirst(val) {
    if (!val)
        return '';
    return _.lowerFirst(val);
}
export function upperFirst(val) {
    if (!val)
        return '';
    return _.upperFirst(val);
}
export function assign(obj1, obj2) {
    return _.assign(obj1, obj2);
}
export function keys(obj) {
    return _.keys(obj);
}
export function values(obj) {
    return _.values(obj);
}
export function trim(val, chars) {
    return _.trim(val, chars);
}
export function find(collection, predicate, fromIndex) {
    return _.find(collection, predicate, fromIndex);
}
export function isObject(val) {
    return _.isObject(val);
}
export function indexOf(arr, val) {
    return _.indexOf(arr, val);
}
export function map(collection, iteratee) {
    return _.map(collection, iteratee);
}
//# sourceMappingURL=index.js.map