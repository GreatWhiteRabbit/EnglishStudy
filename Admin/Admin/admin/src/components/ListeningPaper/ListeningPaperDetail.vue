<template>
  <div>
    <el-skeleton :rows="30" animated v-show="loadShow"/>
    <!--  显示听力内容-->
    <el-container>
      <!--  听力主体内容-->
      <el-container>
        <el-header>
          <div class="content">
            <el-row class="container-marked">
              <el-col class="blog-content" :span="22" :offset="1">
                <div>
                  <header class="article-header blog-animation">
                    <el-row>
                      <div class="istitle">
                        <h1
                            class="article-title"
                        >{{ paper.paperTitle }}</h1>
                      </div>
                    </el-row>

                    <el-row style="margin-top: 25px">
                      <el-col :span="12">
                        <audio controls :src="paper.audio"></audio>
                        <!--                        <vue-audio
                                                    :audio-source="paper.audio"
                                                ></vue-audio>-->
                      </el-col>
                      <!--                      <el-col :span="12">
                                              <i class="el-icon-download">
                                                <el-link  style="font-size: 20px" :href="paper.audio" type="primary">下载音频</el-link>
                                              </i>
                                            </el-col>-->
                    </el-row>
                  </header>
                  <hr/>
                </div>
              </el-col>
            </el-row>
          </div>
        </el-header>
<!--样式调到我想吐-->
        <el-main>
          <div class="content" >
            <el-row class="container-marked">
              <el-col class="blog-content" :span="22" :offset="1">
                <div id="write" class="blog-content blog-animation" style="padding-bottom: 20px; text-align: left">
                  <div v-for="(item,index1) in paper.partList" :key="item.partId">
                    <el-row style="color: #f56c6c;margin-top: 20px" >
                      <el-col ><span>{{ item.partTitle }}</span></el-col>
                    </el-row>
                    <el-row  v-for="(question,index2) in item.listeningQuestionList" :key="question.questionId">
                      <el-row style="margin-top: 20px;" :id="setId(index1,index2)">
                        <el-col>
                          <sapn>第{{ getIndex(index1, index2) + 1 }}题 {{ question.title }}</sapn>
                        </el-col>
                      </el-row>
                      <el-row >
                        <el-radio-group v-model="radio[getIndex(index1,index2)]">
                          <el-col style="margin-top: 10px">
                            <el-radio-button  label="A">A {{ question.optionsA }}</el-radio-button>
                          </el-col>
                          <el-col style="margin-top: 10px">
                            <el-radio-button label="B">B {{ question.optionsB }}</el-radio-button>
                          </el-col>
                          <el-col style="margin-top: 10px">
                            <el-radio-button label="C">C {{ question.optionsC }}</el-radio-button>
                          </el-col>
                          <el-col style="margin-top: 10px">
                            <el-radio-button label="D">D {{ question.optionsD }}</el-radio-button>
                          </el-col>
                        </el-radio-group>
                      </el-row>
                      <el-row style="margin-top: 20px;color: #fdbe3d">
                        <el-col>
                          <span>正确答案：{{ question.answer }}</span>
                        </el-col>
                      </el-row>
                    </el-row>
                    <el-row style="margin-top: 20px;line-height: 1.5;color: #3cbfef">
                      <el-col>
                        <span>听力原文： {{ item.originalText }}</span>
                      </el-col>
                    </el-row>
                  </div>
                </div>
              </el-col>
            </el-row>
          </div>
        </el-main>
      </el-container>
      <!--   侧边栏 -->
      <el-aside width="400px">
        <el-row style="margin-top: 100px">
          <el-col>
            <el-button @click="updateListening" type="primary" icon="el-icon-edit">修改</el-button>
            <el-button type="danger" icon="el-icon-delete">删除</el-button>
          </el-col>
        </el-row>
        <el-row>
          <div class="selectButton">
            <div style="width: 70px;height: 50px" v-for="item in questionCount" :key="item">
              <el-button @click="itemButton(item)">{{ item }}</el-button>
            </div>
          </div>

        </el-row>
      </el-aside>
    </el-container>

  </div>
</template>

<script>
/*
* 听力试题详情
* */


