import Vue from 'vue'
import App from './App.vue'
import router from './router/router'
import VueAudio from 'vue-audio-better'
import {HttpGet,HttpPost} from "@/utils/HttpRequest";
import ElementUI from 'element-ui';
import * as echarts from 'echarts';

// 引入element-ui全部css
import 'element-ui/lib/theme-chalk/index.css';
import qs from 'qs';
// 阿里巴巴字体图标
 import '@/assets/iconfont/iconfont.css'

import 'nprogress/nprogress.css' //这个样式必须引入

import vueSeamless from 'vue-seamless-scroll'
Vue.prototype.$echarts = echarts


Vue.use(vueSeamless)
// 关闭Vue的生产提示
Vue.config.productionTip=false

Vue.prototype.HttpGet = HttpGet;
Vue.prototype.HttpPost = HttpPost;

Vue.use(ElementUI)
Vue.use(qs);
Vue.use(VueAudio)

router.beforeEach((to, from, next) => {
    if (to.meta.title) {
        document.title = to.meta.title
    }
    next()
    const route = document.getElementsByClassName('route-content')[0]
    if (route) {
        route.classList.remove('route-filter')
    }
})

new Vue({
    router,
    render: h => h(App)
}).$mount('#app')