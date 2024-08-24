<template>
  <div>
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
                      <el-col :span="6">
                        <div class="istitle">
                          <h1 v-if="editPaperTitle === false"
                              class="article-title"
                          >{{ paper.paperTitle }}</h1>
                          <el-input placeholder="请输入标题" v-else v-model="input"></el-input>
                        </div>
                      </el-col>
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
          <div class="content">
            <el-row class="container-marked">
              <el-col class="blog-content" :span="22" :offset="1">
                <div id="write" class="blog-content blog-animation" style="padding-bottom: 20px; text-align: left">
                  <div v-for="(item,index1) in paper.partList" :key="item.partId">
                    <el-row style="color: #f56c6c;margin-top: 20px">
                      <el-col :span="18"><span>{{ item.partTitle }}</span></el-col>
                      <el-col :span="1">
                        <el-button @click="showEditDialog(index1)" type="primary" icon="el-icon-edit"
                                   circle></el-button>
                      </el-col>
                      <el-col :span="1">
                        <el-button @click="deletePartById(index1)"  type="danger" icon="el-icon-delete" circle></el-button>
                      </el-col>
                    </el-row>
                    <el-row v-for="(question,index2) in item.listeningQuestionList" :key="question.questionId">
                      <el-row style="margin-top: 20px;" :id="setId(index1,index2)">
                        <el-col :span="18">
                          <sapn>第{{ getIndex(index1, index2) + 1 }}题 {{ question.title }}</sapn>
                        </el-col>
                        <el-col :span="1">
                          <el-button @click="showEditQuestionDialog(index1,index2)"  type="primary" icon="el-icon-edit"
                                     circle></el-button>
                        </el-col>
                        <el-col :span="1">
                          <el-button @click="deleteQuestionById(index1,index2)" type="danger" icon="el-icon-delete" circle></el-button>
                        </el-col>
                        <el-col :span="1">
                          <el-button v-show="index2 === paper.partList[index1].listeningQuestionList.length-1"
                                     @click="showAddQuestionDialog(index1)"
                                     type="info" icon="el-icon-plus" circle></el-button>
                        </el-col>
                      </el-row>
                      <el-row>
                        <el-radio-group v-model="radio[getIndex(index1,index2)]">
                          <el-col style="margin-top: 10px">
                            <el-radio-button label="A">A {{ question.optionsA }}</el-radio-button>
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
            <el-row>
              <el-col :span="12">
                <el-button @click="uploadDialogShow = true"  type="info">上传音频<i
                    class="el-icon-upload el-icon--right"></i>
                </el-button>
              </el-col>
              <el-col :span="8">
                <el-tooltip content="添加题目和小题目" placement="bottom">
                  <el-button @click="addPartDialogShow = true" type="primary">
                    <i class="el-icon-plus"></i>
                  </el-button>
                </el-tooltip>
              </el-col>
            </el-row>
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
    <!-- 显示编辑part的dialog窗口-->
    <el-dialog
        title="修改题目"
        :visible.sync="editorDialogShow"
        width="80%"
        :before-close="handleClose">
      <el-row>
        <el-col :span="14">
          <sapn>{{ editDialogData.partTitle }}</sapn>
        </el-col>
        <el-col :span="6">
          <el-row :gutter="20">
            <el-col :span="14">
              <el-button type="primary" @click="originalTextDialog = true">编辑</el-button>
            </el-col>
          </el-row>
        </el-col>
        <el-dialog
            width="60%"
            title="修改标题和听力原文"
            :visible.sync="originalTextDialog"
            append-to-body>
          <el-form>
            <el-form-item label="标题" label-width="100px">
              <el-input v-model="editDialogData.partTitle" autocomplete="off" type="textarea"></el-input>
            </el-form-item>
            <el-form-item label="听力原文" label-width="100px" style="height: 100%">
              <el-input v-model="editDialogData.originalText" autosize autocomplete="off" type="textarea"></el-input>
            </el-form-item>
          </el-form>
        </el-dialog>
      </el-row>
      <el-row>
        <span>听力原文 {{ editDialogData.originalText }}</span>
      </el-row>
      <span slot="footer" class="dialog-footer">
    <el-button @click="editorDialogShow = false" >取 消</el-button>
    <el-button @click="saveEditDialog" type="primary" >确 定</el-button>
  </span>
    </el-dialog>
    <!-- 显示添加part的dialog窗口-->
    <el-dialog
        title="添加题目"
        :visible.sync="addPartDialogShow"
        width="80%"
        :before-close="handleClose">
      <el-row>
        <el-col :span="14">
          <sapn>{{ addPartDialogData.partTitle }}</sapn>
        </el-col>
        <el-col :span="6">
          <el-row :gutter="20">
            <el-col :span="14">
              <el-button type="primary" @click="originalTextDialog = true">编辑题目和听力原文</el-button>
            </el-col>
            <el-col :span="1">
              <el-button @click="addQuestion2" type="info">添加题目</el-button>
            </el-col>
          </el-row>

        </el-col>
        <el-dialog
            width="60%"
            title="添加标题和听力原文"
            :visible.sync="originalTextDialog"
            append-to-body>
          <el-form>
            <el-form-item label="标题" label-width="100px">
              <el-input v-model="addPartDialogData.partTitle" autocomplete="off" type="textarea"></el-input>
            </el-form-item>
            <el-form-item label="听力原文" label-width="100px" style="height: 100%">
              <el-input v-model="addPartDialogData.originalText" autosize autocomplete="off" type="textarea"></el-input>
            </el-form-item>
          </el-form>
        </el-dialog>
      </el-row>
      <el-table
          ref="multipleTable"
          :data="addPartDialogData.listeningQuestionList"
          tooltip-effect="dark"
          style="width: 100%"
      >

        <el-table-column
            label="题目"
            :show-overflow-tooltip="true"
        >
          <template slot-scope="scope">
            <span v-if="scope.$index !== questionIndex" style="margin-left: 10px">{{ scope.row.title }}</span>
            <el-input v-else v-model="scope.row.title" placeholder="请输入标题"/>
          </template>
        </el-table-column>
        <el-table-column
            label="A选项"
            :show-overflow-tooltip="true"
        >
          <template slot-scope="scope">
            <span v-if="scope.$index !== questionIndex" style="margin-left: 10px">{{ scope.row.optionsA }}</span>
            <el-input v-else v-model="scope.row.optionsA" placeholder="请输入选项A"/>
          </template>
        </el-table-column>
        <el-table-column
            label="B选项"
            :show-overflow-tooltip="true"
        >
          <template slot-scope="scope">
            <span v-if="scope.$index !== questionIndex" style="margin-left: 10px">{{ scope.row.optionsB }}</span>
            <el-input v-else v-model="scope.row.optionsB" placeholder="请输入选项B"/>
          </template>
        </el-table-column>
        <el-table-column
            label="C选项"
            :show-overflow-tooltip="true"
        >
          <template slot-scope="scope">
            <span v-if="scope.$index !== questionIndex" style="margin-left: 10px">{{ scope.row.optionsC }}</span>
            <el-input v-else v-model="scope.row.optionsC" placeholder="请输入选项C"/>
          </template>
        </el-table-column>
        <el-table-column
            label="D选项"
            :show-overflow-tooltip="true"
        >
          <template slot-scope="scope">
            <span v-if="scope.$index !== questionIndex" style="margin-left: 10px">{{ scope.row.optionsD }}</span>
            <el-input v-else v-model="scope.row.optionsD" placeholder="请输入选项D"/>
          </template>
        </el-table-column>
        <el-table-column
            label="正确答案"
            :show-overflow-tooltip="true"
        >
          <template slot-scope="scope">
            <span v-if="scope.$index !== questionIndex" style="margin-left: 10px">{{ scope.row.answer }}</span>
            <el-input v-else v-model="scope.row.answer" placeholder="请输入答案"/>
          </template>
        </el-table-column>

        <el-table-column label="操作">
          <template slot-scope="scope">
            <el-button
                type="primary"
                @click="questionIndex = scope.$index"
            >编辑
            </el-button>
            <el-button
                type="danger" icon="el-icon-delete"
                @click="deleteQuestionByIndex2(scope.$index)"
            >
            </el-button>
          </template>
        </el-table-column>
      </el-table>
      <el-row>
        <span>听力原文 {{ addPartDialogData.originalText }}</span>
      </el-row>
      <span slot="footer" class="dialog-footer">
    <el-button @click="addPartDialogShow = false">取 消</el-button>
    <el-button type="primary" @click="saveEdit2">确 定</el-button>
  </span>
    </el-dialog>
    <!--上传听力音频-->
    <el-dialog title="上传听力音频" :visible.sync="uploadDialogShow">
      <el-form>
        <!--        上传听力音频-->
        <el-upload
            class="upload-demo"
            ref="upload"
            accept=".wma,.mp3,.wav,.flac"
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
          <el-button style="margin-left: 10px;" size="small" type="success" @click="submitUpload">上传到服务器
          </el-button>
          <el-button @click="uploadDialogShow = false">取 消</el-button>

        </el-upload>
      </el-form>

    </el-dialog>
    <!-- 显示编辑question的dialog窗口-->
    <el-dialog
        title="修改问题的内容"
        :visible.sync="editQuestionDialogShow"
        width="80%"
        :before-close="handleClose">
      <el-form :model="editQuestionDialogData">
        <el-form-item label="问题" label-width="100px">
          <el-input v-model="editQuestionDialogData.title" autosize autocomplete="off" type="textarea"></el-input>
        </el-form-item>
        <el-form-item label="A选项" label-width="100px" >
          <el-input v-model="editQuestionDialogData.optionsA" autocomplete="off" type="textarea"></el-input>
        </el-form-item>
        <el-form-item label="B选项" label-width="100px" >
          <el-input v-model="editQuestionDialogData.optionsB" autocomplete="off" type="textarea"></el-input>
        </el-form-item>
        <el-form-item label="C选项" label-width="100px" >
          <el-input v-model="editQuestionDialogData.optionsC" autocomplete="off" type="textarea"></el-input>
        </el-form-item>
        <el-form-item label="D选项" label-width="100px" >
          <el-input v-model="editQuestionDialogData.optionsD" autocomplete="off" type="textarea"></el-input>
        </el-form-item>
        <el-form-item label="正确答案" label-width="100px" >
          <el-input v-model="editQuestionDialogData.answer" autocomplete="off" type="textarea"></el-input>
        </el-form-item>

      </el-form>
      <span slot="footer" class="dialog-footer">
    <el-button @click="editQuestionDialogShow = false" >取 消</el-button>
    <el-button @click="saveEditQuestionDialog" type="primary" >确 定</el-button>
  </span>
    </el-dialog>
    <!-- 添加question的dialog窗口-->
    <el-dialog
        title="添加问题的内容"
        :visible.sync="addQuestionDialogShow"
        width="80%"
        :before-close="handleClose">
      <el-form :model="addQuestionDialogData">
        <el-form-item label="问题" label-width="100px">
          <el-input v-model="addQuestionDialogData.title" autosize autocomplete="off" type="textarea"></el-input>
        </el-form-item>
        <el-form-item label="A选项" label-width="100px" >
          <el-input v-model="addQuestionDialogData.optionsA" autocomplete="off" type="textarea"></el-input>
        </el-form-item>
        <el-form-item label="B选项" label-width="100px" >
          <el-input v-model="addQuestionDialogData.optionsB" autocomplete="off" type="textarea"></el-input>
        </el-form-item>
        <el-form-item label="C选项" label-width="100px" >
          <el-input v-model="addQuestionDialogData.optionsC" autocomplete="off" type="textarea"></el-input>
        </el-form-item>
        <el-form-item label="D选项" label-width="100px" >
          <el-input v-model="addQuestionDialogData.optionsD" autocomplete="off" type="textarea"></el-input>
        </el-form-item>
        <el-form-item label="正确答案" label-width="100px" >
          <el-input v-model="addQuestionDialogData.answer" autocomplete="off" type="textarea"></el-input>
        </el-form-item>

      </el-form>
      <span slot="footer" class="dialog-footer">
    <el-button @click="addQuestionDialogShow = false" >取 消</el-button>
    <el-button @click = "saveAddQuestionDialog" type="primary" >确 定</el-button>
  </span>
    </el-dialog>


  </div>
