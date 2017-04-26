var slide = {
	$ : function(objName){if(document.getElementById){return eval('document.getElementById("'+objName+'")')}else{return eval('document.all.'+objName)}},
	isIE : navigator.appVersion.indexOf("MSIE")!=-1?true:false,

	//Event
	addEvent : function(obj,eventType,func){if(obj.attachEvent){obj.attachEvent("on" + eventType,func);}else{obj.addEventListener(eventType,func,false)}},
	delEvent : function(obj,eventType,func){
		if(obj.detachEvent){obj.detachEvent("on" + eventType,func)}else{obj.removeEventListener(eventType,func,false)}
	},
	//Cookie
	readCookie : function(l){var i="",I=l+"=";if(document.cookie.length>0){var offset=document.cookie.indexOf(I);if(offset!=-1){offset+=I.length;var end=document.cookie.indexOf(";",offset);if(end==-1)end=document.cookie.length;i=document.cookie.substring(offset,end)}};return i},

	writeCookie : function(O,o,l,I){var i="",c="";if(l!=null){i=new Date((new Date).getTime()+l*3600000);i="; expires="+i.toGMTString()};if(I!=null){c=";domain="+I};document.cookie=O+"="+escape(o)+i+c},
	//Style
	readStyle:function(i,I){if(i.style[I]){return i.style[I]}else if(i.currentStyle){return i.currentStyle[I]}else if(document.defaultView&&document.defaultView.getComputedStyle){var l=document.defaultView.getComputedStyle(i,null);return l.getPropertyValue(I)}else{return null}},
	absPosition : function(obj,parentObj){ //λ��
		var left = 0;
		var top = 0;
		var tempObj = obj;
		try{
			do{
				left += tempObj.offsetLeft;
				top += tempObj.offsetTop;
				tempObj = tempObj.offsetParent;
			}while(tempObj.id!=document.body && tempObj.id!=document.documentElement && tempObj != parentObj && tempObj!= null);
		}catch(e){};
		return {left:left,top:top};
	},
	_getJsData : function(url,callback){
		var _script = document.createElement("script");
		_script.type = "text/javascript";
		_script.language = "javascript";

		_script[_script.onreadystatechange === null ? "onreadystatechange" : "onload"] = function(){
			if(this.onreadystatechange){
				if(this.readyState != "complete" && this.readyState != "loaded") {return;}
			};
			if(callback){callback()};
			setTimeout(function(){_script.parentNode.removeChild(_script)},1000);
		};
		_script.src = url;
		
		document.getElementsByTagName("head")[0].appendChild(_script);
		
	},
	style : {
		setOpacity : function(obj,opacity){
			if(typeof(obj.style.opacity) != 'undefined'){
				obj.style.opacity = opacity;
			}else{
				obj.style.filter = 'Alpha(Opacity=' + (opacity*100) + ')';
			};
		}
	},
	extend : {
		show : function(obj,timeLimit){
			if(slide.readStyle(obj,'display') === 'none'){
				obj.style.display = 'block';
			};
			slide.style.setOpacity(obj,0);
			if(!timeLimit){
				timeLimit = 200;
			};
			var opacity = 0,step = timeLimit / 20;
			clearTimeout(obj._extend_show_timeOut);
			obj._extend_show_timeOut = setTimeout(function(){
				if(opacity >= 1){
					return;
				};
				opacity += 1/step;
				slide.style.setOpacity(obj,opacity);
				obj._extend_show_timeOut = setTimeout(arguments.callee,20);
				
			},20);
		},
		hide : function(obj,timeLimit){
			if(!timeLimit){
				timeLimit = 200;
			};
			slide.style.setOpacity(obj,1);
			var opacity = 1,step = timeLimit / 20;
			clearTimeout(obj._extend_show_timeOut);
			obj._extend_show_timeOut = setTimeout(function(){
				if(opacity <= 0){
					obj.style.display = 'none';
					slide.style.setOpacity(obj,1);
					return;
				};
				opacity -= 1/step;
				slide.style.setOpacity(obj,opacity);
				obj._extend_show_timeOut = setTimeout(arguments.callee,20);
				
			},20);
		},
		actPX : function(obj,key,start,end,speed,endFn,u){
			if(typeof(u) == 'undefined'){u = 'px'};
			clearTimeout(obj['_extend_actPX' + key.replace(/\-\.\=/,'_') + '_timeOut']);
			if(start > end){
				speed = - Math.abs(speed);
			}else{
				speed = Math.abs(speed);
			};
			var now = start;
			var length = end - start;
			obj['_extend_actPX' + key.replace(/\-\.\=/,'_') + '_timeOut'] = setTimeout(function(){
				now += speed;
				

				var space = end - now;
				if(start < end){
					if(space < length/3){
						speed = Math.ceil(space/3);
					};
					if(space <= 0){ //end
						obj[key] = end + u;
						if(endFn){endFn()};
						return;
					};
				}else{
					if(space > length/3){
						speed = Math.floor(space/3);
					};
					if(space >= 0){ //end
						obj[key] = end + u;
						if(endFn){endFn()};
						return;
					};
				};
				
				obj[key] = now + u;
				obj['_extend_actPX' + key.replace(/\-\.\=/,'_') + '_timeOut'] = setTimeout(arguments.callee,20);
				
			},20);
		}
	}
};

slide.Step = function(){
	this.stepIndex = 0; //��ǰ����
	this.classBase = 'step_'; //class����
	this.limit = 3; //������
	this.stepTime = 20; //��ʱ��
	this.element = null; //html����
	this._timeObj = null; //setInterval����
	this._type = '+'; //������
};
slide.Step.prototype.action = function(type){
	if(!this.element){return};
	var tempThis = this;
	if(type=='+'){
		this._type = '+';
	}else{
		this._type = '-';
	};
	clearInterval(this._timeObj);
	this._timeObj = setInterval(function(){tempThis.nextStep()},this.stepTime);
};
slide.Step.prototype.nextStep = function(){
	if(this._type == '+'){
		this.stepIndex ++;
	}else{
		this.stepIndex --;
	};
	
	if(this.stepIndex <= 0){
		clearInterval(this._timeObj);
		this.stepIndex = 0;
		if(this._type == '-'){
			if(this.onfirst){this.onfirst()};
		};
	};
	if(this.stepIndex >= this.limit){
		clearInterval(this._timeObj);
		this.stepIndex = this.limit;
		if(this._type == '+'){
			if(this.onlast){this.onlast()};
		};
	};
	this.element.className = this.classBase + this.stepIndex;
	
	if(this.onstep){this.onstep()};
};

