function Ad_AddEvent(obj,evtType,func,cap){
    cap=cap||false;
	if(obj.addEventListener){
	    obj.addEventListener(evtType,func,cap);
		return true;
	}else if(obj.attachEvent){
        if(cap){
	        obj.setCapture();
	        return true;
	    }else{
		    return obj.attachEvent("on" + evtType,func);
		}
	}else{
		return false;
    }
}
function Ad_GetPageScroll(){
    var xScroll,yScroll;
	if (self.pageXOffset) {
		xScroll = self.pageXOffset;
	} else if (document.documentElement && document.documentElement.scrollLeft){
		xScroll = document.documentElement.scrollLeft;
	} else if (document.body) {
		xScroll = document.body.scrollLeft;
	}
	if (self.pageYOffset) {
		yScroll = self.pageYOffset;
	} else if (document.documentElement && document.documentElement.scrollTop){
		yScroll = document.documentElement.scrollTop;
	} else if (document.body) {
		yScroll = document.body.scrollTop;
	}
	arrayPageScroll = new Array(xScroll,yScroll);
	return arrayPageScroll;
}
function Ad_GetPageSize(){
    var xScroll, yScroll;
    if (window.innerHeight && window.scrollMaxY) {	
        xScroll = document.body.scrollWidth;
        yScroll = window.innerHeight + window.scrollMaxY;
    } else if (document.body.scrollHeight > document.body.offsetHeight){
        xScroll = document.body.scrollWidth;
        yScroll = document.body.scrollHeight;
    } else {
        xScroll = document.body.offsetWidth;
        yScroll = document.body.offsetHeight;
    }
    var windowWidth, windowHeight;
    if (self.innerHeight) {
        windowWidth = self.innerWidth;
        windowHeight = self.innerHeight;
    } else if (document.documentElement && document.documentElement.clientHeight) {
        windowWidth = document.documentElement.clientWidth;
        windowHeight = document.documentElement.clientHeight;
    } else if (document.body) {
        windowWidth = document.body.clientWidth;
        windowHeight = document.body.clientHeight;
    }	
    if(yScroll < windowHeight){
        pageHeight = windowHeight;
    } else { 
        pageHeight = yScroll;
    }
    if(xScroll < windowWidth){	
        pageWidth = windowWidth;
    } else {
        pageWidth = xScroll;
    }
    arrayPageSize = new Array(pageWidth,pageHeight,windowWidth,windowHeight) 
    return arrayPageSize;
}

var AdMoveConfig=new Object();
AdMoveConfig.IsInitialized=false;
AdMoveConfig.ScrollX=0;
AdMoveConfig.ScrollY=0;
AdMoveConfig.MoveWidth=0;
AdMoveConfig.MoveHeight=0;
AdMoveConfig.Resize=function(){
    var winsize=Ad_GetPageSize();
    AdMoveConfig.MoveWidth=winsize[2];
    AdMoveConfig.MoveHeight=winsize[3];
    AdMoveConfig.Scroll();
}
AdMoveConfig.Scroll=function(){
    var winscroll=Ad_GetPageScroll();
    AdMoveConfig.ScrollX=winscroll[0];
    AdMoveConfig.ScrollY=winscroll[1];
}
Ad_AddEvent(window,"resize",AdMoveConfig.Resize);
Ad_AddEvent(window,"scroll",AdMoveConfig.Scroll);
function Ad_Move(id){
    if(!AdMoveConfig.IsInitialized){
        AdMoveConfig.Resize();
        AdMoveConfig.IsInitialized=true;
    }
    var obj=document.getElementById(id);
    obj.style.position="absolute";
    var W=AdMoveConfig.MoveWidth-obj.offsetWidth;
    var H=AdMoveConfig.MoveHeight-obj.offsetHeight;
    var x = W*Math.random(),y = H*Math.random();
    var rad=(Math.random()+1)*Math.PI/6;
    var kx=Math.sin(rad),ky=Math.cos(rad);
    var dirx = (Math.random()<0.5?1:-1), diry = (Math.random()<0.5?1:-1);
    var step = 1;
	var type = 1;
    var interval;
    this.SetLocation=function(vx,vy){x=vx;y=vy;}
	this.SetType=function(ty){type=ty;}
    this.SetDirection=function(vx,vy){dirx=vx;diry=vy;}
    obj.CustomMethod=function(){
        obj.style.left = (x + AdMoveConfig.ScrollX) + "px";
        obj.style.top = (y + AdMoveConfig.ScrollY) + "px";
		if (type == 1){
			rad=(Math.random()+1)*Math.PI/6;
			W=AdMoveConfig.MoveWidth-obj.offsetWidth;
			H=AdMoveConfig.MoveHeight-obj.offsetHeight;
			x = x + step*kx*dirx;
			if (x < 0){dirx = 1;x = 0;kx=Math.sin(rad);ky=Math.cos(rad);} 
			if (x > W){dirx = -1;x = W;kx=Math.sin(rad);ky=Math.cos(rad);}
			y = y + step*ky*diry;
			if (y < 0){diry = 1;y = 0;kx=Math.sin(rad);ky=Math.cos(rad);} 
			if (y > H){diry = -1;y = H;kx=Math.sin(rad);ky=Math.cos(rad);}
		}
    }
    this.Run=function(){
		if (type == 3){
			obj.style.left = (x + AdMoveConfig.ScrollX) + "px";
        	obj.style.top = (y + AdMoveConfig.ScrollY) + "px";
		}else{
        	var delay = 20;
			interval=setInterval(obj.CustomMethod,delay);
			obj.onmouseover=function(){clearInterval(interval);}
			obj.onmouseout=function(){interval=setInterval(obj.CustomMethod, delay);}
		}
    }
}
