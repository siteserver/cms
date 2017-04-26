/**
 
 @Name: layer拓展库，依赖于layer
 @Date: 2013.12.14
 @Author: 贤心
 @Versions：1.0.0
 @Api：http://sentsin.com/jquery/layer
 @Desc: 本拓展会持续更新

 **/
 
layer.use('skin/layer.ext.css', function(){
    
    //仿系统prompt
    layer.prompt = function(parme, yes, no){
        var log = {}, parme = parme || {}, conf = {
            area: ['auto', 'auto'],
            offset: [parme.top || '200px', ''],
            title: parme.title || '信息',
            dialog: {
                btns: 2,
                type: -1,
                msg: '<input type="'+ function(){
                    if(parme.type === 1){ //密码
                        return 'password';
                    } else if(parme.type === 2) {
                        return 'file';
                    } else {
                        return 'text';
                    }
                }() +'" class="xubox_prompt xubox_form" id="xubox_prompt" value="" />',
                yes: function(index){
                    var val = log.prompt.val();
                    yes && yes(val);
                    if(val === ''){
                        log.prompt.focus();
                    } else {
                        layer.close(index);
                    }
                }, no: no
            }, success: function(){
                log.prompt = $('#xubox_prompt');
            }
        };
        if(parme.type === 3){
            conf.dialog.msg = '<textarea class="xubox_prompt xubox_form xubox_formArea" id="xubox_prompt"></textarea>'
        }
        return $.layer(conf);
    };
    
    //tab层
    layer.tab = function(parme){
        var log = {}, parme = parme || {}, data = parme.data || [], conf = {
            type: 1,
            border: [0],
            area: ['auto', 'auto'],
            title: false,
            shade : parme.shade,
            move: ['.xubox_tabmove', true],
            closeBtn: false,
            page: {html: '<div class="xubox_tab" style="'+ function(){
                    parme.area = parme.area || [];
                    return 'width:'+ (parme.area[0] || '500px') +'; height:'+ (parme.area[1] || '300px') +'">';
                }()
                +'<span class="xubox_tabmove"></span>'
                +'<div class="xubox_tabtit">'
                +function(){
                    var len = data.length, ii = 1, str = '';
                    if(len > 0){
                        str = '<span class="xubox_tabnow">'+ data[0].title +'</span>';
                        for(; ii < len; ii++){
                            str += '<span>'+ data[ii].title +'</span>';
                        }
                    }
                    
                    return str;
                }() +'</div>'
                +'<ul class="xubox_tab_main">'+ function(){
                    var len = data.length, ii = 1, str = '';
                    if(len > 0){
                        str = '<li class="xubox_tabli xubox_tab_layer">'+ (data[0].content || '请配置content') +'</li>';
                        for(; ii < len; ii++){
                            str += '<li class="xubox_tabli">'+ (data[ii].content || '请配置content') +'</li>';
                        }
                    }
                    return str;
                }() +'</ul>'
                +'<span class="xubox_tabclose" title="关闭">X</span>'
                +'</div>'
            }, success: function(layerE){
                //切换事件
                var btn = $('.xubox_tabtit').children(), main = $('.xubox_tab_main').children(), close = $('.xubox_tabclose');
                btn.on('click', function(){
                    var othis = $(this), index = othis.index();
                    othis.addClass('xubox_tabnow').siblings().removeClass('xubox_tabnow');
                    main.eq(index).show().siblings().hide();
                });
                //关闭层
                close.on('click', function(){
                    layer.close(layerE.attr('times'));
                });
            }
        };
        return $.layer(conf);
    };
    
    //相册层
    layer.photo = function(selector, options){
       
    };

});