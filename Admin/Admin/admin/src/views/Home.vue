<template>
  <el-container class="content" :class="{'is-left':!isMenu,'is-margin':isMenu}">
    <!-- <el-scrollbar style="height: 100%;"> -->
    <el-menu
      class="el-menu-vertical-demo"
      @open="handleOpen"
      @close="handleClose"
      :collapse="isMenu"
      style="overflow-y: auto;
    overflow-x: hidden;"
    >
      <el-menu-item index="-1" class="logo" style="padding:0px;text-align:center;">
        <i v-if="isMenu">LOGO</i>
        <span slot="title">
          <img src="~@/assets/logo.svg" height="40" />
        </span>
      </el-menu-item>

      <el-submenu v-for="item of homeList" :key="item.id" :index="item.id">
        <template slot="title">
          <i :class="item.icon"></i>
          <span slot="title">{{item.name}}</span>
        </template>
        <router-link tag="span" v-for="value of item.children" :key="value.id" :to="value.url">
          <el-menu-item :index="value.id">
            <span :class="{active:$route.path===value.url}">
              <i :class="value.icon"></i>
              {{value.name}}
            </span>
          </el-menu-item>
        </router-link>
      </el-submenu>
    </el-menu>
    <el-container>
      <el-header class="header">
        <div class="menu" :class="{change:!isMenu}" @click="toggleMenu">
          <div class="bar1"></div>
          <div class="bar2"></div>
          <div class="bar3"></div>
        </div>
        <div class="admin">
          <el-popover placement="bottom" width="50" trigger="hover">
            <ul class="admin-ul">
              <li @click="getProfile">个人中心</li>
              <li @click="logOut">退出</li>
            </ul>

            <el-avatar slot="reference" shape="square"  fit="cover" :src="avatar_img"></el-avatar>
          </el-popover>
        </div>
      </el-header>

      <el-container>
        <el-main style="padding-top:65px">
          <router-view></router-view>
        </el-main>
        <el-footer style="color:#fff;font-size: 14px;text-align:center">
            <p>Copyright © 2023 -
           {{getCurrentYear}}
           大白兔 All Rights Reserved.</p>
        </el-footer>
      </el-container>
    </el-container>
  </el-container>
</template>

<script>


export default {
  name: "Home",
  components: {},
  data() {
    return {
      isCollapse: false,
      isMenu: false,
      homeList: [
        {
          id: "1",
          name: "单词管理",
          icon: "el-icon-document",
          children: [
            {
              id: "1-1",
              icon: "el-icon-edit-outline",
              name: "添加单词",
              url: "/home/words/add",
            },
            {
              id: "1-2",
              icon: "el-icon-tickets",
              name: "单词列表",
              url: "/home/words/list",
            },

          ],
        },
        {
          id: "2",
          name: "用户管理",
          icon: "el-icon-user",
          children: [
            {
              id: "2-1",
              icon: "el-icon-edit-outline",
              name: "用户列表",
              url: "/home/user/list",
            },
            {
              id: "2-2",
              icon: "el-icon-edit-outline",
              name: "添加用户",
              url: "/home/user/add",
            },
          ],
        },
        {
          id: "3",
          name: "阅读理解",
          icon: "el-icon-link",
          children: [
            {
              id: "3-1",
              icon: "el-icon-edit-outline",
              name: "添加阅读理解",
              url: "/home/passage/add",
            },
            {
              id: "3-2",
              icon: "el-icon-edit-outline",
              name: "阅读理解列表",
              url: "/home/passage/list",
            }
          ],
        },
        {
          id: "4",
          name: "每日英语",
          icon: "el-icon-location-outline",
          children: [
            /*{
              id: "4-1",
              icon: "el-icon-edit-outline",
              name: "添加每日英语",
              url: "/home/everyday/add",
            },*/
            {
              id: "4-2",
              icon: "el-icon-edit-outline",
              name: "每日英语列表",
              url: "/home/everyday/list",
            },
          ],
        },
        {
          id: "5",
          name: "听力管理",
          icon: "el-icon-chat-dot-round",
          children: [
            {
              id: "5-2",
              icon: "el-icon-edit-outline",
              name: "听力列表",
              url: "/home/listening/list",
            }

          ],
        },
        {
          id: "6",
          name: "消息/资源",
          icon: "el-icon-chat-dot-round",
          children: [
            {
              id: "6-1",
              icon: "el-icon-edit-outline",
              name: "系统消息",
              url: "/home/sys_msg/list",
            },
            {
              id: "6-2",
              icon: "el-icon-edit-outline",
              name: "评论区",
              url: "/home/comment/list",
            },
            {
              id: "6-3",
              icon: "el-icon-edit-outline",
              name: "资源",
              url: "/home/resource/list",
            }

          ],
        }
      ],
      avatar_img:"",
    };
  },

  methods: {
    toggleMenu() {
      this.isMenu = !this.isMenu;
    },
    // 退出登录
    logOut() {
      this.HttpPost("/user/logout",{
        "token":localStorage.getItem("token")
      })
      localStorage.clear(); // 不管服务器是否异常，总之就是要退出
      this.$router.push("/login");
    },
    handleOpen(key, keyPath) {
      console.log(key, keyPath);
    },
    handleClose(key, keyPath) {
      console.log(key, keyPath);
    },
    getUserInfo(){
      // 获取用户用户信息
      let userId = localStorage.getItem("userId");
      this.HttpGet("/user/" + userId)
          .then(res => {
            this.avatar_img = res.data.image;
            localStorage.setItem("avatarImg",res.data.image);
          })
    },
    // 前往个人中心
    getProfile() {
      // 跳转到修改页面，并且开启新的页面
      let newUrl = this.$router.resolve({
        path: '/profile/' + localStorage.getItem("userId"),

      });
      window.open(newUrl.href, "_blank");
    }
  },
  created() {
    if(localStorage.getItem("userId") === null) {
      this.$router.push("/login");
    }else {
      this.getUserInfo();
    }
  },
  mounted() {},
  computed:{
    getCurrentYear() {
      var date = new Date();
      return date.getFullYear();
    }
  }
};
</script>
<style lang="stylus" scoped>
@import '~@/assets/style/home.styl';