</template>

<script>
export default {
  name: "UpdateListeningPaper",
  data() {
    return {
      listeningPaperId:-1, // 试题ID
      input: "", // 标题
      editPaperTitle: false, // 修改听力标题
      uploadDialogShow: false, // 上传听力dialog框,
      paper: {}, // 获取到的听力模板内容
      questionCount: 0, // 选择题的总数,
      url: '', // 听力音频路径
      radio: [],
      addQuestionDialogShow:false, // 添加question时的dialog显示框
      addQuestionDialogData:{
        partId:0,
        title:"",
        optionsA:'',
        optionsB:'',
        optionsC:'',
        optionsD:'',
        answer: ""}, // 添加question时dialog中的数据
      editQuestionDialogShow:false, // 编辑question的dialog显示框
      editQuestionDialogData:{
        title:"",
        optionsA:'',
        optionsB:'',
        optionsC:'',
        optionsD:'',
        answer: ""
      },
      editorDialogShow: false, // 编辑的dialog显示框
      editDialogData: {
        partTitle: '',
        originalText: ''
      }, // 当点击编辑时dialog中的数据
      addPartDialogShow: false, // 添加part时的dialog显示框
      addPartDialogData: {
        partTitle: '',
        listeningQuestionList: [],
        originalText: ''
      }, // 当点击添加按钮时显示dialog中的数据
      editIndex: -1, // partList中的index
      questionIndex: -1,// 编辑小题题目的下标
      originalTextDialog: false, // 编辑题目和听力原文的dialog
      addUp: false, // 在这一行之前添加元素标志位
      Header: {
        Authorization: "Bearer" + " " + localStorage.getItem("token")
      },
      uploadURL: '/apis/paper/upload/',
      fileList: [],
    }
  },
  methods: {
    // 根据id获取听力内容
    getPaperDetailById() {
      this.HttpGet('/paper/' + this.listeningPaperId)
          .then(res => {
            if (res.success === false) {
              this.$router.push('/404');
            } else {
              this.paper = res.data;
              // 加载音频
              this.url = res.data.audio;
              this.questionCount = 0;
              // 获取所有的试题数
              for (let i = 0; i < this.paper.partList.length; i++) {
                this.questionCount += this.paper.partList[i].listeningQuestionList.length;
              }
              this.radio = [];
              // 设置radio的个数
              for (let i = 0; i < this.questionCount; i++) {
                this.radio.push("");
              }
            }
            this.loadShow = false;

          })
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

    // 修改试题的标题
    updatePaperTitle() {
      this.input = this.paper.paperTitle;
      this.editPaperTitle = true;
    },
    // 将修改标题的内容提交到服务器
    submitUpdatePaperTitle() {
      this.editPaperTitle = false;
      if(this.input === "") {
        this.$message({
          type:"error",
          message:"标题不能为空"
        })
      }
      else {
        this.HttpPost("/paper/update/paper", {
          id: this.listeningPaperId,
          paperTitle: this.input
        }).then(res => {
            if(res.data === true) {
              this.$message({
                type:'success',
                message:"修改成功"
              });
              this.paper.paperTitle = this.input;
            }
        })
      }
    },

    // 删除part
    deletePartById(index) {
      this.$confirm('此操作将会删除此部分的所有题目', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        // 执行删除操作
        let partId = this.paper.partList[index].partId;
        this.HttpPost('/paper/delete/part/' + this.listeningPaperId + "/" + partId)
            .then(res => {
              if(res.data === true) {
                this.$notify({
                  type:"success",
                  message:"删除成功"
                });
                // 删除paper中的数据
                this.paper.partList.splice(this.editIndex,1);
                // 刷新题目总数
                this.updateCountAndArray();
              }
              else {
                this.$notify({
                  type:'error',
                  message:"删除失败，服务器繁忙，请稍后再试"
                });
              }
            })
      }).catch(() => {
        console.log("取消删除part");
      });
    },

    // 显示编辑dialog窗口
    showEditDialog(index) {
      this.editIndex = index;
      this.editorDialogShow = true;
      this.editDialogData.partTitle = this.paper.partList[index].partTitle;
      this.editDialogData.originalText = this.paper.partList[index].originalText;
    },
    // 将编辑的内容提交到服务器
    saveEditDialog() {
      // 先判断编辑前后的值是否一致，如果一致，不提交到服务器
      if(this.editDialogData.partTitle === this.paper.partList[this.editIndex].partTitle
      && this.editDialogData.originalText === this.paper.partList[this.editIndex].originalText) {
        console.log("编辑前后内容一致");
        this.editorDialogShow = false;
      }
      else {
        let partId = this.paper.partList[this.editIndex].partId;
        // 将修改的内容提交到服务器中
        this.HttpPost("/paper/update/part/" + this.listeningPaperId + "/" + partId, {
          originalText:this.editDialogData.originalText,
          partTitle:this.editDialogData.partTitle
        } )
            .then(res => {
              if(res.data === true) {
                this.$notify({
                  type:"success",
                  message:"修改成功"
                });
                // 直接修改值，不从服务器中获取数据，防止对服务器造成很大的压力
                this.paper.partList[this.editIndex].partTitle = this.editDialogData.partTitle;
                this.paper.partList[this.editIndex].originalText = this.editDialogData.originalText;
              }
              else {
                this.$notify({
                  type:"error",
                  message:"修改失败"
                });
              }
              this.editorDialogShow = false;
            })
      }
    },

    // 删除题目
    deleteQuestionById(index1,index2) {
      this.$confirm('确定删除改题目', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        // 先判断该part是否只存在这唯一的一个题目，如果是唯一的题目，那么删除part，如果不是，删除question
        if(this.paper.partList[index1].listeningQuestionList.length === 1) {
          // 执行删除part操作
          let partId = this.paper.partList[index1].partId;
          this.HttpPost("/paper/delete/part/" + this.listeningPaperId + "/" + partId)
              .then(res => {
                if(res.data === true) {
                  this.$notify({
                    type:"success",
                    message:"删除成功"
                  });
                  // 删除paper中的数据
                  this.paper.partList.splice(index1,1);
                  // 刷新题目总数
                  this.updateCountAndArray();
                }
                else {
                  this.$notify({
                    type:'error',
                    message:"删除失败，服务器繁忙，请稍后再试"
                  })
                }
              })
        }
        else {
          // 执行删除question操作
          let partId = this.paper.partList[index1].partId;
          let questionId = this.paper.partList[index1].listeningQuestionList[index2].questionId;
          this.HttpPost('/paper/delete/question/' + this.listeningPaperId + "/" + partId  + "/" + questionId)
              .then(res => {
                if(res.data === true) {
                  this.$notify({
                    type:'success',
                    message:"删除成功"
                  });
                  // 删除list中的数据
                  this.paper.partList[index1].listeningQuestionList.splice(index2,1);
                  // 更新题目总数
                  this.updateCountAndArray();
                }
              })
        }

      }).catch(() => {
       console.log("取消删除题目操作");
      });
    },

    // 显示添加question的dialog
    showAddQuestionDialog(index){
      this.editIndex = index;
      this.addQuestionDialogShow = true;
    },
    // 将添加后的question上传到服务器
    saveAddQuestionDialog() {
      // 先判断是否题目信息是否完整
      if(this.addQuestionDialogData.title === "" || this.addQuestionDialogData.optionsA === ""
      || this.addQuestionDialogData.optionsB === "" || this.addQuestionDialogData.optionsC === ""
      || this.addQuestionDialogData.optionsD === "" || this.addQuestionDialogData.answer === "") {
        this.$message({
          type:'error',
          message:"请将题目信息填写完整"
        });
      }
      // 将添加的数据上传到服务器
      else {
        let partId = this.paper.partList[this.editIndex].partId;
        this.addQuestionDialogData.partId = partId;
        this.HttpPost('/paper/add/question/' + this.listeningPaperId,this.addQuestionDialogData)
            .then(res => {
              if(res.data === true) {
                this.$notify({
                  type:"success",
                  message:"添加成功"
                });
                // 重新加载数据
                this.getPaperDetailById();
              }
              else {
                this.$notify({
                  type:'error',
                  message:"服务器繁忙，请稍后再试"
                });
              }
              this.addQuestionDialogShow = false;
            })
      }


    },

    // 显示编辑题目窗口
    showEditQuestionDialog(index1,index2) {
      this.editQuestionDialogShow = true;
      this.editIndex = index1;
      this.questionIndex = index2;
      let question = this.paper.partList[index1].listeningQuestionList[index2];
      this.editQuestionDialogData.title =  question.title ;
      this.editQuestionDialogData.optionsA = question.optionsA;
      this.editQuestionDialogData.optionsB = question.optionsB;
      this.editQuestionDialogData.optionsC = question.optionsC;
      this.editQuestionDialogData.optionsD = question.optionsD;
      this.editQuestionDialogData.answer = question.answer;
    },
    // 将编辑后的题目内容提交到服务器中
    saveEditQuestionDialog() {
      // 先判断前后内容是否一致
      let question = this.paper.partList[this.editIndex].listeningQuestionList[this.questionIndex];
      if(this.editQuestionDialogData.title === question.title
      && this.editQuestionDialogData.optionsA === question.optionsA
      && this.editQuestionDialogData.optionsB === question.optionsB
      && this.editQuestionDialogData.optionsC === question.optionsC
      && this.editQuestionDialogData.optionsD === question.optionsD
      && this.editQuestionDialogData.answer === question.answer) {
        console.log("题目编辑前后内容一致");
        this.editQuestionDialogShow = false;
      }
      // 将修改的内容提交到服务器中
      else {
        let partId = this.paper.partList[this.editIndex].partId;
        let questionId = this.paper.partList[this.editIndex].listeningQuestionList[this.questionIndex].questionId;
        this.HttpPost('/paper/update/question/' + this.listeningPaperId + "/" + partId + "/" + questionId,{
          title:this.editQuestionDialogData.title,
          optionsA:this.editQuestionDialogData.optionsA,
          optionsB:this.editQuestionDialogData.optionsB,
          optionsC:this.editQuestionDialogData.optionsC,
          optionsD:this.editQuestionDialogData.optionsD,
          answer:this.editQuestionDialogData.answer
        }).then(res => {
          if(res.data === true) {
            this.$notify({
              type:'success',
              message:"修改成功"
            });
            // 修改页面内容
            this.paper.partList[this.editIndex].listeningQuestionList[this.questionIndex].title = this.editQuestionDialogData.title;
            this.paper.partList[this.editIndex].listeningQuestionList[this.questionIndex].optionsA = this.editQuestionDialogData.optionsA;
            this.paper.partList[this.editIndex].listeningQuestionList[this.questionIndex].optionsB = this.editQuestionDialogData.optionsB;
            this.paper.partList[this.editIndex].listeningQuestionList[this.questionIndex].optionsC = this.editQuestionDialogData.optionsC;
            this.paper.partList[this.editIndex].listeningQuestionList[this.questionIndex].optionsD = this.editQuestionDialogData.optionsD;
            this.paper.partList[this.editIndex].listeningQuestionList[this.questionIndex].answer = this.editQuestionDialogData.answer;
          }
          else {
            this.$notify({
              type:'error',
              message:"修改失败"
            });
          }
          this.editQuestionDialogShow = false;
        })
      }
    },

    // 点击dialog的外面或者X号时跳出提示，element-ui自带的
    handleClose(done) {
      this.$confirm('还有信息未保存，确认关闭？')
          .then(_ => {
            done();
          })
          .catch(_ => {
          });
    },

    // 在添加dialog中删除小题目
    deleteQuestionByIndex2(index) {
      this.addPartDialogData.listeningQuestionList.splice(index, 1);
    },
    // 在添加dialog窗口中添加小题目
    addQuestion2() {
      let question = {
        title: "",
        optionsA: '',
        optionsB: '',
        optionsC: '',
        optionsD: '',
        answer: ''
      };
      this.addPartDialogData.listeningQuestionList.push(question);
      this.questionIndex = this.addPartDialogData.listeningQuestionList.length - 1;
    },

    // 添加dialog保存编辑操作
    saveEdit2() {
      let partCondition = true,lengthCondition = true,questionCondition = true;
      // 先检查题目和听力原文是否缺失
      if(this.addPartDialogData.partTitle === "" || this.addPartDialogData.originalText === "") {
        this.$message({
          type:'error',
          message:"标题或者听力原文内容不能为空"
        });
        partCondition = false;
      }
      // 检查questionList是否为空
      if(this.addPartDialogData.listeningQuestionList.length === 0) {
        this.$message({
          type:'error',
          message:"题目还没有添加！请添加题目"
        });
        lengthCondition = false;
      }
      // 检查questionList中的信息是否完整
      for(let i = 0; i < this.addPartDialogData.listeningQuestionList.length; i++) {
        let question = this.addPartDialogData.listeningQuestionList[i];
        if(question.title === "" || question.optionsA === "" || question.optionsB === ""
        || question.optionsC === "" || question.optionsD === "" || question.answer === "") {
          this.$message({
            type:'error',
            message:"题目中存在不完善的信息！请先完善题目信息"
          });
          questionCondition = false;
          break;
        }
      }
      // 将新添加的数据发送到服务器端
      if(partCondition === true && lengthCondition === true && questionCondition === true) {
        this.HttpPost('/paper/add/part',{
          listeningPaperId:this.listeningPaperId,
          originalText:this.addPartDialogData.originalText,
          partTitle:this.addPartDialogData.partTitle,
          listeningQuestionList:this.addPartDialogData.listeningQuestionList
        })
            .then(res => {
              if(res.data === true) {
                this.$notify({
                  type:"success",
                  message:"添加成功"
                });
                // 从服务器中重新获取数据
                this.getPaperDetailById();
              }
              else {
                this.$notify({
                  type:"error",
                  message:"服务器繁忙，请稍后再试"
                });
              }
              this.addPartDialogShow = false;
            })
      }

    },

    // 更新 questionCount和radio数组
    updateCountAndArray() {
      // 获取所有的试题数
      this.questionCount = 0;
      for (let i = 0; i < this.paper.partList.length; i++) {
        this.questionCount += this.paper.partList[i].listeningQuestionList.length;
      }
      this.radio = [];
      // 设置radio的个数
      for (let i = 0; i < this.questionCount; i++) {
        this.radio.push("");
      }
    },

    // 上传听力音频
    submitUpload() {
      this.$refs.upload.submit();
    },
    handleChange(file, fileList) {
      if (fileList.length > 0) {
        this.fileList = [fileList[fileList.length - 1]]  // 这一步，是 展示最后一次选择的csv文件
      }
    },
    // 上传成功
    onSuccess(res) {
      this.paper.audio = res.data; // 获取返回的音频路径
      let audio = res.data;
      // 修改音频地址
      this.HttpPost('/paper/update/paper',{
        id:this.listeningPaperId,
        audio:audio
      }).then(res => {
        if(res.data === true) {
          this.$notify({
            message: '上传成功',
            type: "success"
          });
        }
      })
      this.uploadDialogShow = false;
    },
    // 上传失败
    onError() {
      this.$notify({
        message: '上传失败',
        type: 'error'
      })
    },

    // 当跳转到此页面时，跳出一个系统的提示
    systemInfo() {
      this.$notify({
        title: '系统提示',
        message: '如果需要添加新的题目，只能从最后开始添加，' +
            '在题目之间添加新的题目并不能够保证题目的正确顺序和您所预期的一致',
        duration: 0,
        type:"warning"
      });
    }

  },
  created() {
    this.listeningPaperId = this.$route.params.id;
    this.getPaperDetailById();
    // 跳出系统提示
    this.systemInfo();
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
    // 设置div的id，方便锚点跳转
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
  },

  mounted() {
    // 存一份this
    let _this = this;
    window.onbeforeunload = function (e) {
      // 那个路由页面需要，就把path的名字修改成那个，比如我当前页面的path是/vue
      if (_this.$route.path === "/listening/add") {
        // 兼容IE8和Firefox 4之前的事件对象写法（不加也行，现在少有项目兼容老版本浏览器了）
        e = e || window.event;
        if (e) {
          e.returnValue = "returnValue属性值的文字不能自定义，写不写都行的";
        }
        // Chrome支持, Safari支持, Firefox 4版本以后支持, Opera 12版本以后支持 , IE 9版本以后支持
        return "returnValue属性值的文字不能自定义，写不写都行的";
      }

    };
  },
  beforeDestroy() {
    // 离开页面时候再清除
    window.onbeforeunload = () => {
    };
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