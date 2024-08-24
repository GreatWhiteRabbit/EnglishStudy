<template>
  <div>
    <el-skeleton :rows="30" animated v-show="loadShow"/>
    <!--  显示听力内容-->
    <el-container>
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
        <!--没有做过题目显示如下内容-->
        <el-main v-if="!isDone">
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
                          <sapn>第{{ getIndex(index1, index2) + 1 }}题 </sapn>
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
                    </el-row>
                  </div>
                </div>
              </el-col>
            </el-row>
          </div>
        </el-main>
<!--        做过题目显示如下内容-->
        <el-main v-else>
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
                        <my-select :question="question" :question-answer="answer[getIndex(index1,index2)].answer"
                                   :your-answer="record.list.list[getIndex(index1,index2)].answer"/>
                        <el-row style="margin-top: 20px; text-align: center;color: #51cacc">
                          <div @click="showAnswerButton(getIndex(index1,index2))">
                            <el-col >
                              点击查看答案
                              <i class="el-icon-caret-bottom el-icon-right"></i>
                            </el-col>
                          </div>

                        </el-row>
                        <el-row v-show="answerShow[getIndex(index1,index2)] === true">
                          <el-row >
                            <el-col style="font-size: 18px; margin-top: 20px;line-height: 1.5;">答案: {{answer[getIndex(index1,index2)].answer}}</el-col>
                          </el-row>
                        </el-row>
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
            <el-button v-show="!isDone" @click="beforeSubmit" type="primary">提交</el-button>
            <el-button @click="deleteRecordById" v-show="isDone" type="danger" icon="el-icon-delete">重做</el-button>
          </el-col>
        </el-row>
        <el-row>
          <!--          没有做过题目显示下面的按钮样式-->
          <div v-if="!isDone" class="selectButton">
            <div style="width: 70px;height: 50px" v-for="item in questionCount" :key="item">
              <el-button v-if="radio[item-1]" type="success" @click="itemButton(item)">{{ item }}</el-button>
              <el-button v-else @click="itemButton(item)">{{ item }}</el-button>
            </div>
          </div>
          <!--          做过题目显示下面的按钮样式-->
          <div v-else class="selectButton">
            <div style="width: 70px;height: 50px" v-for="item in questionCount" :key="item">
              <el-button v-if="answerIsTrue(item-1)" type="success" @click="itemButton(item)">{{ item }}</el-button>
              <el-button v-else type="danger" @click="itemButton(item)">{{ item }}</el-button>
            </div>
          </div>
        </el-row>
        <el-row v-show="isDone" style="color: gray;font-size: 18px;">
          <el-col>
            <span>
              上次做题时间
              <i class="el-icon-time el-icon--right"></i>
              {{ timeChange(record.time) }}
            </span>
          </el-col>
        </el-row>
        <el-row v-show="isDone" style="color: black; font-size: 20px;margin-top: 10px;">
          <el-col>
            <span>
              准确率：
              {{ record.accuracy * 100 }}%
            </span>
          </el-col>
        </el-row>
      </el-aside>
    </el-container>
  </div>
</template>

<script>
/*
* 阅读理解试题详情
* */

import MySelect from "@/components/MySelect";