var getData = {
	initF:false,
	curUrl : "",
	fillData : function(flagPre){	//��������
		epidiascope.clearData();
		var images = slide_data.images;
		var flashPic = "",flashTxt = "";
		for(var i=0;i<images.length;i++){
			var title,pic,intro,previewPic,imageId;
			pic = images[i].imageUrl;
			title = images[i].title;
			intro = images[i].intro;
			previewPic = images[i].previewUrl;
			imageId = images[i].id;
	
			epidiascope.add({
				src : pic,
				title : title,
				text : intro,
				lowsrc_b : previewPic,
				lowsrc_s : previewPic,
				id : imageId
			});
			
			//for flash
			if(flashPic != ""){flashPic += "|"};
			flashPic += pic;
			if(flashTxt != ""){flashTxt += "|"};
			if(!getData.initF){
				//��һ�γ�ʼ��ʱ��Ҫת�������ַ�
				flashTxt += encodeURIComponent(title) + "#����" + encodeURIComponent(intro.replace(/<.*?>/g,''));
			}else{
				flashTxt += title + "#����" + intro.replace(/<.*?>/g,'');
			}
		}
		
		//����ҳ���ϵ�����ͼ������һͼ������һͼ��
		var efpPrePic = slide.$("efpPrePic");
		var efpPreTxt = slide.$("efpPreTxt");
		var efpNextPic = slide.$("efpNextPic");
		var efpNextTxt = slide.$("efpNextTxt");
		efpPrePic.getElementsByTagName("a")[0].href = slide_data.prev_album.url;
		efpPrePic.getElementsByTagName("img")[0].src = slide_data.prev_album.previewUrl;
		efpPrePic.getElementsByTagName("img")[0].alt = slide_data.prev_album.title;
		efpPrePic.getElementsByTagName("img")[0].title = slide_data.prev_album.title;
		efpPreTxt.getElementsByTagName("a")[0].href = slide_data.prev_album.url;
		efpPreTxt.getElementsByTagName("a")[0].title = slide_data.prev_album.title;
		efpNextPic.getElementsByTagName("a")[0].href = slide_data.next_album.url;
		efpNextPic.getElementsByTagName("img")[0].src = slide_data.next_album.previewUrl;
		efpNextPic.getElementsByTagName("img")[0].alt = slide_data.next_album.title;
		efpNextPic.getElementsByTagName("img")[0].title = slide_data.next_album.title;
		efpNextTxt.getElementsByTagName("a")[0].href = slide_data.next_album.url;
		efpNextTxt.getElementsByTagName("a")[0].title = slide_data.next_album.title;
		
		document.title = slide_data.slide.title;//�����ĵ�����
		
		if(!getData.initF){
			//ֻ��Ҫ��ʼ��һ��epidiascope��flash
			epidiascope.init();
			fullFlash(flashTxt,flashPic);
			getData.initF = true;
		}else{
			epidiascope.initNot();
			//��flash������
			getFullScreenFlash().onMethod(flashPic+"---"+flashTxt);
			if(flagPre){
				//�������ص�����һ�飬����������һ��
				epidiascope.select(images.length-1);
			}else{
				epidiascope.select(0);
			}			
		}
	}
};

