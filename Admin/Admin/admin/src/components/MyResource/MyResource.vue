<template>
<div>
  <el-row>
    <el-button type="info" @click="showDialog">上传资源<i class="el-icon-upload el-icon--right"></i></el-button>
    <el-button @click="codeEdit = !codeEdit">修改提取码</el-button>
    <el-input style="margin-left: 15px;width: 200px;" v-show="codeEdit" placeholder="提取码" v-model="code"></el-input>
    <el-button v-show="codeEdit" @click="updateCode" type="primary">确定</el-button>
    <span v-show="!codeEdit" style="margin-left: 15px;color: gray">提取码  {{code}}</span>
  </el-row>

  <el-table
      ref="multipleTable"
      :data="list"
      tooltip-effect="dark"
      style="width: 100%"
  >
    <el-table-column
        label="ID"
        :show-overflow-tooltip="true"
    >
      <template slot-scope="scope">
        <span style="margin-left: 10px">{{ scope.row.resourceId }}</span>
      </template>
    </el-table-column>

    <el-table-column
        label="资源名称"
        :show-overflow-tooltip="true"
    >
      <template slot-scope="scope">
        <span style="margin-left: 10px">{{ scope.row.name }}</span>
      </template>
    </el-table-column>

    <el-table-column
        label="封面"
        :show-overflow-tooltip="true"
    >
      <template slot-scope="scope">
        <el-image style="height: 50px;width: 50px" :src="scope.row.image"></el-image>
      </template>
    </el-table-column>
    <el-table-column
        label="资源地址"
        :show-overflow-tooltip="true"
    >
      <template slot-scope="scope">
        <span style="margin-left: 10px">{{ scope.row.url }}</span>
      </template>
    </el-table-column>

    <el-table-column
        label="是否显示"
        :show-overflow-tooltip="true"
    >
      <template slot-scope="scope">
        <el-switch @change="showOrNot(scope.$index)" v-model="!scope.row.delete_Sign"></el-switch>
      </template>
    </el-table-column>

    <el-table-column
        label="下载量"
        :show-overflow-tooltip="true"
    >
      <template slot-scope="scope">
        <span style="margin-left: 10px">{{ scope.row.sum }}</span>
      </template>
    </el-table-column>

    <el-table-column
        label="上传时间"
        :show-overflow-tooltip="true"
    >
      <template slot-scope="scope">
        <span style="margin-left: 10px">{{ timeChange(scope.row.time) }}</span>
      </template>
    </el-table-column>

    <el-table-column
        label="操作"
    >
      <template slot-scope="scope">
        <el-button @click="deleteResource(scope.$index)" type="danger" icon="el-icon-delete" circle></el-button>
      </template>
    </el-table-column>


  </el-table>

  <el-pagination
                 background
                 layout="sizes, prev, pager, next"
                 :hide-on-single-page="true"
                 :page-size="currentSize"
                 :current-page.sync="currentPage"
                 @current-change="handleCurrentChange"
                 @size-change="handleSizeChange"
                 :page-sizes="[10,20,30]"
                 :total="total"
                 style="display: flex;justify-content: center;margin-bottom:10px"
  >
  </el-pagination>
  <el-dialog title="上传资源"
             :before-close="handleClose"
             :visible.sync="dialogShow">
    <el-form>
      <el-form-item>
        <el-input v-model="resourceForm.name" placeholder="资源名称"></el-input>
      </el-form-item>
<!--      上传封面-->
      <el-upload
          v-if="resourceForm.image === ''"
          class="avatar-uploader"
          :action=uploadImageURL
          list-type="picture-card"
          :show-file-list="false"
          :on-success="handleAvatarSuccess"
          :before-upload="beforeAvatarUpload"
          :onError="uploadError"
          :headers="Header"
          :name="file"
      >
        <i class="el-icon-plus">封面</i>
      </el-upload>
      <el-row v-else>
        <el-col :span="6">
          <img height="60"  :src="resourceForm.image" class="avatar" />
        </el-col>
        <el-col :span="6">
          <el-button @click="deleteImage">X</el-button>
        </el-col>
      </el-row>

