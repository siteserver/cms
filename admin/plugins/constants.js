export default (ctx, inject) => {
  const url = {
    install: '/install/'
  }
  const constants = {
    url: url
  }

  inject('constants', constants)
}
