import request from '@/utils/request'

const url = 'https://api.siteserver.cn/v1.2'

export function getTemplates(params) {
  return request({
    url: url + '/templates', // 假地址 自行替换
    method: 'get',
    params
  })
}