var epidiascope = {
	picTitleId : "d_picTit",
	picMemoId : "d_picIntro",
	picTimeId : 'd_picTime',
	picListId : "efpPicListCont",
	BigPicId : "d_BigPic",
	picArrLeftId : "efpLeftArea",
	picArrRightId : "efpRightArea",
	playButtonId : "ecbPlay",
	statusId : "ecpPlayStatus",
	mainBoxId : "efpBigPic",
	PVUrl_a : null,
	PVUrl_m : null,
	repetition : false, //ѭ������
	prefetch : false, //Ԥ��ͼƬ
	autoPlay : false, //�Զ�����
	mode : 'player', //ģʽ player|list
	autoPlayTimeObj : null,
	timeSpeed : 5,
	maxWidth : 948,
	filmstrips : [],
	prefetchImg : [],
	commNum : [],
	selectedIndex : -1,
	previousPicList : {},
	nextPicList : {},
	loadTime : 0,
	add : function(s){
		this.filmstrips.push(s);
		if(this.prefetch){ //Ԥ��ͼƬ
			var tempImg = new Image();
			tempImg.src = s.src;
			this.prefetchImg.push(tempImg);
		};
	},
	clearData : function(){
		this.selectedIndex = -1;
		this.filmstrips = [];
	},
	init : function(){
		var tempThis = this;
		var tempWidth = 0;
		if(this.filmstrips.length * 110 < slide.$(this.picListId).offsetWidth){
			tempWidth = Math.round(slide.$(this.picListId).offsetWidth / 2 - this.filmstrips.length * 110/2);
		};
		var commKey = "";
		var tempHTML = '<div style="width:32760px;padding-left:' + tempWidth + 'px;">',i;
		for(i=0;i<this.filmstrips.length;i++){
			//���б�
			tempHTML += '<div class="pic" id="slide_' + i + '"><table cellspacing="0"><tr><td class="picCont"><table cellspacing="0"><tr><td class="pb_01"></td><td class="pb_02"></td><td class="pb_03"></td></tr><tr><td class="pb_04"></td><td><a href="javascript:epidiascope.select(' + i + ');" onclick="this.blur();"><img src="' + this.filmstrips[i].lowsrc_s + '" width="50" height="34" alt="' + this.filmstrips[i].title + '" oncontextmenu="event.returnValue=false;return false;" /></a></td><td class="pb_05"></td></tr><tr><td class="pb_06"></td><td class="pb_07"></td><td class="pb_08"></td></tr></table></td></tr></table></div>';
		};
		
		slide.$(this.picListId).innerHTML = tempHTML + "</div>";

		//
		slide.$(this.picArrLeftId).onclick = function(){epidiascope.previous();epidiascope.stop();};
		slide.$(this.picArrRightId).onclick = function(){epidiascope.next();epidiascope.stop();};
		
		if(window.location.href.indexOf('2010.')!=-1){
			this.autoPlay = true;
			slide.$(this.picArrRightId).onclick = function(){epidiascope.next();epidiascope.play();};
		};
		
		//��ť
		this.buttonNext = new epidiascope.Button('ecbNext'); //��һҳ
		this.buttonPre = new epidiascope.Button('ecbPre'); //��һҳ
		this.buttonPlay = new epidiascope.Button('ecbPlay'); //������ͣ
		this.buttonMode = new epidiascope.Button('ecbMode'); //ģʽ�л�
		this.buttonFullScreen = new epidiascope.Button('ecbFullScreen','2'); //ȫ��
		
		this.buttonSpeed = new epidiascope.Button('ecbSpeed'); //�ٶ�
		this.buttonModeReturn = new epidiascope.Button('ecbModeReturn'); //ģʽ�л�
		
		this.buttonPre.element.onclick = function(){epidiascope.previous();epidiascope.stop();};
		this.buttonNext.element.onclick = function(){epidiascope.next();epidiascope.stop();};
		this.buttonMode.element.onclick = function(){epidiascope.setMode('list');};
		this.buttonModeReturn.element.onclick = function(){epidiascope.setMode('player');};
		this.buttonFullScreen.element.onclick = function(){epidiascope.fullScreen.chk()};
		//
		this.BigImgBox = slide.$(this.BigPicId);

		//��ֹ�Ҽ�
		this.BigImgBox.oncontextmenu = function(e){
			e = e?e:event;
			e.returnValue=false;
			return false;
		};
		
		this._imgLoad = function(){
			if(epidiascope.maxWidth == 0 ){return};
			if(this.width > epidiascope.maxWidth){
				this.width = epidiascope.maxWidth;
				//this.height = Math.round(epidiascope.maxWidth * this.height / this.width);
			};
			if(this.width < 948){
				slide.$('d_BigPic').style.paddingTop = "15px";
				this.style.border = "1px solid #000";
			}else{
				slide.$('d_BigPic').style.paddingTop = "0px";
				this.style.border = "none";
				this.style.borderBottom = "1px solid #e5e6e6";
			};
			
			//����loadingͼƬ
			clearTimeout(tempThis._hideBgTimeObj);
			slide.$('d_BigPic').className = '';
		};
		
		this._preLoad = function(){
			var tempTime = new Date().getTime();
			var deltaTime = tempTime - epidiascope.loadTime;
			if(deltaTime > 5000){
				var tempLoadImage = new Image().src = this.src;
			}
		}
		
		if(this.filmstrips && this.filmstrips[0]){
			this.createImg(this.filmstrips[0].src);
		}
		
		var page;
		var imgId = window.location.search.match(/img=(\d+)/i);
		if(imgId){			
			imgId = imgId[1];
			page = 0;
			for(var i = 0, len = this.filmstrips.length; i<len; i++){
				if(parseInt(this.filmstrips[i]['id']) == parseInt(imgId)){
					page = i;
					break;
				}
			}
		}else{		
			page = window.location.hash.match(/p=(\d+)/i);			
			if(page){
				page = page[1] - 1;
				if(page<0 || page >= this.filmstrips.length){
					page = 0;
				};
			}else{
				page = 0;
			};
		}
		this.select(page);
		
		if(!slide.isIE){
			this.BigImgBox.style.position = 'relative';
			this.BigImgBox.style.overflow = "hidden";
			
		}else{
			
			clearInterval(this._ieButHeiTimeObj);
			this._ieButHeiTimeObj = setInterval(function(){tempThis.setPicButtonHeight()},300);
		};
		//������һͼ��
		var nextPics = slide.$('efpNextGroup').getElementsByTagName('a');
		slide.$('nextPicsBut').href = nextPics[0].href;
		
		if(this.autoPlay){this.play()}else{this.stop()};
		
		
		if(this.onstart){this.onstart()};
		//iPad���ݴ���
		this.iPad.init();
	},
	initNot : function(){
		//���ǵ�һ�εĳ�ʼ����ȥ��һЩ��ť�¼����ظ����ء�
		var tempThis = this;
		var tempWidth = 0;
		if(this.filmstrips.length * 110 < slide.$(this.picListId).offsetWidth){
			tempWidth = Math.round(slide.$(this.picListId).offsetWidth / 2 - this.filmstrips.length * 110/2);
		};
		var commKey = "";
		var tempHTML = '<div style="width:32760px;padding-left:' + tempWidth + 'px;">',i;
		for(i=0;i<this.filmstrips.length;i++){
			//���б�
			tempHTML += '<div class="pic" id="slide_' + i + '"><table cellspacing="0"><tr><td class="picCont"><table cellspacing="0"><tr><td class="pb_01"></td><td class="pb_02"></td><td class="pb_03"></td></tr><tr><td class="pb_04"></td><td><a href="javascript:epidiascope.select(' + i + ');" onclick="this.blur();"><img src="' + this.filmstrips[i].lowsrc_s + '"  width="50" height="34" alt="' + this.filmstrips[i].title + '"  onload="DrawImage(this);" oncontextmenu="event.returnValue=false;return false;" /></a></td><td class="pb_05"></td></tr><tr><td class="pb_06"></td><td class="pb_07"></td><td class="pb_08"></td></tr></table></td></tr></table></div>';
		};
		
		slide.$(this.picListId).innerHTML = tempHTML + "</div>";
		this.createImg(this.filmstrips[0].src);

		var page;
		var imgId = window.location.search.match(/img=(\d+)/i);
		if(imgId){			
			imgId = imgId[1];
			page = 0;
			for(var i = 0, len = this.filmstrips.length; i<len; i++){
				if(parseInt(this.filmstrips[i]['id']) == parseInt(imgId)){
					page = i;
					break;
				}
			}
		}else{		
			page = window.location.hash.match(/p=(\d+)/i);			
			if(page){
				page = page[1] - 1;
				if(page<0 || page >= this.filmstrips.length){
					page = 0;
				};
			}else{
				page = 0;
			};
		}
		this.select(page);
		setTimeout(function(){tempThis.picList.foucsTo(page + 1)},500);
		
		if(!slide.isIE){
			this.BigImgBox.style.position = 'relative';
			this.BigImgBox.style.overflow = "hidden";
			
		}else{
			clearInterval(this._ieButHeiTimeObj);
			this._ieButHeiTimeObj = setInterval(function(){tempThis.setPicButtonHeight()},300);
		};
		
		//�б�ģʽ��ʼ����־
		this.listInitStatus = false;
		
		//������һͼ��
		var nextPics = slide.$('efpNextGroup').getElementsByTagName('a');
		slide.$('nextPicsBut').href = nextPics[0].href;
		if(this.autoPlay){this.play()}else{this.stop()};
		if(this.onstart){this.onstart()};
	},
	readTry : 0,
	createImg : function(src){
		if(this.ImgObj1){
			this.BigImgBox.removeChild(this.ImgObj1);
		};
		this.ImgObj1 = document.createElement("img");
		this.ImgObj1.onmousedown = function(){return false};
		this.ImgObj1.galleryImg = false;
		this.ImgObj1.onload = this._imgLoad;
		if(src){
			this.ImgObj1.src = src;
		};
		this.BigImgBox.appendChild(this.ImgObj1);
	},
	select : function(num,type){
		var tempThis = this;
		if(this.endSelect.status == 1){
			this.endSelect.close();
		};
		if(num == this.selectedIndex){return};
		var i;
		if(num >= this.filmstrips.length || num < 0){return};
		
		slide.$(this.picTitleId).innerHTML = this.filmstrips[num].title;
		slide.$(this.picMemoId).innerHTML = this.filmstrips[num].text;
		slide.$(this.picTimeId).innerHTML = this.filmstrips[num].date;
		
		//����loadingͼƬ1����
		slide.$('d_BigPic').className = '';
		clearTimeout(this._hideBgTimeObj);
		this._hideBgTimeObj = setTimeout("slide.$('d_BigPic').className='loading'",500);
	
		this.createImg();
		
		this.ImgObj1.style.opacity = 0;
		
		if(this._timeOut){
			for(i=0;i<this._timeOut.length;i++){
				clearTimeout(this._timeOut[i]);
			};
		};
		this._timeOut = [];

		if (slide.isIE) {
		    var isIE6 = !window.XMLHttpRequest; 
			this.ImgObj1.src = '/sitefiles/assets/s.gif';
			if (!isIE6 && !!this.ImgObj1.filters) this.ImgObj1.filters[0].Apply();
			
			this.ImgObj1.src = this.filmstrips[num].src;
			if (!isIE6 && !!this.ImgObj1.filters) this.ImgObj1.filters[0].Play();
		}else{
			this.ImgObj1.src = this.filmstrips[num].src;
			for(i = 0;i <= 3;i ++){
				this._timeOut[i] = setTimeout("epidiascope.ImgObj1.style.opacity = " + i * 0.3,i * 100);
			};
			this._timeOut[i] = setTimeout("epidiascope.ImgObj1.style.opacity = 1;",4 * 100);
		};
		
		if(slide.$("slide_" + this.selectedIndex)){slide.$("slide_" + this.selectedIndex).className = "pic"};
		slide.$("slide_" + num).className = "picOn";
		this.selectedIndex = num;

		this.picList.foucsTo(num + 1); //����

		slide.$("total").innerHTML = '(<span class="cC00">'+(num + 1) + "</span>/" + this.filmstrips.length + ')';
		if(this.autoPlay){this.play()};
		
		//Ԥ����һ��
		if(!this.prefetch && num < this.filmstrips.length - 1){ //δԤ��ȫ��ͼƬ
			this.reLoad = new Image();
			this.reLoad.src = this.filmstrips[num + 1].src;
			this.loadTime = new Date().getTime();
			this.reLoad.onload = this._preLoad;
		};
	},
	setPicButtonHeight : function(){
		slide.$(this.picArrLeftId).style.height = slide.$(this.picArrRightId).style.height = slide.$(this.picArrLeftId).parentNode.offsetHeight + 'px';
	},
	setPageInfo : function(num){		
		window.location.hash = "p="+Math.round(num+1); 
	},
	next : function(type){
		var tempNum = this.selectedIndex + 1;
		if(tempNum >= this.filmstrips.length){
			if(this.repetition){ //ѭ������
				tempNum = 0;
			}else{
				this.endSelect.open(); //ѡ��
				return;
			};
		};
		//�Զ����ţ��ж�����ͼƬ�Ƿ�����
		if(type=="auto"){
			var testImg = new Image();
			testImg.src = this.filmstrips[tempNum].src;
			if(!testImg.complete){
				return;
			};
		};
		this.select(tempNum,type);
	},
	previous : function(){
		var tempNum = this.selectedIndex - 1;
		if(tempNum < 0){ //ѭ������
			if(this.repetition){
				tempNum = this.filmstrips.length - 1
			}else{
				return;
			};
		};
		this.select(tempNum);
	},
	play : function(){
		clearInterval(this.autoPlayTimeObj);
		this.autoPlayTimeObj = setInterval("epidiascope.next('auto')",this.timeSpeed*1000);
		slide.$(this.playButtonId).onclick = function(){epidiascope.stop()};
		slide.$(this.statusId).className = "stop";
		slide.$(this.statusId).title = "��ͣ";
		this.autoPlay = true;
	},
	stop : function(){
		clearInterval(this.autoPlayTimeObj);
		slide.$(this.playButtonId).onclick = function(){epidiascope.play();epidiascope.next('auto');};
		slide.$(this.statusId).className = "play";
		slide.$(this.statusId).title = "����";
		this.autoPlay = false;
	},
	
	rePlay : function(){ //���²���
		if(this.endSelect.status == 1){this.endSelect.close()};
		this.autoPlay = true;
		this.select(0);
	},
	downloadPic : function(){ //����ͼƬ
		var thisFilmstrip = this.filmstrips[this.selectedIndex];

	},
	setMode : function(mode){ //�л�ģʽ
		this.speedBar.close();
		if(this.endSelect.status == 1){
			this.endSelect.close();
		};
		if(mode == 'list'){
			if(!this.listInitStatus){
				this.listInit();
			};
			
			this.buttonSpeed.hide();
			this.buttonFullScreen.hide();
			this.buttonPlay.hide();
			this.buttonNext.hide();
			this.buttonPre.hide();
			slide.$('ecbLine').style.visibility = 'hidden';
			this.buttonMode.element.style.display = 'none';
			this.buttonModeReturn.element.style.display = 'block';
			this.buttonModeReturn.rePosi();
			
			this.stop();
			this.mode = 'list';
			
			this.listSelect(this.selectedIndex);
			
			slide.$('eFramePic').style.display = 'none';
			slide.$('ePicList').style.display = 'block';
			
			this.listView();
		}else{
			window.scroll(0,0);
			this.buttonSpeed.show();
			this.buttonFullScreen.show();
			this.buttonPlay.show();
			this.buttonNext.show();
			this.buttonPre.show();
			slide.$('ecbLine').style.visibility = 'visible';
			this.buttonMode.element.className = '';
			
			this.buttonMode.element.style.display = 'block';
			this.buttonModeReturn.element.style.display = 'none';
			
			this.mode = 'player';
			
			slide.$('eFramePic').style.display = 'block';
			slide.$('ePicList').style.display = 'none';
			
			//this.select(this.listSelectedIndex);
		};
	},
	switchMode : function(){
		if(this.mode == 'list'){
			this.setMode('player');
		}else{
			this.setMode('list');
		};
	},
	listData : null,
	listFrameId : 'ePicList',
	listSelectedIndex : null,
	listSelect : function(num){
		if(num<0 || num >= this.filmstrips.length){return};
		if(this.listSelectedIndex !== null){
			if(slide.$('picList_' + this.listSelectedIndex)){slide.$('picList_' + this.listSelectedIndex).className = 'picBox'};
		};
		this.listSelectedIndex = num;
		if(slide.$('picList_' + this.listSelectedIndex)){slide.$('picList_' + this.listSelectedIndex).className = 'picBox selected'};
	},
	listInit : function(){
		var tempThis = this;
		var html = '';
		for(var i=0;i<this.filmstrips.length;i++){
			html += '<div class="picBox" id="picList_'+ i +'" onmousemove="epidiascope.listSelect('+i+')" onclick="epidiascope.select(epidiascope.listSelectedIndex);epidiascope.setMode(\'player\');"><table cellspacing="0"><tr><td><img src="' + this.filmstrips[i].lowsrc_b + '" alt="" /></td></tr></table><h3>' + this.filmstrips[i].title + '</h3></div>';
		};
		slide.$(this.listFrameId).innerHTML = html;
		this.listInitStatus = true;
	},
	listRowSize : 4, //ÿ�и���
	listView : function(){
		var element = slide.$('picList_' + this.listSelectedIndex);
		
		var bodyHeight = document.documentElement.clientHeight==0?document.body.clientHeight:document.documentElement.clientHeight;
		var scrollTop = document.documentElement.scrollTop==0?document.body.scrollTop:document.documentElement.scrollTop;
		
		var posi = slide.absPosition(element,document.documentElement);
		if((posi.top + (element.offsetHeight * 0.3)) < scrollTop || (posi.top + (element.offsetHeight * 0.7)) > scrollTop + bodyHeight){
			window.scroll(0,posi.top - Math.round((bodyHeight - element.offsetHeight)/2));
		};
	},
	listMoveUp : function(){
		var newNum = this.listSelectedIndex - this.listRowSize;
		if(newNum<0){
			return;
		};
		this.listSelect(newNum);
		this.listView();
	},
	listMoveDown : function(){
		var newNum = this.listSelectedIndex + this.listRowSize;
		if(newNum>=this.filmstrips.length){
			nweNum = this.filmstrips.length - 1;
		};
		this.listSelect(newNum);
		this.listView();
	},
	listMoveLeft : function(){
		var newNum = this.listSelectedIndex - 1;
		if(newNum<0){
			return;
		};
		this.listSelect(newNum);
		this.listView();
	},
	listMoveRight : function(){
		var newNum = this.listSelectedIndex + 1;
		if(newNum>=this.filmstrips.length){
			return;
		};
		this.listSelect(newNum);
		this.listView();
	}
};
epidiascope.fullScreen = {
	status : 'window',
	chk : function(){ //�����Ƿ�֧��flashȫ��
		var flash_i = false;
		if (navigator.plugins) {
			for (var i=0; i < navigator.plugins.length; i++) {
				if (navigator.plugins[i].name.toLowerCase().indexOf("shockwave flash") >= 0) {
					flash_i = true;
				};
			};
			if(flash_i == false){
				this.full();
			};
		};
	},
	full : function(){
		if(this.status == 'window'){
			this.status = 'fullScreen';
			document.body.className = 'statusFull';
		}else{
			this.status = 'window';
			document.body.className = '';
		};
	}
};
		
