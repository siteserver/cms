const STATUS_NOT_MODIFIED = 304
const STATUS_BAD_REQUEST = 400
const STATUS_UNAUTHORIZED = 401
const STATUS_PAYMENT_REQUIRED = 402
const STATUS_FORBIDDEN = 403
const STATUS_NOT_FOUND = 404
const STATUS_METHOD_NOT_ALLOWED = 405
const STATUS_NOT_ACCEPTABLE = 406
const STATUS_PROXY_AUTHENTICATION_REQUIRED = 407
const STATUS_REQUEST_TIMEOUT = 408
const STATUS_CONFLICT = 409
const STATUS_GONE = 410
const STATUS_LENGTH_REQUIRED = 411
const STATUS_INTERNAL_SERVER_ERROR = 500

export enum ERestMethod {
  GET,
  POST,
  PUT,
  DELETE,
  PATCH
}

export class ERestMethodUtils {
  static getValue(method: ERestMethod): string {
    if (method === ERestMethod.GET) {
      return "GET"
    } else if (method === ERestMethod.POST) {
      return "POST"
    } else if (method === ERestMethod.PUT) {
      return "PUT"
    } else if (method === ERestMethod.DELETE) {
      return "DELETE"
    } else if (method === ERestMethod.PATCH) {
      return "PATCH"
    }
    return "POST"
  }

  static equals(methodStr: string, method: ERestMethod) {
    return (ERestMethodUtils.getValue(method) === methodStr)
  }

  static errorCode(status: number): string {
    switch (status) {
      case STATUS_BAD_REQUEST:
        return 'Bad Request'
      case STATUS_UNAUTHORIZED:
        return 'Unauthorized'
      case STATUS_PAYMENT_REQUIRED:
        return 'Payment Required'
      case STATUS_FORBIDDEN:
        return 'Forbidden'
      case STATUS_NOT_FOUND:
        return 'Not Found'
      case STATUS_METHOD_NOT_ALLOWED:
        return 'Method Not Allowed'
      case STATUS_NOT_ACCEPTABLE:
        return 'Not Acceptable'
      case STATUS_PROXY_AUTHENTICATION_REQUIRED:
        return 'Proxy Authentication Required'
      case STATUS_REQUEST_TIMEOUT:
        return 'Request Timeout'
      case STATUS_CONFLICT:
        return 'Conflict'
      case STATUS_GONE:
        return 'Gone'
      case STATUS_LENGTH_REQUIRED:
        return 'Length Required'
      case STATUS_INTERNAL_SERVER_ERROR:
        return 'Internal Server Error'
    }
    return 'Unknown Error'
  }
}
