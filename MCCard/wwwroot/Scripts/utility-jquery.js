(function () {
    /*** Array ***/
    Array.prototype.forEach = function (_Fn, _Index) { if (typeof _Fn != "function") return; var _Index = arguments[1]; for (var i = 0; i < this.length; i++) if (i in this) _Fn.call(_Index, this[i], i, this); };
    Array.prototype.indexOf = function (_V, _B) { for (var i = _B || 0; i < this.length; i++) { if (this[i] == _V) { return i; } }; return -1; };
    Array.prototype.lastIndexOf = function (_V, _B) { _B = _B || 0; for (var i = this.length - 1; i >= _B; i--) { if (this[i] == _V) { return i; } }; return -1; };
    Array.prototype.insert = function (_Index, _V, _Agt) { if (_Index >= 0) { var arrayA = this.slice(0, _Index), arrayB = this.slice(_Index); arrayA[(_Agt) && _Agt == true ? _Index : arrayA.length] = _V; return arrayA.concat(arrayB); } };
    Array.prototype.unique = function () { var arrayA = []; for (var i = 0; i < this.length; i++) { if (arrayA.indexOf(this[i], 0) == -1) arrayA.push(this[i]); }; return arrayA; };
    Array.prototype.walk = function (_Fn) { var arrayA = [], i = this.length; while (i--) { arrayA.push(_Fn(this[i])); }; return arrayA.reverse(); };
    Array.prototype.max = function () { var arrA = []; for (var i = 0; i < this.length; i++) if (isNum(this[i]) || isNumber(this[i])) arrA.push(this[i]); var mathResult = Math.max.apply({}, arrA); return (mathResult) ? mathResult : null; };
    Array.prototype.min = function () { var arrA = []; for (var i = 0; i < this.length; i++) if (isNum(this[i]) || isNumber(this[i])) arrA.push(this[i]); var mathResult = Math.min.apply({}, arrA); return (mathResult) ? mathResult : null; };
    Array.prototype.getElementsByID = Array.prototype.getByID = function (_N) { return $.getByID(_N, this); };
    Array.prototype.getElementsByName = Array.prototype.getByName = function (_N) { return $.getByName(_N, this); };
    Array.prototype.getElementsByClass = Array.prototype.getByClass = function (_N) { return $.getByClass(_N, this); };
    Array.prototype.getElementsByTag = Array.prototype.getByTag = function (_N) { return $.getByTag(_N, this) };
    Array.prototype.getElementsByAtt = Array.prototype.getByAtt = function (_AN, _V) { return $.getByAtt(_AN, _V, this); };
    Array.prototype.getElementsByType = Array.prototype.getByType = function (_N) { return $.getByType(_N, this); };
    Array.prototype.css = function (_Cls) { this.method = $.css; this.method(_Cls, this); delete this.method; };
    Array.prototype.disp = function (_S) { this.method = $.css.disp; this.method(_S, this); delete this.method; };
    Array.prototype.disb = function (_S) { this.method = $.css.disb; this.method(_S, this); delete this.method; };
    Array.prototype.seled = function (_Seled) { return $.fn.select.seled(this, _Seled); };
    /*** String ***/
    String.prototype.toLower = function () { return this.toLowerCase(); };
    String.prototype.toUpper = function () { return this.toUpperCase(); };
    String.prototype.Replace = function (_OldValue, _NewValue) { return this.split(_OldValue).join(_NewValue); };
    String.prototype.trim = function () { return this.replace(/^\s+|\s+$/g, '') };
    String.prototype.ltrim = function () { return this.replace(/^\s+/g, '') };
    String.prototype.rtrim = function () { return this.replace(/\s+$/g, '') };
    String.prototype.getLength = function () { return this.replace(/[^\x00-\xff]/g, "**").length; };
    String.prototype.endWith = function (_Mh) { return this.length > 0 ? this.substr(this.length - _Mh.length) == _Mh : '' == _Mh; };
    String.prototype.startWith = function (_Mh) { return this.length > 0 ? this.indexOf(_Mh) == 0 : '' == _Mh; };
    String.prototype.toArray = function () { var arrayA = []; for (var i = 0; i < this.length; i++) arrayA.push(this.substr(i, 1)); return arrayA; };
    String.prototype.isIPv4 = function () { return (function (oIP) { if (!oIP || oIP.length == 0) return false; if (/^(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3})$/.test(oIP) == false) return false; else if (RegExp.$1 < 1 || RegExp.$1 > 254 || RegExp.$2 < 0 || RegExp.$2 > 254 || RegExp.$3 < 0 || RegExp.$3 > 254 || RegExp.$4 < 1 || RegExp.$4 > 254) return false; return true; })(oIP); };
    String.prototype.isMail = function () { return this.length == 0 ? false : /^[a-zA-Z0-9\._\+]+@([a-zA-Z0-9\.-]\.)*[a-zA-Z0-9\.-]+$/.test(this); };
    String.prototype.isID = function () { return this.length == 0 ? false : /^[A-Z](1|2)\d{8}$/g.test(this); };
    String.prototype.isCCard = function () { return this.length == 0 ? false : /^\d{4}-\d{4}-\d{4}-\d{4}$/g.test(this); };
    String.prototype.isMask = function () { return this.length == 0 ? false : !this.isCH() && this.replace(/\W|\_/g, '').length == 0; };
    String.prototype.isEng = function () { return this.length == 0 ? false : this.replace(/[a-zA-Z]/g, '').length == 0; };
    String.prototype.isNum = function () { return this.length > 0 && this.replace(/\d/g, '').length == 0; };
    String.prototype.isCH = function () { return this.length == 0 ? false : this.replace(/[^\x00-\xff]/g, '').length == 0; };
    String.prototype.isHTML = function () { return this.length == 0 ? false : /<(.*)>.*<\/(.*)\>|<(.*)\/>/.test(this); };
    String.prototype.replaceByDate = function () { return /.*\s.*/.test(this) ? this.replace(/\s.*/g, '').replace(/\W/g, '/') + ' ' + this.replace(/.*\s/g, '').replace(/\W/g, ':') : this.replace(/\s/g, '').replace(/\W/g, '/'); };
    String.prototype.compareDate = function (_Date) {
        var statusNum = -2; var dayNum = -1;
        var dateA = this;
        if (dateA && dateA.length > 0 && _Date) {
            dateA = Date.parse(dateA.toString().replaceByDate()); _Date = Date.parse(_Date.toString().replaceByDate());
            if (new Date(dateA) < new Date(_Date)) statusNum = -1;
            else if (new Date(dateA) > new Date(_Date)) statusNum = 1;
            else statusNum = 0;
            dayNum = Math.floor(Math.abs(dateA - _Date) / (1000 * 3600 * 24));
        }; return { status: statusNum, day: dayNum };
    };
    String.prototype.isDateTime = function () {
        if (this.length == 0) return false;
        checkYMD = function (_YMD) { return /^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$/g.test(_YMD.replace(/\s(.*)/g, '').replace(/\W/g, '-')); };
        checkHMS = function (_HMS) { return /^(20|21|22|23|[0-1]\d):[0-5]\d:[0-5]\d$/g.test(_HMS.replace(/(.*)\s/g, '').replace(/\W/g, ':')); };
        if (/^\d{4}\W\d{2}\W\d{2}$/g.test(this)) return checkYMD(this);
        else if (/^\d{2}\W\d{2}\W\d{2}$/g.test(this)) return checkHMS(this);
        return checkYMD(this) && checkHMS(this);
    };
    String.prototype.padLeft = function (_Cut, _CS) { this.padLeft = function (_Str, _Cut, _CS) { if (_Str.length >= _Cut) return _Str; else return this.padLeft(_CS + _Str, _Cut, _CS); }; return this.padLeft(this, _Cut, _CS); };
    String.prototype.padRight = function (_Cut, _CS) { this.padRight = function (_Str, _Cut, _CS) { if (_Str.length >= _Cut) return _Str; else return this.padRight(_Str + _CS, _Cut, _CS); }; return this.padRight(this, _Cut, _CS); };
    String.prototype.toNumber = function () { if (this.length == 0) return 0; else return parseInt(this); };
    String.prototype.toHex = function () { if (this.length == 0) return 'FF'; else return this.toNumber().toHex(); };
    String.prototype.htmlEnCode = function () { var s = this; if (s.length > 0) { s = s.replace(/&/g, "&gt;"); s = s.replace(/</g, "&lt;"); s = s.replace(/>/g, "&gt;"); s = s.replace(/\s/g, "&nbsp;"); s = s.replace(/\'/g, "&#39;"); s = s.replace(/\"/g, "&quot;"); s = s.replace(/\n/g, "<br>"); }; return s; };
    String.prototype.htmlDeCode = function () { var s = this; if (s.length > 0) { s = s.replace(/&gt;/g, "&"); s = s.replace(/&lt;/g, "<"); s = s.replace(/&gt;/g, ">"); s = s.replace(/&nbsp;/g, " "); s = s.replace(/&#39;/g, "\'"); s = s.replace(/&quot;/g, "\""); s = s.replace(/<br>/ig, "\n"); }; return s; };
    String.prototype.urlEnCode = function () { return encodeURI(this); };
    String.prototype.urlDeCode = function () { return decodeURI(this); };
    String.prototype.toAscii = function () {
        toAscii = function (_Str) {
            var symbols = " !\"#$%&'()*+'-./0123456789:;<=>?@";
            if (_Str.length > 0) {
                var loAZ = "abcdefghijklmnopqrstuvwxyz";
                symbols += loAZ.toUpperCase();
                symbols += "[\\]^_`";
                symbols += loAZ;
                symbols += "{|}~";
                var loc = symbols.indexOf(_Str);
                if (loc > -1) return (32 + loc);
            }; return (0);
        }; var num = '0'; for (var i = 0; i < this.length; i++) { num += toAscii(this.charAt(i)) }; return num;
    }
    String.prototype.base64Encode = function () { return $.base64.encode(this); };
    String.prototype.base64Decode = function () { return $.base64.decode(this); };
    /*** Number ***/
    Number.prototype.toHex = function () { var hexArray = new Array('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'); var _MathRound = Math.round(this); return (hexArray[Math.floor(_MathRound / 16) % 16] + hexArray[_MathRound % 16]); };
    Number.prototype.toNumber = function () { return this.toString().toNumber(); };
    /*** Date ***/
    Date.prototype.now = function (_Mode) {
        var nowDate = new Date();
        var myDate = nowDate.getFullYear() + '/' + (nowDate.getMonth() + 1).toString().padLeft(2, '0') + '/' + nowDate.getDate().toString().padLeft(2, '0')
        var myTime = nowDate.getHours().toString().padLeft(2, '0') + ':' + nowDate.getMinutes().toString().padLeft(2, '0') + ':' + nowDate.getSeconds().toString().padLeft(2, '0')
        switch (_Mode || '') {
            case 't': return myTime.trim();
            case 'd': return myDate.trim();
            default: return myDate + ' ' + myTime.trim();
        };
    };
    Date.prototype.compareDate = function (_Date) { return this.toString().compareDate(Date); };
    userAgent = navigator.userAgent.toLowerCase();
    isIE = /msie/.test(userAgent) && !/opera/.test(userAgent);
    isFF = /mozilla/.test(userAgent) && !/(compatible|webkit)/.test(userAgent);
    isOP = /opera/.test(userAgent);
    isSF = /webkit/.test(userAgent);
    getObj = function (_Obj, _ON, _Def, _MinNum) {
        if (isNull(_Obj) || isNull(_Obj[_ON])) return _Def;
        if (!isNumber(_Def)) return !isNull(_Obj) && !isNull(_Obj[_ON]) ? _Obj[_ON] : _Def;
        else { var obj = _Obj[_ON]; if (!isNull(_MinNum) && isNumber(_MinNum)) return !isNumber(obj) || Number(obj) > Number(_MinNum) ? _Def : obj; else return isNumber(obj) ? obj : _Def; };
    };
    /*** Hashtable ***/
    hashTable = function () {
        chankKey = function (_Key) { _Key = isNum(_Key) || isNumber(_Key) ? 'c' + _Key.toString() : _Key; return _Key; };
        this.hash = {};
        this.add = function (_Key, _Val) { _Key = chankKey(_Key); _Val = isNum(_Val) || isNumber(_Val) ? _Val.toString() : _Val; if (_Key && _Val) { if (!this.contains(_Key)) { this.hash[_Key] = isNull(_Val) ? null : _Val; return true; } else return false; }; return false; };
        this.update = function (_Key, _Val) { _Key = chankKey(_Key); _Val = isNum(_Val) || isNumber(_Val) ? _Val.toString() : _Val; if (_Key && _Val) { if (this.contains(_Key)) { this.hash[_Key] = isNull(_Val) ? null : _Val; return true; } else return false; }; return false; };
        this.remove = function (_Key) { _Key = chankKey(_Key); delete this.hash[_Key] };
        this.count = function () { var i = 0; for (var _Key in this.hash) i++; return i; };
        this.items = function (_Key) { _Key = chankKey(_Key); return this.hash[_Key] };
        this.contains = function (_Key) { _Key = chankKey(_Key); return !isNull(this.hash[_Key]) };
        this.clear = function () { for (var obj in this.hash) delete this.hash[obj] };
    };
    isArray = function (_Obj) { return !isNull(_Obj) && _Obj.constructor && _Obj.constructor == Array; }
    isObject = function (_Obj) { return !isNull(_Obj) && typeof _Obj == "object"; }
    isBool = function (_Obj) { return !isNull(_Obj) && typeof _Obj == "boolean"; }
    isString = function (_Obj) { return !isNull(_Obj) && typeof _Obj == "string"; }
    isNumber = function (_Obj) { return !isNull(_Obj) && typeof _Obj == "number"; }
    isFunction = function (_Obj) { return !isNull(_Obj) && typeof _Obj == "function"; }
    isNull = function (_Obj) { return _Obj == null || typeof _Obj == "undefined"; }
    isNum = function (_Number) { var strP = /^\d+(\.\d+)?$/; return strP.test(_Number) && parseFloat(_Number) == _Number; }
})();
var Base64 = { _keyStr: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=", encode: function (e) { if (e == null || e.length == 0) return ''; var t = ""; var n, r, i, s, o, u, a; var f = 0; e = Base64._utf8_encode(e); while (f < e.length) { n = e.charCodeAt(f++); r = e.charCodeAt(f++); i = e.charCodeAt(f++); s = n >> 2; o = (n & 3) << 4 | r >> 4; u = (r & 15) << 2 | i >> 6; a = i & 63; if (isNaN(r)) { u = a = 64 } else if (isNaN(i)) { a = 64 } t = t + this._keyStr.charAt(s) + this._keyStr.charAt(o) + this._keyStr.charAt(u) + this._keyStr.charAt(a) } return t }, decode: function (e) { if (e == null || e.length == 0) return ''; var t = ""; var n, r, i; var s, o, u, a; var f = 0; e = e.replace(/[^A-Za-z0-9+/=]/g, ""); while (f < e.length) { s = this._keyStr.indexOf(e.charAt(f++)); o = this._keyStr.indexOf(e.charAt(f++)); u = this._keyStr.indexOf(e.charAt(f++)); a = this._keyStr.indexOf(e.charAt(f++)); n = s << 2 | o >> 4; r = (o & 15) << 4 | u >> 2; i = (u & 3) << 6 | a; t = t + String.fromCharCode(n); if (u != 64) { t = t + String.fromCharCode(r) } if (a != 64) { t = t + String.fromCharCode(i) } } t = Base64._utf8_decode(t); return t }, _utf8_encode: function (e) { e = e.replace(/rn/g, "n"); var t = ""; for (var n = 0; n < e.length; n++) { var r = e.charCodeAt(n); if (r < 128) { t += String.fromCharCode(r) } else if (r > 127 && r < 2048) { t += String.fromCharCode(r >> 6 | 192); t += String.fromCharCode(r & 63 | 128) } else { t += String.fromCharCode(r >> 12 | 224); t += String.fromCharCode(r >> 6 & 63 | 128); t += String.fromCharCode(r & 63 | 128) } } return t }, _utf8_decode: function (e) { var t = ""; var n = 0; var r = c1 = c2 = 0; while (n < e.length) { r = e.charCodeAt(n); if (r < 128) { t += String.fromCharCode(r); n++ } else if (r > 191 && r < 224) { c2 = e.charCodeAt(n + 1); t += String.fromCharCode((r & 31) << 6 | c2 & 63); n += 2 } else { c2 = e.charCodeAt(n + 1); c3 = e.charCodeAt(n + 2); t += String.fromCharCode((r & 15) << 12 | (c2 & 63) << 6 | c3 & 63); n += 3 } } return t } }

var allMask = [' ', '~', '`', '|', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '=', '_', '+', '[', ']', '{', '}', '|', ';', '\'', ':', '\"', ',', '.', '/', '<', '>', '?', '＋', '－', '÷', '±', '√', '＜', '＞', '＝', '≦', '≧', '≠', '∞', '≒', '≡', '﹢', '﹣', '﹤', '﹥', '﹦', '～', '∩', '∪', '⊥', '∠', '∟', '⊿', '㏒', '㏑', '∫', '∮', '∵', '∴', '×', '¹', '²', '³', '\\'];
var urlMask = [' ', '~', '`', '|', '!', '#', '$', '%', '^', '*', '(', ')', '-', '_', '+', '[', ']', '{', '}', '|', '\'', '\"', ',', '.', '<', '>'];

var errColor = '#ffccff';
var defColor = '#fff';
var disColor = '#d4d0c8';

function isNull(obj) { return obj == null || typeof obj == "undefined"; }
function isNumber(obj) { return !isNull(obj) && typeof obj == "number"; }
function getObj(obj, name, def, minnum) {
    if (isNull(obj) || isNull(obj[name])) return def;
    if (!isNumber(def)) return !isNull(obj) && !isNull(obj[name]) ? obj[name] : def;
    else { var obj = obj[name]; if (!isNull(minnum) && isNumber(minnum)) return !isNumber(obj) || Number(obj) > Number(minnum) ? def : obj; else return isNumber(obj) ? obj : def; };
};

function changeBgColor(Color, Control) {
    if (!Control) return false;
    //Control.css("background-color", Color);
    return errColor != Color;
};

function checkInput(Cls) {
    var obj = new Object;
    obj.control = getObj(Cls, 'control', null);
    if (isNull(obj.control)) {
        alert('Element is null');
        return false;
    };
    obj.prompt = getObj(Cls, 'prompt', null);
    obj.promptFailText = getObj(Cls, 'promptFailText', '');
    obj.promptFailClass = getObj(Cls, 'promptFailClass', '');
    obj.promptPassText = getObj(Cls, 'promptPassText', '');
    obj.promptPassClass = getObj(Cls, 'promptPassClass', '');

    changeBgColor(defColor, obj.control);

    var minLength = getObj(Cls, 'minLength', 0);
    var maxLength = getObj(Cls, 'maxLength', 0);
    obj.maskFormat = getObj(Cls, 'maskFormat', null);
    obj.isMail = getObj(Cls, 'isMail', false);
    obj.isID = getObj(Cls, 'isID', false);
    obj.isCCard = getObj(Cls, 'isCCard', false);
    obj.isHTML = getObj(Cls, 'isHTML', false);
    obj.isIPv4 = getObj(Cls, 'isIPv4', false);
    obj.isEng = getObj(Cls, 'isEng', false);
    obj.isNum = getObj(Cls, 'isNum', false);
    obj.isCH = getObj(Cls, 'isCH', false);
    obj.isMask = getObj(Cls, 'isMask', false);
    var message1 = '';
    var message2 = '輸入格式錯誤';
    checkLengthMin = function (Str, Length) {
        return Number(Str.trim().length) >= Length;
    };
    checkLengthMax = function (Str, Length) {
        return Number(Str.trim().length) <= Length;
    };
    if (minLength <= 0 && obj.control.val().trim().length == 0) return true;

    var check = true;

    if (!isNull(minLength) && minLength > 0 && !checkLengthMin(obj.control.val(), Number(minLength))) {
        check = false; changeBgColor(errColor, obj.control);
    }
    else if (!isNull(maxLength) && maxLength > 0 && !checkLengthMax(obj.control.val(), Number(maxLength))) {
        check = false; changeBgColor(errColor, obj.control);
    };

    if (check) {
        if (!isNull(obj.isMail) && obj.isMail == true) {
            if (!obj.control.val().isMail()) {
                check = false; changeBgColor(errColor, obj.control);
            };
        }
        else if (!isNull(obj.isCCard) && obj.isCCard == true) {
            if (!obj.control.val().isCCard()) {
                check = false; changeBgColor(errColor, obj.control);
            };
        }
        else if (!isNull(obj.isID) && obj.isID == true) {
            if (!obj.control.val().isID()) {
                check = false; changeBgColor(errColor, obj.control);
            };
        }
        else if (!isNull(obj.isHTML) && obj.isHTML == true) {
            if (!obj.control.val().isHTML()) {
                check = false; changeBgColor(errColor, obj.control);
            };
        }
        else if (!isNull(obj.isIPv4) && obj.isIPv4 == true) {
            if (!obj.control.val().isIPv4()) {
                check = false; changeBgColor(errColor, obj.control);
            };
        }
        else {
            var maskFlag = !isNull(obj.isMask) && (isArray(obj.isMask) || obj.isMask == true);
            var engFlag = !isNull(obj.isEng) && obj.isEng == true;
            var chFlag = !isNull(obj.isCH) && obj.isCH == true;
            var numFlag = !isNull(obj.isNum) && obj.isNum == true;
            checkEng = function (_Str) { return _Str.isEng() };
            checkNum = function (_Str) { return _Str.isNum() };
            checkCH = function (_Str) { return _Str.isCH() };
            checkMask = function (_Str, obj) {
                if (isArray(obj.isMask)) {
                    var strExp = '';
                    obj.isMask.forEach(function (e) {
                        strExp = strExp.length == 0 ? '\\' + e : strExp + '|\\' + e;
                    }); if (strExp.length > 0) {
                        if (_Str == '\r' || _Str == '\n') return true;
                        else return eval("/" + strExp + "/g").test(_Str);
                    }
                }
                else return _Str.isMask();
            };
            var checkFlag = true;
            for (var i = 0; i < obj.control.val().length; i++) {
                var s = obj.control.val().substr(i, 1);
                checkFlag = false;
                if (!checkFlag) {
                    checkFlag = checkMask(s, obj); if (maskFlag == false && checkFlag == true) checkFlag = false;
                };
                if (!checkFlag && engFlag) checkFlag = checkEng(s);
                if (!checkFlag && numFlag) checkFlag = checkNum(s);
                if (!checkFlag && chFlag) checkFlag = checkCH(s);
                if (!checkFlag) { check = false; break; };
            }; if (!checkFlag) changeBgColor(errColor, obj.control);
        }
    }
    if (!check) {
        var message = '';
        if (minLength > 0) {
            if (minLength == maxLength) {
                message = '<img style="width: 25px;" src="/wwwroot/Image/desktop_img/ic_error.png" alt="" />' + '輸入的長度必須為' + maxLength;
            }
            else {
                message = '<img style="width: 25px;" src="/wwwroot/Image/desktop_img/ic_error.png" alt="" />' + '輸入的長度請在' + minLength + '~' + maxLength + '個字範圍內';
            }
        }
        else message = '<img style="width: 25px;" src="/wwwroot/Image/desktop_img/ic_error.png" alt="" />' + '輸入的長度必須為' + maxLength;

        if (isNull(obj.prompt)) {
            alertBox(message, obj.control);
        }
        else {
            obj.prompt.html(message);
            obj.prompt.attr('class', obj.promptFailClass);
        }
        obj.control.focus();
    }
    else if (!isNull(obj.prompt) && !isNull(obj.promptPassText) && !isNull(obj.promptPassClass)) {
        obj.prompt.html(obj.promptPassText);
        obj.prompt.attr('class', obj.promptPassClass);
    }
    return check;
};

function alertBox(Message, Control) {
    if ($('#messageBox')) showMessageBox({ title: '訊息', body: Message, control: Control });
    else alert(Message);
}

function prompt(Cls, IsPass) {
    var obj = new Object;
    obj.prompt = getObj(Cls, 'prompt', null);
    if (isNull(obj.prompt)) return;
    obj.promptFailText = getObj(Cls, 'promptFailText', '');
    obj.promptFailClass = getObj(Cls, 'promptFailClass', '');
    obj.promptPassText = getObj(Cls, 'promptPassText', '');
    obj.promptPassClass = getObj(Cls, 'promptPassClass', '');

    if (!IsPass) {
        obj.prompt.html(obj.promptFailText);
        obj.prompt.attr('class', obj.promptFailClass);
    }
    else {
        obj.prompt.html(obj.promptPassText);
        obj.prompt.attr('class', obj.promptPassClass);
    }
}

function checkDate(Cls) {
    var obj = new Object;
    obj.control = getObj(Cls, 'control', null);
    if (isNull(obj.control)) {
        alert('Element is null');
        return false;
    };
    changeBgColor(defColor, obj.control);

    var date = obj.control.val();
    var isRepublic = getObj(Cls, 'isRepublic', false);
    var isRequired = getObj(Cls, 'isRequired', false);

    if (isRequired && !date.length > 0) { alert("未輸入日期, 請檢查"); return changeBgColor(errColor, obj.control); }

    if (date.split('/').length == 3) {
        if (isRepublic) {
            var dateArr = date.split('/');
            date = '';
            if (dateArr[0].isNum()) date = (dateArr[0].toNumber() + 1911);
            if (dateArr[1].isNum()) date += '/' + dateArr[1];
            if (dateArr[2].isNum()) date += '/' + dateArr[2];
        }
    }
    if (date.endWith('2/29')) return true;

    if (date.length > 0) date += ' 00:00:00';

    if (!date.isDateTime() && date.length > 0) {
        alert("日期格式輸入錯誤, 請檢查"); return changeBgColor(errColor, obj.control);
    };
    return true;
};

function checkDateRange(Cls) {
    var obj = new Object;
    obj.startDate = getObj(Cls, 'startDate', null);
    if (isNull(obj.startDate)) {
        alert('Start date element is null');
        return false;
    };
    if (!checkDate({ control: obj.startDate, isRepublic: true })) return false;

    obj.endDate = getObj(Cls, 'endDate', null);
    if (isNull(obj.endDate)) {
        alert('End date element is null');
        return false;
    };
    if (!checkDate({ control: obj.endDate, isRepublic: true })) return false;

    changeBgColor(defColor, obj.startDate);

    changeBgColor(defColor, obj.endDate);

    changeDateTime = function (Date) {
        if (Date.split('/').length == 3) {
            var dateArr = Date.split('/');
            Date = '';
            if (dateArr[0].isNum()) Date = (dateArr[0].toNumber() + 1911);
            if (dateArr[1].isNum()) Date += '/' + dateArr[1];
            if (dateArr[2].isNum()) Date += '/' + dateArr[2];
        }
        if (Date.length > 0) Date += ' 00:00:00';
        return Date;
    };
    var startDate = changeDateTime(obj.startDate.val());

    var endDate = changeDateTime(obj.endDate.val());

    if (startDate.compareDate(endDate).status > 0) {
        alert("開始日期請勿大於結束日期, 請檢查");
        changeBgColor(errColor, obj.startDate);
        changeBgColor(errColor, obj.endDate);
        return false;
    };
    return true;
};

function changeMoneyFormat(Money) {
    if (/[^0-9\.]/.test(Money)) return Money;
    Money = Money.replace(/^(\d*)$/, "$1.");
    Money = (Money + "00").replace(/(\d*\.\d\d)\d*/, "$1");
    Money = Money.replace(".", ",");
    var re = /(\d)(\d{3},)/;
    while (re.test(Money)) {
        Money = Money.replace(re, "$1,$2");
    }
    Money = Money.replace(/,(\d\d)$/, ".$1");
    return Money.replace(/^\./, "0.").Replace('.00', '');
};
function rechangeMoneyFormat(Money) {
    return Money.Replace(',', '').Replace('.00', '');
};

function openWindow(Url, WindowName, Height, Width) {
    window.open(Url, WindowName, "width=" + Width + ",height=" + Height + ",scrollbars=yes,scroll=yes");
    return false;
};

msgBoxPosition = function () {
    try {
        var oBackground = document.getElementById("background");
        var oContent = document.getElementById("messageBox");
        var dw = $(document).width();
        var dh = $(document).height();
        oBackground.style.width = dw + 'px';
        oBackground.style.height = dh + 'px';
        var scrollTop = document.documentElement.scrollTop || document.body.scrollTop;
        var l = (document.documentElement.clientWidth - oContent.offsetWidth) / 2;
        var t = ((document.documentElement.clientHeight - oContent.offsetHeight) / 2) + scrollTop;
        if (t < 80) t = 80;
        oContent.style.left = l + 'px';
        oContent.style.top = t + 'px';
        if ($('#msgbody').height() >= document.documentElement.clientHeight) $('#messageBoxBody').height(350);
    }
    catch (e) { }
}

function showMessageBox(Cls) {
    $(document).ready(function () {
        $("#PopupMessageTitle").text(getObj(Cls, 'title', ''));
        $("#PopupMessageBody").text(getObj(Cls, 'body', ''));
        $('.PopupMessage').modal('show');
    });
};
//function showMessageBox(Cls) {
//    $(document).ready(function () {
//        var messageBox = $('#messageBox');
//        if (!messageBox) return;
//        $(messageBox).css({ 'z-index' : '9999'  });
//        var title = getObj(Cls, 'title', '');
//        if (title == '') $('#messageBoxTitle').attr('class', 'message-box-title-not-back');
//        var textAlign = getObj(Cls, 'textAlign', '');
//        if (textAlign != 'center') {
//            $('#messageBoxBody').css("text-align", textAlign);
//            $('#messageBoxBody').css("padding-left", '20px');
//            $('#messageBoxBody').css("padding-right", '20px');
//        }
//        $('#msgtitle').html(title);
//        $('#msgbody').html(getObj(Cls, 'body', ''));
//        $("#background, #messageBox").show();

//        $("#btnMessageBoxConfirm, #btnMessageBoxClose").bind("click", function () {
//            $("#background, #messageBox").hide();
//            var url = getObj(Cls, 'url', '');
//            if (url != '') window.location.replace(url);
//            var control = getObj(Cls, 'control', null);
//            if (control) control.focus();
//        });
//        msgBoxPosition();
//    });
//};
function subBannerResize() {
    if ($('#imgSubBanner').length == 0) return;
    var s = window.innerWidth >= 1280 ? 480 / 1920 : 320 / 640;
    if (window.innerWidth >= 1280 && $('#imgSubBanner')[0].src != $('#imgSubBanner').attr('pcurl')) $('#imgSubBanner')[0].src = $('#imgSubBanner').attr('pcurl');
    else if (window.innerWidth < 1280 && $('#imgSubBanner')[0].src != $('#imgSubBanner').attr('mobileurl')) $('#imgSubBanner')[0].src = $('#imgSubBanner').attr('mobileurl');
    var eh = (window.innerWidth * s);
    var image = $('#imgSubBanner');
    image.height(eh);
    $('#imgSubBanner, #imgNewsBanner').width(window.innerWidth * 1.02);
};

function stepBarOnload(img) {
    var stepbar = $(img);
    stepbar.attr('load', 'Y');
    stepBarResize();
}

function stepBarResize() {
    if ($('.step-bar').length > 0) {
        var stepbar = $('.step-bar')[0];
        if ($(stepbar).attr('load') != null) {
            var div = $(stepbar.parentNode);
            if ($(stepbar).attr('ow') == null) {
                $(stepbar).attr('ow', $(stepbar).width());
                $(stepbar).attr('oh', $(stepbar).height());
            }
            var size = createThumbnail($(stepbar).attr('ow'), $(stepbar).attr('oh'), div.width(), div.height());
            $(stepbar).width(size.w); $(stepbar).height(size.h);
        }
    }
}
function navbarItemResize() {
    if ($('.pc').length > 0) for (var i = 0; i < $('.pc').length; i++) if (window.innerWidth > 1279) $($('.pc')[i]).show(); else $($('.pc')[i]).hide();
    if ($('.mobile').length > 0) for (var i = 0; i < $('.mobile').length; i++) if (window.innerWidth <= 1279) $($('.mobile')[i]).show(); else $($('.mobile')[i]).hide();
}
$(document).ready(function () {
    $('#btnNavbar').click(function () { $('#menu').css('display', $('#menu').css('display') == 'none' ? 'block' : 'none'); });
    navbarItemResize(); subBannerResize(); stepBarResize();
});
$(window).resize(function () {
    if (window.innerWidth > 1279 && $('#menu').css('display') == 'none') { $('#menu').css('display', 'block'); }
    else if (window.innerWidth <= 1279 && $('#menu').css('display') == 'block') { $('#menu').css('display', 'none'); }
    navbarItemResize(); subBannerResize(); stepBarResize();
});
function conPosition(oContent) {
    try {
        var oBackground = document.getElementById("background_sign");
        if (oBackground == null) return;
        $(oBackground).css({ 'z-index': '9997' });
        var dw = $(document).width();
        var dh = $(document).height();
        oBackground.style.width = dw + 'px';
        oBackground.style.height = dh + 'px';
        var scrollTop = document.documentElement.scrollTop || document.body.scrollTop;
        var l = (document.documentElement.clientWidth - oContent.offsetWidth) / 2;
        var t = ((document.documentElement.clientHeight - oContent.offsetHeight) / 2) + scrollTop;
        if (document.documentElement.clientWidth < 1280) t += 80;
        oContent.style.left = l + 'px';
        oContent.style.top = '0px';
        $(oContent).css({ 'z-index': '9998' });
    }
    catch (e) { }
}

function createThumbnail(ow, oh, w, h) {
    var x_ratio = w / ow, y_ratio = h / oh;
    if ((ow <= w) && (oh <= h)) return { 'w': ow, 'h': oh };
    else if ((x_ratio * oh) < h) return { 'w': w, 'h': x_ratio * oh };
    else return { 'w': y_ratio * ow, 'h': h };
}