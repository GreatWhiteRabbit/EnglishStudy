<template>
  <el-container>
    <el-main v-if="lastRecord.show">
<!--      复习上次记忆的单词-->
      <el-card  class="box-card">
        <div slot="header" class="clearfix">
          <el-button @click="preWord2" :disabled="lastRecord.index === 0" style="float: left; padding: 3px 0;font-size: 45px;" type="text">
            <i class="el-icon-back"></i>
          </el-button>
          <span style="margin-left: 20%;margin-right: 40%;font-size: 25px;color: gray">复习上次{{typeName[type-1]}}单词</span>
          <el-button @click="nextWord2"  style="float: right; padding: 3px 0;font-size: 45px" type="text">
            <i class="el-icon-right"></i>
          </el-button>
        </div>
        <el-row>
          <el-col style="font-size: 25px;color: gray">{{lastRecord.word[lastRecord.index].words}}</el-col>
          <el-col style="font-size: 25px;color: red">/{{lastRecord.word[lastRecord.index].phonetic}}/</el-col>
        </el-row>
        <el-row>
          <el-radio-group v-model="lastRecord.word[lastRecord.index].choice" @change="changeHandler2">
            <el-col style="margin-top: 20px">
              <el-radio-button :label="0">{{lastRecord.word[lastRecord.index].paraphrase_1}}</el-radio-button>
            </el-col>
            <el-col style="margin-top: 20px">
              <el-radio-button :label="1">{{lastRecord.word[lastRecord.index].paraphrase_2}}</el-radio-button>
            </el-col>
            <el-col style="margin-top: 20px">
              <el-radio-button :label="2">{{lastRecord.word[lastRecord.index].paraphrase_3}}</el-radio-button>
            </el-col>
            <el-col style="margin-top: 20px">
              <el-radio-button :label="3">{{lastRecord.word[lastRecord.index].paraphrase_4}}</el-radio-button>
            </el-col>
          </el-radio-group>
        </el-row>
      </el-card>

    </el-main>
    <el-main v-else>
<!--      记忆从服务器中获取的单词-->
      <el-card v-if="!review" class="box-card">
        <div slot="header" class="clearfix">
          <el-button @click="preWord" :disabled="currentIndex === 0" style="float: left; padding: 3px 0;font-size: 45px;" type="text">
            <i class="el-icon-back"></i>
          </el-button>
          <span style="margin-left: 40%;margin-right: 40%;font-size: 25px;color: gray">{{typeName[type-1]}}</span>
          <el-button @click="nextWord"  style="float: right; padding: 3px 0;font-size: 45px" type="text">
            <i class="el-icon-right"></i>
          </el-button>
        </div>
        <el-row>
          <el-col style="font-size: 25px;color: gray">{{practiceList[currentIndex].words}}</el-col>
          <el-col style="font-size: 25px;color: red">/{{practiceList[currentIndex].phonetic}}/</el-col>
        </el-row>
        <el-row>
          <el-radio-group v-model="practiceList[currentIndex].choice" @change="changeHandler">
            <el-col style="margin-top: 20px">
              <el-radio-button :label="practiceList[currentIndex].paraphrase_1"></el-radio-button>
            </el-col>
            <el-col style="margin-top: 20px">
             <el-radio-button :label="practiceList[currentIndex].paraphrase_2"></el-radio-button>
           </el-col>
            <el-col style="margin-top: 20px">
              <el-radio-button :label="practiceList[currentIndex].paraphrase_3"></el-radio-button>
            </el-col>
            <el-col style="margin-top: 20px">
             <el-radio-button :label="practiceList[currentIndex].paraphrase_4"></el-radio-button>
           </el-col>
          </el-radio-group>
        </el-row>
      </el-card>
