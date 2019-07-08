import request from '@/utils/request'

export function getSites() {
  return request({
    url: '/sites',
    method: 'get'
  })
}

export function getTableNames() {
  return request({
    url: '/sites/tableNames',
    method: 'get'
  })
}

export function create(data) {
  return request({
    url: '/sites',
    method: 'post',
    data
  })
}
