<template>
  <div>
    <el-row>
      <el-select v-model="value"
                 placeholder="请选择"
                 @change="valueChange"
      >
        <el-option
            v-for="item in options"
            :key="item.value"
            :label="item.label"
            :value="item.value"
        >
        </el-option>
      </el-select>
      <el-col :span="6">
        <el-input v-model="searchInput" placeholder="单词或中文释义"></el-input>
      </el-col>
      <el-button
          type="danger"
          icon="el-icon-delete"
          size="large"
          @click="remove">删除
      </el-button>
      <el-button type="primary" icon="el-icon-search" @click="search">搜索</el-button>
      <el-button type="primary" @click="addWord">添加单词</el-button>
      <el-button type="info" @click="showTable">上传<i class="el-icon-upload el-icon--right"></i></el-button>
    </el-row>
    <el-table
        ref="multipleTable"
        :data="list"
        tooltip-effect="dark"
        style="width: 100%"
        :default-sort="{prop: 'time', order: 'descending'}"
        @selection-change="handleSelectionChange">
      <el-table-column
          type="selection"
          width="55">
      </el-table-column>
      <el-table-column
          label="ID"
          :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span style="margin-left: 10px">{{ scope.row.wordId }}</span>
        </template>
      </el-table-column>

      <el-table-column
          label="单词"
          :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span style="margin-left: 10px">{{ scope.row.words }}</span>
        </template>
      </el-table-column>

      <el-table-column
          label="音标"
          :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span style="margin-left: 10px">{{ scope.row.phonetic }}</span>
        </template>
      </el-table-column>
      <el-table-column
          label="释义"
          :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span style="margin-left: 10px">{{ scope.row.paraphrase }}</span>
        </template>
      </el-table-column>

      <el-table-column label="操作">
        <template slot-scope="scope">
          <el-button
              @click="update(scope.$index)">修改
          </el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-pagination
        background
        layout="sizes, prev, pager, next"
        :page-size="pageSize"
        :current-page.sync="currentPage"
        @current-change="handleCurrentChange"
        @size-change="handleSizeChange"
        :page-sizes="[10,20,30,50,100]"
        :total="total"
        style="display: flex;justify-content: center;margin-bottom:10px"
    >
    </el-pagination>
    <el-dialog title="上传单词excel文件" :visible.sync="tableShow">
      <el-select v-model="value2"
                 placeholder="请选择"
                 @change="valueChange"
      >
        <el-option
            v-for="item in options2"
            :key="item.value"
            :label="item.label"
            :value="item.value"
        >
        </el-option>
      </el-select>
      <el-form>
        <!--        上传excel文件-->
        <el-upload
            class="upload-demo"
            ref="upload"
            accept=".xls,.xlsx"
            :file-list="fileList"
            :on-change="handleChange"
            :action=uploadURL
            :show-file-list="true"
            :on-success="onSuccess"
            :on-error="onError"
            :headers="Header"
            :name="file"
            :auto-upload="false">
          <el-button slot="trigger" size="small" type="primary">选取文件</el-button>
          <el-button style="margin-left: 10px;" size="small" type="success" @click="beforeSubmit">上传到服务器
          </el-button>
          <el-button @click="cancelUpload">取 消</el-button>

        </el-upload>
      </el-form>

    </el-dialog>

    <el-dialog title="修改单词音标获取释义" :visible.sync="wordTableShow">
      <el-form :model="form">
        <el-form-item label="单词" label-width="100px">
          <span style="margin-left: 10px">{{ form.words }}</span>
        </el-form-item>
        <el-form-item label="音标" label-width="100px" style="height: 60px">
          <el-input v-model="form.phonetic" autocomplete="off" type="textarea"></el-input>
        </el-form-item>
        <el-form-item label="释义" label-width="100px">
          <el-input v-model="form.paraphrase" autocomplete="off" type="textarea"></el-input>
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="wordUpdateCancel">取 消</el-button>
        <el-button type="primary" @click="updateSure">更 新</el-button>

      </div>
    </el-dialog>
  </div>
</template>

