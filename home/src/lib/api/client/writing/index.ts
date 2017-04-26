import * as http from '../../http'
import * as models from '../../../models'
import * as utils from '../../../utils'

export class Writing {
  private request: http.APIRequest

  constructor(request: http.APIRequest) {
    this.request = request
  }

  createContent(publishmentSystemId: number, nodeId: number, content: models.Content, cb: (err: models.Error, res: {}) => void) {
    this.request.post(`/writing/actions/create_content`, utils.assign(content, {
      publishmentSystemId, nodeId
    }), cb)
  }

  deleteContent(publishmentSystemId: number, nodeId: number, id: number, cb: (err: models.Error, res: {}) => void) {
    this.request.post(`/writing/actions/delete_content`, {
      publishmentSystemId, nodeId, id
    }, cb)
  }

  editContent(publishmentSystemId: number, nodeId: number, id: number, content: models.Content, cb: (err: models.Error, res: {}) => void) {
    this.request.post(`/writing/actions/edit_content`, utils.assign(content, {
      publishmentSystemId, nodeId, id
    }), cb)
  }

  getContent(publishmentSystemId: number, nodeId: number, id: number, cb: (err: models.Error, res: {
    content: models.Content
  }) => void) {
    this.request.post(`/writing/actions/get_content`, {
      publishmentSystemId, nodeId, id
    }, cb)
  }

  getContents(publishmentSystemId: number, nodeId: number, searchType: string, keyword: string, dateFrom: string, dateTo: string, page: number, cb: (err: models.Error, res: {
    results: Array<models.Content>
    totalPage: number
  }) => void) {
    this.request.post(`/writing/actions/get_contents`, {
      publishmentSystemId, nodeId, searchType, keyword, dateFrom, dateTo, page
    }, cb)
  }

  getSites(cb: (err: models.Error, res: Array<models.PublishmentSystem>) => void) {
    this.request.post('/writing/actions/get_sites', null, cb)
  }

  getChannels(publishmentSystemId: number, cb: (err: models.Error, res: Array<{
    nodeId: number
    nodeName: string
  }>) => void) {
    this.request.post(`/writing/actions/get_channels`, {
      publishmentSystemId
    }, cb)
  }

  getTableStyles(publishmentSystemId: number, nodeId: number, cb: (err: models.Error, res: Array<models.TableStyle>) => void) {
    this.request.post(`/writing/actions/get_table_styles`, {
      publishmentSystemId, nodeId
    }, cb)
  }
}
