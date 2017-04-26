import * as http from '../../http'
import * as models from '../../../models'

export class Files {
  private request: http.APIRequest

  constructor(request: http.APIRequest) {
    this.request = request
  }

  getUploadSiteFilesUrl(siteId: number, uploadType: 'image' | 'video' | 'file'): string {
    return this.request.getURL(`/files/actions/upload_site_files?siteId=${siteId}&uploadType=${uploadType}`)
  }

  getUploadAvatarUrl(): string {
    return this.request.getURL(`/files/actions/upload_avatar`)
  }

  uploadAvatarResize(size: number, x: number, y: number, relatedUrl: string, cb?: (err: models.Error, res: {
    avatarUrl: string
  }) => void) {
    this.request.post('/files/actions/upload_avatar_resize', {
      size, x, y, relatedUrl
    }, cb)
  }
}
