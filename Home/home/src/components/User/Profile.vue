<template>
  <el-container>
    <el-aside>
      <el-menu
          class="el-menu-vertical-demo"
          :collapse="isCollapse"
          style="overflow-y: auto;overflow-x: hidden;"
          :default-openeds="openAll"
          @close="handleClose"
          ref="menus"
      >
        <el-submenu v-for="item of homeList" :key="item.id" :index="item.id">
          <template slot="title">
            <i :class="item.icon"></i>
            <span slot="title">{{ item.name }}</span>
          </template>
          <router-link tag="span" v-for="value of item.children" :key="value.id" :to="value.url">
            <el-menu-item :index="value.id">
            <span :class="{active:$route.path===value.url}">
              <i :class="value.icon"></i>
              {{ value.name }}
            </span>
            </el-menu-item>
          </router-link>
        </el-submenu>
      </el-menu>
    </el-aside>
    <el-main>
      <router-view></router-view>
    </el-main>
  </el-container>
</template>

<script>

export default {
  name: "Profile",
  data() {
    return {
      isCollapse: false,
      openAll:['1','2','3','4'],
      homeList: [
        {
          id: "1",
          name: "首页",
          icon: "",
          children: [
            {
              id: "1-1",
              name: "首页",
              icon: "el-icon-s-home",
              url:'/profile/home/' + localStorage.getItem("userId")
            }
          ]

        },
        {
          id: "2",
          name: "消息",
          icon: "",
          children: [
            {
              id: "2-1",
              name: "系统消息",
              icon: "el-icon-chat-dot-square",
              url:'/profile/message/' + localStorage.getItem("userId")
            },
            {
              id: "2-2",
              name: "回复我的",
              icon: "el-icon-chat-dot-square",
              url:'/profile/reply/' + localStorage.getItem("userId")
            }
          ]
        },
        {
          id: "3",
          name: "历史记录",
          icon: "el-icon-time",
          children: [
            {
              id: "3-1",
              icon: "iconfont icon-tubiao",
              name: "单词",
              url: "/profile/history/word/" + localStorage.getItem("userId")
            },
            {
              id: "3-2",
              icon: "iconfont icon-tingliwang",
              name: "听力",
              url: "/profile/history/listening/" + localStorage.getItem("userId")
            },
            {
              id: "3-3",
              icon: "iconfont icon-yuedu",
              name: "阅读理解",
              url: "/profile/history/passage/" + localStorage.getItem("userId")
            },
            {
              id: "3-4",
              icon: "iconfont icon-ribao",
              name: "每日新闻",
              url: "/profile/history/everyday/" + localStorage.getItem("userId")
            }
          ],
        },
        {
          id: "4",
          name: "设置",
          icon: "",
          children: [
            {
              id: "4",
              name: "设置",
              icon: "el-icon-setting",
              url:'/profile/setting/' + localStorage.getItem("userId")
            }
          ]
        },
      ],
    };
  },
  methods:{
    handleClose(key, keyPath) {
      this.$refs.menus.open(keyPath);
    },
  }
}
</script>

<style lang="stylus" scoped>
.el-container {
  margin-left: 25vh;
  margin-right: 15vh;
  background-color white;
  height 100%;
}
.el-menu >>> .el-submenu .el-submenu__title .el-submenu__icon-arrow {
  visibility: hidden
}

</style>