export default {

  name: "ListeningDetail",
  components: {MySelect},
  data() {
    return {
      listeningPaperId: -1,
      paper: {},
      loadShow: true, // 加载,
      questionCount: 0, // 选择题的总数,
      radio: [],
      isDone: false, // 判断是否已经做过这份试题
      record: {}, // 做题记录,
      answer: [], // 答案
      answerShow: [] // 显示答案
    }
  },
  methods: {
    // 获取听力试题详情
    getListeningById() {
      this.HttpGet('/paper/' + this.listeningPaperId)
          .then(res => {
            if (res.success === false) {
              this.$router.push('/404');
            } else {
              this.paper = res.data;
              // 修改页面的title
              document.title = res.data.paperTitle + "-英语学习平台";
              // 获取题目的总数
              this.questionCount = 0;
              for (let i = 0; i < res.data.partList.length; i++) {
                this.questionCount += res.data.partList[i].listeningQuestionList.length;
              }
              this.radio = [];
              // 设置radio的个数
              for (let i = 0; i < this.questionCount; i++) {
                this.radio.push("");
              }
              // 获取答案 注：因为有点懒了，写后端代码时不想写的太过麻烦，所以直接把答案和题目一同传过来了
              this.answer = [];
              for(let i = 0; i < res.data.partList.length; i++) {
                for(let j = 0; j < res.data.partList[i].listeningQuestionList.length; j++) {
                  let answer = {
                    answer:res.data.partList[i].listeningQuestionList[j].answer
                  };
                  this.answer.push(answer);
                }
              }
            }
            this.loadShow = false;
          })
    },
    // 获取做题记录
    getListeningRecord() {
      if (localStorage.getItem("userId") === null && localStorage.getItem("token") === null) {
        console.log("用户未登录，不能获取听力做题记录");
      } else {
        let userId = localStorage.getItem("userId");
        // 从服务器中获取数据
        this.HttpGet('/paperRecord/paper/' + userId + '/' + this.listeningPaperId)
            .then(res => {
              if (res.success === false) {
                console.log("用户还未做过阅读理解");
              } else {
                this.record = res.data;
                this.isDone = true;
              }
            })
      }
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
    },
    // 提交答题记录到服务器前的操作
    beforeSubmit() {
      // 先判断用户是否登录，如果用户没有登录，跳转到登录页面
      if (localStorage.getItem("userId") === null || localStorage.getItem("token") === null) {
        this.$router.push('/login');
      }
      // 将用户答题情况提交到服务器
      else {
        // 1.先判断是否已经做过该试题但是还没有删除，那么先删除做题记录再重新提交
        if (this.isDone === true) {
          this.$confirm('此操作将会覆盖原来的答题记录, 是否继续?', '提示', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
          }).then(() => {
            this.deleteRecordById();
            this.submit();
          }).catch(() => {
            console.log("取消覆盖原来的答题记录")
          })
        } else {
          this.submit();
        }
      }
    },
    // 提交到服务器
    submit() {
      // 先判断是否完成的所有的题目答题
      let finish = true;
      for (let i = 0; i < this.radio.length; i++) {
        if (this.radio[i] === "") {
          finish = false;
          break;
        }
      }
      if (finish === false) {
        this.$notify({
          type: 'error',
          message: "还有未完成的题目，请全部完成后提交",
          title: "系统提示"
        });
      } else {
        let userId = localStorage.getItem("userId");
        let list = [];

        for(let i = 0; i < this.paper.partList.length; i++) {
          for(let j = 0; j < this.paper.partList[i].listeningQuestionList.length; j++) {
            let answerDetail = {
              questionId: '',
              answer: ''
            }
            answerDetail.questionId = this.paper.partList[i].listeningQuestionList[j].questionId;
            answerDetail.answer = this.radio[this.getIndex(i,j)];
            list.push(answerDetail);
          }
        }
        // 计算准确率
        let accuracy = this.calculateAccuracy();
        // 将答题情况提交到服务器
        this.HttpPost('/paperRecord/add/' + userId + '/' + this.listeningPaperId + "/" + accuracy,JSON.stringify({list}))
            .then(res => {
              if(res.data === 0) {
                this.$notify({
                  type:"error",
                  message:"服务器繁忙，请稍后再试",
                  title:"系统提示"
                })
              }
              else {
                let recordId = res.data;
                // 获取做题记录
                this.HttpGet('/paperRecord/' + userId + '/' + recordId)
                    .then(res => {
                      if(res.success === false) {
                        console.log("做题情况提交失败，获取做题记录失败");
                      }
                      else {
                        this.record = res.data;
                        this.isDone = true;
                      }
                    })
              }
            })
      }
    },

    // 计算准确率
    calculateAccuracy() {
      let accuracy = 0;
      for(let i = 0; i < this.radio.length; i++) {
        if(this.radio[i] === this.answer[i].answer) {
          accuracy++;
        }
      }
      accuracy = accuracy/this.radio.length;
      return accuracy;
    },

    // 删除做题记录
    deleteRecordById() {
      let userId = localStorage.getItem("userId");
      let list = [];
      list.push(this.record.listeningPaperRecordId);
      this.HttpPost('/paperRecord/delete/' + userId, JSON.stringify({list}))
          .then(res => {
            if (res.data === 0) {
              this.$notify({
                type: 'error',
                message: "服务器繁忙，请稍后",
                title: '系统提示'
              })
            } else {
              this.$notify({
                type: 'success',
                message: "重做设置成功",
                title: "系统提示"
              });
              this.isDone = false;
            }
          })
    },
    // 显示答案
    showAnswerButton(index) {

      this.answerShow = [];
      for (let i = 0; i < this.radio.length; i++) {
        this.answerShow.push(false);
      }
      this.answerShow[index] = true;
    }

  },
  created() {
    this.listeningPaperId = this.$route.params.id;
    this.getListeningById();
    // 获取做题记录
    this.getListeningRecord();
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
    },
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
    },
    // 判断答案是否正确
    answerIsTrue() {
      return (index) => {
        if (this.record.list.list[index].answer === this.answer[index].answer) {
          return true;
        }
        return false;
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
  height: auto !important;
}

.el-aside {
  background-color: #ffffff;
  color: #333;
  text-align: center;
  border-left: #7de0f7;
  border-left-style: solid;
  line-height 100%;
  height 90vh;
}

.el-main {
  color: #333;
  text-align: center;
  line-height: 100%;
  padding: 0 !important;
  height: calc(100vh - 400px);
  margin-left 2vh;
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
  background-color: rgb(245, 245, 255)
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
  padding: 0 0 25px 0;
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