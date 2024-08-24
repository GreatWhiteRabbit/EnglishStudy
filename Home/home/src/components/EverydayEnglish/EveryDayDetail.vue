<template>
  <el-container>
    <el-main class="left">
      <el-row class="header">
        <el-col>{{everyday.title}}</el-col>
        <el-col style="color: gray;font-size: 20px">{{timeChange(everyday.time)}}</el-col>
      </el-row>
      <el-divider></el-divider>
      <el-row class="content">
      <el-col>{{everyday.content}}</el-col>
      </el-row>
    </el-main>
    <el-divider direction="vertical"></el-divider>
    <el-main class="right">
      <el-row class="header" style="color: #fa9a84">
        <el-col>{{everyday.titleTranslations}}</el-col>
      </el-row>
      <el-divider></el-divider>
      <el-row>
        <el-col>{{everyday.translations}}</el-col>
      </el-row>
    </el-main>
  </el-container>
</template>

<script>
export default {
  name: "EveryDayDetail",
  data() {
    return {
      everydayId:-1,
      everyday:{},

    }
  },
  methods:{
    // 根据id获取每日英语
    getEverydayById() {
      this.HttpGet('/everyday/' + this.everydayId)
          .then(res => {
            if(res.success === false) {
              this.$router.push('/404');
            }
            else {
              this.everyday = res.data;
            }
          })
    },
    // 获取并更新记录
    getRecord() {
      if(localStorage.getItem("userId") === null && localStorage.getItem("token") === null) {
        console.log("未登录，不能获取每日英语记录");
      }
      else {
        let UserId = localStorage.getItem("userId");
        this.HttpGet('/everydayRecord/record/' + UserId + '/' + this.everydayId)
            .then(res => {
              if(res.data === 0) {
                console.log("用户还没有记录");
                // 添加记录
                this.HttpPost('/everydayRecord/' + UserId + '/' + this.everydayId)
                    .then(res => {
                      console.log("添加每日英语阅读记录结果" + res.data);
                    })
              }
              // 更新记录
              else {
                let recordId = res.data;
                this.HttpPost('/everydayRecord/update/' + UserId + "/" + recordId)
                    .then(res => {
                      console.log("更新每日英语阅读记录结果" + res.data);
                    })
              }
            })
      }
    },

  },
  created() {
    this.everydayId = this.$route.params.id;
    this.getEverydayById();
    this.getRecord()
  },
  computed:{
    timeChange() {
      return (time) => {
        var date = new Date(time);
        let Y = date.getFullYear() + '-';
        let M = (date.getMonth()+1 < 10 ? '0'+(date.getMonth()+1) : date.getMonth()+1) + '-';
        let D = date.getDate() + ' ';
        let h = date.getHours() + ':';
        let m = date.getMinutes() + ':';
        let s = date.getSeconds();
        return Y + M + D + h + m + s;
      }
    }
  }
}
</script>

<style lang="stylus" scoped>
.el-container {
  margin-left 25vh;
  margin-right 15vh;
  background-color white;
  height calc(100vh - 100px)
}
.el-main {
  text-align left;
}
.header {
  font-size 23px;
  color #51cacc;
  text-align left;
}
.content {
  font-size 20px;
  color #434242;
  line-height 1.5;
  font-family "Arial Narrow";
}
 .left {
   overflow: scroll;
 }
.right {
  overflow: scroll;
}


</style>