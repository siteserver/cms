export var EAlertType;
(function (EAlertType) {
    EAlertType[EAlertType["SUCCESS"] = 0] = "SUCCESS";
    EAlertType[EAlertType["INFO"] = 1] = "INFO";
    EAlertType[EAlertType["WARNING"] = 2] = "WARNING";
    EAlertType[EAlertType["DANGER"] = 3] = "DANGER";
})(EAlertType || (EAlertType = {}));
export class EAlertTypeUtils {
    static getValue(type) {
        if (type === EAlertType.SUCCESS) {
            return "success";
        }
        else if (type === EAlertType.INFO) {
            return "info";
        }
        else if (type === EAlertType.WARNING) {
            return "warning";
        }
        else if (type === EAlertType.DANGER) {
            return "danger";
        }
        return "";
    }
}
export class Alert {
    constructor(alertType, message, pageUrl, pageText) {
        this.alertType = alertType;
        this.message = message;
        this.pageUrl = pageUrl;
        this.pageText = pageText;
    }
}
//# sourceMappingURL=index.js.map