import Vue from 'vue'
import VueRouter from 'vue-router'

const NotFound = () => import('../views/NotFound');
const Home = () => import ('../views/Home.vue');
const Login = () => import ('../views/Login.vue');
const EverydayEnglishList = () => import('../components/EverydayEnglish/EverydayEnglishList');
const UpdateEverydayEnglish = () => import('../components/EverydayEnglish/UpdateEverydayEnglish');
const Profile = () =>  import('../components/User/Profile');
const UserList =() => import('../components/User/UserList');
const AddUser =() => import('../components/User/AddUser');
const WordList =() => import('../components/Words/WordList');
const AddWord =() => import('../components/Words/AddWord');
const PassageList =() => import('../components/Passage/PassageList');
const UpdatePassage =() => import('../components/Passage/UpdatePassage');
const  AddPassage =() => import('../components/Passage/AddPassage');
const ListeningPaperList =() => import('../components/ListeningPaper/ListeningPaperList');
const ListeningPaperDetail =() => import('../components/ListeningPaper/ListeningPaperDetail');
const AddListeningPaper =() => import('../components/ListeningPaper/AddListeningPaper');
const UpdateListeningPaper =() => import('../components/ListeningPaper/UpdateListeningPaper');
const SystemMessageList = () => import('../components/SystemMessage/SystemMessageList');
const Comment = () => import('../components/SystemMessage/Comment');
const MyResource = () => import('../components/MyResource/MyResource');

Vue.use(VueRouter)
const name = "后台系统"
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

const routes = [{
        path: '/',
        redirect: {
            name: 'Login'
        },
    },
    {
        path:'/404',
        name:"404",
        component: NotFound
    },
    {
        path:'*',
        redirect:{
            name:"404"
        }
    },
    {
        path: '/home',
        name: 'Home',
        component: Home,
        meta:{
            title: '首页-' + name,
            nav: nav
        },
        children: [
            {
                path: 'everyday/list',
                component: EverydayEnglishList,
                meta:{
                    title: '每日英语列表-' + name,
                    nav: nav
                }
            },
            {
                path: 'user/list',
                component: UserList,
                meta:{
                    title: '用户列表-' + name,
                    nav: nav
                }
            },
            {
                path: 'user/add',
                component: AddUser,
                meta:{
                    title: '添加用户-' + name,
                    nav: nav
                }
            },
            {
                path: 'words/list',
                component: WordList,
                meta:{
                    title: '单词列表-' + name,
                    nav: nav
                }
            },
            {
                path: 'words/add',
                component: AddWord,
                meta:{
                    title: '添加单词-' + name,
                    nav: nav
                }
            },
            {
                path: 'passage/list',
                component: PassageList,
                meta:{
                    title: '阅读理解列表-' + name,
                    nav: nav
                }
            },
            {
                path: 'passage/add',
                component: AddPassage,
                meta:{
                    title: '添加阅读理解-' + name,
                    nav: nav
                }
            },
            {
                path: 'listening/list',
                component: ListeningPaperList,
                meta:{
                    title: '听力列表-' + name,
                    nav: nav
                }
            },
            {
                path: 'sys_msg/list',
                component: SystemMessageList,
                meta:{
                    title: '系统消息-' + name,
                    nav: nav
                }
            },
            {
                path: 'comment/list',
                component: Comment,
                meta:{
                    title: '评论-' + name,
                    nav: nav
                }
            },
            {
                path: 'resource/list',
                component: MyResource,
                meta:{
                    title: '资源-' + name,
                    nav: nav
                }
            },

        ]
    },
    {
        path: '/login',
        name: 'Login',
        component: Login,
        meta:{
            title: '登录-' + name,
            nav: nav
        }
    },
    {
        name:'UpdateEverydayEnglish',
        path: "/updateEverydayEnglish/:id",
        component:UpdateEverydayEnglish,
        meta:{
            title: '修改每日英语-' + name,
            nav: nav
        }
    },
    {
        path: '/profile/:id',
        component: Profile,
        meta:{
            title: '个人中心-' + name,
            nav: nav
        }
    },
    {
        path: '/updatePassage/:id',
        component: UpdatePassage,
        meta:{
            title: '阅读理解-' + name,
            nav: nav
        }
    },
    {
        path: '/listening/detail/:id',
        component: ListeningPaperDetail,
        meta:{
            title: '听力试题详情-' + name,
            nav: nav
        }
    },
    {
        path: '/listening/add',
        component: AddListeningPaper,
        meta:{
            title: '添加听力-' + name,
            nav: nav
        }
    },
    {
        path: '/listening/update/:id',
        component: UpdateListeningPaper,
        meta:{
            title: '修改听力试题内容-' + name,
            nav: nav
        }
    },
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