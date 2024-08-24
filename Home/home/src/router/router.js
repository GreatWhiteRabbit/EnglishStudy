import Vue from 'vue'
import VueRouter from 'vue-router'
Vue.use(VueRouter)
const Login = () => import ('../views/Login.vue')
const ForgetPassword = () => import('../views/ForgetPassword');
const Sign = () => import ('../views/Sign.vue')
const NotFound = () => import('../views/NotFound');
const Home = () => import('../views/Home');
const WordList = () => import('../components/Word/WordList');
const PassageList = () => import('../components/Passage/PassageList')
const PassageDetail = () => import('../components/Passage/PassageDetail');
const ListeningList = () => import('../components/Listening/ListeningList');
const ListeningDetail = () => import('../components/Listening/ListeningDetail');
const EveryDayList = () => import('../components/EverydayEnglish/EveryDayList');
const EveyDayDetail = () => import('../components/EverydayEnglish/EveryDayDetail');
const WordPractice = () => import('../components/Word/WordPractice');
const Profile = () => import('../components/User/Profile');
const UserHome = () => import('../components/User/UserHome')
const Message = () => import('../components/User/Message')
const ReplyComment = () => import('../components/User/ReplyComment');
const Setting = () => import('../components/User/Setting');
const PassageHistory = () => import('../components/User/PassageHistory');
const EverydayHistory = () => import('../components/User/EverydayHistory');
const ListeningHistory = () => import('../components/User/ListeningHistory');
const WordHistory = () => import('../components/User/WordHistory');
const Communication = () => import('../components/Communication/Communication');
const ReplyCommentDetail = () => import('../components/Communication/ReplyCommentDetail');
const MyResource = () => import('../components/MyResource/MyResource');

