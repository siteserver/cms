import * as React from 'react'
import client from '../../client'

declare var config: {
  domainAPI: string
}

export function getUploadFilesProps(url: string, multi: boolean, accept: string, success: (fileUrls: Array<string>) => void, error: (errorMessage: string) => void, progress: () => void) {
  //return {}
  // return {
  //   action: url + "&access_token=" + Auth.getToken(),
  //   headers: {
  //     'X-Access-Token': Auth.getToken()
  //   },
  //   multiple: multi,
  //   dataType: 'json',
  //   accept: accept,
  //   maxFileSize: 5000000, // 5 MB
  //   withCredentials: false,
  //   onSuccess(ret) {
  //     if (typeof ret === 'string') {
  //       success(JSON.parse(ret.replace('<pre>', '').replace('</pre>', '')))
  //     } else {
  //       success(ret)
  //     }
  //   }
  // }
  return {
    action: url,
    multiple: multi,
    dataType: 'json',
    accept: accept,
    maxFileSize: 5000000, // 5 MB
    withCredentials: true,
    onStart(files) {
    },
    beforeUpload(file) {
      if (['docx', 'doc', 'xlsx', 'xls', 'rar', 'zip', 'tz', 'txt', 'pdf', 'png', 'jpg', 'jpeg'].indexOf(file.name.split('.')[1]) === -1) {
        error('不能上传该类型文件！')
        return false;
      }
    },
    onSuccess(ret) {
      let res = (typeof ret === 'string') ? JSON.parse(ret.replace('<pre>', '').replace('</pre>', '')) : ret
      if (res.fileUrls) {
        success(res.fileUrls)
      } else {
        error(res.errorMessage)
      }
    },
    onProgress(step, file) {
      progress && progress()
    },
    onError(err) {
      error(err.errorMessage)
    }
  }
}

export function getUploadAvatarProps(url: string, success: (avatarUrl: string, relatedUrl: string) => void, error: (errorMessage: string) => void, progress: () => void) {
  return {
    action: url,
    multiple: false,
    dataType: 'json',
    accept: "image/png, image/jpeg, image/gif",
    maxFileSize: 5000000, // 5 MB
    withCredentials: true,
    onStart(files) {
    },
    onSuccess(ret) {
      let res = (typeof ret === 'string') ? JSON.parse(ret.replace('<pre>', '').replace('</pre>', '')) : ret
      if (res.avatarUrl) {
        success(res.avatarUrl, res.relatedUrl)
      } else {
        error(res.errorMessage)
      }
    },
    onProgress(step, file) {
      progress && progress()
    },
    onError(err) {
      error(err.errorMessage)
    }
  }
}
