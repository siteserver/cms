import request from '@/utils/request'

export function getThemes() {
  return request({
    url: '/themes',
    method: 'get'
  })
}