.logo >>> .el-tooltip {
  padding: 0px !important;
}
>>>.v-note-wrapper.fullscreen{
    z-index: 1700!important;
  }

.active {
  acitve();
}
</style>
<style  scoped>
.admin {
  width: 40px;
  height: 40px;
  position: absolute;
  right: 30px;
  top: 10px;
}
.admin-ul li {
  height: 35px;
  text-align: center;
  line-height: 35px;
  transition: all 0.3s;
}
.admin-ul li:hover {
  cursor: pointer;
  color: #21d4fd;
}
.el-header,
.el-footer {
  background-color: #b3c0d1;
  color: #333;
  line-height: 60px;
}

.el-aside {
  background-color: #d3dce6;
  color: #333;
  text-align: center;
  line-height: 200px;
}

.el-menu-vertical-demo:not(.el-menu--collapse) {
  width: 200px;
  min-height: 400px;
}

body > .el-container {
  margin-bottom: 40px;
}

.el-container:nth-child(5) .el-aside,
.el-container:nth-child(6) .el-aside {
  line-height: 260px;
}

.el-container:nth-child(7) .el-aside {
  line-height: 320px;
}
.menu {
  cursor: pointer;
  width: 25px;
  height: 25px;
  margin: 17.5px 0;
  margin-left: 64px;
  transition: all 0.4s;
}
.content {
  margin: 0;
  transition: all 0.4s;
  margin-left:64px
}

.bar1,
.bar2,
.bar3 {
  width: 25px;
  height: 4px;
  background-color: #fff;
  margin: 4px 0;
  transition: 0.4s;
}

.change .bar1 {
  transform: rotate(45deg) translate(4px, 7.2px);
}

.change .bar2 {
  opacity: 0;
}

.change .bar3 {
  transform: rotate(-45deg) translate(4px, -7.2px);
}
 .el-menu-vertical-demo {
    position: fixed;
    top: 0px;
    left: 0;
    bottom: 0;
    z-index: 1600;
  }
  .el-header{
    position: fixed;
    top: 0;
    left: 0;
    z-index:1550;
    width:100%
  }
  .is-left {
    transition: all 0.4s;
    margin-left: 200px;
  }
  .is-left .menu{
    margin-left: 200px;
  }
  
@media (max-width: 768px) {
  .el-menu--collapse {
    width: 0px !important;
    overflow: hidden;
  }
  /* .content{
  } */
  .is-margin {
    transition: all 0.4s;
    margin-left:0px!important
  }
  .menu{
    margin-left: 0;
    transition: all 0.4s;
  }
}
</style>