epidiascope.speedBar = { //�ٶ���
	boxId : "SpeedBox", //����id
	contId : "SpeedCont", //����id
	slideId : "SpeedSlide", //����id
	slideButtonId : "SpeedNonius", //����id
	infoId : "ecbSpeedInfo", //��Ϣid
	grades : 10, //�ȼ���
	grade : 5, //�ȼ�
	_slideHeight : 112, //�����߶�
	_slideButtonHeight : 9, //�����߶�
	_baseTop : 4, //top����
	_marginTop : 0,
	_mouseDisparity : 0,
	_showStep : 0,
	_showType : 'close',
	_showTimeObj : null,
	init : function(){
		var tempThis = this;
		this._marginTop = Math.round(this._slideHeight/this.grades * (this.grade - 1));
		
		slide.$(this.slideButtonId).style.top = this._marginTop + this._baseTop + "px";
		slide.$(this.infoId).innerHTML = this.grade + "��";
		
		//����Ч��
		this.step = new slide.Step();
		this.step.element = slide.$(this.contId);
		this.step.limit = 6;
		this.step.stepTime = 20;
		this.step.classBase = 'speedStep_';
		this.step.onfirst = function(){
			epidiascope.buttonSpeed.setStatus('ok');
			slide.$(epidiascope.speedBar.boxId).style.display = 'none';
		};
		
		slide.$(this.slideId).onselectstart = function(){return false};
		slide.$(this.slideButtonId).onmousedown = function(e){tempThis.mouseDown(e);return false};
		slide.$(this.slideId).onmousedown = function(e){tempThis.slideClick(e);return false};
		
		epidiascope.buttonSpeed.element.onmousedown = function(){tempThis.show();return false;};
		epidiascope.buttonSpeed.element.onselectstart = function(){return false};
	},
	show : function(){
		if(this._showType == 'close'){
			this.open();
		}else{
			this.close();
		};
	},
	open : function(){
		var tempThis = this;
		this._showType = 'open';
		var tempMouseDown = document.onmousedown;
		var mousedown = function(e){
			e = window.event?event:e;
			if(e.stopPropagation){ //��ֹð��
				e.stopPropagation();
			}else{
				window.event.cancelBubble = true;
			};
			var eventObj = e.target?e.target:e.srcElement;
			
			while(eventObj != slide.$(tempThis.boxId) && eventObj != epidiascope.buttonSpeed.element){
				if(eventObj.parentNode){
					eventObj = eventObj.parentNode;
				}else{
					break;
				};
			};
			if(eventObj == slide.$(tempThis.boxId) || eventObj == epidiascope.buttonSpeed.element){
				return;
			}else{
				tempThis.close();
			};
			slide.delEvent(document,'mousedown',mousedown);
		};
		slide.addEvent(document,'mousedown',mousedown);
		
		epidiascope.buttonSpeed.setStatus('down');
		slide.$(this.boxId).style.display = 'block';
		
		this.step.action('+');
	},
	close : function(){
		var tempThis = this;
		this._showType = 'close';
		epidiascope.buttonSpeed.setStatus('ok');
		this.step.action('-');
	},
	slideClick : function(e){
		e = window.event?event:e;
		var Y = e.layerY?e.layerY:e.offsetY;
		if(!Y){return};
		
		this._marginTop = Y - Math.round(this._slideButtonHeight/2);
		if(this._marginTop<0){this._marginTop=0};
		this.grade = Math.round(this._marginTop/(this._slideHeight/this.grades) + 1);
		slide.$(this.slideButtonId).style.top = this._marginTop + this._baseTop + "px";
		slide.$(this.infoId).innerHTML = this.grade + "��";
		
		if(this.onend){this.onend()};
	},
	setGrade : function(num){
		this.grade = num;
		epidiascope.timeSpeed = this.grade;
		this._marginTop = Math.round(this._slideHeight/this.grades * (this.grade - 1));
		
		slide.$(this.slideButtonId).style.top = this._marginTop + this._baseTop + "px";
		slide.$(this.infoId).innerHTML = this.grade + "��";
		slide.writeCookie("eSp",this.grade,720);
	},
	mouseDown : function(e){
		var tempThis = this;
		e = window.event?window.event:e;
		this._mouseDisparity = (e.pageY?e.pageY:e.clientY) - this._marginTop;
		document.onmousemove = function(e){tempThis.mouseOver(e)};
		document.onmouseup = function(){tempThis.mouseEnd()};
	},
	mouseOver : function(e){
		e = window.event?window.event:e;
		this._marginTop = (e.pageY?e.pageY:e.clientY) - this._mouseDisparity;
		if(this._marginTop > (this._slideHeight - this._slideButtonHeight)){this._marginTop = this._slideHeight - this._slideButtonHeight};
		if(this._marginTop < 0){this._marginTop = 0;};
		slide.$(this.slideButtonId).style.top = this._marginTop + this._baseTop + "px";

		this.grade = Math.round(this._marginTop/(this._slideHeight/this.grades) + 1);

		if(this.onmover){this.onmover()};
	},
	mouseEnd : function(){
		if(this.onend){this.onend()};
		
		document.onmousemove = null;
		document.onmouseup = null;
	},
	onmover : function(){
		slide.$(this.infoId).innerHTML = this.grade + "��";
	},
	onend : function(){
		slide.writeCookie("eSp",this.grade,720);
		epidiascope.timeSpeed = this.grade;
		if(epidiascope.autoPlay){epidiascope.play()};
	}
};

