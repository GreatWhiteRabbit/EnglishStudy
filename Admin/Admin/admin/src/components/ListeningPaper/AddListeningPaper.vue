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
                      <el-col :span="6">
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

                      <el-col :span="6">
                        <el-row>
                          <el-col style="margin-left: 30vh">
                            <el-button @click="updateTitle" v-show="uploadButtonShow && editPaperTitle === false"
                                       type="primary">修改标题
                            </el-button>
                            <el-button @click="saveUpdateTile" v-show="uploadButtonShow && editPaperTitle === true"
                                       type="primary">确定
                            </el-button>
                          </el-col>
                        </el-row>
                      </el-col>
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
                        <el-button @click="deletePart(index1)" type="danger" icon="el-icon-delete" circle></el-button>
                      </el-col>
                      <el-col :span="2">
                        <el-tooltip content="在此行前插入数据" placement="top">
                          <el-button @click="addElementUp(index1)">
                            <i class="el-icon-plus"></i>
                            <i class="el-icon-arrow-up"></i>
                          </el-button>
                        </el-tooltip>
                      </el-col>
                      <el-col :span="1">
                        <el-tooltip content="在此行后插入数据" placement="bottom">
                          <el-button @click="addElementDown(index1)">
                            <i class="el-icon-plus"></i>
                            <i class="el-icon-arrow-down"></i>
                          </el-button>
                        </el-tooltip>
                      </el-col>
                    </el-row>
                    <el-row v-for="(question,index2) in item.listeningQuestionList" :key="question.questionId">
                      <el-row style="margin-top: 20px;" :id="setId(index1,index2)">
                        <el-col :span="18">
                          <sapn>第{{ getIndex(index1, index2) + 1 }}题 {{ question.title }}</sapn>
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
              <el-col>
                <el-button @click="uploadDialogShow = true" v-show="uploadButtonShow" type="info">上传音频<i
                    class="el-icon-upload el-icon--right"></i>
                </el-button>
                <el-button type="primary" @click="getTemplateFromServe">导入模板</el-button>
                <el-button type="success" v-show="uploadButtonShow" @click="submit">提交</el-button>
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
    <!-- 显示编辑dialog窗口-->
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
              <el-button type="primary" @click="originalTextDialog = true">编辑题目和听力原文</el-button>
            </el-col>
            <el-col :span="1">
              <el-button @click="addQuestion" type="info">添加题目</el-button>
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
      <el-table
          ref="multipleTable"
          :data="editDialogData.listeningQuestionList"
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
                @click="deleteQuestionByIndex(scope.$index)"
            >
            </el-button>
          </template>
        </el-table-column>
      </el-table>
      <el-row>
        <span>听力原文 {{ editDialogData.originalText }}</span>
      </el-row>
      <span slot="footer" class="dialog-footer">
    <el-button @click="cancel">取 消</el-button>
    <el-button type="primary" @click="saveEdit">确 定</el-button>
  </span>
    </el-dialog>
    <!-- 显示添加dialog窗口-->
    <el-dialog
        title="添加题目"
        :visible.sync="addEditorDialogShow"
        width="80%"
        :before-close="handleClose">
      <el-row>
        <el-col :span="14">
          <sapn>{{ addEditDialogData.partTitle }}</sapn>
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
              <el-input v-model="addEditDialogData.partTitle" autocomplete="off" type="textarea"></el-input>
            </el-form-item>
            <el-form-item label="听力原文" label-width="100px" style="height: 100%">
              <el-input v-model="addEditDialogData.originalText" autosize autocomplete="off" type="textarea"></el-input>
            </el-form-item>
          </el-form>
        </el-dialog>
      </el-row>
      <el-table
          ref="multipleTable"
          :data="addEditDialogData.listeningQuestionList"
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
        <span>听力原文 {{ addEditDialogData.originalText }}</span>
      </el-row>
      <span slot="footer" class="dialog-footer">
    <el-button @click="cancel2">取 消</el-button>
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

  </div>
</template>

