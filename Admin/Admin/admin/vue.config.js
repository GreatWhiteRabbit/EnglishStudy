const { defineConfig } = require('@vue/cli-service')
module.exports = defineConfig({
  lintOnSave:false,
  devServer: {
    port:8079,
    proxy: {
      "/apis": {
        target: "https://127.0.0.1:7031", // 需要请求的地址
        // target: "https://127.0.0.1:5001", // 需要替换的地址
        changeOrigin: true, // 是否跨域
        ws:true,
        pathRewrite: {
          "^/apis": "" // 替换target中的请求地址，也就是说，在请求的时候，url用'/proxy'代替'http://ip.taobao.com'
        }
      }
    },
    client:{
      overlay:false
    }
  },
  productionSourceMap: false,
})
