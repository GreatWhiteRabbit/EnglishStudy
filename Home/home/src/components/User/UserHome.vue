<template>
  <div>
    <el-row>
      <el-col :span="4">
        <el-avatar
            style="width: 130px; height: 130px"
            :src="userInfo.image"
            fit="cover"></el-avatar>
      </el-col>
      <el-col style="line-height: 30px;margin-top: 20px;" :span="8">
        <el-row style="font-size: 20px;color: black">
          <span>{{userInfo.nickName}}</span>
        </el-row>
        <el-row style="font-size: 19px;color: #f56c6c">
          <span>email:{{userInfo.email}}</span>
        </el-row>
        <el-row style="font-size: 18px;color: gray">
          <span>UID:{{getUID(userInfo.userId)}}</span>
        </el-row>

      </el-col>
    </el-row>
    <el-divider/>
    <el-row>
      <el-row>
        <el-col :span="20">
          <span>消息</span>
        </el-col>
        <el-col style="color: #3cdee0;  user-select: none; -webkit-user-select: none;-moz-user-select: none;-ms-user-select:none;"
                @click.native="goToMessage" :span="4">
          <i>更多>></i>
        </el-col>
      </el-row>
      <el-row style="margin-top: 10px" v-show="message.length !== 0">
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
      </el-row>
      <el-empty :image-size="100" v-show="message.length === 0" description="暂无消息"></el-empty>
    </el-row>
    <el-divider/>
    <el-row>
      <el-row>
        <el-col :span="20">
          <span>单词</span>
        </el-col>
        <el-col style="color: #3cdee0;  user-select: none; -webkit-user-select: none;-moz-user-select: none;-ms-user-select:none;"
                @click.native="goToWord" :span="4">
          <i>更多>></i>
        </el-col>
      </el-row>
      <el-row v-show="wordRecord.length !== 0">
        <el-row v-for="item in wordRecord" :key="item.words">
          <el-row style="font-size: 18px;">
            <el-col style="color: #7c7878" :span="6">
              <span>{{item.words}}</span>
            </el-col>
            <el-col style="color: #f56c6c" :span="6">
              <span>/{{item.phonetic}}/</span>
            </el-col>
            <el-col style="color: #f19a9a" :span="12">
              <span>{{item.paraphrase}}</span>
            </el-col>
          </el-row>
        </el-row>
      </el-row>
      <el-empty :image-size="100" v-show="wordRecord.length === 0" description="暂无数据"></el-empty>
    </el-row>
    <el-divider/>
    <el-row>
      <el-row>
        <el-col :span="20">
          <span>阅读记录</span>
        </el-col>
        <el-col style="color: #3cdee0;  user-select: none; -webkit-user-select: none;-moz-user-select: none;-ms-user-select:none;"
                @click.native="goToEveryday" :span="4">
          <i>更多>></i>
        </el-col>
      </el-row>
      <el-row v-show="everydayRecord.length !== 0">
        <el-col :span="5" v-for="(item, index) in everydayRecord" :key="item" :offset="index > 0 ? 1 : 1">
          <router-link :to="'/detail/everyday/' + item.everyday_id" target="_blank">
            <el-card :body-style="{ padding: '0px' }" shadow="hover">
              <el-image :src="everydayImage" fit="cover"></el-image>
              <div style="padding: 12px; height: 60px">
                <div style="overflow: hidden;height: 25px;" >
                  <el-popover
                      placement="bottom-start"
                      trigger="hover"
                      :content="item.title">
                    <span slot="reference">{{item.title}}</span>
                  </el-popover>
                </div>
                <div class="bottom clearfix">
                  <time style="color: gray" class="time">{{ getTime(item.time) }}</time>
                </div>
              </div>
            </el-card>
          </router-link>
        </el-col>
      </el-row>
      <el-empty :image-size="100" v-show="everydayRecord.length === 0" description="暂无数据"></el-empty>
    </el-row>
    <el-divider/>
  </div>

</template>

<script>
export default {
  name: "UserHome",
  data() {
    return {
      userInfo:{}, // 个人信息
      message:[], // 系统消息
      wordRecord:[], // 单词记录
      everydayRecord:[],
      everydayImage:require('@/assets/image/everyday.png'),
    }
  },
  methods:{
    // 获取用户信息
      getUserInfo() {
        let userId = localStorage.getItem("userId");
        this.HttpGet("/user/" + userId)
            .then(res => {
              this.userInfo = res.data;
            })
      },
    // 获取系统消息
    getSystemMessage() {
        let userId = localStorage.getItem("userId");
      this.HttpGet('/sys_msg/notify/' + userId + '/1/3')
          .then(res => {
            this.message = res.data;
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
    // 获取单词记录
    getWordRecord() {
      let userId = localStorage.getItem("userId");
      this.HttpGet('/wordRecord/history/' + userId + '/' + 1+ '/' + 1 + '/' + 10)
          .then(res => {
            this.wordRecord = res.data.wordDTOs;
          })
    },
    // 获取阅读记录
    getEverydayRecord() {
      let userId = localStorage.getItem("userId");
      this.HttpGet('/everydayRecord/' + userId + '/' + 1 + '/' + 8)
          .then(res => {
            this.everydayRecord = res.data.recordList;
          })
    },
    // 前往系统消息页面
    goToMessage() {
        this.$router.push('/profile/message/' + localStorage.getItem("userId"));
    },
    // 前往单词记录页面
    goToWord() {
        this.$router.push("/profile/history/word/" + localStorage.getItem("userId"));
    },
    // 前往做题的历史界面
    goToEveryday() {
        this.$router.push("/profile/history/everyday/" + localStorage.getItem("userId"));
    }
  },
  created() {
    this.getUserInfo();
    this.getSystemMessage();
    this.getWordRecord();
    this.getEverydayRecord();
  },
  computed:{
    getUID() {
      return (id) => {
        if(id >=1 && id < 10) return "english" + "000" + id;
        else if(id >= 10 && id < 100) return "english" + "00" + id;
      else if(id >= 100 && id < 1000)  return "english" + "0" + id;
      else if(id >= 1000 && id < 10000)  return "english" + id;
      }
    },
    // 将时间戳转换为几小时前
    getTime() {
      return (time) => {
        const now = new Date();
        const date = new Date(time);
        const diff = (now - date)/1000; // 毫秒转换为秒
        console.log(diff);
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

<style lang="stylus" scoped>

</style>