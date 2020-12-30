UE.registerUI('xiumi', function (editor, uiName) {
    var btn = new UE.ui.Button({
        name   : 'xiumi-connect',
        title  : '秀米',
        onclick: function () {
            var dialog = new UE.ui.Dialog({
                iframeUrl: '/sitefiles/assets/lib/ueditor/third-party/custom/xiumi-ue-dialog-v5.html',
                editor   : editor,
                name     : 'xiumi-connect',
                title    : "秀米图文消息助手",
                cssRules : "width: " + (window.innerWidth - 60) + "px;" + "height: " + (window.innerHeight - 60) + "px;",
            });
            dialog.render();
            dialog.open();
        }
    });

    return btn;
});

UE.registerUI('135editor',function(editor,uiName){
    // var dialog = new UE.ui.Dialog({
    //     iframeUrl: editor.options.UEDITOR_HOME_URL+'dialogs/135editor/135EditorDialogPage.html',
    //     cssRules:"width:"+ parseInt(document.body.clientWidth*0.9) +"px;height:"+(window.innerHeight -50)+"px;",
    //     editor:editor,
    //     name:uiName,
    //     title:"135编辑器"
    // });
    // dialog.fullscreen = false;
    // dialog.draggable = false;
    var btn = new UE.ui.Button({
        name:'135editor-connect',       
        title:'135编辑器',
        onclick:function () {
            // dialog.render();
            // dialog.open();

            // 由于登录存在跨域问题，请使用如下方式调用135编辑器
            var editor135 = window.open('https://www.135editor.com/simple_editor.html?callback=true&appkey=')
            // setTimeout(function(){
            //     editor135.postMessage(editor.getContent(),'*');
            // },3000);
            
            window.addEventListener('message', function (event) {
                if (typeof event.data !== 'string') {
                    if(event.data.ready) {
                        editor135.postMessage(editor.getContent(),'*');
                    }
                    return;
                };
                editor.setContent(event.data);
                editor.fireEvent("catchRemoteImage");
            }, false);
        }
    });
    return btn;
},undefined);
// 修改最后的undefined参数为数字序号，比如5，可调整135编辑器按钮的顺序。默认出现在最后面