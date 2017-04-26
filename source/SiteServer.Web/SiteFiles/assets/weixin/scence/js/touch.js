/**
 + ---------------------------------------- +
 + 版权所有2013年，Touch底层框架1.0
 + Author: luzhichao
 + QQ: 190135180
 + Mail: luzhichao@shiqutech.com
 + ---------------------------------------- +
 + Date: 2014-03-31
 + ---------------------------------------- +
**/
(function (window, undefined){
	var win 		= window,
		doc			= document,
		tagNameExpr = /^[\w-]+$/,
		idExpr 		= /^#([\w-]*)$/,
		classExpr 	= /^\.([\w-]+)$/,
		readyRE 	= /complete|loaded|interactive/,
		REG_TRIM    = /^\s+|\s+$/g,
		REG_SPLIT   = /[\.\s]\s*\.?/,
		REG_CLASS   = /[\n\t]/g,
		AP 			= Array.prototype,
		indexOf 	= AP.indexOf, 
		filter 		= AP.filter,
		trim 		= String.prototype.trim,
		toString 	= Object.prototype.toString,
		objectE		= {
			"[object Boolean]"	: 'Boolean',
			"[object Number]"	: 'Number',
			"[object String]"	: 'String',
			"[object Function]"	: 'Function',
			"[object Array]"	: 'Array',
			"[object Date]"		: 'Date',
			"[object Object]"	: 'Object',
			"[object RegExp]"	: 'RegExp',
			"[object Error]"	: 'Error'
		},
		ua 			= navigator.userAgent,
		getIeVersion= function (){
			var retVal = -1,
			ua, re;
			if(navigator.appName === 'Microsoft Internet Explorer'){
				ua = navigator.userAgent;
				re = new RegExp('MSIE ([0-9]{1,})');
				if(re.exec(ua) !== null){
					retVal = parseInt(RegExp.$1);
				}
			}
			return retVal;
		},
		platform	= {
			ieVersion 	: getIeVersion(),
			ie 			: getIeVersion() !== -1,
			android		: ua.match(/Android/i) === null ? false : true,
			iPhone		: ua.match(/iPhone/i) === null ? false : true,
			iPad		: ua.match(/iPad/i) === null ? false : true,
			iPod		: ua.match(/iPod/i) === null ? false : true,
			winPhone	: ua.match(/Windows Phone/i) === null ? false : true,
			IOS			: (ua.match(/iPad/i) === null ? false : true) || (ua.match(/iPhone/i) === null ? false : true),
			touchDevice	: "ontouchstart" in window
		},
		cssNumber	= { 'column-count': 1, 'columns': 1, 'font-weight': 1, 'line-height': 1,'opacity': 1, 'z-index': 1, 'zoom': 1 },
		camelize	= function (str){
			return str.replace(/-+(.)?/g, function(match, chr){ return chr ? chr.toUpperCase() : '' })
		},
		dasherize	= function (str){
			return str.replace(/::/g, '/')
			.replace(/([A-Z]+)([A-Z][a-z])/g, '$1_$2')
			.replace(/([a-z\d])([A-Z])/g, '$1_$2')
			.replace(/_/g, '-')
			.toLowerCase()
		},
		maybeAddPx	= function (name, value){
			return (typeof value == "number" && !cssNumber[dasherize(name)]) ? value + "px" : value;
		},startEvt,moveEvt,endEvt,
		getTouchPos = function(e){
			var t = e.touches;
			if(t && t[0]){
				return { x : t[0].clientX , y : t[0].clientY };
			}
			return { x : e.clientX , y: e.clientY };
		},
		getDist = function(p1 , p2){
			if(!p1 || !p2) return 0;
			return Math.sqrt((p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y));
		},
		Touch 		= function (selector, context){return new Touch.fn.init(selector, context);};
		//选择不同事件
		if(platform.touchDevice){
			startEvt	= "touchstart";
			moveEvt		= "touchmove";
			endEvt 		= "touchend";
		}
		else{
			startEvt	= "mousedown";
			moveEvt		= "mousemove";
			endEvt 		= "mouseup";            
		}
	/**
	 * 原型链
	 * @name Touch.prototype
	 * @object
	 */
	Touch.fn = Touch.prototype = {
		/**
		 * 判断String类型
		 * @name isString
		 * @param {object}    o     判断对象
		 * return String
		 */
		isString : function (o){
			return typeof o === 'string';
		},
		/**
		 * 检测是否为Function
		 * @name isFunction
		 * @param {object} 		o 	判断对象
		 * return boolean
		 */
		isFunction : function (o){
			return toString.call(o) === '[object Function]';
		},
		/**
		 * 检测是否为Array
		 * @name isArray
		 * @param {object} 		o 	判断对象
		 * return boolean
		 */
		isArray : function (o) {
			return toString.call(o) === '[object Array]';
		},
		/**
		 * 检测是否为Object
		 * @name isObject
		 * @param {object} 		o 	判断对象
		 * return boolean
		 */
		isObject : function (o) {			
			return (objectE[toString.call(o)] || "object") == "object";
		},
		/**
		 * 检测是否为Document
		 * @name isDocument
		 * @param {object} 		o 	判断对象
		 * return boolean
		 */
		isDocument : function (o){
			return o != null && o.nodeType == o.DOCUMENT_NODE
		},
		/**
		 * 判断Boolean类型
		 * @name isBoolean
		 * @param {object}    o     判断对象
		 * return boolean
		 */
		isBoolean : function (o){
			return typeof o === 'boolean';
		},
		/**
		 * 数组排重
		 * @name remArray
		 * @param {array}   arr     数组
		 * return boolean
		 */
		remArray : function (arr){
			arr.sort();
			var ret = [arr[0]], i = 1;
			for (i = 1; i < arr.length; i++) {
				if (arr[i] !== ret[ret.length - 1]) {
					ret.push(arr[i]);
				}
			}
			return ret;
		},
		/**
		 * 查找在数组中第几位
		 * @name indexOf
		 * @param {object} 	elem 	查找元素
		 * @param {array} 	arr 	数组
		 * return number
		 */
		indexOf : indexOf ?
		function (elem, arr) {
			return indexOf.call(arr, elem);
		} :
		function (elem, arr) {
			for(var i = 0, len = arr.length; i < len; ++i) {
				if(arr[i]===elem){
					return i;
				} 
			}
			return -1;
		},
		/**
		 * 去掉前后空格
		 * @name trim
		 * @param {string}  str 处理字符串
		 * return string
		 */
		trim: trim ? function (str){
			return (str === undefined) ? '' : trim.call(str);
		} : function (str){
			return (str === undefined) ? '' : str.toString().replace(REG_TRIM, '');
		},
		/**
		 * 将NodeList转换成数组
		 * @name makeArray
		 * @param {object} 	o 	NodeList
		 * return array
		 */
		makeArray : function (o) {
			var self = this;
			if(o===null || o=== undefined) return [];
			if(self.isArray(o)) return o;
			if(typeof o.length !== 'number' || typeof o === 'string' || self.isFunction(o)) {
				return [o];
			}
			return AP.slice.call(o);
		},
		find : function (element, selector){
			var found;
			if($.fn.isDocument(element) && idExpr.test(selector)){
				return (found = element.getElementById(RegExp.$1)) ? [found] : []
			}
			else if(element.nodeType !== 1 && element.nodeType !== 9){
				return []
			}
			else{
				if(classExpr.test(selector)){
					return this.makeArray(element.getElementsByClassName(RegExp.$1))
				}
				else{
					if(tagNameExpr.test(selector)){
						return this.makeArray(element.getElementsByTagName(selector))
					}
					else{
						return this.makeArray(element.querySelectorAll(selector))
					}
				}
			}
		},
		/**
		 * 判断条件类型
		 * @name judgeNode
		 * @param {object}  elem            当前元素
		 * @param {string}  condition       查找条件
		 * @return object
		 */
		judgeNode : function (elem, condition){
			if(condition && classExpr.test(condition)){
				return (' ' + elem.className + ' ').indexOf(' ' + condition.substring(1) + ' ') >= 0;
			}
			else if(condition && idExpr.test(condition)){
				return elem.id == condition.substring(1);
			}
			else if(condition && tagNameExpr.test(condition)){
				return elem.tagName.toLowerCase() == condition.toLowerCase();
			}
			else if(condition === undefined){
				return true;
			}
		},
		/**
		 * 判断Node属性
		 * @name nodeProperty
		 * @param {object}  elem        当前元素
		 * @param {string}  property    查找属性
		 * @param {string}  condition   查找条件
		 * @return object
		 */
		nodeProperty : function (elem, property, condition){
			var self = this,
				elem = elem[property];
			while(elem && !self.isDocument(elem)){
				if(elem.nodeType === 1 && condition && self.judgeNode(elem, condition)){
					return elem;
				}
				else if(elem.nodeType === 1 && condition === undefined){
					return elem;
				}
				elem = elem[property];
			}
		},
		transitionend : function (){
			var ret, endEventNames, div, handler, i;
			if('ontransitionend' in win){
				return 'transitionend';
			}
			else if('onwebkittransitionend' in win){
				return 'webkitTransitionEnd';
			}
			else if('transition' in doc.body.style){
				return 'transitionend';
			}
            return false;
		},		
		init : function (selector, context){
			var context = context || doc,
				self	= this,
				result;			
			if(!selector){
				return self.selector.objectArray();
			}
			else if(self.isFunction(selector)){
				return $(doc).ready(selector)
			}
			else if(self.isArray(selector)){
				return self.selector.objectArray(selector, selector);
			}
			else if(self.isObject(selector)){
				return self.selector.objectArray(self.makeArray(selector), selector);
			}
			else if(self.selector.isOA(selector)){
				return selector;
			}
			else{
				//优先使用原始的
				if(idExpr.test(selector)) {
					result = self.selector.id(selector.replace("#",""));
					if(result){
						return self.selector.objectArray([result], selector);
					}
					else{
						return self.selector.objectArray();	
					}
				}
				else if(tagNameExpr.test(selector)){
					result = self.selector.tagName(selector,context);
				}
				else if(classExpr.test(selector)){
					result = self.selector.className(selector.replace(".",""),context);
				}
				else{
					result = context.querySelectorAll(selector);
				}
				return self.selector.objectArray(self.makeArray(result), selector);
			}
		},
		selector : {
			id : function (id){
				return doc.getElementById(id);
			},
			tagName : function (tagName, context){
				return (context || doc).getElementsByTagName(tagName);
			},
			className : function(className, context){
				var children, elements, i, l, classNames,
					context	= context || doc,
					self	= this;
				if(context.getElementsByClassName){
					return context.getElementsByClassName(className);
				}
				else{
					children = context.getElementsByTagName('*');
					elements = [];
					for(i = 0, l = children.length; i < l; ++i){
						if(children[i].className && self.indexOf(className, children[i].className.split(' ')) >= 0){
							elements.push(children[i]);
						}
					}
					return elements;
				}
			},			
			isOA : function (object){
				return object instanceof this.objectArray;
			},
			objectArray : function (domList, selector){
				domList 			= domList || [];
				domList.__proto__ 	= this.fn;
				domList.selector 	= selector || '';
				return domList
			},
			fn : {
				/**
				 * 批量获取
				 * @name  domBatch
				 * @param {object || string}    selector        选择器
				 * @param {string}              property        属性
				 * @return array
				 */
				domBatch : function (selector, property){
					var ret = [];
					this.each(function (){
						if($.fn.nodeProperty(this, property, selector)){
							ret.push($.fn.nodeProperty(this, property, selector));
						}
					});
					return $($.fn.remArray(ret));
				},
				slice : function (){
					return $([].slice.apply(this, arguments));
				},
				ready : function (callback){
					if(readyRE.test(document.readyState)){
						callback($)
					}
					else{
						document.addEventListener('DOMContentLoaded', function(){callback($)}, false);
					}
					return this
				},
				each : function (callback){
					[].every.call(this, function(elem, index){
						return callback.call(elem, index, elem) !== false;
					})
					return this;
				},
				size : function (){
					return this.length;
				},
				remove : function (){
					return this.each(function(){
						(this.parentNode != null) && this.parentNode.removeChild(this);
					})
				},
				eq : function (index){
					return index === -1 ? this.slice(index) : this.slice(index, + index + 1);
				},
				index : function (element){
					for(var i=0,len=this.length;i<len;i++){
						if(this[i]==element){
							return i;
						}
					}
				},
				first : function (){
					var el = this[0];
					return el && !$.fn.isObject(el) ? el : $(el);
				},
				last : function (){
					var el = this[this.length - 1]
					return el && !$.fn.isObject(el) ? el : $(el)
				},
				find : function(selector){
					var result, self = this;
					if(this.length == 1){
						result = $(selector, this[0]);
					}
					else{
						result = [];
						for(var i=0,len=this.length;i<len;i++){
							result = result.concat($.fn.find(this[i], selector))
						}
					}
					return $(result);
				},
				/**
				 * 父级元素
				 * @name parent
				 * @param {string}    selector    查找条件
				 * @return object
				 */
				parent : function (selector){
					return this.domBatch(selector, 'parentNode');
				},
				/**
				 * 获取当前元素的previousSibling
				 * @name  prev
				 * @param {string}    selector    查找条件
				 * @return object
				 */
				prev : function (selector){
					return this.domBatch(selector, 'previousSibling');
				},
				/**
				 * 获取当前元素的nextSibling
				 * @name  next
				 * @param {string}    selector    查找条件
				 * @return object
				 */
				next : function (selector){
					return this.domBatch(selector, 'nextSibling');
				},
				/**
				 * 获取当前对象的所有兄弟节点
				 * @name  siblings
				 * @param {string}    selector    查找条件
				 * @return object
				 */
				siblings : function (selector){
					var elems = this, result = [], i = 0, len = elems.length;
					for(; i < len; ++i){
						var elem = elems[i];
						if(elem && elem.nodeType === 1){
							var elemF = elem.parentNode.firstChild;							
							while(elemF){
								if(elemF.nodeType == 1 && elemF !== elem && $.fn.judgeNode(elemF, selector)){
									result.push(elemF)
								}
								elemF = elemF.nextSibling;
							}
						}
					}
					return $(result);
				},
				/**
				 * 判断class是否存在
				 * @name  hasClass
				 * @param {object || string}    selector    选择器
				 * @param {string}              value       值
				 * @return boolean
				 */
				hasClass : function(name){
					var i 	= 0,
						len = this.length;
					for(; i < len; ++i){
						var elemClass = this[i].className,
							classNames	= name.split(' ');
						if(elemClass){
							var className = ' ' + elemClass + ' ', j = 0, ret = true;
							for(; j < classNames.length; ++j){
								if (className.indexOf(' ' + classNames[j] + ' ') < 0) {
									return false;
									break;
								} 
							}
							if(ret)return true; 
						}	
					}
				},
				/**
				 * 添加class
				 * @name  addClass
				 * @param {string}	name	值
				 * @return ignore
				 */
				addClass : function (name){
					var self = this;
					return self.each(function (){
						var elem 		= this,
							elemClass 	= elem.className,
							classNames	= name.split(' ');
						if(!elemClass){
							elem.className = name;
						} 
						else{
							var className 	= ' ' + elemClass + ' ',
								setClass	= elemClass,
								i 			= 0;
							for(; i < classNames.length; ++i){
								if(className.indexOf(' ' + classNames[i] + ' ') < 0){
									setClass += ' ' + classNames[i];
								}
							}
							elem.className = $.fn.trim(setClass);
						}
						return self;
					});
				},
				/**
				 * 删除class
				 * @name  removeClass
				 * @param {string}              name       值
				 * @return ignore
				 */
				removeClass : function (name){
					var self = this;
					return self.each(function (){
						var elem 		= this,
							elemClass 	= elem.className,
							classNames	= name.split(' ');
						if(elemClass){
							if(!classNames){
								elem.className = '';
							} 
							else{
								var className = (' ' + elemClass + ' ').replace(REG_CLASS, ' '), i = 0;
								for(; i < classNames.length; i++){
									className = className.replace(' ' + classNames[i] + ' ', ' ');
								}
								elem.className = $.fn.trim(className);
							}
						}
						return self;
					});
				},
				/**
				 * 替换class
				 * @name  replaceClass
				 * @param {string}              oldClassName    原始class
				 * @param {string}              newClassName    新class
				 * @return ignore
				 */
				replaceClass : function (oldClassName, newClassName){
					return this.removeClass(oldClassName).addClass(newClassName);
				},
				/**
				 * 删除添加class
				 * @name  toggleClass
				 * @param {string}		name 	值
				 * @param {boolean}		state 	是否替换
				 * @return ignore
				 */
				toggleClass : function (name, state){
					var isBool 	= $.fn.isBoolean(state),
						self	= this,
						has;
					return self.each(function (){
						var i = 0, className;
						for (; i < cl; ++i) {
							className = classNames[i];
							has = isBool ? !state : self.hasClass(className);
							self[has ? 'removeClass' : 'addClass'](className);
						}
						return self;
					});
				},
				/**
				 * 添加自定义属性
				 * @name  attr
				 * @param {string || json}      attr           值
				 * @param {string}              value          回调函数
				 * @return string
				 */
				attr : function (attr, value){
					var self = this;
					for(var i=0,len=self.length;i<len;i++){
						var elem = self[i];
						if(arguments.length == 1){
							if(typeof attr == 'object'){
								for(var propName in attr)elem.setAttribute(propName, attr[propName]);
							}
							else{
								return elem.attributes[attr] ? elem.attributes[attr].nodeValue : undefined;
							}
						}
						else if(arguments.length == 2){
							elem.setAttribute(attr, value);
						}
						return self;
					}
				},
				/**
				 * 删除自定义属性
				 * @name  removeAttr
				 * @param {string}		attr 	属性
				 * @return ignore
				 */
				removeAttr : function(attr){   
					var self = this;
					return self.each(function (){
						this.removeAttribute(attr);
						return self;
					});
				},
				/**
				 * DOM碎片整理
				 * @name safeFrag
				 * @function
				 * @return object
				 */
				safeFrag : function (dom){
					/* 碎片存储 */
					var oDiv        = document.createElement('div'),
					oFragment   = document.createDocumentFragment();
					switch(typeof dom){
						case 'string':
							oDiv.innerHTML  = dom;
							while(oDiv.childNodes[0])oFragment.appendChild(oDiv.childNodes[0]);
							break;
						default:
							oFragment.appendChild(dom);
					}
					oDiv = null;
					return oFragment;
				},				
				/**
				 * DOM节点外部前插入
				 * @name before
				 * @param {object || string}    dom         插入元素
				 * @function
				 */
				before : function (dom){
					var self = this;
					return self.each(function (){
						this.parentNode.insertBefore(self.safeFrag(dom), this);
						return self;
					});
				},
				/**
				 * DOM节点外部后插入
				 * @name after
				 * @param {object || string}    dom         插入元素
				 * @function
				 */
				after : function (dom){
					var self = this;
					return self.each(function (){
						this.parentNode.insertBefore(self.safeFrag(dom), this.nextSibling);
						return self;
					});
				},
				/**
				 * DOM节点内部前插入
				 * @name prepend
				 * @param {object || string}    dom         插入元素
				 * @function
				 */
				prepend : function (dom){
					var self = this;
					return self.each(function (){
						this.insertBefore(self.safeFrag(dom), this.firstChild);
						return self;
					});
				},
				/**
				 * DOM节点内部后插入
				 * @name append
				 * @param {object || string}    selector    选择器
				 * @param {object || string}    dom         插入元素
				 * @function
				 */
				append : function (dom){
					var self = this;
					return self.each(function (){
						this.appendChild(self.safeFrag(dom));
						return self;
					});
				},
				/**
				 * 获取当前元素left值
				 * @name  left
				 * @return number
				 */
				left : function (){
					return this[0] ? this[0].getBoundingClientRect().left : 0;
				},
				/**
				 * 获取当前元素top值
				 * @name  top
				 * @return number
				 */
				top : function (){
					return this[0] ? this[0].getBoundingClientRect().top + (document.body.scrollTop || document.documentElement.scrollTop) : 0;
				},
				/**
				 * 获取当前元素right值
				 * @name  right
				 * @return number
				 */
				right : function (){
					return this[0] ? document.documentElement.clinetWidth + (document.body.scrollLeft || document.documentElement.scrollLeft) - this[0].getBoundingClientRect().right : 0;
				},
				/**
				 * 获取当前元素bottom值
				 * @name  bottom
				 * @return number
				 */
				bottom : function (selector){
					return this[0] ? document.documentElement.clinetHeight + (document.body.scrollTop || document.documentElement.scrollTop) - this[0].getBoundingClientRect().bottom : 0;
				},
				/**
				 * 获取滚动条Top位置
				 * @name scrollTop
				 * @return number
				 */
				scrollTop : function (){
					return document.documentElement.scrollTop || document.body.scrollTop;
				},
				/**
				 * 获取滚动条高度
				 * @name scrollHeight
				 * @return number
				 */
				scrollHeight : function (){
					return document.documentElement.scrollHeight || document.body.scrollHeight;
				},
				/**
				 * 获取DOM元素高
				 * @name height
				 * @return number
				 */
				height : function (){
					return this[0] ? this[0].offsetHeight : 0;
				},
				/**
				 * 获取DOM元素宽
				 * @name width
				 * @return number
				 */
				width : function (){
					return this[0] ? this[0].offsetWidth : 0;
				},
				/**
				 * 设置html
				 * @name html
				 * @param {string}              html        输入内容
				 * @return number
				 */
				html : function (html){
					var self = this;
					if(html){
						self.each(function (){
							this.innerHTML = html;
						});
						return self;
					}
					else{
						return this[0].innerHTML;
					}
				},
				css : function (property, value){
					var self 	= this,
						css 	= '',
						vendors	= ['-webkit-', '-o-', '-ms-' ,'-moz-'];
					/**
					 * 获取样式属性
					 * @name    styleProperty
					 * @param   {string} style      样式版本
					 * @param   {string} attr       样式属性
					 * @function
					 * @return string
					 */
					function styleProperty(style, attr){
						/* 判断CSS2样式 */
						if(attr in style){
							return attr;
						}
						/* 判断CSS3样式 */
						var capAttr = attr.charAt(0).toUpperCase() + attr.slice(1),
							cssAttr = attr,
							i       = vendors.length;
						while(i--){
							if(arguments[arguments.length-1]){
								attr = vendors[i] + capAttr.toLowerCase();
							}
							else{
								var vendor = vendors[i].replace(/-/g, '');
								attr = vendor.charAt(0).toUpperCase() + vendor.slice(1) + capAttr;
							}
							if (attr in style){
								return attr;
							}
						}
						return cssAttr;
					};
					if(arguments.length < 2 && typeof property == 'string'){
						return self[0] && (self[0].style[styleProperty(self[0].style, property)] || getComputedStyle(self[0], '').getPropertyValue(styleProperty(self[0].style, property)))
					}
					if(typeof property == 'string'){
						if (!value && value !== 0){
							self.each(function(){
								this.style.removeProperty(styleProperty(this.style, property, true));
							});
						}
						else{
							css = styleProperty(self[0].style, property, true) + ":" + maybeAddPx(property, value);
						}
					}
					else{
						for (key in property){
							if (!property[key] && property[key] !== 0){
								self.each(function(){this.style.removeProperty(styleProperty(this.style, key, true))});
							}
							else{
								css += styleProperty(self[0].style, key, true) + ':' + maybeAddPx(key, property[key]) + ';';
							}
						}
					}
					return self.each(function(){this.style.cssText += ';' + css});
				},
				/**
				 * 显示dom
				 * @name show
				 * @return object
				 */
				show : function (){
					return this.each(function (){
						return $(this).css({"display" : "block"});
					});
				},
				/**
				 * 隐藏dom
				 * @name hide
				 * @return object
				 */
				hide : function (){
					return this.each(function (){
						return $(this).css({"display" : "none"});
					});
				},			
				isDomEvent : function (obj, evtType){
					if(("on" + evtType).toLowerCase() in obj){
						return evtType;
					}
					else if($.fn.transitionend && (evtType.toLowerCase() === 'transitionend' || evtType === $.fn.transitionend)){
						return $.fn.transitionend();
					}
					return false;
				},
				bindDomEvent : function (obj, evtType, handler){
					var oldHandler;
					if(obj.addEventListener){
						obj.addEventListener(evtType, handler, false);
					}
					else{
						evtType = evtType.toLowerCase();
						if(obj.attachEvent){
							obj.attachEvent('on' + evtType, handler);
						}
						else{
							oldHandler = obj['on' + evtType];
							obj['on' + evtType] = function(){
								if(oldHandler){
									oldHandler.apply(this, arguments);
								}
								return handler.apply(this, arguments);
							}
						}
					}
				},
				unbindDomEvent : function (obj, evtType, handler){
					if(obj.removeEventListener){
						obj.removeEventListener(evtType, handler, false);
					}
					else{
						evtType = evtType.toLowerCase();
						if(obj.detachEvent){
							obj.detachEvent('on' + evtType, handler);
						}
						else{
							// TODO: 对特定handler的去除
							obj['on' + evtType] = null;
						}
					}
				},
				on : function (evtType, handler){					
					var self = this;
					return self.each(function (){
						var tmpEvtType,elem = this;
						//单函数绑定多事件
						if($.fn.isString(evtType) && evtType.indexOf(" ") > 0){
							evtType = evtType.split(" ");
							for(var i = evtType.length; i--;){
								self.on(evtType[i], handler);
							}
							return self;
						}
						//多函数绑定
			            if($.fn.isArray(handler)){
			                for(var i=handler.length;i--;){
			                    self.on(evtType, handler[i]);
			                }
			                return self;
			            }			            
						//json事件绑定
						if($.fn.isObject(evtType)){
							for(var eName in evtType){
								self.on(eName, evtType[eName]);
							}
							return self;
						}
						//dom 原始事件
						if(tmpEvtType = self.isDomEvent(elem,evtType)){
							evtType = tmpEvtType;
							self.bindDomEvent(elem, evtType, handler);
							return self;
						}

						//dom 原始事件
						if(elem && (tmpEvtType = self.isDomEvent(elem,evtType))){
							evtType = tmpEvtType;
							self.bindDomEvent(elem, evtType, handler);
							return self;
						}
						//tap事件
						if(self.customEvent[evtType]){
							self.customEvent[evtType](elem, handler, self);
							return;
						}
						//自定义事件
						if(!elem.events){
							elem.events = {};
						}
						if(!elem.events[evtType]){
							elem.events[evtType] = [];
						}
						elem.events[evtType].push(handler);
						return self;
					});
				},
				unon : function (evtType, handler){
					var self = this;
					return self.each(function (){
						var tmpEvtType,elem = this;
						//单函数绑定多事件
						if($.fn.isString(evtType) && evtType.indexOf(" ") > 0){
							evtType = evtType.split(" ");
							for(var i = evtType.length; i--;){
								self.unon(evtType[i], handler);
							}
							return self;
						}
						//多函数绑定
			            if($.fn.isArray(handler)){
			                for(var i=handler.length;i--;){
			                    self.unon(evtType, handler[i]);
			                }
			                return self;
			            }			            
						//json事件绑定
						if($.fn.isObject(evtType)){
							for(var eName in evtType){
								self.on(eName, evtType[eName]);
							}
							return self;
						}
						//dom 原始事件
						if(tmpEvtType = self.isDomEvent(elem,evtType)){
							evtType = tmpEvtType;
							self.unbindDomEvent(elem, evtType, handler);
							return self;
						}

						//dom 原始事件
						if(elem && (tmpEvtType = self.isDomEvent(elem,evtType))){
							evtType = tmpEvtType;
							self.unbindDomEvent(elem, evtType, handler);
							return self;
						}
						//tap事件
						if(self.customEvent[evtType]){
							//self.customEvent.tap(elem, handler, self);
							return;
						}

						if(!evtType) {
							elem.events={}; 
							return;
						}

						if(elem.events){
							if(!handler) {
								elem.events[evtType]=[];
								return;
							}
							if(elem.events[evtType]){
								var evtArr = elem.events[evtType];
								for(var i = evtArr.length;i--;){
									if(evtArr[i]==handler){
										evtArr.splice(i,1);
										return;
									}
								}
							}
						}
						return self;
					});
				},				
				customEvent : {
					off : function (elem, evtType, handler, self){

					},
					tap : function (elem, handler, self){
						//按下松开之间的移动距离小于20，认为发生了tap
						var TAP_DISTANCE	= 20,
							//双击之间最大耗时
							DOUBLE_TAP_TIME = 300,
							pt_pos,
							ct_pos,
							pt_up_pos,
							pt_up_time,
							//evtType,
							startEvtHandler = function(e){
								var touches = e.touches;
								if(!touches || touches.length == 1){//鼠标点击或者单指点击
									ct_pos = pt_pos = getTouchPos(e);
								}
							},
							moveEvtHandler	= function(e){
								e.preventDefault();
								ct_pos = getTouchPos(e);
							},
							endEvtHandler	= function(e){
								var now 	= Date.now(),
									dist 	= getDist(ct_pos , pt_pos),
									up_dist = getDist(ct_pos , pt_up_pos);

								if(dist < TAP_DISTANCE){
									if(pt_up_time && now - pt_up_time < DOUBLE_TAP_TIME && up_dist < TAP_DISTANCE){
										evtType = "doubletap";
									}
									else{
										evtType = "tap";
									}
									handler.call(elem,{
										target	: e.target,
										oriEvt	: e,
										type 	: evtType
									});
								}
								pt_up_pos	= ct_pos;
								pt_up_time	= now;
							}
						self.on(startEvt,	startEvtHandler);
						self.on(moveEvt,	moveEvtHandler);
						self.on(endEvt,		endEvtHandler);
						return self;
					}
				},
				/**
				 * 运动框架
				 * @name animate				 
				 * @param {object}      json        运动属性
				 * @param {function}    callback    回调函数
				 * @param {number}      times       运行时间
				 * @param {string}      tween       运动形式
				 * @param {array}      	delay    	延迟时间
				 * return object
				 */
				animate : function (json, callback, times, tween, delay){
					var self 	= this,
						arg 	= arguments;
					
					self.each(function (){						
						var elem 	= this,
							ajson	= {};						
						for(var len=arg.length,i=0;i<len;i++){
							if($.fn.isArray(arg[i]) && arg[i].length > 0){
								ajson['array'] = arg[i][0]
							}
							else{
								ajson[ (!isNaN(arg[i]) && typeof arg[i] != 'boolean') ? typeof parseInt(arg[i]) : typeof arg[i] ] = arg[i];
							}
						}
						
						elem.queue = [];
						elem.queue.push(ajson);
						(elem.timer === undefined || elem.timer === null) && self.Animate(elem);
					});
				},
				/**
				 * 运动处理函数
				 * @name Animate
				 * @param {string}      elem    当前元素
				 * return object
				 */
				Animate : function (elem){
					var self	= this,
						data	= elem.queue.shift(),
						json 	= data['object']	|| {},
						is3d 	= data['boolean']	|| false,
						times	= data['number']	|| 400,
						tween	= data['string']	|| 'linear',
						delay	= data['array']		|| 0;
					
					var aStr =  "all"
						+ ' ' + times + 'ms ' 
						+ tween
						+ ' ' + delay + 'ms';
					$(elem).css('transition', aStr);
					setTimeout(function (){
						$(elem).css(json);
					},0);
					$(elem).on('TransitionEnd', function (){
						$(elem).css('transition', '');
						typeof callback == 'function' && callback();
					});
					/* 运动回调函数 */
					function callback(){
						clearInterval(elem.timer);
						elem.timer = setTimeout(function (){
							(data['function'] || function(){}).call(elem);
							elem.timer = null;
						},30);
					}
					return;
				},
			}
		}
	};
/**
 + ---------------------------------------- +
 + 可以使用touch外用方法接口
 + ---------------------------------------- +
 + Date: 2014-04-17
 + ---------------------------------------- +
**/
Touch.extend = Touch.fn.extend = function (obj, prop){
	if(!prop){
		prop = obj,
		obj  = this;
	}
	for(var propName in prop)obj[propName] = prop[propName];
}

Touch.extend({
	/**
	 * ajax
	 * @name ajax
	 * @param {string}				url			请求地址
	 * @param {function | object}	fnSucc		回调函数
	 * @param {function | string}	callback	回调函数
	 * @param {string}				type		请求类型
	 * @function
	 */
	ajax : function(url, fnSucc, callback, type)
	{
		/* 判断请求类型 */
		if(typeof fnSucc == 'function')type = type || callback;
		if(type.toUpperCase() != 'GETJSON'){
			/* 声明AJAX */
			var oAjax	= null;
			var oData	= '';
			window.XMLHttpRequest ? oAjax = new XMLHttpRequest() : oAjax = new ActiveXObject('Msxml2.XMLHTTP') || new ActiveXObject('Microsoft.XMLHTTP');
			
			/* AJAX请求 */
			oAjax.open(type, url, true);
			
			/* post方法设置服务器响应请求 */
			if(type.toUpperCase()=='POST'){
				oAjax.setRequestHeader("Content-Type","application/x-www-form-urlencoded;");
				for(var propName in fnSucc)oData += (oData == '' ? '' : '&') + propName + '=' + fnSucc[propName];
			}
			type.toUpperCase()=='POST' ? oAjax.send(oData) : oAjax.send();
			oAjax.onreadystatechange = function(){
				if(oAjax.readyState == 4)
				oAjax.status == 200 && (typeof fnSucc == 'function' ? fnSucc(oAjax.responseText) : callback(oAjax.responseText));
			};
		}
		else{
			/* getjson跨域方法 */
			var funName		= 'JSONP'+parseInt(Math.random() * 100000);
	
			//funName			= 'rundemo2';//testing
	
			var url			= url.replace('touch?',funName);
			window[funName]	= fnSucc;
			
			/* 创建跨域标签 */
			var oGetJson	= document.createElement('script');
			oGetJson.src	= url;
			document.body.appendChild(oGetJson);
			
			/* 判断IE */
			document.all?
			oGetJson.onreadystatechange = function(){document.body.removeChild(oGetJson);}:
			oGetJson.onload				= function(){document.body.removeChild(oGetJson);};
		}
	}
});
Touch.fn.init.prototype = Touch.fn;
window.Touch = Touch
'$' in window || (window.$ = Touch)
})(window, undefined);