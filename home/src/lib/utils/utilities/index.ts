import * as React from 'react'
import _ from 'lodash'
import moment, * as moments from 'moment'
import 'moment/locale/zh-cn'

export function getValidateCode(): string {
  var validateCode = ""
  var s = [
    '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
  ]
  for (var i = 0; i < 4; i++) {
    validateCode += s[_.random(s.length - 1)]
  }
  return validateCode
}

export function toDate(o: Date | string): Date {
  if (typeof o === 'object') return o

  if (typeof o === 'string') {
    if (moment(o).isValid()) {
      return moment(o).toDate()
    }
  }

  return new Date()
}

export function dateDiff(start: any, end: any): number {
  const startDate = toDate(start)
  const endDate = toDate(end)
  var datediff = endDate.getTime() - startDate.getTime()
  const d = (datediff / (24 * 60 * 60 * 1000))
  return d
}

export function toDateString(date: any): string {
  let str = ''
  if (typeof date === 'object') {
    let now = moment(date).format('YYYY-MM-DD')
    str = now
  } else if (typeof date === 'string') {
    str = date
  }
  str = str.replace(/\//g, '-')
  if (str.indexOf(' ') !== -1) {
    str = str.substr(0, str.indexOf(' '))
  }

  return str
}

export function toTimeString(date: any): string {
  let str = ''
  if (typeof date === 'object') {
    let now = moment(date).format('hh:mm:ss')
    str = now
  } else if (typeof date === 'string') {
    str = date
  }
  str = str.replace(/\//g, '-')
  if (str.indexOf(' ') !== -1) {
    str = str.substr(str.indexOf(' '))
  }

  return str
}

export function isMobile(str: string): boolean {
  const regPartton = /^1[3-9]+\d{9}$/
  return regPartton.test(str)
}

export function isEmail(str) {
  var regPartton = /^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$/
  return regPartton.test(str)
}

export function isError(errors: { [index: string]: boolean }): boolean {
  for (let key in errors) {
    if (errors.hasOwnProperty(key) && errors[key]) {
      return true
    }
  }
  return false;
};

export function stopAndPrevent(e: React.SyntheticEvent) {
  stop(e)
  prevent(e)
}

export function stop(e: React.SyntheticEvent) {
  if (e) {
    e.stopPropagation()
  }
}

export function prevent(e: React.SyntheticEvent) {
  if (e) {
    e.preventDefault()
  }
}

export function getContent(str: string, maxNum: number): string {
  let content = (str || '').substr(0, maxNum)
  if ((str || '').length > maxNum) {
    content += '...'
  }
  return content
}

export function getFileName(attachUrl: string, len?: number) {
  len = len || 5
  try {
    let fileName = attachUrl.substr(attachUrl.lastIndexOf('/') + 1)
    if (fileName.lastIndexOf('_') !== -1) {
      fileName = fileName.substr(0, fileName.lastIndexOf('_'))
    }
    return getContent(fileName, len)
  } catch (e) { }
  return '查看'
}

export function getQueryStringValue(search, key) {
  return decodeURIComponent(search.replace(new RegExp("^(?:.*[&\\?]" + encodeURIComponent(key).replace(/[\.\+\*]/g, "\\$&") + "(?:\\=([^&]*))?)?.*$", "i"), "$1"));
}

export function findIgnoreCase(val: string, arr: Array<string>): string {
  val = toLower(val)
  for (var o of arr) {
    if (toLower(o) === val) {
      return o
    }
  }
  return undefined
}

export function parseInt(val: string): number {
  return _.parseInt(val)
}

export function parseBool(val: string): boolean {
  return toLower(val) === 'true'
}

export function toLower(val: string): string {
  if (!val) return ''
  return _.toLower(val)
}

export function lowerFirst(val: string): string {
  if (!val) return ''
  return _.lowerFirst(val)
}

export function upperFirst(val: string): string {
  if (!val) return ''
  return _.upperFirst(val)
}

export function assign(obj1: any, obj2: any): any {
  return _.assign(obj1, obj2)
}

export function keys(obj: any): Array<string> {
  return _.keys(obj)
}

export function values(obj: any): Array<any> {
  return _.values(obj)
}

export function trim(val: any, chars: string): string {
  return _.trim(val, chars)
}

export function find(collection: Array<any>|Object, predicate?: Function, fromIndex?: number): any {
  return _.find(collection, predicate, fromIndex)
}

export function isObject(val: any): boolean {
  return _.isObject(val)
}

export function indexOf(arr: Array<any>, val: any): number {
  return _.indexOf(arr, val)
}

export function map(collection: Array<any>, iteratee: Function): Array<any> {
  return _.map(collection, iteratee)
}