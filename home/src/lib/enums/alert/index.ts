export enum EAlertType {
  SUCCESS,
  INFO,
  WARNING,
  DANGER
}

export class EAlertTypeUtils {
  static getValue(type: EAlertType): string {
    if (type === EAlertType.SUCCESS) {
      return "success"
    } else if (type === EAlertType.INFO) {
      return "info"
    } else if (type === EAlertType.WARNING) {
      return "warning"
    } else if (type === EAlertType.DANGER) {
      return "danger"
    }
    return ""
  }
}

export class Alert {
  constructor(public alertType: EAlertType, public message: string, public pageUrl?: string, public pageText?: string) { }
}