<!--      重复记忆刚才记忆的单词-->
      <el-card v-else class="box-card">
        <div slot="header" class="clearfix">
          <el-button @click="preErrorWord" :disabled="currentErrorIndex === 0" style="float: left; padding: 3px 0;font-size: 45px;" type="text">
            <i class="el-icon-back"></i>
          </el-button>
          <span style="margin-left: 40%;margin-right: 40%;font-size: 25px;color: gray">{{typeName[type-1]}}</span>
          <el-button @click="nextErrorWord"  style="float: right; padding: 3px 0;font-size: 45px" type="text">
            <i class="el-icon-right"></i>
          </el-button>
        </div>
        <el-row>
          <el-col style="font-size: 25px;color: gray">{{errorList[currentErrorIndex].words}}</el-col>
          <el-col style="font-size: 25px;color: red">/{{errorList[currentErrorIndex].phonetic}}/</el-col>
        </el-row>
        <el-row>
          <el-radio-group v-model="errorList[currentErrorIndex].choice" @change="changeErrorHandler">
            <el-col style="margin-top: 20px">
              <el-radio-button :label="errorList[currentErrorIndex].paraphrase_1"></el-radio-button>
            </el-col>
            <el-col style="margin-top: 20px">
              <el-radio-button :label="errorList[currentErrorIndex].paraphrase_2"></el-radio-button>
            </el-col>
            <el-col style="margin-top: 20px">
              <el-radio-button :label="errorList[currentErrorIndex].paraphrase_3"></el-radio-button>
            </el-col>
            <el-col style="margin-top: 20px">
              <el-radio-button :label="errorList[currentErrorIndex].paraphrase_4"></el-radio-button>
            </el-col>
          </el-radio-group>
        </el-row>
      </el-card>
      <el-row style="margin-left: 45%">
        <el-button type="primary" style="font-size: 20px" @click="addWordRecord">记完了</el-button>
      </el-row>
    </el-main>
  </el-container>
</template>