epidiascope.picList = { //�б�����
	leftArrId : "efpListLeftArr",
	rightArrId : "efpListRightArr",
	picListId : "efpPicListCont",
	timeoutObj : null,
	pageWidth : 110,
	totalWidth : 0,
	offsetWidth : 0,
	lock : false,
	init : function(){
		slide.$(this.rightArrId).onmousedown = function(){epidiascope.picList.leftMouseDown()};
		slide.$(this.rightArrId).onmouseout = function(){epidiascope.picList.leftEnd("out");this.className='';};
		slide.$(this.rightArrId).onmouseup = function(){epidiascope.picList.leftEnd("up")};
		slide.$(this.leftArrId).onmousedown = function(){epidiascope.picList.rightMouseDown()};
		slide.$(this.leftArrId).onmouseout = function(){epidiascope.picList.rightEnd("out");this.className='';};
		slide.$(this.leftArrId).onmouseup = function(){epidiascope.picList.rightEnd("up")};
		this.totalWidth = epidiascope.filmstrips.length * this.pageWidth;
		this.offsetWidth = slide.$(this.picListId).offsetWidth;

	},
	leftMouseDown : function(){
		if(this.lock){return};
		this.lock = true;
		this.timeoutObj = setInterval("epidiascope.picList.moveLeft()",10);
	},
	rightMouseDown : function(){
		if(this.lock){return};
		this.lock = true;
		this.timeoutObj = setInterval("epidiascope.picList.moveRight()",10);
	},
	moveLeft : function(){
		if(slide.$(this.picListId).scrollLeft + 10 > this.totalWidth - this.offsetWidth){
			slide.$(this.picListId).scrollLeft = this.totalWidth - this.offsetWidth;
			this.leftEnd();
		}else{
			slide.$(this.picListId).scrollLeft += 10;
		};
	},
	moveRight : function(){
		slide.$(this.picListId).scrollLeft -= 10;
		if(slide.$(this.picListId).scrollLeft == 0){this.rightEnd()};
	},
	leftEnd : function(type){
		if(type=="out"){if(!this.lock){return}};
		clearInterval(this.timeoutObj);
		this.lock = false;
		this.move(30);
	},
	rightEnd : function(type){
		if(type=="out"){if(!this.lock){return}};
		clearInterval(this.timeoutObj);
		this.lock = false;
		this.move(-30);
	},
	foucsTo : function(num){
		if(this.lock){return;};
		this.lock = true;

		var _moveWidth = Math.round(num * this.pageWidth - this.offsetWidth / 2) - 33;
		
		_moveWidth -= slide.$(this.picListId).scrollLeft;

		if(slide.$(this.picListId).scrollLeft + _moveWidth < 0){
			_moveWidth = - slide.$(this.picListId).scrollLeft;
		};
		if(slide.$(this.picListId).scrollLeft + _moveWidth >= this.totalWidth - this.offsetWidth){
			_moveWidth = this.totalWidth - this.offsetWidth - slide.$(this.picListId).scrollLeft;
		};
		
		this.move(_moveWidth);
	},
	move : function(num){
		var thisMove = num/4;
		if(Math.abs(thisMove)<1 && thisMove!=0){
			thisMove = (thisMove>=0?1:-1)*1;
		}else{
			thisMove = Math.round(thisMove);
		};

		var temp = slide.$(this.picListId).scrollLeft + thisMove;
		if(temp <= 0){slide.$(this.picListId).scrollLeft = 0;this.lock = false;return;}
		if(temp >= this.totalWidth - this.offsetWidth){slide.$(this.picListId).scrollLeft = this.totalWidth - this.offsetWidth;this.lock = false;return;}
		slide.$(this.picListId).scrollLeft += thisMove;
		num -= thisMove;
		if(Math.abs(num) <= 1){this.lock = false;return;}else{
			setTimeout("epidiascope.picList.move(" + num + ")",10)
		}
	}
};
//���̿���
epidiascope.keyboard = {
	_timeObj : null,
	status : 'up',
	init : function(){
		var tempThis = this;
		slide.addEvent(document,'keydown',function(e){tempThis.keyDown(e)});
		slide.addEvent(document,'keyup',function(e){tempThis.keyUp(e)});
		
		this.step = new slide.Step();
		this.step.element = slide.$('efpClew');
		this.step.limit = 5;
		this.step.stepTime = 30;
		this.step.classBase = 'efpClewStep_';
		
		if(!this.closeObj){
			this.closeObj = document.createElement('span');
			this.closeObj.style.display = 'block';
			this.closeObj.id = 'efpClewClose';
			slide.$('efpClew').appendChild(this.closeObj);
			
			this.closeObj.onclick = function(){tempThis.clewClose()};
		};
		
		//��ʾ����
		this.clewNum = parseInt(slide.readCookie('eCn'));
		if(isNaN(this.clewNum)){this.clewNum = 0};
		if(this.clewNum<5){
			//this.clewNum ++;
			//slide.writeCookie('eCn',this.clewNum,24*7);
			this.clewOpen();
		};
		
	},
	clewClose : function(){
		this.step.action('-');
		slide.writeCookie('eCn',6,24*7);
	},
	clewOpen : function(){
		this.step.action('+');
	},
	keyDown : function(e){
		if(this.status == 'down'){return};
		this.status = 'down';
		e = window.event?event:e;
		var obj = e.target?e.target:e.srcElement;
		if(obj.tagName == 'INPUT' || obj.tagName == 'SELECT' || obj.tagName == 'TEXTAREA'){
			if(e.stopPropagation){ //��ֹð��
				e.stopPropagation();
			}else{
				window.event.cancelBubble = true;
			};
			return;
		};
		
		var stopKey = false; //�Ƿ���ֹ����
		if(epidiascope.mode == 'list'){ //�б�ģʽ
			if(e.keyCode == 40){
				epidiascope.listMoveDown();
				stopKey = true;
			};
			if(e.keyCode == 37){
				epidiascope.listMoveLeft();
				stopKey = true;
			};
			if(e.keyCode == 38){
				epidiascope.listMoveUp();
				stopKey = true;
			};
			if(e.keyCode == 39){
				epidiascope.listMoveRight();
				stopKey = true;
			};
			if(e.keyCode == 13){
				epidiascope.setMode('player');
				epidiascope.select(epidiascope.listSelectedIndex);
				stopKey = true;
			};
			
		}else{ //Ĭ��ģʽ
			if(e.keyCode == 39){
				epidiascope.next();
				stopKey = true;
				this.clewClose();
			};
			if(e.keyCode == 37){
				epidiascope.previous();
				stopKey = true;
				this.clewClose();
			};
		};
		
		if(e.keyCode == 9){
			epidiascope.switchMode();
			stopKey = true;
		};
			
		if(stopKey === true){
			if(e.preventDefault){
				e.preventDefault();
			}else{
				e.returnValue=false;
			};
		};
	},
	keyUp : function(){
		this.status = 'up';
	}
};

