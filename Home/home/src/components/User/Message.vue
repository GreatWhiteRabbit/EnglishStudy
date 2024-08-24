<template>
<div>
  <el-row style="margin-bottom: 10px;color: #7c7878;font-size: 20px;">
    系统消息
  </el-row>
  <el-row v-show="message.length === 0">
    <el-empty description="暂无数据"></el-empty>
  </el-row>
  <el-row v-for="(item,index) in message" :key="item.receiveId">
    <el-col :span="20">
      <el-row>
        <span style="font-size: 20px">{{item.content}}</span>
      </el-row>
      <el-row style="margin-top: 10px;color: #7c7878">
        <span>{{item.title}}</span>
        <span style="margin-left: 20px">{{getTime(item.time)}}</span>
        <el-divider></el-divider>
      </el-row>
    </el-col>
    <el-col :span="2">
      <el-button @click="readSystemMessage(index)" type="primary" v-show="item.read === 0">已读</el-button>
    </el-col>
  </el-row>
  <el-row v-show="message.length !== 0">
    <el-row v-if="!noMoreMessage">
      <span @click="nextPageMessage"  style="
      user-select: none; -webkit-user-select: none;-moz-user-select: none;-ms-user-select:none;
      font-size: 25px;margin-left: 40%;color: #f56c6c">查看更多(o>ε(o>ｕ(≧∩≦)</span>
    </el-row>
    <el-row  v-else>
      <span style="font-size: 25px;margin-left: 40%;color: #f56c6c">没有更多了 ╮(╯▽╰)╭</span>
    </el-row>
  </el-row>

</div>
</template>

<script>
export default {
  name: "Message",
  data() {
    return {
      message:[],
      currentPage:1,
      currentSize:10,
      noMoreMessage:false,// 没有更多消息了
    }
  },
  methods:{
    // 获取系统消息
    getMessage() {
      let userId = localStorage.getItem("userId");
      this.HttpGet('/sys_msg/notify/' + userId + '/' + this.currentPage + '/' + this.currentSize)
          .then(res => {
            if(res.data.length === 0) {
              this.noMoreMessage = true;
            }
            else {
              for (let i = 0; i < res.data.length; i++) {
                this.message.push(res.data[i]);
              }
            }
          })
    },
    // 将系统消息设置为已读
    readSystemMessage(index) {
      let userId = localStorage.getItem("userId");
      this.HttpPost('/sys_msg/read/' + userId + '/' + this.message[index].receiveId)
          .then(res => {
            if(res.data === true) {
              this.message[index].read = 1;
            }
          })
    },
    // 获取更多系统消息
    nextPageMessage() {
      this.currentPage += 1;
      this.getMessage();
    }

  },
  created() {
    this.getMessage();
  },
  computed:{
    // 将时间戳转换为几小时前
    getTime() {
      return (time) => {
        const now = new Date();
        const date = new Date(time);
        const diff = (now - date)/1000; // 毫秒转换为秒
        if (diff < 60) {
          return "刚刚";
        } else if (diff < 3600) {
          return Math.floor(diff / 60) + "分钟前";
        } else if (diff < 86400) {
          return Math.floor(diff / 3600) + "小时前";
        } else if (diff < 2592000) {
          return Math.floor(diff / 86400) + "天前";
        } else if(diff < 2592000 * 12) {
          return Math.floor(diff / 2592000) + "月前";
        }
        else {
          const year = date.getFullYear();
          const month = date.getMonth() + 1;
          const day = date.getDate();
          return `${year}-${month < 10 ? '0' + month : month}-${day < 10 ? '0' + day : day}`;
        }
      }
    }
  }
}
</script>

<style scoped>

</style>