<script>
export default {
  // 练习单词
  name: "WordPractice",
  data() {
    return {
      userId:-1,
      type:-1,
      wordList:[], // 服务器中获取的数据
      errorList:[], // 要复习的单词
      practiceList:[], // 要练习的单词
      wordRecordList:[], // 要添加到服务器的数据
      size:20,
      lastWordId:0,
      typeName:['四级','六级','考研'],
      currentIndex:0,
      currentErrorIndex:0,
      review:false, // 是否开始复习,
      lastRecord:{
        word:[],
        index:-1,
        show:false
      }, // 上一次记忆的单词记录
    }
  },
  methods: {
    // 获取要记忆的单词
    getWord() {
      this.HttpGet('/wordRecord/' + this.userId + "/" + this.type + '/' + this.size)
          .then(res => {
            for (let i = 0; i < res.data.length; i++) {
              this.wordList.push(res.data[i]);
            }
            this.mixWord();
          })
    },
    // 添加记忆单词记录
    addWordRecord() {
      if(this.lastWordId !== 0) {
        let id = this.wordList[this.lastWordId].id;
        let list = this.wordRecordList;
        this.HttpPost('/wordRecord/add/' + this.userId + '/' + this.type + "/" + id, JSON.stringify({list}))
            .then(res => {
              if (res.data === 0) {
                console.log("添加失败");
              } else {
                console.log("添加成功");

              }
            })
      }
      else {
        console.log("没有记单词");
      }
    },
    // 混合单词中文释义
    mixWord() {
      this.practiceList = [];
      for (let i = 0; i < this.wordList.length; i++) {
        let word = {
          id: '',
          words: '',
          phonetic: '',
          paraphrase_1: '',
          paraphrase_2: '',
          paraphrase_3: '',
          paraphrase_4: '',
          answer: '',
          choice: ''
        }
        word.id = this.wordList[i].id;
        word.words = this.wordList[i].words;
        word.phonetic = this.wordList[i].phonetic;
        word.paraphrase_1 = this.wordList[(i + 1) % this.wordList.length].paraphrase;
        word.paraphrase_2 = this.wordList[(i + 2) % this.wordList.length].paraphrase;
        word.paraphrase_3 = this.wordList[(i + 3) % this.wordList.length].paraphrase;
        word.paraphrase_4 = this.wordList[(i + 4) % this.wordList.length].paraphrase;
        word.answer = i % 4;
        if (i % 4 === 0) word.paraphrase_1 = this.wordList[i].paraphrase;
        else if (i % 4 === 1) word.paraphrase_2 = this.wordList[i].paraphrase;
        else if (i % 4 === 2) word.paraphrase_3 = this.wordList[i].paraphrase;
        else if (i % 4 === 3) word.paraphrase_4 = this.wordList[i].paraphrase;
        this.practiceList.push(word);
      }
    },

    //
    preWord2() {
      this.lastRecord.index -=1;
    },
    //
    nextWord2() {
      this.lastRecord.index +=1;
      if(this.lastRecord.index === this.lastRecord.word.length) {
        this.lastRecord.show = false;
      }
    },
    //
    changeHandler2(value) {
      if(this.lastRecord.word[this.lastRecord.index].answer !== value) {
        this.lastRecord.word.push(this.lastRecord.word[this.lastRecord.index]);
        this.lastRecord.word[this.lastRecord.index+1].choice = "-1";
        this.lastRecord.index += 1;
      }
      else {
        this.nextWord2();
      }
    },


    // 上一个
    preWord() {
      this.currentIndex -= 1;
    },
    // 上一个复习单词
     preErrorWord() {
      this.currentErrorIndex -=1;
  },
    // 下一个复习单词
    nextErrorWord() {
      // 如果复习到最后一个单词，重新从服务器中获取要记忆的单词
      if(this.currentErrorIndex === this.errorList.length - 1) {
        this.size = this.size + 20;
        this.wordList = [];
        this.getWord();
        this.errorList = [];
        this.review = false;
      }
      else {
        this.currentErrorIndex += 1;
      }
    },
    // 下一个
    nextWord() {
    // 如果下标不是最后一个，记忆下一个单词
    if(this.currentIndex !== this.practiceList.length - 1) {
      this.currentIndex +=1;
    }
    // 如果下标是最后一个，那么开始复习刚才记忆的单词
    else {
      // 开始复习单词
      for(let i = 0; i < this.errorList.length; i++) {
        this.errorList[i].choice = "";
      }
      this.review = true;
    }
    },
    changeHandler(value) {
      // 不管选择是否正确，将要复习的单词添加到errorList中
      this.errorList.push(this.practiceList[this.currentIndex]);
      // 添加单词到wordRecord中
      if(this.currentIndex >= this.lastWordId) {
        this.lastWordId = this.currentIndex;
      }
      if(this.wordRecordList.includes(this.practiceList[this.currentIndex].words) === false) {
        this.wordRecordList.push(this.practiceList[this.currentIndex].words);
      }
      // 如果正确，记忆下一个单词
      let answer = this.practiceList[this.currentIndex].answer;
      if(answer === 0) answer = this.practiceList[this.currentIndex].paraphrase_1;
      else if(answer === 1) answer = this.practiceList[this.currentIndex].paraphrase_2;
      else if(answer === 2) answer = this.practiceList[this.currentIndex].paraphrase_3;
      else if(answer === 3) answer = this.practiceList[this.currentIndex].paraphrase_4;
      // 判断答案是否正确
      if(value === answer) {
        this.nextWord();
      }
      else {
        console.log("单词中文释义选择错误");
      }
    },
    changeErrorHandler(value) {
      // 如果正确，复习下一个单词
      let answer = this.errorList[this.currentErrorIndex].answer;
      if(answer === 0) answer = this.errorList[this.currentErrorIndex].paraphrase_1;
      else if(answer === 1) answer = this.errorList[this.currentErrorIndex].paraphrase_2;
      else if(answer === 2) answer = this.errorList[this.currentErrorIndex].paraphrase_3;
      else if(answer === 3) answer = this.errorList[this.currentErrorIndex].paraphrase_4;
      // 判断答案是否正确
      if(value === answer) {
        this.nextErrorWord();
      }
      else {
        console.log("单词中文释义选择错误");
      }
    },
    // 获取上一次记忆的单词
    getLastWordRecord() {
      this.HttpGet('/wordRecord/last/' + this.userId + "/" + this.type)
          .then(res => {
            this.lastRecord.word = [];
              if(res.data.length !== 0) {
                let translation = ["adj.英俊的"," art.一(个) 每一(个)单"," vt. 丢弃 放弃 抛弃 n. 放纵"," n. 能力 能耐 本领"," adv. (在)国外 海外(一般作表语) 到处 到国外 广为流传",
                " adv. 独立地 完全地 绝对地"," adj. 抽象的 理论的 n. 摘要 抽象的东西 vt. 移除 摘要 偷 vi. 做摘要"," n. 私立中学 专科院校 学院 学术 学会",
                " n. 加速 促进 加速度"," adj. 可接受的 合意的 受欢迎的"," n. 进入 接近(的机会) 使用之权 通道 入口 vt. (电脑)存取进入"," a. 意外的 偶然的 附属的 非本质的 n. 偶然 不重要的东西 < 音>变调的临时符号",
                " n. 招待设备 预定铺位 住处 膳宿 适应 和解"," vt. 使一致 调解 赠予 给予 n. 一致 调和 协议 自愿 vi. (与)一致"," n. 记述 解释 根据 理由 帐目 报告 估计 利益 好处 vi. 报账 解释 导致 vt. 把...视为 归咎(于)"," adj. 准确的 正确无误的",
                " adj. 惯常的 习惯的"," n. 完成 成就 成绩 达到"," vt. 占有 取得 获得 学到"," vi. 行动 表演 表现 充当 见效 vt. 表演 扮演 n. 行为 行动法案 （戏剧、歌剧等的）一幕"," n. 活动 活跃 活力 行动 vt. 表演 adj. 厉害",
                " adj. 实际的 真实的 现行的"," adj. 尖的 锐的 敏锐的 敏锐的 激烈的"," vt. 添加 附加 掺加 增加"," n. 地址 住址 致词 讲话 演说 谈吐 (处理问题的)技巧 vt."," vt. 调整 调节 校正 使...适于 vi. 适应",
                " n. 允许进入 承认 许可 入会费"," n. 成年人adj.成年的 成人的 成熟的"," n. 优点 优势 有利条件 好处 vt. 有利于"," n. 劝告 忠告 意见 建议"," adj. 飞机的 航空的 飞行的"," n.慈爱 爱 爱慕 影响 喜爱 感情"," n. 非洲",
                " n. 下午 午后 int. 下午好"," prep. 倚在 倚靠 逆 对着 反对 违背 防御 相比 相对"," n. 代理人 代理商 特工人员 药剂"," n. 极度痛苦 挣扎"," n. 农业 农艺 农学"," abbr. 人工智能= ( Artificial Intelligence )",
                " n. 目标 对准 枪法 vt. 瞄准 针对 vi. 瞄准 针对 致力 旨在打算"," n. 航空公司 航线"," n. 惊恐 惊慌 忧虑 警报 警告 报警器 vt .使...惊慌 警告装报警器"," adj. 活着的 有活力 活跃的",
                " adv. 几乎 差不多 adj. 几乎"," adv. 向前 (与某人)一道 prep. 沿着"," adv. 亦 也 而且 还 同样地"," conj. 尽管 虽然"," adv.完全 总共 总而言之","n.(缩)上午 午前"," n.雄心，抱负，野心，精力 vt.有..野心 追求",
                " n.美洲 美国"," prep在...之中 在...之间(=among)"," n.安培"," n.分析 分解 解析"," adj.古代的 古老的 n.古人 古货币"," n.盎格鲁人"," n.踝 踝关节"];
              let sum = 0;
                for(let i = 0; i < res.data.length; i++) {
                  let practice = {
                    words: '',
                    phonetic: '',
                    paraphrase_1: '',
                    paraphrase_2: '',
                    paraphrase_3: '',
                    paraphrase_4: '',
                    answer: '',
                    choice: ''
                  }
                  practice.words = res.data[i].words;
                  practice.phonetic = res.data[i].phonetic;
                  practice.paraphrase_1 = translation[sum % translation.length];
                  sum++;
                  practice.paraphrase_2 = translation[sum % translation.length];
                  sum++;
                  practice.paraphrase_3 = translation[sum % translation.length];
                  sum++;
                  practice.paraphrase_4 = translation[sum % translation.length];
                  sum++;
                  practice.answer = i % res.data.length;
                  if (i % 4 === 0) practice.paraphrase_1 = res.data[i].paraphrase;
                  else if (i % 4 === 1) practice.paraphrase_2 = res.data[i].paraphrase;
                  else if (i % 4 === 2) practice.paraphrase_3 = res.data[i].paraphrase;
                  else if (i % 4 === 3) practice.paraphrase_4 = res.data[i].paraphrase;
                  this.lastRecord.word.push(practice);
                }
                this.lastRecord.index = 0;
                this.lastRecord.show = true;
              }
          })
    }
  },
  created() {
    this.lastWordId = 0;
    if(localStorage.getItem("userId") === null && localStorage.getItem("token") === null) {
      console.log("未登录，不能练习记忆单词");
      this.$notify({
        type:"info",
        message:"请先登录，之后才能练习单词",
        title:"系统提示"
      })
      this.$router.push('/');
    }
    else {
      this.userId = localStorage.getItem("userId");
      this.type = this.$route.query.type;
      this.getLastWordRecord();
      this.getWord();
      this.$notify({
        title: '友善提示',
        message: '，记忆单词之前会先复习上次记忆的单词，记忆单词之后一定要点击下面的“记完了”按钮，否则将不会上传到系统中',
        duration: 0
      })
    }
  },

  mounted () {
   /* // 监听页面关闭事件
    window.addEventListener( 'beforeunload', e => this.addWordRecord() );
  /!*  // 监听路由变化
    window.addEventListener('hashchange', e => {
      this.addWordRecord();
    })*!/*/

  },


}
</script>

<style lang="stylus" scoped>
.el-container {
  margin-left 25vh;
  margin-right 15vh;
  background-color white;
  height calc(100vh - 100px);
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
  text-align center;
  margin-left 15vh;
  width: 80%;
  height 75vh;
}
.option {
  display: flex;
  flex-direction: column;
}
.option span {
  display: inline-block;
  background-color: #d7d5d5;
  margin-top: 10px;
  width: 80%;
  word-wrap:break-word;
  font-size: 18px;
  word-break:normal;
  border-radius: 10px;
  margin-right: 50px;

}

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
</style>