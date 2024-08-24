<template>
  <div class="navbar">
    <el-menu  :default-active="activeIndex" class="el-menu-demo" mode="horizontal" @select="handleSelect">
      <el-menu-item style="margin-left: 20vh" >
        <el-image @click="goToHome"
            style="width: 130%; height: 130%"
            :src="logo"
            fit="cover"></el-image>
      </el-menu-item>
      <el-menu-item style="margin-left: 5vh" index="1">单词</el-menu-item>
      <el-menu-item index="2">听力</el-menu-item>
      <el-menu-item index="3">阅读理解</el-menu-item>
      <el-menu-item index="4">每日新闻</el-menu-item>
      <el-menu-item index="5">学习交流区</el-menu-item>
      <el-menu-item index="6">资料</el-menu-item>

      <el-menu-item style="margin-left: 5vh">
        <el-row>
          <el-col :span="12" v-if="isLogin">
            <el-popover
                placement="bottom"
                width="100px"
                trigger="hover"
            >
              <div slot="reference" style="width: 60px">
                <div >
                  <router-link :to="'/profile/home/' + userId"  target="_blank">
                    <el-avatar  :size="50" :src="getUserImage()"></el-avatar>
                  </router-link>
                </div>
              </div>
              <ul style="padding:0;margin:0">
                <li @click="goToProfile" class="user">个人中心</li>
              </ul>
              <ul style="padding:0;margin:0">
                <li @click="logOut" class="user">退出登录</li>
              </ul>
            </el-popover>

          </el-col>
          <el-row v-else>
            <el-col :span="11"><span @click="login">登录</span></el-col>
            <el-col :span="2"><span>/</span></el-col>
            <el-col :span="11"><span @click="sign">注册</span></el-col>
          </el-row>
          <el-col :span="12" v-show="isLogin">
            <el-col @click.native="goToMessage" :span="14">
              <el-badge v-if="message === 0" class="item">
                <!--              系统消息-->
                <el-button style="border: none" size="small">
                  <i style="color: gray; font-size: 25px;" class="el-icon-chat-dot-square"></i>
                </el-button>
              </el-badge>
              <el-badge v-else :value="message" class="item">
                <!--              系统消息-->
                <el-button style="border: none" size="small">
                  <i style="color: gray; font-size: 25px;" class="el-icon-chat-dot-square"></i>
                </el-button>
              </el-badge>
            </el-col>
            <el-col  :span="1">
              <el-row>
                <el-button @click="gotoHistory" size="small" style="font-size: 18px;border: none;">
                  历史
                  <i style="color: gray;font-size: 25px" class="el-icon-time el-icon--bottom"></i>
                </el-button>
              </el-row>
            </el-col>
          </el-col>
        </el-row>
      </el-menu-item>
    </el-menu>
  </div>
</template>

<script>


import * as signalR from "@aspnet/signalr";

