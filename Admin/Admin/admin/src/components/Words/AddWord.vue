<template>
  <div>
    <el-row>
      <el-select v-model="value"
                 placeholder="请选择"
      >
        <el-option
            v-for="item in options"
            :key="item.value"
            :label="item.label"
            :value="item.value"
        >
        </el-option>
      </el-select>
      <el-button type="primary" @click="beforeSubmit">提交</el-button>
    </el-row>
    <el-table
        ref="multipleTable"
        :data="wordList"
        tooltip-effect="dark"
        style="width: 100%"
    >

      <el-table-column
          label="单词"
          :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span style="margin-left: 10px" v-if="scope.row.isEdit">{{ scope.row.words }}</span>
          <el-input v-model="wordList[scope.$index].words" placeholder="请输入单词" v-else></el-input>
        </template>
      </el-table-column>

      <el-table-column
          label="音标"
          :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span style="margin-left: 10px"  v-if="scope.row.isEdit">{{ scope.row.phonetic }}</span>
          <el-input v-model="wordList[scope.$index].phonetic" placeholder="请输入音标" v-else></el-input>
        </template>
      </el-table-column>
      <el-table-column
          label="释义"
          :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span style="margin-left: 10px"  v-if="scope.row.isEdit">{{ scope.row.paraphrase }}</span>
          <el-input v-model="wordList[scope.$index].paraphrase" type="textarea"
                    placeholder="请输入释义" v-else></el-input>
        </template>
      </el-table-column>


      <el-table-column label="操作">
        <template slot-scope="scope" >
          <el-button  v-if="scope.row.isEdit === true" type="primary" icon="el-icon-edit" @click="editRow(scope.$index)">编辑</el-button>
          <el-button v-else type="info" icon="el-icon-edit" disabled>编辑</el-button>
          <el-button type="danger" icon="el-icon-delete" @click="deleteRow(scope.$index)">删除</el-button>
          <el-button type="success" @click="savaRow(scope.$index)">保存</el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-row style="background-color: #409EFF" >
      <el-button @click="addRow" style="margin-left: 50%;font-size: 25px;color: white;background-color: #409EFF;border: none">+</el-button>
    </el-row>
  </div>
</template>

<script>


export default {
  name: 'AddWord',
  data() {
    return {
      wordList:[],
      edit:false, // 是否在此页面编辑过
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
      value:1,
    }
  },
  methods: {
    // 添加一行
    addRow() {
      this.edit = true;
      if(this.wordList.length !== 0) {
        let edit = true; // 判断是否存在未编辑的数据
        for(let i = 0; i < this.wordList.length; i++) {
          if(this.wordList[i].isEdit === false) {
            edit = false;
          }
        }
        if(edit === false) {
          this.$notify({
            message:"还有未保存的数据",
            type:'warning'
          })
        }
        else {
          let data = {
            "words":"",
            "paraphrase":"",
            "phonetic":"",
            "isEdit":false
          };
          this.wordList.push(data);
        }
      }
      else {
        let data = {
          "words":"",
          "paraphrase":"",
          "phonetic":"",
          "isEdit":false
        };
        this.wordList.push(data);
      }
    },
    // 删除一行
    deleteRow(row) {
      this.wordList.splice(row,1);
    },
    // 提交到服务器前的提示
    beforeSubmit() {
      let title = "";
      if(this.value === 1) title = "四级";
      else if(this.value === 2) title = "六级";
      else if(this.value === 3) title = "考研";
      this.$confirm('确定上传' + title + "词汇到服务器？", '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        this.submit();
      }).catch(() =>{

      });
    },
    // 提交到服务器
    submit() {
      let wordList = [];
      for(let i = 0; i < this.wordList.length; i++) {
        let data = {};
        data.words = this.wordList[i].words;
        data.paraphrase = this.wordList[i].paraphrase;
        data.phonetic = this.wordList[i].phonetic;
        wordList.push(data);
      }
      // 发送网络请求
      this.HttpPost('/word/addlist/' + this.value,
          JSON.stringify({wordList})
      )
          .then(res => {
            if(res.data === 0) {
              this.$notify({
                message:"添加单词失败，请检查是否单词表已经存在这些词汇",
                title:"系统提示",
                type:"error"
              })
            }
            else {
              this.$notify({
                message:"成功上传了" + res.data +"数据",
                title:"系统提示",
                type:"success"
              });
              this.edit = false;
            }
          })
    },
    // 编辑一行
    editRow(row) {
      this.userList[row].isEdit = false;
    },
    // 保存一行的数据
    savaRow(row) {
      if(this.wordList[row].words === "" || this.wordList[row].paraphrase === ""
      || this.wordList[row].phonetic === "") {
        this.$notify({
          message:"邮箱或密码不能为空",
          type:'warning',
          title:'系统提示'
        })
      }
      else {
        this.wordList[row].isEdit = true;
      }
    }

  },
  components: {},

  beforeRouteLeave (to, from, next) {
    if (this.edit === true) {
      setTimeout(() => {
        this.$confirm('页面内容未保存，是否退出此页面', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }).then(() => {
          next();
        }).catch(() => {
          next(false)
        })
      }, 200);
    } else {
      next();
    }
  }
}
</script>

<style lang="stylus">

</style>


