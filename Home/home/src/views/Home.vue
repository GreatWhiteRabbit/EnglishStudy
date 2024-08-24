<template>
  <div>
    <nav-bar/>
    <el-container>
      <el-main class="main">
        <el-row>
          <el-card class="box-card">
            <div slot="header" class="clearfix">
              <span style="font-size: 20px;color: #51cacc">单词</span>
              <el-button @click="goToWord" style="float: right; padding: 3px 0;font-size: 16px" type="text">更多 >>
              </el-button>
            </div>
            <div v-for="item in wordList" :key="item" style="font-size: 20px;margin-top: 2px">
              <el-row>
                <el-col :span="8">{{ item.words }}</el-col>
                <el-col :span="8">{{ item.phonetic }}</el-col>
                <el-col :span="8">{{ item.paraphrase }}</el-col>
              </el-row>
            </div>
          </el-card>
        </el-row>
        <el-row>
          <el-card class="box-card">
            <div slot="header" class="clearfix">
              <span style="font-size: 20px;color: #51cacc">听力</span>
              <el-button @click="goToListening" style="float: right; padding: 3px 0;font-size: 16px" type="text">更多
                >>
              </el-button>
            </div>
            <el-row>
              <el-col :span="5" v-for="(item, index) in  listeningList" :key="item" :offset="index > 0 ? 1 : 1">
                <router-link :to="'/detail/listening/' + item.listeningPaperId" target="_blank">
                  <el-card :body-style="{ padding: '0px' }" shadow="hover">
                    <img :src="listenImage" class="image">
                    <div style="padding: 12px;height: 32px;overflow: hidden;">
                      <el-popover
                          placement="bottom-start"
                          trigger="hover"
                          :content="item.paperTitle">
                        <span slot="reference" style="overflow: hidden">{{ item.paperTitle }}</span>
                      </el-popover>
                    </div>
                  </el-card>
                </router-link>

              </el-col>
            </el-row>
          </el-card>
        </el-row>
        <el-row>
          <el-card class="box-card">
            <div slot="header" class="clearfix">
              <span style="font-size: 20px;color: #51cacc">阅读理解</span>
              <el-button @click="goToPassage" style="float: right; padding: 3px 0;font-size: 16px" type="text">更多 >>
              </el-button>
            </div>
            <el-row>
              <el-col :span="5" v-for="(item, index) in passageList" :key="item" :offset="index > 0 ? 1 : 1">
                <router-link :to="'/detail/passage/' + item.passageId" target="_blank">
                  <el-card :body-style="{ padding: '0px' }" shadow="hover">
                    <img :src="passageImage" class="image">
                    <div style="padding: 12px;height: 32px;overflow: hidden;">
                      <el-popover
                          placement="bottom-start"
                          trigger="hover"
                          :content="item.title">
                        <span slot="reference" style="overflow: hidden">{{ item.title }}</span>
                      </el-popover>

                    </div>
                  </el-card>
                </router-link>

              </el-col>
            </el-row>
          </el-card>
        </el-row>
        <el-row>
          <el-card class="box-card">
            <div slot="header" class="clearfix">
              <span style="font-size: 20px;color: #51cacc">每日新闻</span>
              <el-button @click="goToEveryday" style="float: right; padding: 3px 0;font-size: 16px" type="text">更多
                >>
              </el-button>
            </div>
            <el-row>
              <el-col :span="5" v-for="(item, index) in everydayList" :key="item" :offset="index > 0 ? 1 : 1">
                <router-link :to="'/detail/everyday/' + item.everyday_id" target="_blank">
                  <el-card :body-style="{ padding: '0px' }" shadow="hover">
                    <img :src="everydayImage" class="image">
                    <div style="padding: 12px; height: 60px">
                      <div style="overflow: hidden;height: 25px;">
                        <el-popover
                            placement="bottom-start"
                            trigger="hover"
                            :content="item.title">
                          <span slot="reference">{{ item.title }}</span>
                        </el-popover>
                      </div>
                      <div class="bottom clearfix">
                        <time class="time">{{ timeChange(item.time) }}</time>
                      </div>
                    </div>
                  </el-card>
                </router-link>
              </el-col>
            </el-row>
          </el-card>
        </el-row>
      </el-main>
      <el-aside class="aside">
      </el-aside>
    </el-container>
  </div>
</template>

<script>
import NavBar from "@/views/NavBar";

export default {
  // 系统主页
  name: "Home",
  components: {NavBar},
  data() {
    return {
      wordList: [], // 获取部分单词列表
      passageList: [], // 获取阅读理解部分列表
      listeningList: [], // 获取听力部分列表
      everydayList: [], // 获取每日英语部分列表
      listenImage:require('@/assets/image/listen.png'),
      passageImage: require('@/assets/image/passage.png'),
      everydayImage: require('@/assets/image/everyday.png')
    }
  },
  methods: {
    // 获取首页显示的单词列表
    getWord() {
      this.HttpGet('/word/initlist', {
        type: 1,
        init: 'a',
        page: 1,
        size: 10
      })
          .then(res => {
            this.wordList = res.data.wordDTOs;
          })
    },
    // 获取首页显示的阅读理解列表
    getPassage() {
      this.HttpGet('/passage/1/8')
          .then(res => {
            this.passageList = res.data.passageDTOList;
          })
    },
    // 获取首页显示的听力列表
    getListening() {
      this.HttpGet('/paper/list/1/8')
          .then(res => {
            this.listeningList = res.data.list;
          })
    },
    // 获取首页显示的每日英语列表
    getEverydayEnglish() {
      this.HttpGet('/everyday/1/8')
          .then(res => {
            this.everydayList = res.data.list;
          })
    },
    // 跳转到单词列表页面
    goToWord() {
      this.$router.push('/word');
    },

    // 跳转到听力列表页面
    goToListening() {
      this.$router.push('/listening');
    },
    // 跳转到阅读理解列表页面
    goToPassage() {
      this.$router.push('/passage');
    },
    // 跳转到每日英语列表页面
    goToEveryday() {
      this.$router.push('/everyday');
    },
    // 跳转到评论区页面
    goToCommunication() {
      this.$router.push('/communication')
    }
  },
  created() {
    this.getWord();
    this.getPassage();
    this.getListening();
    this.getEverydayEnglish();
  },
  computed: {
    timeChange() {
      return (time) => {
        var date = new Date(time);
        let Y = date.getFullYear() + '-';
        let M = (date.getMonth() + 1 < 10 ? '0' + (date.getMonth() + 1) : date.getMonth() + 1) + '-';
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
// card样式
.time {
  font-size: 13px;
  color: #999;
}

.bottom {
  margin-top: 13px;
  line-height: 12px;
}

.button {
  padding: 0;
  float: right;
}

.image {
  width: 100%;
  display: block;
}

.text {
  font-size: 14px;
}

.item {
  margin-bottom: 18px;
}

.clearfix:before,
.clearfix:after {
  display: table;
  content: "";
}

.clearfix:after {
  clear: both
}

.box-card {
  width: 100%;

  flex-wrap wrap;
}


.main {
  margin-left: 25vh;
}

.aside {
  margin-right: 12vh;

}
</style>