<!--      上传资源-->
      <el-upload
          style="margin-top: 15px"
          class="upload-demo"
          ref="upload"
          :file-list="fileList"
          :on-change="handleChange"
          :action="uploadResourceURL"
          :show-file-list="true"
          :on-success="onSuccess"
          :on-error="uploadError"
          :headers="Header"
          :name="file"
          :auto-upload="false">
        <el-button slot="trigger" size="small" type="primary">选取文件</el-button>
        <el-button style="margin-left: 10px;" size="small" type="success" @click="submitUpload">上传到服务器
        </el-button>
      </el-upload>
    </el-form>
    <span slot="footer" class="dialog-footer">
    <el-button @click="dialogShow = false">取 消</el-button>
    <el-button type="primary" @click="addResource">确 定</el-button>
  </span>
  </el-dialog>
</div>
</template>

<script>
export default {
  name: "MyResource",
  data() {
    return {
      list: [],
      page: 1,
      currentSize: 10,
      size: 10,
      currentPage: 1,
      total: 1,
      code:'',
      codeEdit:false,
      Header:{
        Authorization: "Bearer" + " " +  localStorage.getItem("token")
      },
      uploadImageURL:'/apis/resource/upload/image',
      uploadResourceURL:'/apis/resource/upload/resource',
      fileList:[],
      dialogShow:false,
      resourceForm:{
        name:'',
        url:'',
        image:'',
      }
    }
  },
  methods:{
    // 更改页码
    handleCurrentChange(page) {
      this.currentPage = page;
      this.getResourceList();
    },
    // 更改页面大小
    handleSizeChange(size) {
      this.currentSize = size;
      this.getResourceList();
    },
    // 分页获取资源
    getResourceList() {
      this.HttpGet('/resource/admin/' + this.currentPage + "/" + this.currentSize)
          .then(res => {
            this.total = res.data.total;
            this.list = res.data.list;
          })
    },
    // 获取提取码
    getResourceCode() {
      this.HttpGet('/resource/code')
          .then(res => {
            this.code = res.data;
          })
    },
    // 添加资源
    addResource() {
      if(this.resourceForm.name === "" ||
      this.resourceForm.url === "" || this.resourceForm.image === "") {
        this.$notify({
          type:"error",
          message:"名称、封面或者资源不完整",
          title:"上传提示"
        });
        return;
      }
      this.HttpPost('/resource/add',
          {
            name:this.resourceForm.name,
            url:this.resourceForm.url,
            image:this.resourceForm.image
          })
          .then(res => {
            if(res.data === true) {
              this.$notify({
                message:"上传资源成功",
                type:'success',
                title:"上传提示"
              })
              this.getResourceList();
              this.dialogShow = false;
            }
            else {
              this.$notify({
                message:"上传资源失败",
                type:'error',
                title:"上传提示"
              });
              this.dialogShow = false;
            }
          })
    },
    // 修改资源是否显示
    showOrNot(index) {
      let resourceId = this.list[index].resourceId;
      let delete_Sign = !this.list[index].delete_Sign;
      this.HttpPost("/resource/show/" + resourceId + "/" + delete_Sign)
          .then(res => {
            if(res.data === true) {
              this.$notify({
                type:'success',
                message:"修改成功",
                title:"修改提示"
              });
              this.list[index].delete_Sign = delete_Sign;
            }
            else {
              this.$notify({
                type:'error',
                message:"修改失败",
                title:'修改提示'
              })
            }
          })
    },
    // 修改提取码
    updateCode() {
      if(this.code === "") {
        this.$notify({
          type:'error',
          message:"提取码不能为空",
          title:"系统提示"
        });
        return;
      }
      else {
        this.HttpPost('/resource/update/' + this.code)
            .then(res => {
              if(res.data === true) {
                this.$notify({
                  type:'success',
                  message:"修改成功",
                  title:"系统提示"
                });
              }
              else {
                this.$notify({
                  type:"error",
                  message:"修改失败",
                  title:"系统提示"
                })
              }
              this.codeEdit = false;
            })
      }
    },
    // 删除资源
    deleteResource(index) {
      let resourceId = this.list[index].resourceId;
      this.HttpPost('/resource/delete/' + resourceId)
          .then(res => {
            if(res.data === true) {
              this.list.splice(index,1);
              this.$notify({
                type:'success',
                message:"删除成功",
                title:"系统提示"
              })
            }
            else {
              this.$notify({
                type:'error',
                message:"删除失败",
                title:"系统提示"
              })
            }
          })
    },




    // 显示添加资源dialog
    showDialog() {
      this.resourceForm.url = "";
      this.resourceForm.name = "";
      this.resourceForm.image = "";
      this.dialogShow = true;
    },
    // 删除封面
    deleteImage() {
      this.resourceForm.image = "";
    },
    handleAvatarSuccess(res, file) {
      this.resourceForm.image= res.data;
      this.$notify({
        message:"上传成功",
        type:'success',
        title:'系统提示'
      });
    },
    beforeAvatarUpload(file) {
      const arr = [
        "image/jpeg",
        "image/gif",
        "image/jpg",
        "image/png",
        "image/x-png",
        "image/pjpeg",
      ];
      const isJPG = arr.indexOf(file.type) >= 0;
      const isLt2M = file.size / 1024 / 1024 < 4;
      if (!isJPG) {
        this.$notify({
          message:'文件格式不正确',
          title:"系统提示",
          type:'error'
        })
      }
      if (!isLt2M) {
        this.$notify({
          message:'图片大小不能超过4MB',
          title:"系统提示",
          type:'error'
        })
      }
      return isJPG && isLt2M;
    },
    uploadError() {
      this.$notify({
        message:"上传失败",
        title:'系统提示',
        type:"error"
      })
    },

    // 上传资源
    submitUpload() {
      this.$refs.upload.submit();
    },
    handleChange(file, fileList) {
      if (fileList.length > 0) {
        this.fileList = [fileList[fileList.length - 1]]  //
        if(this.fileList[0].size >= 1024 * 1024 * 1024){
          this.$notify({
            message:"文件大小不能超过1G",
            title:'上传提示',
            type:'error'
          })
          this.fileList.splice(0,1);
        }
      }
    },
    // 上传成功
    onSuccess(res) {
      this.resourceForm.url = res.data;
      this.$notify({
        message:'上传成功',
        type:"success"
      });
    },

    handleClose(done) {
      this.$confirm('确认关闭？')
          .then(_ => {
            done();
          })
          .catch(_ => {});
    }
  },
  created() {
    this.getResourceList();
    this.getResourceCode();
  },
  computed: {
    timeChange() {
      return (time) => {
        var date = new Date(time);
        let Y = date.getFullYear() + '-';
        let M = (date.getMonth() + 1 < 10 ? '0' + (date.getMonth() + 1) : date.getMonth() + 1) + '-';
        let D = date.getDate() + ' ';
        return Y + M + D;
      }
    }
  }

}
</script>

<style lang="stylus" scoped>
.avatar-uploader .el-upload {
  border: 1px dashed #d9d9d9;
  border-radius: 6px;
  cursor: pointer;
  position: relative;
  overflow: hidden;
}
.avatar-uploader .el-upload:hover {
  border-color: #409EFF;
}
.avatar-uploader-icon {
  font-size: 28px;
  color: #8c939d;
  width: 178px;
  height: 178px;
  line-height: 178px;
  text-align: center;
}
.avatar {
  width: 178px;
  height: 178px;
  display: block;
}
</style>