<script>
export default {
  name: "AddListeningPaper",
  data() {
    return {
      input: "", // 标题
      editPaperTitle: false, // 修改听力标题
      uploadButtonShow: false, // 上传听力音频按钮
      uploadDialogShow: false, // 上传听力dialog框,
      paper: {}, // 获取到的听力模板内容
      questionCount: 0, // 选择题的总数,
      url: '', // 听力音频路径
      radio: [],
      editorDialogShow: false, // 编辑的dialog显示框
      editDialogData: {
        partTitle: '',
        listeningQuestionList: [],
        originalText: ''
      }, // 当点击编辑时dialog中的数据
      addEditorDialogShow: false, // 添加时的dialog显示框
      addEditDialogData: {
        partTitle: '',
        listeningQuestionList: [],
        originalText: ''
      }, // 当点击添加按钮时显示dialog中的数据
      editIndex: -1, // 编辑题目的下标
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
    // 保存修改的标题
    saveUpdateTile() {
      this.paper.paperTitle = this.input;
      this.editPaperTitle = false;
    },
    // 修改标题
    updateTitle() {
      this.editPaperTitle = true;
      this.input = this.paper.paperTitle;
    },
    // 从服务器中获取模板数据
    getTemplateFromServe() {
      // 只获取一条数据就行了
      this.HttpGet('/paper/list/1/1')
          .then(res => {
            this.paper = res.data.list[0];
            this.url = res.data.audio;
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
            this.uploadButtonShow = true;
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
    // 删除part
    deletePart(index) {
      this.paper.partList.splice(index, 1);
      this.updateCountAndArray();
    },
    // 取消编辑操作
    cancel() {
      this.editorDialogShow = false;
    },
    // 保存编辑操作
    saveEdit() {
      // 如果没有question，那么删除part
      if (this.editDialogData.listeningQuestionList.length === 0) {
        this.paper.partList.splice(this.editIndex, 1);
      } else {
        // 先清空question数组
        this.paper.partList[this.editIndex].listeningQuestionList = [];
        for (let i = 0; i < this.editDialogData.listeningQuestionList.length; i++) {
          this.paper.partList[this.editIndex].listeningQuestionList.push(this.editDialogData.listeningQuestionList[i]);
        }
        this.paper.partList[this.editIndex].partTitle = this.editDialogData.partTitle;
        this.paper.partList[this.editIndex].originalText = this.editDialogData.originalText;
      }
      this.editorDialogShow = false;
      this.updateCountAndArray();
    },
    // 显示编辑dialog窗口
    showEditDialog(index) {
      this.editIndex = index;
      this.editorDialogShow = true;
      this.editDialogData.partTitle = this.paper.partList[index].partTitle;
      this.editDialogData.originalText = this.paper.partList[index].originalText;
      this.editDialogData.listeningQuestionList = [];
      for (let i = 0; i < this.paper.partList[index].listeningQuestionList.length; i++) {
        this.editDialogData.listeningQuestionList.push(this.paper.partList[index].listeningQuestionList[i]);
      }
      // 不能直接赋值，直接赋值会使两个变量的值一致，可能两个变量指向了同一个地址吧，不太懂js
      // this.editDialogData.listeningQuestionList = this.paper.partList[index].listeningQuestionList;
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
    // 删除小题目
    deleteQuestionByIndex(index) {
      this.editDialogData.listeningQuestionList.splice(index, 1);
    },
    // 添加小题目
    addQuestion() {
      let question = {
        title: "",
        optionsA: '',
        optionsB: '',
        optionsC: '',
        optionsD: '',
        answer: ''
      };
      this.editDialogData.listeningQuestionList.push(question);
      this.questionIndex = this.editDialogData.listeningQuestionList.length - 1;
    },

    // 在添加dialog中删除小题目
    deleteQuestionByIndex2(index) {
      this.addEditDialogData.listeningQuestionList.splice(index, 1);
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
      this.addEditDialogData.listeningQuestionList.push(question);
      this.questionIndex = this.addEditDialogData.listeningQuestionList.length - 1;
    },

    // 添加dialog取消编辑操作
    cancel2() {
      this.addEditorDialogShow = false;
    },
    // 添加dialog保存编辑操作
    saveEdit2() {
      // 如果没有question，跳出提示
      if (this.addEditDialogData.listeningQuestionList.length === 0
          || this.addEditDialogData.originalText === ""
          || this.addEditDialogData.partTitle === "") {
        this.$confirm('标题或者听力原文或者题目不完整，此操作将不会保存上数据?', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        }).then(() => {
          this.addEditorDialogShow = false;
        }).catch(() => {
        });
      } else {
        let item = {
          partTitle: this.addEditDialogData.partTitle,
          originalText: this.addEditDialogData.originalText,
          listeningQuestionList: this.addEditDialogData.listeningQuestionList
        };
        if (this.addUp === true) {
          this.paper.partList.splice(this.editIndex, 0, item);
        } else {
          this.paper.partList.splice(this.editIndex + 1, 0, item);
        }
        this.addEditorDialogShow = false;
        this.updateCountAndArray();
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

    // 在此行之前添加元素
    addElementUp(index) {
      this.addUp = true;
      this.addEditorDialogShow = true;
      this.editIndex = index;
    },
    // 在此行之后添加元素
    addElementDown(index) {
      this.addUp = false;
      this.addEditorDialogShow = true;
      this.editIndex = index;
    },
    // 将添加的数据提交服务器中
    submit() {
      let listeningPaper = {
        paperTitle: this.paper.paperTitle,
        audio: this.paper.audio,
        partList: []
      };
      let partList = [];
      for (let i = 0; i < this.paper.partList.length; i++) {
        let part = {
          originalText: this.paper.partList[i].originalText,
          partTitle: this.paper.partList[i].partTitle,
          listeningQuestionList: []
        };
        for (let j = 0; j < this.paper.partList[i].listeningQuestionList.length; j++) {
          let listeningQuestion = {
            optionsA: this.paper.partList[i].listeningQuestionList[j].optionsA,
            optionsB: this.paper.partList[i].listeningQuestionList[j].optionsB,
            optionsC: this.paper.partList[i].listeningQuestionList[j].optionsC,
            optionsD: this.paper.partList[i].listeningQuestionList[j].optionsD,
            answer: this.paper.partList[i].listeningQuestionList[j].answer,
            title: this.paper.partList[i].listeningQuestionList[j].title
          };
          part.listeningQuestionList.push(listeningQuestion);
        }
        partList.push(part);
      }
      listeningPaper.partList = partList;
      let list = [];
      list.push(listeningPaper);
      this.HttpPost('/paper/add/single', JSON.stringify({list})).then(res => {
        if (res.success === true) {
          this.$notify({
            title: "系统提示",
            message: "上传成功",
            type: 'success'
          });
          let id = res.data;
          this.$router.push({
                path: '/listening/detail/' + id,
                params: {
                  id: id
                }
              }
          )
        } else {
          this.$notify({
            title: "系统提示",
            message: "上传失败",
            type: "error"
          })
        }
      })
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
      this.$notify({
        message: '上传成功',
        type: "success"
      });
      this.paper.audio = res.data; // 获取返回的音频路径
      this.uploadDialogShow = false;
    },
    // 上传失败
    onError() {
      this.$notify({
        message: '上传失败',
        type: 'error'
      })
    },

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