export default {

  name: "ListeningPaperDetail",
  data() {
    return {
      listeningPaperId: -1,
      paper: {},
      loadShow: true, // 加载,
      questionCount: 0, // 选择题的总数,
      url: '', // 音频路径,
      radio: []
    }
  },
  methods: {
    // 获取听力试题详情
    getPaperDetailById() {
      this.HttpGet('/paper/' + this.listeningPaperId)
          .then(res => {
            if (res.success === false) {
              this.$router.push('/404');
            } else {
              this.paper = res.data;
              // 加载音频
              this.url = res.data.audio;
              // 获取所有的试题数
              for (let i = 0; i < this.paper.partList.length; i++) {
                this.questionCount += this.paper.partList[i].listeningQuestionList.length;
              }
              // 设置radio的个数
              for (let i = 0; i < this.questionCount; i++) {
                this.radio.push("");
              }
            }
            this.loadShow = false;
          })
    },
    // 修改听力试题的内容
    updateListening() {
    this.$router.push({
      path:'/listening/update/' + this.listeningPaperId,
      params:{
        id:this.listeningPaperId
      }
    })
    },
    // 删除听力试题
    deleteById() {
      this.$confirm('是否删除该试题', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        // 删除试题操作
        this.HttpPost('/paper/delete/' + this.listeningPaperId)
            .then(res => {
              if(res.data === true) {
                this.$notify({
                  title:"系统提示",
                  message:"删除成功",
                  type:'success'
                });
                this.$router.push('/home/listening/list')
              }
              else {
                this.$notify({
                  title:"系统 提示",
                  message:"删除失败",
                  type:'error'
                })
              }
            })
      }).catch(() => {
        console.log("取消删除");
      });
    },
    // 点击右侧的选择题题号跳转到对应的选择题位置
    itemButton(index) {
      //找到锚点id
      let selectId = "question" + index;
      let toElement = document.getElementById(selectId);
      //如果对应id存在，就跳转
      if (selectId) {
        console.log(toElement, "toElement");
        toElement.scrollIntoView({
          block: "start",//默认跳转到顶部
          behavior: "smooth"//滚动的速度
        });
      }

    }
  },
  created() {
    this.listeningPaperId = this.$route.params.id;
    this.getPaperDetailById();
  },
  computed: {
    getIndex() {
      return (index1, index2) => {
        let index = 0;
        for (let i = 0; i < index1; i++) {
          index += this.paper.partList[i].listeningQuestionList.length;
        }
        index += index2;
        return index;
      }
    },
    setId() {
      return (index1, index2) => {
        let index = 0;
        for (let i = 0; i < index1; i++) {
          index += this.paper.partList[i].listeningQuestionList.length;
        }
        index += index2;
        index += 1;
        return "question" + index;
      }
    }
  }
}
</script>

<style lang="stylus" scoped>

.selectButton {
  justify-content center;
  width 390px;
  display flex;
  flex-wrap wrap;
  float left;
  margin-top 20px;
}

.el-header {
  height: 250px !important;
}

.el-aside {
  background-color: #f7f7f9;
  color: #333;
  text-align: center;
  border-left: #21d4fd;
  border-left-style: solid;
  line-height 100%;
  height 100vh;
}

.el-main {
  color: #333;
  text-align: center;
  line-height: 100%;
  padding: 0 !important;
  height: calc(100vh - 250px);
}

/*自定义el-radio-button样式*/
::v-deep .el-radio-button__inner {
  width: 800px;
  height: 40px;
/*  border: 0 !important;*/
  font-size: 18px;
  font-weight: 400;
  color: #1d1c1c;
  line-height: 14px;
  outline: none;
  box-shadow: none;
  text-align: left;
  background-color: rgb(245,245,255)
}
// 修改激活后的样式
::v-deep .el-radio-button__orig-radio:checked + .el-radio-button__inner {
  background: rgb(64, 158, 255);
  border: 0 !important;
  color: white;
  line-height: 14px;
  outline: none;
  box-shadow: none;
  text-align: left;
}

.grid-content {

  border-radius: 4px;
  min-height: 36px;
}

.row-bg {
  padding: 10px 0;
  background-color: #f9fafc;
}

.container >>> .el-divider__text {
  background-color: #f8f8f8;
}

.content {
  padding: 0 0 20px 0;
  font-size 20px;

  .container-marked {
    border-radius: 6px;
    background-color #fff
    margin auto;
    max-width: 1320px;
    width: 95%;

    .blog-info {
      border-left: 3px solid var(--main-6);
      padding-left: 8px;
      border-radius: 4px;
      margin: 30px 0
    }

    .blog-info p {
      text-align: left;
      color: #4d4d4d;
      font-size: 14px;
      margin: 0;
    }


  }
}

@keyframes next {
  0% {
    transform: translateY(3px);
  }

  100% {
    transform: translateY(0px);
  }
}

@media (max-width: 992px) {
  .isimg {
    height: 240px !important;
  }

  .article-title {
    font-size: 22px !important;
  }

  .container-marked {
    width: 100% !important
  }

}


.article-header {
  padding: 30px 0 15px 25px;
  position: relative;
  color: #4d4d4d;
  overflow: hidden;
  border-radius: 4px;
}

.article-title {
  margin: 10px 0;
  text-align: left;
  font-size: 30px;
  font-weight: 700;
}

.article-title:after {
  display none
}

.article-img {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 0;
  filter: blur(6px);
}

.isimg {
  height: 350px;
  border: none;
  text-align: center;
  padding: 0;
}

.istitle {
  position: absolute;
  width: 90%;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  z-index: 2;
  color: #51cacc;

  span {
    color: #51cacc;
  }
}

.archive-article-date {
  color: #ed6d6d;
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  text-align: left;
  font-size 20px;
}

.archive-article-date .icon {
  margin: 5px 5px 5px 0;
}

[class*=' icon-'], [class^=icon-] {
  speak: none;
  font-style: normal;
  font-weight: 400;
  font-variant: normal;
  text-transform: none;
  line-height: 1;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}


</style>