<script>
export default {
  name: "WordList",
  data() {
    return {
      options: [
        {
          value: 1,
          label: '四级'
        },
        {
          value: 2,
          label: '六级'
        },
        {
          value: 3,
          label: '考研'
        }
      ],
      // 上传文件时选择类型
      options2: [
        {
          value: 1,
          label: '四级'
        },
        {
          value: 2,
          label: '六级'
        },
        {
          value: 3,
          label: '考研'
        }
      ],
      total: 1, // 单词总数
      page: 1,
      pageSize: 10, // 当前页码
      size: 10,
      currentPage: 1, // 当前页,
      value: 1,
      value2:1,
      list: [],
      tableShow: false, // 上传文件的窗口
      wordTableShow: false,// 修改单词的窗口
      form: {
        words: '',
        paraphrase: '',
        phonetic: ''
      },
      fileList: [],
      Header: {
        Authorization: "Bearer" + " " + localStorage.getItem("token")
      },
      searchInput: "", // 搜索框输入内容,
      index: -1,
      uploadURL:"/apis/word/addfile/",// 文件上传路径
    }
  },
  methods: {
    // 将MySQL中的单词全部放进redis中的hash结构中
    pushAllWord() {
      this.HttpGet("/word/hash")
          .then(res => {
            if (res.success === false) {
              this.$notify({
                message: "服务器内部错误",
                type: "error",
                title: "系统提示"
              })
            }
          })
    },
    // 获取单词
    getWordList() {
      // 获取单词集合
      this.HttpGet('/word/all/' + this.value + '/' + this.currentPage + '/' + this.pageSize)
          .then(res => {
            this.list = res.data;
          })
      // 获取单词总数
      this.HttpGet('/word/sum', {
        "type": this.value
      }).then(res => {
        this.total = res.data;
      })
    },
    // 根据关键字获取单词
    searchByKeywords() {
      this.HttpGet('/word/adminsearch', {
        "keywords": this.searchInput,
        "page": this.currentPage,
        "size": this.pageSize
      }).then(res => {
        this.list = res.data;
        this.total = 1;
      })
    },
    // 监听select中的value的变化
    valueChange() {
      this.search();
    },
    // 选择框被选择时，获取id
    handleSelectionChange(val) {
      this.multipleSelection = val;
    },
    // 更改页码
    handleCurrentChange(page) {
      this.currentPage = page;
      this.search();
    },
    // 更改页面大小
    handleSizeChange(size) {
      this.pageSize = size;
      this.search();
    },
    // 跳出修改单词窗口
    update(index) {
      this.index = index;
      this.wordTableShow = true; // 显示修改单词窗口
      this.tableShow = false; // 将文件上传窗口隐藏
      this.form.words = this.list[index].words;
      this.form.phonetic = this.list[index].phonetic;
      this.form.paraphrase = this.list[index].paraphrase;
    },
    // 取消修改单词
    wordUpdateCancel() {
      this.wordTableShow = false;
      this.index = -1;
    },
    // 将要修改的单词提交到服务器
    updateSure() {
      let wordList = []
      let data = {};
      data.wordId = this.list[this.index].wordId;
      data.words = this.form.words;
      data.phonetic = this.form.phonetic;
      data.paraphrase = this.form.paraphrase;
      data.type = this.list[this.index].type;
      wordList.push(data);
      let type = this.list[this.index].type;
      // 将修改的内容提交到服务器
      this.HttpPost('/word/update/' + type,
        JSON.stringify({wordList})
      ).then(res => {
        if(res.data === -1) {
          this.$notify({
            type:'error',
            message:"修改失败"
          });
        }
        else {
          this.$notify({
            type:'success',
            message:'修改成功'
          });
          this.search();
        }
        this.wordTableShow = false; // 隐藏单词修改窗口
        this.index = -1;
      })
    },
    // 删除单词
    remove() {

      let wordList = [];
      for(let i = 0; i < this.multipleSelection.length; i++) {
        let data = {};
        data.wordId = this.multipleSelection[i].wordId;
        data.words = this.multipleSelection[i].words;
        data.phonetic = this.multipleSelection[i].phonetic;
        data.paraphrase = this.multipleSelection[i].paraphrase;
        data.type = this.multipleSelection[i].type;
        wordList.push(data);
      }
      this.HttpPost('/word/delete/' + this.value,
      JSON.stringify({wordList}))
          .then(res => {
            this.$notify({
              message:"成功删除了" + res.data + "条数据",
              title:"系统提示",
              type:"success"
            });
            this.search();
          })
    },
    // 上传文件前的提示
    beforeSubmit() {
      let title = "";
      if(this.value2 === 1) title = "四级";
      else if(this.value2 === 2) title = "六级";
      else if(this.value2 === 3) title = "考研";
      this.$confirm('确定上传' + title + "词汇文件到服务器？", '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.submit();
      }).catch(() =>{

      });
    },
    // 上传文件
    submit() {
      this.uploadURL = this.uploadURL + this.value2;
      this.$refs.upload.submit();
    },
    // 取消上传文件
    cancelUpload() {
      this.tableShow = false;
    },
    // 添加单词
    addWord() {
      // 跳转到添加单词页面
      this.$router.push("/home/words/add");
    },
    // 显示上传文件窗口
    showTable() {
      this.tableShow = true; // 显示文件上传窗口
      this.wordTableShow = false; // 关闭单词修改窗口
    },
    // 搜索功能
    search() {
      // 如果搜索内容为“”，那么执行getWordList方法
      if (this.searchInput === "") {
        this.getWordList();
      }
      // 如果搜索内容不为“”，那么执行searchByKeywords方法
      else {
        this.searchByKeywords();
      }
    }
    ,
    // 上传单词excel文件
    submitUpload() {
      this.$refs.upload.submit();
    },
    handleChange(file, fileList) {
      if (fileList.length > 0) {
        this.fileList = [fileList[fileList.length - 1]]  // 这一步，是 展示最后一次选择的excel文件
      }
    },
    // 上传成功
    onSuccess() {
      this.$notify({
        message: '上传成功',
        type: "success"
      });
      this.search();
      this.tableShow = false;
    },
    // 上传失败
    onError() {
      this.$notify({
        message: '上传失败',
        type: 'error'
      })
    },
  },
  created() {
    this.search();
  }
}
</script>

<style scoped>

</style>