export default {
  // 页面导航栏
  name: "NavBar",
  data() {
    return {
      activeIndex: '0',
      isLogin: false,// 是否登录
      url: "", // 用户头像
      message: 0,// 系统消息
      pathList: [
        '/word',
        '/listening',
        '/passage',
        '/everyday',
        '/communication',
          '/resource'
      ],
      logo:require('@/assets/image/logo.png'),
      signalrConnection:'',
      userId:''
    }
  },
  methods: {
    // 前往历史页面
    gotoHistory() {
      let userId = localStorage.getItem("userId");
      let path = "/profile/history/everyday/" + userId;
      let newUrl = this.$router.resolve({
        path:path,
        params:{
          id:userId
        }
      });
      window.open(newUrl.href, "_blank");
    },
    handleSelect(key, keyPath) {
      this.$router.push(this.pathList[key-1]);
    },
    // 退出登录
    logOut() {
      console.log("退出登录");
      localStorage.clear();
      this.isLogin = false;
      this.$router.push('/');
    },
    // 前往个人中心页面
    goToProfile() {
      let userId = localStorage.getItem("userId");
      let path = "/profile/home/" + userId;
      let newUrl = this.$router.resolve({
        path:path,
        params:{
          id:userId
        }
      });
      window.open(newUrl.href, "_blank");
    },
    // 获取系统消息和回复的消息
    getSystemMessage() {
      let userId = localStorage.getItem("userId");
      if(userId !== null)  {
        this.message = 0;
        // 获取系统消息
        this.HttpGet('/sys_msg/count/' + userId)
            .then(res => {
              this.message += res.data;
            })
        // 获取回复消息
        this.HttpGet('/comment/count/' + userId)
            .then(res => {

              this.message += res.data;
            })
      }
    },
    // 前往消息页面
    goToMessage() {
      let userId = localStorage.getItem("userId");
      let path = "/profile/message/" + userId;
      let newUrl = this.$router.resolve({
        path:path,
        params:{
          id:userId
        }
      });
      window.open(newUrl.href, "_blank");
    },
    // 前往登录页面
    login() {
      this.$router.push('/login');
    },
    // 前往注册页面
    sign() {
      this.$router.push('/sign');
    },
    // 前往首页
    goToHome() {
      if(this.$route.path !== '/') {
        this.$router.push('/');
      }
    },

    // 连接服务器,signalR不会使用暂时不用了
    connectServe() {
      this.signalrConnection = new signalR.HubConnectionBuilder()
          .withUrl("https://localhost:7031/sys_msg",{
            accessTokenFactory() {
            return  "Bearer" + " " + localStorage.getItem("token") || ''
            },
            skipNegotiation:true,
            transport: signalR.HttpTransportType.WebSockets
          })
          .build();



    },

    // 判断是否登录
    userIsLogin() {
      if (localStorage.getItem("avatar") !== null) {
        this.url = localStorage.getItem("avatar");
      }
      // 判断是否登录
      if (localStorage.getItem("userId") === null && localStorage.getItem("token") === null) {
        this.isLogin = false;
      } else {
        this.isLogin = true;
        this.userId = localStorage.getItem("userId");
      }
    }
  },
  mounted() {
    this.isLogin();
  },
  watch:{
    $route(to,from) {
      this.getSystemMessage();
      this.userIsLogin();
      this.url = localStorage.getItem("avatar");
      if (localStorage.getItem("avatar") !== null) {
        this.url = localStorage.getItem("avatar");
      }
      // 判断是否登录
      if (localStorage.getItem("userId") === null && localStorage.getItem("token") === null) {
        this.isLogin = false;
      } else {
        this.url = localStorage.getItem("avatar");
        this.isLogin = true;
      }

      // 当跳转到word页面，将index设置为1
      if(to.path === '/word' ) {
        this.activeIndex = 1;
      }
      else if(to.path === '/listening') {
        this.activeIndex = 2;
      }
      else if(to.path === '/passage') {
        this.activeIndex = 3;
      }
      else if(to.path === '/everyday') {
        this.activeIndex = 4;
      }
      else if(to.path === '/communication') {
        this.activeIndex = 5;
      }
      else if(to.path === '/resource') {
        this.activeIndex = 6;
      }
      else if(to.path === '/') {
        this.activeIndex = 0;
      }

    }
  },
  computed:{
    // 获取用户头像
    getUserImage() {
      return () => {
        if (localStorage.getItem("avatar") !== null) {
          this.url = localStorage.getItem("avatar");
          return this.url;
        }
        return "";
      }
    }
  }

}
</script>

<style lang="stylus" scoped>

.navbar{
  height 10vh;
  position: sticky;
  top: 0;
  width: 100%;
  z-index 10;
}

.item {
  margin-top: 10px;
  margin-right: 40px;
}

.user
  width: 100%
  height 30px
  text-align center
  line-height 30px

.user:hover
  cursor: pointer;
  routeHover()

::v-deep.el-menu {
  background-color: white;
  height 60px;

  .el-menu-item {
    color: #7c7878;
    padding-left: 20px !important;
    height: 40px;
    margin: 8px;
    border-radius: 4px;
    line-height: 40px;
    font-size 22px;

    i {
      color: #ffffff;
    }
  }

  .el-menu-item:hover,
  .el-menu-item:focus {
    background-color: #9df8da;
    background-position: center;
    linear-gradient(
        to bottom right,
        #6691ff,
        #6269fc,
        #6269fc
    );
  }

  .el-menu-item.is-active {
    color: #27638d;
  }

  .el-submenu__title i {
    color: #bbf9f9;
  }

  .el-menu-item-group {
    .el-menu-item-group__title {
      color: #a0eced;
      padding-left: 30px !important;
    }
  }

  .el-submenu {
    .el-submenu__title {
      padding-left: 30px !important;

      &:hover {
        background-color: lightblue;
        background-position: center;
      }
    }

    .el-menu-item {
      min-width: 180px;
      padding-left: 20px !important;
    }
  }
}

</style>