//����ѡ��
epidiascope.endSelect = {
	endSelectId : "endSelect",
	closeId : "endSelClose",
	rePlayButId : "rePlayBut",
	status : 0, //1:open  0:close
	open : function(){
		this.status = 1;
		slide.$(this.endSelectId).style.display = "block";

		slide.$(this.endSelectId).style.left = Math.round((slide.$(epidiascope.mainBoxId).offsetWidth - slide.$(this.endSelectId).offsetWidth)/2) + "px";
		slide.$(this.endSelectId).style.top = Math.round((slide.$(epidiascope.mainBoxId).offsetHeight - slide.$(this.endSelectId).offsetHeight)/2) + "px";
		epidiascope.stop();
		slide.$(epidiascope.playButtonId).onclick = function(){epidiascope.rePlay()};
		slide.$(this.closeId).onclick = function(){epidiascope.endSelect.close()};
		slide.$(this.rePlayButId).onclick = function(){epidiascope.rePlay()};
	},
	close : function(){
		this.status = 0;
		//slide.$(epidiascope.playButtonId).onclick = function(){epidiascope.play()};
		slide.$(this.endSelectId).style.display = "none";
	}
};
epidiascope.onstart = function(){
	try{document.execCommand('BackgroundImageCache', false, true);}catch(e){};

	//�ٶ���
	epidiascope.speedBar.grade = parseInt(slide.readCookie("eSp"));
	if(isNaN(epidiascope.speedBar.grade)){epidiascope.speedBar.grade = 5};
	epidiascope.speedBar.init();
	epidiascope.speedBar.onend();

	//ͼƬ�б�����
	epidiascope.picList.init();
	
	//��������
	epidiascope.keyboard.init();
};
//��ť���캯��
epidiascope.Button = function(id,classNameNum){
	this.status = 'ok';
	this.id = id;
	this.classNameNum = classNameNum;
	this.init();
};
epidiascope.Button.prototype.init = function(){
	if(!slide.$(this.id)){return};
	var tempThis = this;
	this.element = slide.$(this.id);
	
	if(this.element.offsetWidth == 43){
		this.classNameNum = '1';
	};
	if(!this.classNameNum){
		this.classNameNum = '';
	};
	this.mouseStatus = 'out';
	
	this.bgDiv = document.createElement('div');
	this.bgDiv.className = 'buttonBg' + this.classNameNum;
	this.element.parentNode.style.position = 'relative';
	this.element.style.position = 'relative';
	this.element.style.zIndex = '5';
	this.element.parentNode.appendChild(this.bgDiv);
	this.bgDiv.style.top = this.element.offsetTop - 6 + 'px';
	this.bgDiv.style.left = this.element.offsetLeft - 6 + 'px';
	
	
	//����Ч��
	this.step = new slide.Step();
	this.step.element = this.bgDiv;
	this.step.limit = 3;
	this.step.stepTime = 30;
	this.step.classBase = 'buttonBg' + this.classNameNum + ' bBg' + this.classNameNum + 'S_';
	
	slide.addEvent(this.element,'mouseover',function(){tempThis.mouseover()});
	slide.addEvent(this.element,'mouseout',function(){tempThis.mouseout()});
	slide.addEvent(this.element,'mousedown',function(){tempThis.mousedown()});
	slide.addEvent(this.element,'mouseup',function(){tempThis.mouseup()});
};
epidiascope.Button.prototype.rePosi = function(){
	this.bgDiv.style.top = this.element.offsetTop - 6 + 'px';
	this.bgDiv.style.left = this.element.offsetLeft - 6 + 'px';
};
epidiascope.Button.prototype.mouseover = function(){
	this.mouseStatus = 'in';
	if(this.status != 'down'){
		this.element.className = "hover";
		this.step.action('+');
	};
};
epidiascope.Button.prototype.mouseout = function(){
	this.mouseStatus = 'out';
	if(this.status != 'down'){
		this.element.className = "";
		this.step.action('-');
	};
};
epidiascope.Button.prototype.mouseup = function(){
	if(this.status == 'down'){return;}
	this.element.className = "hover";
};
epidiascope.Button.prototype.mousedown = function(){
	if(this.status == 'down'){return;}
	this.element.className = "active";
};
epidiascope.Button.prototype.setStatus = function(status){
	switch(status){
		case 'ok':
			this.status = 'ok';
			this.element.className = "";
			if(this.mouseStatus == 'in'){
				this.step.action('+');
			}else{
				this.step.action('-');
			};
			break;
		case 'down':
			this.status = 'down';
			this.step.action('-');
			this.element.className = "active";
			break;
	};
};
epidiascope.Button.prototype.hide = function(){
	this.element.style.visibility = 'hidden';
	this.bgDiv.style.visibility = 'hidden';
};
epidiascope.Button.prototype.show = function(){
	this.element.style.visibility = 'visible';

	this.bgDiv.style.visibility = 'visible';
};
epidiascope.iPad = {
	x : 0,
	y : 0,
	lastX : 0,
	lastY : 0,
	status : 'ok',
	init : function(){
		if(!(/\((iPhone|iPad|iPod)/i).test(navigator.userAgent)){ //��֧�ִ���
			return;	
		};
		slide.addEvent(window,'load',function(){setTimeout('window.scrollTo(0,78)'),500});
			      
		slide.$('efpClew').style.backgroundImage = 'url(http://www.slideimg.cn/dy/deco/2010/0513/e_ipad_m_02.png)';
		var tempThis = this;
		slide.addEvent(slide.$('efpBigPic'),'touchstart',function(e){tempThis._touchstart(e)});
		slide.addEvent(slide.$('efpBigPic'),'touchmove',function(e){tempThis._touchmove(e)});
		slide.addEvent(slide.$('efpBigPic'),'touchend',function(e){tempThis._touchend(e)});
	},
	_touchstart : function(e){
		
		this.x = e.touches[0].pageX;
		this.scrollX = window.pageXOffset;
		this.scrollY = window.pageYOffset; //�����ж�ҳ���Ƿ�����
	},
	_touchmove : function(e){
		if(e.touches.length > 1){ //���㴥��
			this.status = 'ok';
			return;
		};
		this.lastX = e.touches[0].pageX;
		var cX = this.x - this.lastX;
		
		if(cX<0){//��һҳ��ֹ����
			if(epidiascope.selectedIndex == 0){
				return;
			};
		};
		
		if(this.status == 'ok'){
			if(this.scrollY == window.pageYOffset && this.scrollX == window.pageXOffset && Math.abs(cX)>50){ //��������
				if(cX>0){//����һҳ��ֹ����
					if(epidiascope.selectedIndex == epidiascope.filmstrips.length - 1){
						if(epidiascope.endSelect.status == 0){
							epidiascope.endSelect.open();
						};
						return;
					};
				};
				
				this.status = 'touch';
				slide.$('efpBigPic').style.textAlign = 'left';
			}else{
				return;
			};
		};
		
		epidiascope.ImgObj1.style.marginLeft = - cX + Math.round((950 - epidiascope.ImgObj1.offsetWidth)/2) + 'px';
		e.preventDefault();
	},
	_touchend : function(e){
		if(this.status != 'touch'){return};
		this.status = 'ok';
		var cX = this.x - this.lastX;
		
		slide.extend.actPX(epidiascope.ImgObj1.style,'marginLeft',epidiascope.ImgObj1.offsetLeft,cX>0?-951:951,200,function(){
			epidiascope.ImgObj1.style.marginLeft = 0;
			slide.$('efpBigPic').style.textAlign = 'center';
			epidiascope.ImgObj1.style.paddingLeft = 0;
			if(cX<0){
				epidiascope.previous();
			}else{
				epidiascope.next();
			};
			//epidiascope.keyboard.clewClose();
		});
	}
	
}
// -------------------------------------------------------------------------------------

function DrawImage(ImgD,iwidth,iheight){
	var image=new Image();
	if(!iwidth)iwidth = 90;
	if(!iheight)iheight = 90; //���������߶ȣ������ȴ�������ֵʱ�ȱ�����С
	image.src=ImgD.src;
	if(image.width>0 && image.height>0){
		var flag=true;
		if(image.width/image.height>= iwidth/iheight){
			if(image.width>iwidth){ 
				ImgD.width=iwidth;
				ImgD.height=(image.height*iwidth)/image.width;
			}else{
				ImgD.width=image.width; 
				ImgD.height=image.height;
			}
		}else{
			if(image.height>iheight){ 
				ImgD.height=iheight;
				ImgD.width=(image.width*iheight)/image.height; 
			}else{
				ImgD.width=image.width; 
				ImgD.height=image.height;
			}
		}
	}
};

//ģ��Select mengjia 2008.12.30
function DivSelect(O,l,I){var C=this;C.id=O;C.divId=l;C.divClassName=I;C.selectObj=slide.$(C.id);if(!C.selectObj){return};var o=C;C.status="close";C.parentObj=C.selectObj.parentNode;while(slide.readStyle(C.parentObj,"display")!="block"){if(C.parentObj.parentNode){C.parentObj=C.parentObj.parentNode}else{break}};C.parentObj.style.position="relative";C.selectObjWidth=C.selectObj.offsetWidth;C.selectObjHeight=C.selectObj.offsetHeight;C.selectPosition=slide.absPosition(C.selectObj,C.parentObj);C.selectObj.style.visibility="hidden";C.divObj=document.createElement("div");C.divObj.id=C.divId;if(C.divClassName){C.divObj.className=C.divClassName};C.parentObj.appendChild(C.divObj);C.divObj.style.width=C.selectObjWidth+"px";C.divObj.style.position="absolute";C.divObj.style.left=C.selectPosition.left+"px";C.divObj.style.top=C.selectPosition.top+"px";C.divObj.onclick=function(){o.click()};C.divObj_count=document.createElement("div");C.divObj.appendChild(C.divObj_count);C.divObj_count.className="ds_cont";C.divObj_title=document.createElement("div");C.divObj_count.appendChild(C.divObj_title);C.divObj_title.className="ds_title";C.divObj_button=document.createElement("div");C.divObj_count.appendChild(C.divObj_button);C.divObj_button.className="ds_button";C.divObj_list=document.createElement("div");C.divObj.appendChild(C.divObj_list);C.divObj_list.className="ds_list";C.divObj_list.style.display="none";C.divObj_listCont=document.createElement("div");C.divObj_list.appendChild(C.divObj_listCont);C.divObj_listCont.className="dsl_cont";C.list=[];var i;for(var c=0;c<C.selectObj.options.length;c++){i=document.createElement("p");C.list.push(i);C.divObj_listCont.appendChild(i);i.innerHTML=C.selectObj.options[c].innerHTML;if(C.selectObj.selectedIndex==c){C.divObj_title.innerHTML=i.innerHTML};i.onmouseover=function(){this.className="selected"};i.onmouseout=function(){this.className=""};i.onclick=function(){o.select(this.innerHTML)}};C.select=function(i){var l=this;for(var I=0;I<l.selectObj.options.length;I++){if(l.selectObj.options[I].innerHTML==i){l.selectObj.selectedIndex=I;if(l.selectObj.onchange){l.selectObj.onchange()};l.divObj_title.innerHTML=i;break}}};C.clickClose=function(I){var i=I.target?I.target:event.srcElement;do{if(i==o.divObj){return};if(i.tagName=="BODY"){break};i=i.parentNode}while(i.parentNode);o.close()};C.open=function(){var i=this;i.divObj_list.style.display="block";i.status="open";slide.addEvent(document,"click",i.clickClose)};C.close=function(){var i=this;i.divObj_list.style.display="none";i.status="close";slide.delEvent(document,"click",i.clickClose)};C.click=function(){var i=this;if(i.status=="open"){i.close()}else{i.open()}}};