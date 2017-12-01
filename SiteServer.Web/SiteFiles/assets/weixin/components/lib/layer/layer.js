/**************************************************************

 @Name: layer v1.7.1 弹层组件开发版
 @Author: 贤心
 @Date: 2014-02-23
 @Blog: http://sentsin.com
 @微博：http://weibo.com/SentsinXu
 @QQ群：218604143(layer组件群2)
 @Copyright: Sentsin Xu(贤心)
 @官网说明：http://sentsin.com/jquery/layer
 @赞助layer: https://me.alipay.com/sentsin
		
 *************************************************************/

;!function(window, undefined){		
"use strict";

var pathType = true, //是否采用自动获取绝对路径。false：将采用下述变量中的配置
pathUrl = 'lily/lib/layer/', //上述变量为false才有效，当前layerjs所在目录(不用填写host，相对站点的根目录即可)。

$, win, ready = {
    hosts: (function(){
        var dk = location.href.match(/\:\d+/);
        dk = dk ? dk[0] : '';
        return 'http://' + document.domain + dk + '/';
    }()),
    
    getPath: function(){
        var js = document.scripts || $('script'), jsPath = js[js.length - 1].src;
        if(pathType){
            return jsPath.substring(0, jsPath.lastIndexOf("/") + 1);
        } else {
            return this.hosts + pathUrl;
        } 
    }
};

//默认内置方法。
window.layer = {
    v : '1.7.1', //版本号
    ie6: !-[1,] && !window.XMLHttpRequest,
    index: 0,
    path: ready.getPath(),
    
    //载入模块
    use: function(module, callback){
        var i = 0, head = $('head')[0];
        var module = module.replace(/\s/g, '');
        var iscss = /\.css$/.test(module);
        var node = document.createElement(iscss ? 'link' : 'script');
        var id = module.replace(/\.|\//g, '');
        if(iscss){
            node.setAttribute('type', 'text/css');
            node.setAttribute('rel', 'stylesheet');
        }
        node.setAttribute((iscss ? 'href' : 'src'), layer.path + module);
        node.setAttribute('id', id);
        if(!$('#'+ id)[0]){
            head.appendChild(node);
        }
        $(node).ready(function(){
            callback && callback();
        });
    },
    
    ready: function(callback){
        return layer.use('skin/layer.css', callback);
    }, 
    
    //普通对话框，类似系统默认的alert()
    alert: function(alertMsg, alertType, alertTit, alertYes){
        return $.layer({
            dialog : {msg : alertMsg, type : alertType, yes : alertYes},
            title : alertTit,
            area: ['auto', 'auto']
        });
    }, 
    
    //询问框，类似系统默认的confirm()
    confirm: function(conMsg, conYes, conTit, conNo){ 
        return $.layer({
            dialog : {msg : conMsg, type : 4, btns : 2, yes : conYes, no : conNo},
            title : conTit
        }); 
    },
    
     //普通消息框，一般用于行为成功后的提醒,默认两秒自动关闭
    msg: function(msgText, msgTime, parme, callback){
        var icon, conf = {title: false, closeBtn: false};
        (msgText == '' || msgText == undefined) && (msgText = '&nbsp;');
        msgTime === undefined && (msgTime = 2);
        if(typeof parme === 'number'){
            icon = parme;
        } else {
            parme = parme || {};
            icon = parme.type;
            conf.success = function(){layer.shift(parme.rate)};
            conf.shade = parme.shade;
        }
        conf.time = msgTime;
        conf.dialog = {msg: msgText, type: icon};
        conf.end = typeof parme === 'function' ? parme : callback;
        return $.layer(conf);	
    }, 
    
    //加载层快捷引用
    load: function(parme, loadIcon){
        if(typeof parme === 'string'){
            return this.msg(parme, 0, 16);
        } else {
            return $.layer({
                time: parme,
                loading: {type : loadIcon},
                bgcolor: !loadIcon ? '' : '#fff',
                shade: [0.1, '#000', !loadIcon ? false : true],
                border :[7,0.3, '#000', (loadIcon === 3 || !loadIcon) ? false : true],
                type : 3,
                title : ['',false],
                closeBtn : [0 , false]
            });
        }
    }, 
    
    //tips层快捷引用
    tips: function(html, follow, parme, maxWidth, guide, style){
        var conf = {type: 4, shade: false, success: function(layerE){
            if(!this.closeBtn){
                layerE.find('.xubox_tips').css({'padding-right': 10});
            }
        }, bgcolor:'', tips:{msg: html, follow: follow}};
        parme = parme || {};
        conf.time = parme.time || parme;
        conf.closeBtn = parme.closeBtn || false
        conf.maxWidth = parme.maxWidth || maxWidth;
        conf.tips.guide = parme.guide || guide;
        conf.tips.style = parme.style || style;
        return $.layer(conf);
    }
};

var Class = function(setings){	
    var config = this.config;
    layer.index++;
    this.index = layer.index;
    this.config = $.extend({} , config , setings);
    this.config.dialog = $.extend({}, config.dialog , setings.dialog);
    this.config.page = $.extend({}, config.page , setings.page);
    this.config.iframe = $.extend({}, config.iframe , setings.iframe);	
    this.config.loading = $.extend({}, config.loading , setings.loading);
    this.config.tips = $.extend({}, config.tips , setings.tips);
    this.creat();
};

Class.pt = Class.prototype;

//默认配置
Class.pt.config = {
    type: 0,
    shade: [0.3 , '#000' , true],
    shadeClose: false,
    fix: true,
    move: ['.xubox_title' , true],
    moveOut: false,
    title: ['信息' , true],
    offset: ['200px' , '50%'],
    area: ['310px' , 'auto'],
    closeBtn: [0 , true],
    time: 0,
    bgcolor: '#fff',
    border: [8 , 0.3 , '#000', true],
    zIndex: 19891014, 
    maxWidth: 400,
    dialog: {btns : 1, btn : ['确定','取消'], type : 3, msg : '', yes : function(index){ layer.close(index);}, no : function(index){ layer.close(index);}
    },
    page: {dom: '#xulayer', html: '', url: ''},
    iframe: {src: 'http://sentsin.com'},
    loading: {type: 0},
    tips: {msg: '', follow: '', guide: 0, isGuide: true, style: ['background-color:#FF9900; color:#fff;', '#FF9900']},
    success: function(layer){}, //创建成功后的回调
    close: function(index){ layer.close(index);}, //右上角关闭回调
    end: function(){} //终极销毁回调
};

Class.pt.type = ['dialog', 'page', 'iframe', 'loading', 'tips'];

//容器
Class.pt.space = function(html){
    var html = html || '', times = this.index, config = this.config, dialog = config.dialog, dom = this.dom,
    ico = dialog.type === -1 ? '' : '<span class="xubox_msg xulayer_png32 xubox_msgico xubox_msgtype' + dialog.type + '"></span>',
    frame = [
    '<div class="xubox_dialog">'+ ico +'<span class="xubox_msg xubox_text" style="'+ (ico ? '' : 'padding-left:20px') +'">' + dialog.msg + '</span></div>',	
    '<div class="xubox_page">'+ html +'</div>',
    '<iframe allowtransparency="true" id="'+ dom.ifr +''+ times +'" name="'+ dom.ifr +''+ times +'" onload="$(this).removeClass(\'xubox_load\');" class="'+ dom.ifr +'" frameborder="0" src="' + config.iframe.src + '"></iframe>',				
    '<span class="xubox_loading xubox_loading_'+ config.loading.type +'"></span>',
    '<div class="xubox_tips" style="'+ config.tips.style[0] +'"><div class="xubox_tipsMsg">'+ config.tips.msg +'</div><i class="layerTipsG"></i></div>'
    ],
    shade = '' , border = '', zIndex = config.zIndex + times,
    shadeStyle = 'z-index:'+ zIndex +'; background-color:'+ config.shade[1] +'; opacity:'+ config.shade[0] +'; filter:alpha(opacity='+ config.shade[0]*100 +');';

    config.shade[2] && (shade = '<div times="'+ times +'" id="xubox_shade' + times + '" class="xubox_shade" style="'+ shadeStyle +'"></div>');	

    config.zIndex = zIndex;
    var title = '', closebtn = '', borderStyle = "z-index:"+ (zIndex-1) +";  background-color: "+ config.border[2] +"; opacity:"+ config.border[1] +"; filter:alpha(opacity="+ config.border[1]*100 +"); top:-"+ config.border[0] +"px; left:-"+ config.border[0] +"px;";

    config.border[3] && (border = '<div id="xubox_border'+ times +'" class="xubox_border" style="'+ borderStyle +'"></div>');
    config.closeBtn[1] && (closebtn = '<a class="xubox_close xulayer_png32 xubox_close' + config.closeBtn[0] +'" href="javascript:;"></a>');
    config.title[1] && (title = '<h2 class="xubox_title"><em>' + config.title[0] + '</em></h2>')
    var boxhtml = '<div times="'+ times +'" showtime="'+ config.time +'" style="z-index:'+ zIndex +'" id="'+ dom.lay +''+ times 
    +'" class="'+ dom.lay +'">'	
    + '<div style="background-color:'+ config.bgcolor +'; z-index:'+ zIndex +'" class="xubox_main">'
    + frame[config.type]
    + title
    + closebtn
    + '<span class="xubox_botton"></span>'
    + '</div>'+ border + '</div>';
    return [shade , boxhtml];
};

//缓存字符
Class.pt.dom = {
    lay: 'xubox_layer',
    ifr: 'xubox_iframe'
};

//创建骨架
Class.pt.creat = function(){
    var that = this , space = '', config = this.config, dialog = config.dialog, title = that.config.title, dom = that.dom, times = that.index;;

    title.constructor === Array || (that.config.title = [title, true]);
    title === false && (that.config.title = [title, false]);

    var page = config.page, body = $("body"), setSpace = function(html){
        var html = html || ''
        space = that.space(html);
        body.append(space[0]);
    };

    switch(config.type){
        case 1:
            if(page.html !== ''){
                setSpace('<div class="xuboxPageHtml">'+ page.html +'</div>');
                body.append(space[1]);
            }else if(page.url !== ''){
                setSpace('<div class="xuboxPageHtml" id="xuboxPageHtml'+ times +'">'+ page.html +'</div>');
                body.append(space[1]);
                $.get(page.url, function(datas){
                    $('#xuboxPageHtml'+ times).html(datas.toString());
                    page.ok && page.ok(datas);
                });
            }else{
                if($(page.dom).parents('.xubox_page').length == 0){
                    setSpace();
                    $(page.dom).show().wrap(space[1]);
                }else{
                    return;	
                }
            }
        break;
        case 2:
            setSpace();
            body.append(space[1]);
        break;
        case 3:
            config.title = ['', false];
            config.area = ['auto', 'auto']; 
            config.closeBtn = ['', false];
            $('.xubox_loading')[0] && layer.close($('.xubox_loading').parents('.'+dom.lay).attr('times'));
            setSpace();
            body.append(space[1]);
        break;
        case 4:
            config.title = ['', false];
            config.area = ['auto', 'auto'];
            config.fix = false;
            config.border = false;
            $('.xubox_tips')[0] && layer.close($('.xubox_tips').parents('.'+dom.lay).attr('times'));
            setSpace();
            body.append(space[1]);
            $('#'+ dom.lay + times).find('.xubox_close').css({top: 6, right: 7});
        break;		
        default: 
            config.title[1] || (config.area = ['auto','auto']);
            $('.xubox_dialog')[0] && layer.close($('.xubox_dialog').parents('.'+dom.lay).attr('times'));
            setSpace();
            body.append(space[1]);
        break;
    };
    
    this.layerS = $('#xubox_shade' + times);
    this.layerB = $('#xubox_border' + times);
    this.layerE = $('#'+ dom.lay + times);

    var layerE = this.layerE;
    this.layerMian = layerE.find('.xubox_main');
    this.layerTitle = layerE.find('.xubox_title');
    this.layerText = layerE.find('.xubox_text');
    this.layerPage = layerE.find('.xubox_page');
    this.layerBtn = layerE.find('.xubox_botton');

    //设置layer面积坐标等数据 
    if(config.offset[1].indexOf("px") != -1){
        var _left = parseInt(config.offset[1]);
    }else{
        if(config.offset[1] == '50%'){
            var _left =  config.offset[1];
        }else{
            var _left =  parseInt(config.offset[1])/100 * win.width();
        }
    };
    layerE.css({left: _left + config.border[0], width: config.area[0], height: config.area[1]});
    config.fix ? layerE.css({top: parseInt(config.offset[0]) + config.border[0]}) : layerE.css({top: parseInt(config.offset[0]) + win.scrollTop() + config.border[0], position: 'absolute'});	

    //配置按钮
    if(config.title[1] && (config.type !== 3 || config.type !== 4)){
        var confbtn = config.type === 0 ? dialog : config;
        confbtn.btn = config.btn || dialog.btn;
        switch(confbtn.btns){
            case 0:
                that.layerBtn.html('').hide();
            break;
            case 1:
                that.layerBtn.html('<a href="javascript:;" class="xubox_yes xubox_botton1">'+ confbtn.btn[0] +'</a>');
            break;
            case 2:
                that.layerBtn.html('<a href="javascript:;" class="xubox_yes xubox_botton2">'+ confbtn.btn[0] +'</a>' + '<a href="javascript:;" class="xubox_no xubox_botton3">'+ confbtn.btn[1] + '</a>');
            break;                
        }
    }

    if(layerE.css('left') === 'auto'){
        layerE.hide();
        setTimeout(function(){
            layerE.show();
            that.set(times);
        }, 500);
    }else{
        that.set(times);
    }
    config.time <= 0 || that.autoclose();
    this.callback();
};

//初始化骨架
Class.pt.set = function(times){
    var that = this, layerE = that.layerE, config = that.config, dialog = config.dialog, page = config.page, loading = config.loading, dom = that.dom;
    that.autoArea(times);
    if(config.title[1]){
        layer.ie6 && that.layerTitle.css({width : layerE.outerWidth()});	
    }else{
        config.type != 4 && layerE.find('.xubox_close').addClass('xubox_close1');
    };

    layerE.attr({'type' :  that.type[config.type]});

    switch(config.type){
        case 1: 	
            layerE.find(page.dom).addClass('layer_pageContent');
            config.shade[2] && layerE.css({zIndex: config.zIndex + 1});
            config.title[1] && that.layerPage.css({top: that.layerTitle.outerHeight()});
        break;
        
        case 2:
            var iframe = layerE.find('.'+ dom.ifr), heg = layerE.height();
            iframe.addClass('xubox_load').css({width: layerE.width()});
            config.title[1] ? iframe.css({top: that.layerTitle.height(), height: heg - that.layerTitle.height()}) : iframe.css({top: 0, height : heg});
            layer.ie6 && iframe.attr('src', config.iframe.src);
        break;
        
        case 3:
        break;
        case 4 :
            var layArea = [0, layerE.outerHeight()], fow = $(config.tips.follow), fowo = {
                width: fow.outerWidth(),
                height: fow.outerHeight(),
                top: fow.offset().top,
                left: fow.offset().left
            }, tipsG = layerE.find('.layerTipsG');

            config.tips.isGuide || tipsG.remove();
            layerE.outerWidth() > config.maxWidth && layerE.width(config.maxWidth);
            
            fowo.tipColor = config.tips.style[1];
            layArea[0] = layerE.outerWidth();
            
            //辨别tips的方位
            fowo.where = [function(){ //上
                fowo.tipLeft = fowo.left;
                fowo.tipTop = fowo.top - layArea[1] - 10;
                tipsG.removeClass('layerTipsB').addClass('layerTipsT').css({'border-right-color': fowo.tipColor});   
            }, function(){ //右
                fowo.tipLeft = fowo.left + fowo.width + 10;
                fowo.tipTop = fowo.top;
                tipsG.removeClass('layerTipsL').addClass('layerTipsR').css({'border-bottom-color': fowo.tipColor}); 
            }, function(){ //下
                fowo.tipLeft = fowo.left;
                fowo.tipTop = fowo.top + fowo.height + 10;
                tipsG.removeClass('layerTipsT').addClass('layerTipsB').css({'border-right-color': fowo.tipColor});
            }, function(){ //左
                fowo.tipLeft = fowo.left - layArea[0] + 10;
                fowo.tipTop = fowo.top;
                tipsG.removeClass('layerTipsR').addClass('layerTipsL').css({'border-bottom-color': fowo.tipColor});
            }];
            fowo.where[config.tips.guide]();
            
            /* 8*2为小三角形占据的空间 */
            if(config.tips.guide === 0){
                fowo.top - (win.scrollTop() + layArea[1] + 8*2) < 0 && fowo.where[2]();
            } else if (config.tips.guide === 1){
                win.width() - (fowo.left + fowo.width + layArea[0] + 8*2) > 0 || fowo.where[3]()
            } else if (config.tips.guide === 2){
                (fowo.top - win.scrollTop() + fowo.height + layArea[1] + 8*2) - win.height() > 0 && fowo.where[0]();
            } else if (config.tips.guide === 3){
               layArea[0] + 8*2 - fowo.left > 0 && fowo.where[1]()
            }
            layerE.css({left: fowo.tipLeft, top: fowo.tipTop});
        break;
        
        default:
            that.layerMian.css({'background-color': '#fff'});
            if(config.title[1]){
                that.layerText.css({paddingTop: 18 + that.layerTitle.outerHeight()});
            }else{
                layerE.find('.xubox_msgico').css({top: 8});
                that.layerText.css({marginTop : 11});	
            }
        break;
    };
    
    config.fadeIn && layerE.css({opacity: 0}).animate({opacity: 1}, config.fadeIn);
    that.move();
};

//自适应宽高
Class.pt.autoArea = function(times){
    
    var that = this, layerE = that.layerE, config = that.config, page = config.page,
    layerMian = that.layerMian, layerBtn = that.layerBtn, layerText = that.layerText,
    layerPage = that.layerPage, layerB = that.layerB, titHeight, outHeight, btnHeight = 0, 
    load = $(".xubox_loading");
    if(config.area[0] === 'auto' && layerMian.outerWidth() >= config.maxWidth){	
        layerE.css({width : config.maxWidth});
    }
    config.title[1] ? titHeight = that.layerTitle.innerHeight() : titHeight = 0;
    switch(config.type){
        case 0:
            var aBtn = layerBtn.find('a');
            outHeight =  layerText.outerHeight() + 20;
            if(aBtn.length > 0){
                btnHeight = aBtn.outerHeight() +  20;
            }
        break;
        case 1:
            outHeight = $(page.dom).outerHeight();
            config.area[0] === 'auto' && layerE.css({width : layerPage.outerWidth()});
            if(page.html !== '' || page.url !== ''){
                outHeight = layerPage.outerHeight();
            }
        break;
        case 3:
            outHeight = load.outerHeight(); 
            layerMian.css({width: load.width()});
        break;
    };
    (config.area[1] === 'auto') && layerMian.css({height: titHeight + outHeight + btnHeight});
    layerB.css({width: layerE.outerWidth() + 2*config.border[0] , height: layerE.outerHeight() + 2*config.border[0]});
    (layer.ie6 && config.area[0] != 'auto') && layerMian.css({width : layerE.outerWidth()});
    (config.offset[1] === '50%' || config.offset[1] == '') && (config.type !== 4) ? layerE.css({marginLeft : -layerE.outerWidth()/2}) : layerE.css({marginLeft : 0});
};

//拖拽层
Class.pt.move = function(){
    var that = this, config = this.config, dom = that.dom, conf = {
        setY: 0,
        moveLayer: function(){
            if(parseInt(conf.layerE.css('margin-left')) == 0){
                var lefts = parseInt(conf.move.css('left'));
            }else{
                var lefts = parseInt(conf.move.css('left')) + (-parseInt(conf.layerE.css('margin-left')))
            }
            if(conf.layerE.css('position') !== 'fixed'){
                lefts = lefts - conf.layerE.parent().offset().left;
                conf.setY = 0
            }
            conf.layerE.css({left: lefts, top: parseInt(conf.move.css('top')) - conf.setY});
        }
    };
    
    config.move[1] && that.layerE.find(config.move[0]).attr('move','ok');
    config.move[1] ? that.layerE.find(config.move[0]).css({cursor: 'move'}) : that.layerE.find(config.move[0]).css({cursor: 'auto'});
    
    $(config.move[0]).on('mousedown', function(M){	
        M.preventDefault();
        if($(this).attr('move') === 'ok'){
            conf.ismove = true;
            conf.layerE = $(this).parents('.'+ dom.lay);
            var xx = conf.layerE.offset().left, yy = conf.layerE.offset().top, ww = conf.layerE.width() - 6, hh = conf.layerE.height() - 6;
            if(!$('#xubox_moves')[0]){
                $('body').append('<div id="xubox_moves" class="xubox_moves" style="left:'+ xx +'px; top:'+ yy +'px; width:'+ ww +'px; height:'+ hh +'px; z-index:2147483584"></div>');
            }
            conf.move = $('#xubox_moves');
            config.moveType && conf.move.css({opacity: 0});
           
            conf.moveX = M.pageX - conf.move.position().left;
            conf.moveY = M.pageY - conf.move.position().top;
            conf.layerE.css('position') !== 'fixed' || (conf.setY = win.scrollTop());
        }
    });
    
    $(document).mousemove(function(M){			
        if(conf.ismove){
            var offsetX = M.pageX - conf.moveX, offsetY = M.pageY - conf.moveY;
            M.preventDefault();

            //控制元素不被拖出窗口外
            if(!config.moveOut){
                conf.setY = win.scrollTop();
                var setRig = win.width() - conf.move.outerWidth() - config.border[0], setTop = config.border[0] + conf.setY;               
                offsetX < config.border[0] && (offsetX = config.border[0]);
                offsetX > setRig && (offsetX = setRig); 
                offsetY < setTop && (offsetY = setTop);
                offsetY > win.height() - conf.move.outerHeight() - config.border[0] + conf.setY && (offsetY = win.height() - conf.move.outerHeight() - config.border[0] + conf.setY);
            }
            
            conf.move.css({left: offsetX, top: offsetY});	
            config.moveType && conf.moveLayer();
            
            offsetX = null;
            offsetY = null;
            setRig = null;
            setTop = null
        }					  						   
    }).mouseup(function(){
        try{
            if(conf.ismove){
                conf.moveLayer();
                conf.move.remove();
            }
            conf.ismove = false;
        }catch(e){
            conf.ismove = false;
        }
        config.moveEnd && config.moveEnd();
    });
};

//自动关闭layer
Class.pt.autoclose = function(){
    var that = this, time = this.config.time, maxLoad = function(){
        time--;
        if(time === 0){
            layer.close(that.index);
            clearInterval(that.autotime);
        }
    };
    this.autotime = setInterval(maxLoad , 1000);
};

ready.config = {
    end : {}
};

Class.pt.callback = function(){
    var that = this, layerE = that.layerE, config = that.config, dialog = config.dialog;
    that.openLayer();
    that.config.success(layerE);
    layer.ie6 && that.IE6();

    layerE.find('.xubox_close').off('click').on('click', function(e){
        e.preventDefault();
        config.close(that.index);
    });
    
    layerE.find('.xubox_yes').off('click').on('click',function(e){
        e.preventDefault();
        config.yes ? config.yes(that.index) : dialog.yes(that.index);
    });
    
    layerE.find('.xubox_no').off('click').on('click',function(e){
        e.preventDefault();
        config.no ? config.no(that.index) : dialog.no(that.index);
    });
    
    this.layerS.off('click').on('click', function(e){
        e.preventDefault();
        that.config.shadeClose && layer.close(that.index);
    });
    
    ready.config.end[that.index] = config.end;
};

Class.pt.IE6 = function(){
    var that = this, layerE = that.layerE, select = $('select'), dom = that.dom;
    var _ieTop =  layerE.offset().top;	
    //ie6的固定与相对定位
    if(that.config.fix){
        var ie6Fix = function(){
            layerE.css({top : $(document).scrollTop() + _ieTop});
        };	
    }else{
        var ie6Fix = function(){
            layerE.css({top : _ieTop});	
        };
    }
    ie6Fix();
    win.scroll(ie6Fix);

    //隐藏select
    $.each(select, function(index , value){
        var sthis = $(this);
        if(!sthis.parents('.'+dom.lay)[0]){
            sthis.css('display') == 'none' || sthis.attr({'layer' : '1'}).hide();
        }
        sthis = null;
    });

    //恢复select
    that.reselect = function(){
        $.each(select, function(index , value){
            var sthis = $(this);
            if(!sthis.parents('.'+dom.lay)[0]){
                (sthis.attr('layer') == 1 && $('.'+dom.lay).length < 1) && sthis.removeAttr('layer').show(); 
            }
            sthis = null;
        });
    }; 
};

//给layer对象拓展方法
Class.pt.openLayer = function(){
    var that = this, dom = that.dom;

    //自适应宽高
    layer.autoArea = function(index){
        return that.autoArea(index);
    };

    //获取layer当前索引
    layer.getIndex = function(selector){
        return $(selector).parents('.'+dom.lay).attr('times');	
    };

    //获取子iframe的DOM
    layer.getChildFrame = function(selector, index){
        index = index || $('.'+ dom.ifr).parents('.'+dom.lay).attr('times');
        return $('#'+ dom.lay + index).find('.'+ dom.ifr).contents().find(selector);	
    };

    //得到当前iframe层的索引，子iframe时使用
    layer.getFrameIndex = function(name){
        return $(name ? '#'+ name : '.'+ dom.ifr).parents('.'+dom.lay).attr('times');
    };

    //iframe层自适应宽高
    layer.iframeAuto = function(index){
        index = index || $('.'+ dom.ifr).parents('.'+dom.lay).attr('times');
        var heg = this.getChildFrame('body', index).outerHeight(),
        lbox = $('#'+ dom.lay + index), tit = lbox.find('.xubox_title'), titHt = 0;
        !tit || (titHt = tit.height());
        lbox.css({height: heg + titHt});
        var bs = -parseInt($('#xubox_border'+ index).css('top'));
        $('#xubox_border'+ index).css({height: heg + 2*bs + titHt});
        $('#'+ dom.ifr + index).css({height: heg});
    };

    //关闭layer
    layer.close = function(index){
        var layerNow = $('#'+ dom.lay + index), shadeNow = $('#xubox_moves, #xubox_shade' + index);
        if(layerNow.attr('type') == that.type[1]){
            if(layerNow.find('.xuboxPageHtml')[0]){
                layerNow.remove();
            }else{
                layerNow.find('.xubox_close,.xubox_botton,.xubox_title,.xubox_border').remove();
                for(var i = 0 ; i < 3 ; i++){
                    layerNow.find('.layer_pageContent').unwrap().hide();
                }
            }
        }else{
            document.all && layerNow.find('#'+ dom.ifr + index).remove();
            layerNow.remove();
        }
        shadeNow.remove();
        layer.ie6 && that.reselect();
        typeof ready.config.end[index] === 'function' && ready.config.end[index]();
        delete ready.config.end[index];
    };

    //关闭加载层
    layer.loadClose = function(){
        var parent = $('.xubox_loading').parents('.'+dom.lay),
        index = parent.attr('times');
        layer.close(index);
    };

    //出场内置动画
    layer.shift = function(type, rate){
        var config = that.config, iE6 = layer.ie6, layerE = that.layerE, cutWth = 0, ww = win.width(), wh = win.height();
        (config.offset[1] == '50%' || config.offset[1] == '') ? cutWth = layerE.outerWidth()/2 : cutWth = layerE.outerWidth();
        var anim = {
            t: {top : config.border[0]},
            b: {top : wh - layerE.outerHeight() - config.border[0]},
            cl: cutWth + config.border[0],
            ct: -layerE.outerHeight(),
            cr: ww - cutWth - config.border[0],
            fn: function(){
                iE6 && that.IE6();	
            }
        };
        switch(type){
            case 'left-top':
                layerE.css({left: anim.cl, top: anim.ct}).animate(anim.t, rate, anim.fn);
            break; 
            case 'top':
                layerE.css({top: anim.ct}).animate(anim.t, rate, anim.fn);
            break;
            case 'right-top':
                layerE.css({left: anim.cr, top: anim.ct}).animate(anim.t, rate, anim.fn);
            break;
            case 'right-bottom':
                layerE.css({left: anim.cr, top: wh}).animate(anim.b, rate, anim.fn);
            break;
            case 'bottom':
                layerE.css({top: wh}).animate(anim.b, rate, anim.fn);
            break;
            case 'left-bottom':
                layerE.css({left: anim.cl, top: wh}).animate(anim.b, rate, anim.fn);
            break;
            case 'left':
                layerE.css({left: -layerE.outerWidth(), marginLeft:0}).animate({left:anim.t.top}, rate, anim.fn);
            break;
            
        };	
    };

    //初始化拖拽元素
    layer.setMove = function(){
        return that.move();
    };

    //给指定层重置属性
    layer.area = function(index, options){
        var nowobect = [$('#'+ dom.lay + index), $('#xubox_border'+ index)],
        type = nowobect[0].attr('type'), main = nowobect[0].find('.xubox_main'),
        title = nowobect[0].find('.xubox_title');
        if(type === that.type[1] || type === that.type[2]){
            nowobect[0].css(options);
            main.css({height: options.height});
            if(type === that.type[2]){
                var iframe = nowobect[0].find('iframe');
                iframe.css({width: options.width, height: title ? options.height - title.outerHeight() : options.height});
            }
            if(nowobect[0].css('margin-left') !== '0px') {
                options.hasOwnProperty('top') && nowobect[0].css({top: options.top - (nowobect[1][0] && parseInt(nowobect[1].css('top')))});
                options.hasOwnProperty('left') && nowobect[0].css({left: options.left + nowobect[0].outerWidth()/2 - (nowobect[1][0] && parseInt(nowobect[1].css('left')))})
                nowobect[0].css({marginLeft : -nowobect[0].outerWidth()/2});
            }
            if(nowobect[1][0]){
                nowobect[1].css({
                    width: parseFloat(options.width) - 2*parseInt(nowobect[1].css('left')), 
                    height: parseFloat(options.height) - 2*parseInt(nowobect[1].css('top'))
                });
            }
        }
    };

    //关闭所有层
    layer.closeAll = function(){
        var layerObj = $('.'+dom.lay);
        $.each(layerObj, function(){
            var i = $(this).attr('times');
            layer.close(i);
        });
    };
    
    //关闭tips层
    layer.closeTips = function(){
        var tips = $('.xubox_tips');
        if(tips[0]){
            layer.close(tips.parents('.xubox_layer').attr('times'));
        }
    };
    
    //重置iframe url
    layer.iframeSrc = function(index, url){
        $('#'+ dom.lay + index).find('iframe').attr('src', url);
    };

    //置顶当前窗口
    layer.zIndex = that.config.zIndex;
    layer.setTop = function(layerNow){
        var setZindex = function(){
            layer.zIndex++;
            layerNow.css('z-index', layer.zIndex + 1);
        };
        layer.zIndex = parseInt(layerNow[0].style.zIndex);
        layerNow.on('mousedown', setZindex);
        return layer.zIndex;
    };
};

//主入口
ready.run = function(){
    $ = jQuery; 
    win = $(window);
    layer.use('skin/layer.css');
    $.layer = function(deliver){
        var o = new Class(deliver);
        return o.index;
    };
};

//为支持CMD规范的模块加载器
var require = '../../init/jquery'; //若采用seajs，需正确配置jquery的相对路径。未用可无视此处。
if(window.seajs){
    define([require], function(require, exports, module){
        ready.run();
        exports.layer = [window.layer, window['$'].layer];
    });
}else{
    ready.run();
}

}(window);