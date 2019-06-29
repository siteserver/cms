import request from '@/utils/request'

export function fetchInfo() {
  return request({
    url: '/cms/info',
    method: 'get'
  })
}