const name = "英语学习平台"
const nav = {
    navNoQuiet: {
        boxShadow: ' 0px 10px 20px 4px  rgba(195,199, 199,0.4)',
        backgroundColor: '#fff',
        color: 'rgba(0,0,0,.5)'
    },
    navQuiet: {
        color: '#fff'
    }
}
const nav1 = {
    navNoQuiet: {
        boxShadow: ' 0px 10px 20px 4px  rgba(195,199, 199,0.4)',
        backgroundColor: '#fff',
        color: 'rgba(0,0,0,.5)'
    },
    navQuiet: {
        backgroundColor: '#fff',
        color: 'rgba(0,0,0,.5)'
    }
}
const routes = [

    {
        path:'/',
        component: Home,
        meta: {
            keepAlive: true, // 需要被缓存
            title: "英语学习平台",
            nav: nav1,
            navBarShow:false, // 隐藏navbar
        }
    },
    {
        path:'/404',
        name:"404",
        component: NotFound,
        meta: {
            keepAlive: false, // 需要被缓存
            title: "页面不见了",
            nav: nav1,
            navBarShow:false,
        }
    },
    {
        path:'*',
        redirect:{
            name:"404"
        },
        meta: {
            keepAlive: false, // 需要被缓存
            title: "页面不见了",
            nav: nav1,
            navBarShow:false,
        }
    },

    {
        path: '/login',
        name: 'Login',
        component: Login,
        meta: {
            title: '登录-' + name,
            nav: nav1,
            navBarShow:false,
        }
    },
    {
        path: '/communication',
        name: 'Communication',
        component: Communication,
        meta: {
            title: '学习交流区-' + name,
            nav: nav1,
            navBarShow:true,
        }
    },
    {
        path: '/forget',
        name: 'ForgetPassword',
        component: ForgetPassword,
        meta: {
            title: '忘记密码-' + name,
            nav: nav1,
            navBarShow:false,
        }
    },
    {
        path: '/sign',
        name: 'Sign',
        component: Sign,
        meta: {
            title: '注册-' + name,
            nav: nav1,
            navBarShow:false,
        }
    },
    {
        path: '/word',
        name: 'word',
        component: WordList,
        meta: {
            keepAlive: false,
            title: '单词-' + name,
            nav: nav1,
            navBarShow:true,
        }
    },
    {
        path: '/passage',
        name: 'passage',
        component: PassageList,
        meta: {
            keepAlive: false,
            title: '阅读理解-' + name,
            nav: nav1,
            navBarShow:true,
        }
    },
    {
        path: '/detail/passage/:id',
        name: 'passageDetail',
        component: PassageDetail,
        meta: {
            keepAlive: true, // 需要被缓存
            title: '阅读理解-' + name,
            nav: nav1,
            navBarShow:true,
        }
    },
    {
        path: '/listening',
        name: 'ListeningList',
        component: ListeningList,
        meta: {
            keepAlive: false,
            title: '听力-' + name,
            nav: nav1,
            navBarShow:true,
        }
    },
    {
        path: '/detail/listening/:id',
        name: 'ListeningDetail',
        component: ListeningDetail,
        meta: {
            keepAlive: true, // 需要被缓存
            title: '听力-' + name,
            nav: nav1,
            navBarShow:true,
        }
    },
    {
        path: '/everyday',
        name: 'EveryDayList',
        component: EveryDayList,
        meta: {
            keepAlive: false,
            title: '每日英语-' + name,
            nav: nav1,
            navBarShow:true,
        }
    },
    {
        path: '/detail/everyday/:id',
        name: 'EverydayDetail',
        component: EveyDayDetail,
        meta: {
            keepAlive: true, // 需要被缓存
            title: '每日英语-' + name,
            nav: nav1,
            navBarShow:true,
        }
    },
    {
        path: '/practice',
        name: 'WordPractice',
        component: WordPractice,
        meta: {
            keepAlive: false,
            title: '单词练习-' + name,
            nav: nav1,
            navBarShow:false,
        }
    },
    {
        path: '/profile',
        name: 'Profile',
        component: Profile,
        meta: {
            keepAlive: false,
            title: '我的-' + name,
            nav: nav1,
            navBarShow:true,
        },
        children:[
            {
                path: 'message/:id',
                name: 'Message',
                component: Message,
                meta: {
                    keepAlive: false,
                    title: '消息-' + name,
                    nav: nav1,
                    navBarShow:true,
                }
            },
            {
                path: 'reply/:id',
                name: 'ReplyComment',
                component: ReplyComment,
                meta: {
                    keepAlive: false,
                    title: '回复我的-' + name,
                    nav: nav1,
                    navBarShow:true,
                }
            },
            {
                path: 'home/:id',
                name: 'UserHome',
                component: UserHome,
                meta: {
                    keepAlive: false,
                    title: '个人首页-' + name,
                    nav: nav1,
                    navBarShow:true,
                }
            },
            {
                path: 'setting/:id',
                name: 'Setting',
                component: Setting,
                meta: {
                    keepAlive: false,
                    title: '设置-' + name,
                    nav: nav1,
                    navBarShow:true,
                }
            },
            {
                path: 'history/passage/:id',
                name: 'PassageHistory',
                component: PassageHistory,
                meta: {
                    keepAlive: false,
                    title: '阅读理解记录-' + name,
                    nav: nav1,
                    navBarShow:true,
                }
            },
            {
                path: 'history/everyday/:id',
                name: 'EverydayHistory',
                component: EverydayHistory,
                meta: {
                    keepAlive: false,
                    title: '每日英语记录记录-' + name,
                    nav: nav1,
                    navBarShow:true,
                }
            },

            {
                path: 'history/listening/:id',
                name: 'ListeningHistory',
                component: ListeningHistory,
                meta: {
                    keepAlive: false,
                    title: '听力记录-' + name,
                    nav: nav1,
                    navBarShow:true,
                }
            }, {
                path: 'history/word/:id',
                name: 'WordHistory',
                component: WordHistory,
                meta: {
                    keepAlive: false,
                    title: '单词记录-' + name,
                    nav: nav1,
                    navBarShow:true,
                }
            },
        ]
    },
    {
        path: '/communication/comment/reply',
        name: 'ReplyCommentDetail',
        component: ReplyCommentDetail,
        meta: {
            title: '评论详情-' + name,
            nav: nav1,
            navBarShow:true,
            keepAlive: true,
        }
    },
    {
        path: '/resource',
        name: 'MyResource',
        component: MyResource,
        meta: {
            title: '资源-' + name,
            nav: nav1,
            navBarShow:true,
            keepAlive: true,
        }
    }


]
const router = new VueRouter({
    mode: 'history',
    routes,
    scrollBehavior(to, from, savedPosition) {
        if (savedPosition) {
            return savedPosition
        } else {
            return { x: 0, y: 0 }
        }
    }
})

export default router