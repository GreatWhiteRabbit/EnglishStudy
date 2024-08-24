<template>
<div class="main">
  <el-row>
    <el-col :span="12">
      <el-input prefix-icon="el-icon-search" v-model="input" placeholder="请输入单词或者中文释义">
      </el-input>
    </el-col>
    <el-col :span="5">
      <el-button @keyup.enter="enterSearch" @click="searchWord" type="primary" icon="el-icon-search">搜索</el-button>
    </el-col>
    <el-col :span="4">
      <el-button @click="gotoWordCourse" type="info">记忆单词</el-button>
    </el-col>
  </el-row>
  <el-row style="margin-left: 5vh;">
    <el-col :span="12">
      <el-radio-group @change="changeType" v-model="type">
        <el-radio-button label="1">四级</el-radio-button>
        <el-radio-button label="2">六级</el-radio-button>
        <el-radio-button label="3">考研词汇</el-radio-button>
      </el-radio-group>
    </el-col>
  </el-row>
  <el-row style="margin-top: 20px;margin-left: 5vh;">
    <el-col :span="13">
      <div >
        <el-radio-group @change="changeHandler" v-model="init">
          <el-radio-button  v-for="item in initList" :key="item" :label="item" >{{item}}</el-radio-button>
        </el-radio-group>
      </div>
    </el-col>
  </el-row>
  <el-row>
    <el-card class="box-card">
      <el-row v-if="list.length !== 0">
        <div  v-for="item in list" :key="item" style="font-size: 20px;margin-top: 2px">
          <el-row style="border-bottom: 1px solid #dcdfe6">
            <el-col :span="8">{{item.words}}</el-col>
            <el-col :span="8">{{item.phonetic}}</el-col>
            <el-col :span="8">{{item.paraphrase}}</el-col>
          </el-row>
        </div>
      </el-row>
      <el-empty  v-else :image-size="300"></el-empty>
    </el-card>
  </el-row>
  <el-row >
    <el-pagination
        background
        layout="sizes, prev, pager, next"
        :page-size="currentSize"
        :current-page.sync="currentPage"
        @current-change="handleCurrentChange"
        @size-change="handleSizeChange"
        :page-sizes="[10,20,30,50,100]"
        :total="total"
        style="display: flex;justify-content: center;margin-bottom:10px"
    >
    </el-pagination>
  </el-row>
</div>
</template>

<script>
export default {
  // 单词列表页面
  name: "WordList",
  data() {
    return {
      total: 1, // 单词总数
      currentSize: 10, // 当前页码
      currentPage: 1, // 当前页,
      list:[], // 单词集合
      type:1,
      init:'a',
      initList:['a','b','c','d','e','f','g','h','i','j','k','l','m',
      'n','o','p','q','r','s','t','u','v','w','x','y','z'],
      input:''
    }
  },
  methods: {
    // 调整页面尺寸
    handleSizeChange(val) {
      this.currentSize = val;
      // 重新获取数据
      // 判断输入框中的内容是否为空，如果内容为空的话，根据首字母查找单词
      if (this.input === "") {
        this.getWord();
      }
      // 如果不为空，执行搜索功能
      else {
        this.searchWord();
      }
    },
    // 调整页面大小
    handleCurrentChange(val) {
      this.currentPage = val;
      if (this.input === "") {
        this.getWord();
      }
      // 如果不为空，执行搜索功能
      else {
        this.searchWord();
      }
    },

    // 监听首字母变化
    changeHandler(value) {
      // 重新获取数据
      this.getWord();
    },
    // 监听type变化
    changeType(value) {
      // 重新获取数据
      this.getWord();
    },
    // 从服务器中获取数据
    getWord() {
      this.HttpGet('/word/initlist', {
        type: this.type,
        init: this.init,
        page: this.currentPage,
        size: this.currentSize
      }).then(res => {
        this.list = res.data.wordDTOs;
        this.total = res.data.total;
      })
    },
    // 搜索单词
    searchWord() {
      if(this.input === "") {
        console.log("输入框中没有内容");
      }
      else {
        this.HttpGet('word/search',{
          keywords:this.input,
          page:this.currentPage,
          size:this.currentSize
        })
            .then(res => {
              this.list = res.data;
              this.total = 1;
            })
      }
    },
    // 回车搜索
    enterSearch() {
      document.onkeydown = e => {
        //13表示回车键，baseURI是当前页面的地址，为了更严谨，也可以加别的，可以打印e看一下
        if (e.keyCode === 13 && e.target.baseURI.match(/word/)) {
          //回车后执行搜索方法
          this.searchWord();
        }
      }

    },
    // 跳转到记忆单词的页面
    gotoWordCourse() {
      let newUrl = this.$router.resolve({
        path:'/practice',
        query:{
          type:this.type
        }
      });
      window.open(newUrl.href, "_blank");
    }
  },
    created() {
      this.enterSearch();
      this.getWord();
    }

}
</script>

<style lang="stylus" scoped>

.main{
  margin-left 25vh;
  margin-right 15vh;
  background-color: white;
 /* height: 90vh;*/
  box-shadow:
      inset 0 -3em 3em rgba(240, 240, 245, 0.1),
      0 0 0 2px rgb(255, 255, 255),
      0.3em 0.3em 1em rgba(234, 234, 0, 0.3);
}

::v-deep .el-radio-button__inner {
 font-size 20px
}
// 修改激活后的样式
::v-deep .el-radio-button__orig-radio:checked + .el-radio-button__inner {
 font-size 20px
}

</style>