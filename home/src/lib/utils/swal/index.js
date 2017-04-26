import swal from 'sweetalert';
export class Swal {
    static tip(title, text, isTimer) {
        if (isTimer) {
            swal({
                title: title,
                text: text,
                timer: 2000,
                confirmButtonText: '确认',
                cancelButtonText: '取消',
                html: true
            });
        }
        else {
            swal({
                title: title,
                text: text,
                confirmButtonText: '确认',
                cancelButtonText: '取消',
                html: true
            });
        }
    }
    static success(title, callback) {
        swal({
            title: title,
            type: "success",
            confirmButtonText: '确认',
            html: true
        }, callback);
    }
    static successConfirm(title, confirm) {
        swal({
            title: title,
            type: "success",
            showCancelButton: true,
            confirmButtonText: '确认',
            cancelButtonText: '取消',
            html: true
        }, confirm);
    }
    static error(err, callback) {
        swal({
            title: err.message,
            text: '',
            type: "error",
            confirmButtonText: '确认',
            cancelButtonText: '取消',
            html: true
        }, callback);
    }
    static warning(title, text) {
        swal({
            title: title,
            text: text,
            type: "warning",
            confirmButtonText: '确认',
            cancelButtonText: '取消',
            html: true
        });
    }
    static delete(title, text, confirm) {
        swal({
            title: title,
            text: text,
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#f36c60",
            confirmButtonText: '确认删除',
            cancelButtonText: '取消',
            closeOnConfirm: true,
            html: true
        }, confirm);
    }
    static confirm(title, confirm) {
        swal({
            title: title,
            type: "warning",
            showCancelButton: true,
            confirmButtonColor: "#f36c60",
            confirmButtonText: '确认',
            cancelButtonText: '取消',
            closeOnConfirm: true,
            html: true
        }, confirm);
    }
}
//# sourceMappingURL=index.js.map